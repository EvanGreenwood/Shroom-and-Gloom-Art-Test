//  _____                         _   ___                             _                               
// |_   _| _  _  _ _   _ _   ___ | | / __| ___  _ _   ___  _ _  __ _ | |_  ___  _ _                   
//   | |  | || || ' \ | ' \ / -_)| || (_ |/ -_)| ' \ / -_)| '_|/ _` ||  _|/ _ \| '_|                  
//   |_|   \_,_||_||_||_||_|\___||_| \___|\___||_||_|\___||_|  \__,_| \__|\___/|_|                    
//                                                                                                    
//----------------------------------------------------------------------------------------------------

#region Usings
using System;
using Framework;
using MathBad;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using Unity.EditorCoroutines.Editor;
#endif

using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;
using Random = UnityEngine.Random;
#endregion

[RequireComponent(typeof(SplineContainer))]
public class TunnelGenerator : MonoBehaviour
{
#region Tunnel Data
    public enum TunnelSide
    {
        Left,
        Right
    }

    [Serializable]
    public class TunnelWallData
    {
        public string name;
        public SpriteRenderer[] wallPrefabs;
        public float zOffset = 0;
        public float spacing = 0.5f;
        public float width = 1.5f;
        public FloatRange heightOffset = new FloatRange(0f, 0.1f);
        public int verticalLoopLength = 4;
        public int verticalLoopIncrement = 2;
        public FloatRange minMaxAngle = new FloatRange(-20, 20);
        public FloatRange minMaxPosition = new FloatRange(0.6f, 2.6f);
        public float randomRotation = 5f;
        public bool randomFlipY = true;
    }
    //
    [Serializable]
    public class TunnelSurroundData
    {
        public enum SurroundPattern
        {
            Solid,
            Staggered,
            Spiral,
            SpiralStaggered,
            StaggeredOffset,
        }
        public string name;
        public SpriteRenderer[] surroundPrefabs;
        public float zOffset = 0;
        public float spacing = 0.5f;
        public float radiusOffset = 1;
        public float centerHeight = 1;
        public SurroundPattern pattern = SurroundPattern.Staggered;
        public FloatRange angleRange = new FloatRange(44, 316);
        public float angleIncrement = 45;
        public float randomRotation = 5f;
        public bool randomFlipY = true;
        public bool randomFlipX = false;
    }
    //
    [Serializable]
    public class TunnelFloorData
    {
        public string name;
        public SpriteRenderer[] floorPrefabs;
        public float zOffset = 0;
        public float spacing = 0.5f;
        public float randomXPosition = 1.5f;
        public FloatRange minMaxAngle = new FloatRange(-1, 1);
        public float xRotation = 0;
        public FloatRange minMaxPosition = new FloatRange(-0.05f, 0.05f);
        public bool randomFlipX = true;
    }
    //
    [Serializable]
    public class TunnelCeilingData
    {
        public string name;
        public SpriteRenderer[] ceilingPrefabs;
        public float height = 3;
        public float zOffset = 0;
        public float spacing = 0.5f;
        public float randomXPosition = 1.5f;
        public FloatRange minMaxAngle = new FloatRange(-1, 1);
        public float xRotation = 0;
        public FloatRange minMaxPosition = new FloatRange(-0.05f, 0.05f);
        public bool randomFlipX = true;
    }
#endregion

    [SerializeField] Gradient _colorGradient;

    [Header("Particles")]
    [SerializeField]
    ParticleSystem _particles;
    [SerializeField] float _particleSpawnDistance = 1f;

    [Header("Size")]
    [SerializeField]
    float _baseTunnelWidth = 1f;
    [SerializeField] float _lumpyWidth = 0.5f;
    [SerializeField] float _clearingWidth = 1f;
    [SerializeField] float _clearingDepth = 2.33f;
//    [HideInInspector]
    [SerializeField] bool _setHideFlags = true;

    [Header("Elements")]
    [SerializeField]
    TunnelWallData[] wallElements;
    public SpriteRenderer flatWallPrefab;
    public float flatWallSpacing = 1;
    public float flatWallOffset = 1;

    [SerializeField] TunnelSurroundData[] surroundElements;

    [SerializeField] TunnelFloorData[] floorElements;
    public SpriteRenderer flatFloorPrefab;
    public float flatFloorOffset = -1;
    public float flatFloorSpacing = 1f;

    [SerializeField] TunnelCeilingData[] ceilingElements;
    public SpriteRenderer flatCeilingPrefab;
    public float flatCeilingSpacing = 1;
    public float flatCeilingOffset = 1;

    SplineContainer _tunnelSpline;

    List<Transform> _generatedElements;

    [SerializeField] bool _drawGizmos;
    float _tunnelLength = 1;

    [SerializeField]
    // [HideInInspector] 
    bool _generatedIsSelectable = false;

    public SplineContainer tunnelSpline => _tunnelSpline;

    // Tunnel Evaluators
    //----------------------------------------------------------------------------------------------------
    public Vector3 GetClosestPoint(Vector3 point)
    {
        SplineUtility.GetNearestPoint(_tunnelSpline.Spline, point, out float3 nearest, out float t);
        return nearest;
    }

    public float GetNormDistanceFromPoint(Vector3 worldPoint)
    {
        Vector3 localPoint = transform.InverseTransformPoint(worldPoint);
        SplineUtility.GetNearestPoint(_tunnelSpline.Spline, localPoint, out float3 nearest, out float t, 6, 6);
        return t;
    }

    public Vector3 GetLookaheadPoint(float normalizedPosition, float distanceAhead)
    {
        _tunnelSpline.Spline.GetPointAtLinearDistance(normalizedPosition, distanceAhead, out float newT);
        return _tunnelSpline.EvaluatePosition(newT);
    }
    public float GetClosestPositionAndDirection(Vector3 closestPoint, out Vector3 position, out Vector3 direction, out Vector3 up)
    {
        float t = GetNormDistanceFromPoint(closestPoint);
        GetTunnelPositionAndDirection(t, out position, out direction, out up);
        return t;
    }

    public void GetTunnelPositionAndDirection(float t, out Vector3 position, out Vector3 direction, out Vector3 up)
    {
        _tunnelSpline.Spline.Evaluate(t, out float3 vPosition, out float3 vTangent, out float3 vUp);
        position = transform.TransformPoint(vPosition);
        direction = transform.TransformDirection(vTangent.normalize());
        up = transform.TransformDirection(vUp.normalize());
    }
    public void GetTunnelPositionAndDirection(float t, out Vector3 position, out Vector3 direction, out Quaternion rotation, out Vector3 up, out Vector3 perpendicular)
    {
        GetTunnelPositionAndDirection(t, out position, out direction, out up);
        rotation = Quaternion.LookRotation(direction);
        perpendicular = Vector3.Cross(direction, up);
    }

    public float GetTunnelWidth(Vector3 position)
    {
        float t = GetNormDistanceFromPoint(position);
        return GetTunnelWidth(t);
    }
    public float GetTunnelWidth(float t)
    {
        //Get width
        float proximityToMiddle = Mathf.Abs(0.5f - t) * _tunnelLength;

        float clearingM = Mathf.Clamp01(_clearingDepth - proximityToMiddle) / _clearingDepth;
        return _baseTunnelWidth + (1 + Mathf.Sin(t * _tunnelLength * 0.66f)) / 2f * _lumpyWidth + clearingM * _clearingWidth;
    }

    // Spawn Tunnel Element
    //----------------------------------------------------------------------------------------------------
    SpriteRenderer SpawnTunnelElement(SpriteRenderer prefab, Vector3 position, Quaternion rotation, Vector3 forward, bool tryFlipSubGenerators, float normalizedDistance)
    {
        SpriteRenderer sprite = Instantiate(prefab, position, rotation, transform);
        if(_setHideFlags)
        {
            sprite.gameObject.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSave;
            if(!_generatedIsSelectable)
            {
                sprite.gameObject.hideFlags |= HideFlags.HideInHierarchy | HideFlags.NotEditable | HideFlags.HideInInspector;
            }
        }

        //We require a tunnel element on children. But dont require that somebody building levels knows. :)
        if(!sprite.TryGetComponent<TunnelElement>(out TunnelElement element))
        {
            element = sprite.AddComponent<TunnelElement>();
        }

        element.SetColor(_colorGradient.Evaluate(normalizedDistance));

        if(Application.isPlaying)
        {
            StartCoroutine(SetupSubGenerators(element, tryFlipSubGenerators));
        }
        else
        {
#if UNITY_EDITOR
            EditorCoroutineUtility.StartCoroutine(SetupSubGenerators(element, tryFlipSubGenerators), this);
#endif
        }
        IEnumerator SetupSubGenerators(TunnelElement e, bool flip)
        {
            //Give time for TunnelElement and sub generators to run awake, onenable, start
            yield return null;

            e.SubGenerate();

            yield return null;

            if(flip)
            {
                //all sub generators must implement a flip, since walls are flipped.
                e.RequestSubGeneratorFlip();
            }
        }

        _generatedElements.Add(sprite.transform);

        //Help with z fighting.
        sprite.transform.localPosition += forward * RNG.Float(-0.01f, 0.01f);
        return sprite;
    }

    ParticleSystem SpawnParticleSystem(ParticleSystem system, Vector3 position, Quaternion rotation)
    {
        ParticleSystem element = Instantiate(system, position, rotation, transform);
        if(_setHideFlags)
        {
            element.gameObject.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSave;
            if(!_generatedIsSelectable)
            {
                element.gameObject.hideFlags |= HideFlags.HideInHierarchy | HideFlags.NotEditable | HideFlags.HideInInspector;
            }
        }

        _generatedElements.Add(element.transform);

        return element;
    }

    /// <summary>
    /// Zero spacing values will cause an infinite loop and crash the project, so we need to ensure they are not zero.
    /// </summary>
    void EnsureValidParameters()
    {
        _particleSpawnDistance = Mathf.Max(0.1f, _particleSpawnDistance);
        _baseTunnelWidth = Mathf.Max(0.1f, _baseTunnelWidth);
        _lumpyWidth = Mathf.Max(0.1f, _lumpyWidth);
        _clearingWidth = Mathf.Max(0.1f, _clearingWidth);

        foreach(var wallData in wallElements)
        {
            wallData.spacing = Mathf.Max(0.1f, wallData.spacing);
        }
        flatWallSpacing = Mathf.Max(0.1f, flatWallSpacing);

        foreach(var surroundData in surroundElements)
        {
            surroundData.spacing = Mathf.Max(0.1f, surroundData.spacing);
        }

        foreach(var floorData in floorElements)
        {
            floorData.spacing = Mathf.Max(0.1f, floorData.spacing);
        }
        flatFloorSpacing = Mathf.Max(0.1f, flatFloorSpacing);

        foreach(var ceilingData in ceilingElements)
        {
            ceilingData.spacing = Mathf.Max(0.1f, ceilingData.spacing);
        }
        flatCeilingSpacing = Mathf.Max(0.1f, flatCeilingSpacing);
    }

    [Button("Toggle Generated Selectable")]
    public void ToggleSelectable()
    {
        _generatedIsSelectable = !_generatedIsSelectable;
        Generate();
    }

    // Generate
    //----------------------------------------------------------------------------------------------------
    [Button("Refresh")]
    public void Generate()
    {
        EnsureValidParameters();
        if(_generatedElements == null)
        {
            _generatedElements = new List<Transform>();
        }
        else
        {
            foreach(Transform element in _generatedElements)
            {
                if(element == null)
                {
                    //Happens when going between editor and play mode. Object already destroyed. list will be cleared.
                    continue;
                }
                if(Application.isPlaying)
                {
                    Destroy(element.gameObject);
                }
                else
                {
#if UNITY_EDITOR
                    EditorCoroutineUtility.StartCoroutine(DestroyNextFrameBecauseUnity(), this);

                    IEnumerator DestroyNextFrameBecauseUnity()
                    {
                        yield return null;

                        // May happen on play. Ok.
                        if(element != null)
                        {
                            DestroyImmediate(element.gameObject);
                        }
                    }
#endif
                }
            }
            _generatedElements.Clear();
        }

        _tunnelLength = _tunnelSpline.CalculateLength();

        // Vector3 perpendicular = new Vector3(-direction.z, 0, direction.x);
        // Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);

        SpawnWalls();
        SpawnSurrounds();
        SpawnFloors();
        SpawnCeilings();
        SpawnParticles();
    }

    // Walls
    //----------------------------------------------------------------------------------------------------
    void SpawnWalls()
    {
        Vector3 perpendicular;
        foreach(TunnelWallData wallData in wallElements)
        {
            int index = 0;
            float distanceGenerated = wallData.zOffset;

            GenerateWalls(TunnelSide.Left);
            GenerateWalls(TunnelSide.Right);

            void GenerateWalls(TunnelSide side)
            {
                float sign = 1;
                bool flip = false;

                if(side == TunnelSide.Right)
                {
                    sign = -1;
                    flip = true;
                }

                index = 0;
                float distanceM = wallData.zOffset / _tunnelLength;
                while(distanceM < 1)
                {
                    _tunnelSpline.Spline.GetPointAtLinearDistance(distanceM, wallData.spacing, out distanceM);

                    //
                    GetTunnelPositionAndDirection(distanceM, out Vector3 currentPosition, out Vector3 currentDirection,
                                                  out Quaternion currentRotation, out Vector3 up, out perpendicular);
                    // 
                    Vector3 spawnPos = currentPosition + sign * (perpendicular * (GetTunnelWidth(distanceGenerated / _tunnelLength) + wallData.width));
                    SpriteRenderer wall = SpawnTunnelElement(wallData.wallPrefabs.ChooseRandom(), spawnPos, currentRotation, currentDirection, flip, distanceM);

                    index = (index + wallData.verticalLoopIncrement) % (wallData.verticalLoopLength + 1);
                    float indexM = index / (float)wallData.verticalLoopLength;

                    wall.transform.position += (perpendicular * wallData.minMaxPosition.GetValue(indexM)) * sign;
                    wall.transform.position += up * wallData.heightOffset.GetValue(indexM);
                    //Debug.DrawLine(currentPosition, currentPosition + perpendicular, Color.red, 30);

                    wall.transform.Rotate(0, 0, wallData.minMaxAngle.GetValue(indexM) * sign + (Random.value * 2 - 1) * wallData.randomRotation);
                    //
                    wall.color = _colorGradient.Evaluate(distanceM);

                    if(side == TunnelSide.Right)
                    {
                        wall.flipX = true;
                    }

                    if(wallData.randomFlipY) wall.flipY = Random.value > 0.5f;
                    //
                    if(wall.TryGetComponent(out PropCoward propCoward))
                    {
                        propCoward.Setup(side == TunnelSide.Left);
                    }
                }
            }
        }

        GenerateWallFlats(TunnelSide.Left);
        GenerateWallFlats(TunnelSide.Right);

        void GenerateWallFlats(TunnelSide side)
        {
            float sign = 1;
            if(side == TunnelSide.Right)
            {
                sign = -1;
            }

            //Spawn flats
            if(flatWallPrefab == null)
            {
                Debug.LogWarning($"[TunnelGenerator] {gameObject.name}; Flat wall element missing, skipping.", gameObject);
                return;
            }

            float dist = 0;
            GetTunnelPositionAndDirection(dist, out Vector3 lastPosition,
                                          out Vector3 forwardDir, out Quaternion rot, out Vector3 upDir, out perpendicular);
            lastPosition += perpendicular * flatWallOffset * sign;
            while(dist < 1)
            {
                _tunnelSpline.Spline.GetPointAtLinearDistance(dist, flatWallSpacing, out dist);

                //
                GetTunnelPositionAndDirection(dist, out Vector3 currentPosition, out Vector3 currentDirection, out Quaternion currentRotation, out Vector3 up, out perpendicular);

                // 
                Vector3 spawnPos = (currentPosition) + perpendicular * flatWallOffset * sign;
                Vector3 forward = (lastPosition - spawnPos).normalized;
                SpriteRenderer flatWall = SpawnTunnelElement(flatWallPrefab, spawnPos, currentRotation, forward, false, dist);

                flatWall.transform.localRotation *= Quaternion.Euler(0, 90, 90);

                flatWall.color = _colorGradient.Evaluate(dist);

                // Floor flat planes need to be aligned exactly to avoid clipping with large tunnel height change 
                // So we need to rotate them towards the last plane, rather than just sample the current curve.
                lastPosition = spawnPos;
            }
        }
    }

    // Surrounds
    //----------------------------------------------------------------------------------------------------
    void SpawnSurrounds()
    {
        Vector3 perpendicular;
        foreach(TunnelSurroundData surroundData in surroundElements)
        {
            int index = 0;
            float currentAngleIncrement = 0;
            float distanceM = surroundData.zOffset / _tunnelLength;
            while(distanceM < 1)
            {
                _tunnelSpline.Spline.GetPointAtLinearDistance(distanceM, surroundData.spacing, out distanceM);

                //
                GetTunnelPositionAndDirection(distanceM, out Vector3 currentPosition, out Vector3 currentDirection, out Quaternion currentRotation, out Vector3 up, out perpendicular);

                //
                switch(surroundData.pattern)
                {
                    case TunnelSurroundData.SurroundPattern.Solid:
                        while(currentAngleIncrement < 360)
                        {
                            if(surroundData.angleRange.Contains(currentAngleIncrement))
                            {
                                Vector3 circleOffset = MathUtils.GetPointOnUnitCircleXY(currentAngleIncrement + 180, currentRotation);
                                Vector3 spawnPos = currentPosition + currentDirection * 0.0001f * currentAngleIncrement
                                                 + up * surroundData.centerHeight + circleOffset * surroundData.radiusOffset
                                                 + circleOffset.MultiplyY(0.5f) * GetTunnelWidth(distanceM);

                                SpriteRenderer surroundLump = SpawnTunnelElement(surroundData.surroundPrefabs.ChooseRandom(),
                                                                                 spawnPos,
                                                                                 currentRotation, currentDirection, false, distanceM);

                                surroundLump.transform.Rotate(0, 0, -currentAngleIncrement + 90);
                                surroundLump.color = _colorGradient.Evaluate(distanceM);
                                //
                                if(surroundData.randomFlipX) surroundLump.flipX = Random.value > 0.5f;
                                if(surroundData.randomFlipY) surroundLump.flipY = Random.value > 0.5f;
                            }
                            currentAngleIncrement += surroundData.angleIncrement;
                        }
                        currentAngleIncrement -= 360;
                        break;
                    case TunnelSurroundData.SurroundPattern.Staggered:
                    case TunnelSurroundData.SurroundPattern.StaggeredOffset:
                        if(index % 2 == 1)
                        {
                            currentAngleIncrement += surroundData.angleIncrement / 2;
                        }
                        else
                        {
                            currentAngleIncrement -= surroundData.angleIncrement / 2;
                        }
                        if(surroundData.pattern == TunnelSurroundData.SurroundPattern.StaggeredOffset) currentAngleIncrement -= surroundData.angleIncrement / 2;
                        //
                        while(currentAngleIncrement < 360)
                        {
                            if(surroundData.angleRange.Contains(currentAngleIncrement))
                            {
                                Vector3 circleOffset = MathUtils.GetPointOnUnitCircleXY(currentAngleIncrement + 180, currentRotation);
                                Vector3 spawnPos = currentPosition + currentDirection * 0.0001f * currentAngleIncrement
                                                 + up * surroundData.centerHeight + circleOffset * surroundData.radiusOffset
                                                 + circleOffset.MultiplyY(0.5f) * GetTunnelWidth(distanceM);

                                SpriteRenderer surroundLump = SpawnTunnelElement(surroundData.surroundPrefabs.ChooseRandom(), spawnPos,
                                                                                 currentRotation, currentDirection, circleOffset.x > 0, distanceM);
                                surroundLump.transform.Rotate(0, 0, -currentAngleIncrement + 90 + surroundData.randomRotation * (-1 + Random.value * 2));
                                surroundLump.color = _colorGradient.Evaluate(distanceM);
                                //
                                if(surroundData.randomFlipX) surroundLump.flipX = Random.value > 0.5f;
                                if(surroundData.randomFlipY) surroundLump.flipY = Random.value > 0.5f;
                            }
                            currentAngleIncrement += surroundData.angleIncrement;
                        }
                        currentAngleIncrement -= 360;
                        index++;
                        break;
                    case TunnelSurroundData.SurroundPattern.SpiralStaggered:
                    case TunnelSurroundData.SurroundPattern.Spiral:

                        if(!surroundData.angleRange.Contains(currentAngleIncrement))
                        {
                            currentAngleIncrement += surroundData.angleIncrement;
                        }
                        else
                        {
                            Vector3 circleOffset = MathUtils.GetPointOnUnitCircleXY(currentAngleIncrement + (surroundData.pattern == TunnelSurroundData.SurroundPattern.SpiralStaggered ? surroundData.angleIncrement / 2 : 0) + 180, currentRotation);
                            Vector3 spawnPos = currentPosition + currentDirection * 0.0001f * currentAngleIncrement
                                             + up * surroundData.centerHeight + circleOffset * surroundData.radiusOffset
                                             + circleOffset.MultiplyY(0.5f) * GetTunnelWidth(distanceM);
                            SpriteRenderer surroundLump = SpawnTunnelElement(surroundData.surroundPrefabs.ChooseRandom(), spawnPos, currentRotation, currentDirection, circleOffset.x > 0, distanceM);
                            surroundLump.transform.Rotate(0, 0, -currentAngleIncrement + 90 + surroundData.randomRotation * (-1 + Random.value * 2));
                            surroundLump.color = _colorGradient.Evaluate(distanceM);
                            //
                            if(surroundData.randomFlipX) surroundLump.flipX = Random.value > 0.5f;
                            if(surroundData.randomFlipY) surroundLump.flipY = Random.value > 0.5f;
                            //
                        }
                        currentAngleIncrement += surroundData.angleIncrement;
                        if(currentAngleIncrement > 360) currentAngleIncrement -= 360;
                        if(currentAngleIncrement < 0) currentAngleIncrement += 360;
                        //
                        index++;
                        break;
                }
            }
        }
    }

    // Floors
    //----------------------------------------------------------------------------------------------------
    void SpawnFloors()
    {
        Vector3 perpendicular;
        foreach(TunnelFloorData floorData in floorElements)
        {
            float distanceM = floorData.zOffset / _tunnelLength;
            while(distanceM < 1)
            {
                bool flip = true;
                if(floorData.randomFlipX) flip = Random.value > 0.5f;

                _tunnelSpline.Spline.GetPointAtLinearDistance(distanceM, floorData.spacing, out distanceM);

                //
                GetTunnelPositionAndDirection(distanceM, out Vector3 currentPosition, out Vector3 currentDirection, out Quaternion currentRotation, out Vector3 up, out perpendicular);

                // 
                Vector3 spawnPos = currentPosition + perpendicular * (Random.value * 2 - 1) * floorData.randomXPosition;
                SpriteRenderer floor = SpawnTunnelElement(floorData.floorPrefabs.ChooseRandom(), spawnPos, currentRotation, currentDirection, flip, distanceM);

                floor.transform.position += perpendicular * floorData.minMaxPosition.ChooseRandom();
                floor.transform.Rotate(floorData.xRotation, 0, floorData.minMaxAngle.ChooseRandom());

                floor.color = _colorGradient.Evaluate(distanceM);
                floor.flipX = flip;
            }
        }

        //Spawn flats
        if(flatFloorPrefab == null)
        {
            Debug.LogWarning($"[TunnelGenerator] {gameObject.name}; Flat floor element missing, skipping.", gameObject);
            return;
        }

        float dist = 0;
        GetTunnelPositionAndDirection(dist, out Vector3 lastPosition,
                                      out Vector3 forwardDir, out Quaternion rot, out Vector3 upDir, out perpendicular);
        lastPosition += upDir * flatFloorOffset;

        while(dist < 1)
        {
            _tunnelSpline.Spline.GetPointAtLinearDistance(dist, flatFloorSpacing, out dist);

            //
            GetTunnelPositionAndDirection(dist, out Vector3 currentPosition, out Vector3 currentDirection, out Quaternion currentRotation, out Vector3 up, out perpendicular);

            // 
            Vector3 spawnPos = (currentPosition) + up * flatFloorOffset;
            Vector3 forward = (lastPosition - spawnPos).normalized;
            SpriteRenderer flatFloor = SpawnTunnelElement(flatFloorPrefab, spawnPos, currentRotation, forward, false, dist);

            flatFloor.transform.localRotation *= Quaternion.Euler(90, 0, 0);

            flatFloor.color = _colorGradient.Evaluate(dist);

            // Floor flat planes need to be aligned exactly to avoid clipping with large tunnel height change 
            // So we need to rotate them towards the last plane, rather than just sample the current curve.
            lastPosition = spawnPos;
        }
    }

    // Ceilings
    //----------------------------------------------------------------------------------------------------
    void SpawnCeilings()
    {
        Vector3 perpendicular;
        foreach(TunnelCeilingData ceilingData in ceilingElements)
        {
            float distanceM = ceilingData.zOffset / _tunnelLength;
            while(distanceM < 1)
            {
                bool flip = true;
                if(ceilingData.randomFlipX) flip = Random.value > 0.5f;
                _tunnelSpline.Spline.GetPointAtLinearDistance(distanceM, ceilingData.spacing, out distanceM);

                //
                GetTunnelPositionAndDirection(distanceM, out Vector3 currentPosition, out Vector3 currentDirection, out Quaternion currentRotation, out Vector3 up, out perpendicular);
                // 
                Vector3 spawnPos = currentPosition + up * ceilingData.height + perpendicular * (Random.value * 2 - 1) * ceilingData.randomXPosition;
                SpriteRenderer floor = SpawnTunnelElement(ceilingData.ceilingPrefabs.ChooseRandom(), spawnPos, currentRotation, currentDirection, flip, distanceM);

                floor.transform.position += perpendicular * ceilingData.minMaxPosition.ChooseRandom();
                floor.transform.Rotate(ceilingData.xRotation, 0, ceilingData.minMaxAngle.ChooseRandom());
                //
                floor.color = _colorGradient.Evaluate(distanceM);
                floor.flipX = flip;
            }
        }

        //Spawn flats
        if(flatCeilingPrefab == null)
        {
            Debug.LogWarning($"[TunnelGenerator] {gameObject.name}; Flat ceiling element missing, skipping.", gameObject);
            return;
        }
        float dist = 0;
        GetTunnelPositionAndDirection(dist, out Vector3 lastPosition,
                                      out Vector3 forwardDir, out Quaternion rot, out Vector3 upDir, out perpendicular);
        lastPosition += upDir * flatCeilingOffset;
        while(dist < 1)
        {
            _tunnelSpline.Spline.GetPointAtLinearDistance(dist, flatCeilingSpacing, out dist);

            //
            GetTunnelPositionAndDirection(dist, out Vector3 currentPosition, out Vector3 currentDirection, out Quaternion currentRotation, out Vector3 up, out perpendicular);

            // 
            Vector3 spawnPos = (currentPosition) + up * flatCeilingOffset;
            Vector3 forward = (lastPosition - spawnPos).normalized;
            SpriteRenderer flatCeiling = SpawnTunnelElement(flatCeilingPrefab, spawnPos, currentRotation, forward, false, dist);

            flatCeiling.transform.localRotation *= Quaternion.Euler(90, 0, 0);

            flatCeiling.color = _colorGradient.Evaluate(dist);

            // Floor flat planes need to be aligned exactly to avoid clipping with large tunnel height change 
            // So we need to rotate them towards the last plane, rather than just sample the current curve.
            lastPosition = spawnPos;
        }
    }

    // Particles
    //----------------------------------------------------------------------------------------------------
    void SpawnParticles()
    {
        if(_particles == null)
        {
            Debug.LogWarning($"[TunnelGenerator] {gameObject.name}; Missing particles prefab! Skipping...");
            return;
        }
        Vector3 perpendicular;
        foreach(TunnelFloorData floorData in floorElements)
        {
            float distanceM = floorData.zOffset / _tunnelLength;
            while(distanceM < 1)
            {
                _tunnelSpline.Spline.GetPointAtLinearDistance(distanceM, _particleSpawnDistance, out distanceM);
                //
                GetTunnelPositionAndDirection(distanceM, out Vector3 currentPosition, out Vector3 currentDirection, out Quaternion currentRotation, out Vector3 up, out perpendicular);

                // 
                Vector3 spawnPos = currentPosition + perpendicular * (Random.value * 2 - 1) * floorData.randomXPosition;

                Color particleColor = _colorGradient.Evaluate(distanceM);

                //Need to set the particle color on prefab, because prewarm spawns particles before we can set it in instance.
                var prefabMain = _particles.main;
                prefabMain.startColor = particleColor;

                ParticleSystem particles = SpawnParticleSystem(_particles, spawnPos, currentRotation);

                //floor.transform.localPosition = floor.transform.localPosition.PlusY(floorData.minMaxPosition.ChooseRandom());
                //floor.transform.Rotate(floorData.xRotation, 0, floorData.minMaxAngle.ChooseRandom());

                var main = particles.main;
                main.startColor = new ParticleSystem.MinMaxGradient(particleColor);
                var colorOverLifetime = particles.colorOverLifetime;
                ParticleSystem.MinMaxGradient gradient = colorOverLifetime.color;
                gradient.colorMax = particleColor;
                gradient.colorMin = particleColor;
                colorOverLifetime.color = gradient;
            }
        }
    }

    void ConvertTunnelToMeshRenderers() { }

    // MonoBehaviour
    //----------------------------------------------------------------------------------------------------
    void Start()
    {
        Generate();
        ConvertTunnelToMeshRenderers();
    }

    void OnEnable()
    {
        _tunnelSpline = GetComponent<SplineContainer>();
    }

#if UNITY_EDITOR
    EditorCoroutine _onValidateRoutine;
    // Lame hack to get around unity being lame and spamming errors:
    // https://forum.unity.com/threads/sendmessage-cannot-be-called-during-awake-checkconsistency-or-onvalidate-can-we-suppress.537265/
    void OnValidate()
    {
        if(_onValidateRoutine != null)
            return;
        _onValidateRoutine = EditorCoroutineUtility.StartCoroutine(OnValidateRoutine(), this);
    }

    IEnumerator OnValidateRoutine()
    {
        if(this == null)
        {
            _onValidateRoutine = null;
            yield break;
        }
        yield return null;
        _tunnelSpline = GetComponent<SplineContainer>();
        Generate();
        _onValidateRoutine = null;
    }
#endif

    void OnDrawGizmos()
    {
        //
        if(!_drawGizmos)
            return;
    }
}
