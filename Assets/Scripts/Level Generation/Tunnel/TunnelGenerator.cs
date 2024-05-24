//  _____                         _   ___                             _                               
// |_   _| _  _  _ _   _ _   ___ | | / __| ___  _ _   ___  _ _  __ _ | |_  ___  _ _                   
//   | |  | || || ' \ | ' \ / -_)| || (_ |/ -_)| ' \ / -_)| '_|/ _` ||  _|/ _ \| '_|                  
//   |_|   \_,_||_||_||_||_|\___||_| \___|\___||_||_|\___||_|  \__,_| \__|\___/|_|                    
//                                                                                                    
//----------------------------------------------------------------------------------------------------

#region Usings
using System;
using Framework;
using FrameworkWIP;
using JetBrains.Annotations;
using MathBad;
using NaughtyAttributes;
using Ross.EditorRuntimeCombatibility;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Serialization;
using UnityEngine.Splines;
using MathUtils = Framework.MathUtils;
using Random = UnityEngine.Random;
using Spline = UnityEngine.Splines.Spline;
#if UNITY_EDITOR
using Unity.EditorCoroutines.Editor;
using UnityEditor;
#endif
#endregion

[RequireComponent(typeof(SplineContainer))]
public partial class TunnelGenerator : MonoBehaviour
{
    public enum EditorHideMode
    {
        Default, 
        Hidden,
        VisibleAndSave //Careful, you want to save generated elements in the scene?
    }

    public int TunnelIndex { get; set; }

    private const string generatedTunnelTag = "GeneratedTunnelData";
    
    public bool UseSOData = true;
    [ShowIf("UseSOData")]
    public TunnelSettings GenerationSettings;

    public PostProcessVolume TunnelVolume;

    private Transform _generationRoot;
    
    [Space]
    [Tooltip("Default: dont save, rely on regeneration. " +
        "\nHidden: dont save, also hidden in inspector, should be set when not working with tunnel generation." +
        "\n VisibleAndSave: No hide flags are applied, the tunnel elements will be treated as part of the scene and saved in it.")]
    public EditorHideMode EditMode = EditorHideMode.Default;
    
    #region Old Embedded Data
        [FormerlySerializedAs("_colorGradient")]
        [Space]
        [HideIf("UseSOData")] public Gradient Old_ColorGradient;
        [FormerlySerializedAs("_particles")]
        [Header("Particles")]
        [HideIf("UseSOData")] public ParticleSystem Old_Particles;
        [FormerlySerializedAs("_particleSpawnDistance")] [HideIf("UseSOData")] public float Old_ParticleSpawnDistance = 1f;
        [FormerlySerializedAs("_baseTunnelWidth")]
        [Header("Size")]
        [HideIf("UseSOData")] public float Old_BaseTunnelWidth = 1f;
        [FormerlySerializedAs("_lumpyWidth")] [HideIf("UseSOData")] public float Old_LumpyWidth = 0.5f;
        [FormerlySerializedAs("_clearingWidth")] [HideIf("UseSOData")] public float Old_ClearingWidth = 1f;
        [FormerlySerializedAs("_clearingDepth")] [HideIf("UseSOData")] public float Old_ClearingDepth = 2.33f;
        [FormerlySerializedAs("wallElements")]
        [Header("Elements")]
        [HideIf("UseSOData")] public TunnelWallData[] Old_WallElements;
        [FormerlySerializedAs("flatWallPrefab")] [HideIf("UseSOData")] public SpriteRenderer Old_FlatWallPrefab;
        [FormerlySerializedAs("flatWallSpacing")] [HideIf("UseSOData")] public float Old_FlatWallSpacing = 1;
        [FormerlySerializedAs("flatWallOffset")] [HideIf("UseSOData")] public float Old_FlatWallOffset = 1;
        [FormerlySerializedAs("surroundElements")] [HideIf("UseSOData")] public TunnelSurroundData[] Old_SurroundElements;
        [FormerlySerializedAs("floorElements")] [HideIf("UseSOData")] public TunnelFloorData[] Old_FloorElements;
        [FormerlySerializedAs("flatFloorPrefab")] [HideIf("UseSOData")] public SpriteRenderer Old_FlatFloorPrefab;
        [FormerlySerializedAs("flatFloorSpacing")] [HideIf("UseSOData")] public float Old_FlatFloorSpacing = 1f;
        [FormerlySerializedAs("flatFloorOffset")] [HideIf("UseSOData")] public float Old_FlatFloorOffset = -1;
        [FormerlySerializedAs("ceilingElements")] [HideIf("UseSOData")] public TunnelCeilingData[] Old_CeilingElements;
        [FormerlySerializedAs("flatCeilingPrefab")] [HideIf("UseSOData")] public SpriteRenderer Old_FlatCeilingPrefab;
        [FormerlySerializedAs("flatCeilingSpacing")] [HideIf("UseSOData")] public float Old_FlatCeilingSpacing = 1;
        [FormerlySerializedAs("flatCeilingOffset")] [HideIf("UseSOData")] public float Old_FlatCeilingOffset = 1;
        
        
        [Header("\\/ COPY TO SO HERE \\/")]
        [HideIf("UseSOData")] public TunnelSettings SOToCopyInto;
        [HideIf("UseSOData")][Button("Copy Embedded Data To SO")][UsedImplicitly]
        public void CopyOldToNewData()
        {
            SOToCopyInto.CopyValues(this);
        }
      
    #endregion

    #region Redirect Props
        public Gradient ColorGradient => UseSOData ? GenerationSettings.ColorGradient : Old_ColorGradient;
        public ParticleSystem Particles => UseSOData ? GenerationSettings.Particles : Old_Particles;
        public float ParticleSpawnDistance => UseSOData ? GenerationSettings.ParticleSpawnDistance : Old_ParticleSpawnDistance;
        public float BaseTunnelWidth => UseSOData ? GenerationSettings.BaseTunnelWidth : Old_BaseTunnelWidth;
        public float LumpyWidth => UseSOData ? GenerationSettings.LumpyWidth : Old_LumpyWidth;
        public float ClearingWidth => UseSOData ? GenerationSettings.ClearingWidth : Old_ClearingWidth;
        public float ClearingDepth => UseSOData ? GenerationSettings.ClearingDepth : Old_ClearingDepth;
        public TunnelWallData[] WallElements => UseSOData ? GenerationSettings.WallElements : Old_WallElements;
        public SpriteRenderer FlatWallPrefab => UseSOData ? GenerationSettings.FlatWallPrefab : Old_FlatWallPrefab;
        public float FlatWallSpacing => UseSOData ? GenerationSettings.FlatWallSpacing : Old_FlatWallSpacing;
        public float FlatWallOffset => UseSOData ? GenerationSettings.FlatWallOffset : Old_FlatWallOffset;
        public TunnelSurroundData[] SurroundElements => UseSOData ? GenerationSettings.SurroundElements : Old_SurroundElements;
        public TunnelFloorData[] FloorElements => UseSOData ? GenerationSettings.FloorElements : Old_FloorElements;
        public SpriteRenderer FlatFloorPrefab => UseSOData ? GenerationSettings.FlatFloorPrefab : Old_FlatFloorPrefab;
        public float FlatFloorSpacing => UseSOData ? GenerationSettings.FlatFloorSpacing : Old_FlatFloorSpacing;
        public float FlatFloorOffset => UseSOData ? GenerationSettings.FlatFloorOffset : Old_FlatFloorOffset;
        public TunnelCeilingData[] CeilingElements => UseSOData ? GenerationSettings.CeilingElements : Old_CeilingElements;
        public SpriteRenderer FlatCeilingPrefab => UseSOData ? GenerationSettings.FlatCeilingPrefab : Old_FlatCeilingPrefab;
        public float FlatCeilingSpacing => UseSOData ? GenerationSettings.FlatCeilingSpacing : Old_FlatCeilingSpacing;
        public float FlatCeilingOffset => UseSOData ? GenerationSettings.FlatCeilingOffset : Old_FlatCeilingOffset;
        
        void Old_EnsureValidParameters()
        {
            // Zero spacing values will cause an infinite loop and crash the project, so we need to ensure they are not zero.
            //Old only. this is done in SO for new.
            Old_ParticleSpawnDistance = Mathf.Max(0.1f, Old_ParticleSpawnDistance);
            Old_BaseTunnelWidth = Mathf.Max(0.1f, Old_BaseTunnelWidth);
            Old_LumpyWidth = Mathf.Max(0.1f, Old_LumpyWidth);
            Old_ClearingWidth = Mathf.Max(0.1f, Old_ClearingWidth);

            foreach(var wallData in Old_WallElements)
            {
                wallData.Spacing = Mathf.Max(0.1f, wallData.Spacing);
            }
            Old_FlatWallSpacing = Mathf.Max(0.1f, Old_FlatWallSpacing);

            foreach(var surroundData in Old_SurroundElements)
            {
                surroundData.Spacing = Mathf.Max(0.1f, surroundData.Spacing);
            }

            foreach(var floorData in Old_FloorElements)
            {
                floorData.Spacing = Mathf.Max(0.1f, floorData.Spacing);
            }
            Old_FlatFloorSpacing = Mathf.Max(0.1f, Old_FlatFloorSpacing);

            foreach(var ceilingData in Old_CeilingElements)
            {
                ceilingData.Spacing = Mathf.Max(0.1f, ceilingData.Spacing);
            }
            Old_FlatCeilingSpacing = Mathf.Max(0.1f, Old_FlatCeilingSpacing);
        }
        
    #endregion
    
    [SerializeField, HideInInspector] private List<Transform> _elementParents;
    [SerializeField, HideInInspector] private SplineContainer _internalTunnelSpline;
    [SerializeField, HideInInspector] private TunnelMesh _internalTunnelMesh;
    
    private List<Transform> _generatedElements;
    private float _tunnelLength = 1;

    //Needed for setting color before instantiating
    private ParticleSystem _templateInstance;

    #region PROPS
        public SplineContainer Spline
        {
            get
            {
                if(_internalTunnelSpline == null)
                    _internalTunnelSpline = GetComponent<SplineContainer>();
                return _internalTunnelSpline;
            }
        }
        public TunnelMesh Mesh
        {
            get
            {
                if(_internalTunnelMesh == null)
                {
                    _internalTunnelMesh = GetComponent<TunnelMesh>();
                    if(_internalTunnelMesh == null)
                        _internalTunnelMesh = gameObject.AddComponent<TunnelMesh>();
                }

                return _internalTunnelMesh;
            }
        }
        
        public TunnelJoin FrontJoin { get; set; }
        public TunnelJoin BackJoin { get; set; }

        #endregion

    // Tunnel Evaluators
    //----------------------------------------------------------------------------------------------------
    public Vector3 GetClosestPoint(Vector3 worldPoint)
    {
        Vector3 localPoint = transform.InverseTransformPoint(worldPoint);
        SplineUtility.GetNearestPoint(Spline.Spline, localPoint, out float3 nearest, out float t);
        worldPoint = transform.TransformPoint(nearest);
        return worldPoint;
    }

    public float GetNormDistanceFromPoint(Vector3 worldPoint)
    {
        Vector3 localPoint = transform.InverseTransformPoint(worldPoint);
        SplineUtility.GetNearestPoint(Spline.Spline, localPoint, out float3 nearest, out float t, 6, 6);
        return t;
    }

    public Vector3 GetLookaheadPoint(float normalizedPosition, float distanceAhead)
    {
        Spline.Spline.GetPointAtLinearDistance(normalizedPosition, distanceAhead, out float newT);
        return Spline.EvaluatePosition(newT);
    }
    public float GetClosestPositionAndDirection(Vector3 closestPoint, out Vector3 position, out Vector3 direction, out Vector3 up)
    {
        float t = GetNormDistanceFromPoint(closestPoint);
        GetTunnelPositionAndDirection(t, out position, out direction, out up);
        return t;
    }

    public void GetTunnelPositionAndDirection(float t, out Vector3 position, out Vector3 direction, out Vector3 up)
    {
        Spline.Spline.Evaluate(t, out float3 vPosition, out float3 vTangent, out float3 vUp);
        position = transform.TransformPoint(vPosition);
        direction = transform.TransformDirection(vTangent.normalize());
        up = transform.TransformDirection(vUp.normalize());
    }
    
    public void GetTunnelPositionAndDirection(float t, out Vector3 position, out Vector3 direction, out Quaternion rotation, out Vector3 up, out Vector3 perpendicular)
    {
        GetTunnelPositionAndDirection(t, out position, out direction, out up);
        rotation = Quaternion.LookRotation(direction, up);
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
        //Debug.Log(t);
        float proximityToMiddle = Mathf.Abs(0.5f - t) * _tunnelLength;

        float clearingM = Mathf.Clamp01((ClearingDepth - proximityToMiddle) / ClearingDepth);
        return BaseTunnelWidth + (1 + Mathf.Sin(t * _tunnelLength * 0.66f)) / 2f * LumpyWidth + clearingM * ClearingWidth;
    }

    Transform GetGenerationRoot()
    {
        #if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            if (PrefabUtility.IsPartOfAnyPrefab(transform))
            {
                if (_generationRoot == null || _generationRoot == transform)
                {
                    _generationRoot = new GameObject($"{GenerationSettings.name} Generated Data").transform;
                }
            }
        }
        #endif
        return transform;
    }

    // Spawn Tunnel Element
    //----------------------------------------------------------------------------------------------------
    SpriteRenderer SpawnTunnelElement(SpriteRenderer prefab, Vector3 position, Quaternion rotation, Vector3 forward, bool tryFlipSubGenerators, float normalizedDistance)
    {
        
        SpriteRenderer sprite = Instantiate(prefab, position, rotation, GetGenerationRoot());
        if(EditMode != EditorHideMode.VisibleAndSave)
        {
            sprite.gameObject.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSave;
            if(EditMode == EditorHideMode.Hidden)
            {
                sprite.gameObject.hideFlags |= HideFlags.HideInHierarchy | HideFlags.NotEditable | HideFlags.HideInInspector;
            }
        }

        // We require a tunnel element on children. But dont require that somebody building levels knows. :)
        if(!sprite.TryGetComponent(out TunnelElement element))
        {
            element = sprite.AddComponent<TunnelElement>();
        }

        element.SetColor(ColorGradient.Evaluate(normalizedDistance));

        TunnelContext context = new TunnelContext()
                                {
                                    TunnelIndex = TunnelIndex,
                                    DistanceElementIsAt = normalizedDistance * _tunnelLength,
                                    TunnelLength = _tunnelLength
                                };
        element.SetTunnelContext(context);

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
            // Give time for TunnelElement and sub generators to run awake, onenable, start
            yield return null;
            
            e.SubGenerate();

            yield return null;

            if(flip)
            {
                //a ll sub generators must implement a flip, since walls are flipped.
                e.RequestSubGeneratorFlip();
            }
        }

        _generatedElements.Add(sprite.transform);

        // Help with z fighting.
        sprite.transform.localPosition += forward * RNG.Float(-0.01f, 0.01f);
        return sprite;
    }

    ParticleSystem SpawnParticleSystem(ParticleSystem system, Vector3 position, Quaternion rotation)
    {
        ParticleSystem element = Instantiate(system, position, rotation, GetGenerationRoot());
        if(EditMode != EditorHideMode.VisibleAndSave)
        {
            element.gameObject.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSave;
            if(EditMode == EditorHideMode.Hidden)
            {
                element.gameObject.hideFlags |= HideFlags.HideInHierarchy | HideFlags.NotEditable | HideFlags.HideInInspector;
            }
        }

        _generatedElements.Add(element.transform);

        return element;
    }

    Transform SpawnContainer(string parentName)
    {
        Transform root = GetGenerationRoot();
        if (PrefabUtility.IsPartOfAnyPrefab(root))
        {
            root = new GameObject($"{generatedTunnelTag} - {parentName}").transform;
            root.tag = generatedTunnelTag;
            if(EditMode != EditorHideMode.VisibleAndSave)
            {
                root.gameObject.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSave;
                if(EditMode == EditorHideMode.Hidden)
                {
                    root.gameObject.hideFlags |= HideFlags.HideInHierarchy | HideFlags.NotEditable | HideFlags.HideInInspector;
                }
            }
            
            root.SetParent(transform.parent);
        }
        
        return SpawnContainer(parentName, root);
    }

    Transform SpawnContainer(string parentName, Transform parent)
    {
        #if UNITY_EDITOR
        if (PrefabUtility.IsPartOfAnyPrefab(parent))
        {
            throw new InvalidOperationException("Parent is considered prefab, cant spawn container.");
        }
        #endif
        
        Transform container = parent.CreateChild(parentName);
        _elementParents.Add(container);
        if(EditMode != EditorHideMode.VisibleAndSave)
        {
            container.gameObject.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSave;
            if(EditMode == EditorHideMode.Hidden)
            {
                container.gameObject.hideFlags |= HideFlags.HideInHierarchy | HideFlags.NotEditable | HideFlags.HideInInspector;
            }
        }
        return container;
    }

    
    // Generate
    //----------------------------------------------------------------------------------------------------
    [Button("Refresh")]
    public void Generate()
    {
        Old_EnsureValidParameters();

        SplineExtrude extrude = GetComponentInChildren<SplineExtrude>();
        if (extrude)
        {
            extrude.Rebuild();
        }
        
        if(_generatedElements == null)
        {
            _generatedElements = new List<Transform>();
            _elementParents = new List<Transform>();
        }
        else
        {
            foreach(Transform element in _generatedElements)
            {
                if(element == null)
                {
                    // Happens when going between editor and play mode. Object already destroyed. list will be cleared.
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

            for(int i = _elementParents.Count - 1; i >= 0; i--)
            {
                Transform parent = _elementParents[i];
                if(Application.isPlaying)
                {
                    if(parent == null)
                        continue;
                    Destroy(parent.gameObject);
                }
                else
                {
#if UNITY_EDITOR
                    EditorCoroutineUtility.StartCoroutine(DestroyNextFrameBecauseUnity(), this);

                    IEnumerator DestroyNextFrameBecauseUnity()
                    {
                        yield return null;

                        // May happen on play. Ok.
                        if(parent != null)
                        {
                            DestroyImmediate(parent.gameObject);
                        }
                    }
#endif
                }
            }

            _elementParents.Clear();
        }

        if (!Application.isPlaying)
        {
            GameObject[] oldGenerated = GameObject.FindGameObjectsWithTag(generatedTunnelTag);
            for (int i = oldGenerated.Length - 1; i >= 0; i--)
            {
                GameObject generated = oldGenerated[i];
                Safe.Destroy(generated);
            }
        }

        _tunnelLength = Spline.CalculateLength();

        //try
        //{
            SpawnWalls();
            SpawnSurrounds();
            SpawnFloors();
            SpawnCeilings();
            SpawnParticles();
       /* }
        catch (Exception e)
        {
            if (!Application.isPlaying)
            {
                // Weird regeneration prefab error, during compilation. Rather just early exit. Not the end of the world.
                // This will not happen in build but instead you will see unity errors. (should not happen while game is running)
                // In case it does... throw
                Debug.LogError(e.Message);
                return;
            }
            throw;
        }*/

       if (TunnelVolume != null && UseSOData)
       {
           TunnelVolume.profile = GenerationSettings.SplineVolume.Settings.PostProcessingProfile;
           TunnelVolume.weight = 0;

           if (Application.isPlaying)
           {
               TunnelVolume.transform.SetParent(null);
           }
       }

        if (Application.isPlaying)
        {
            Mesh.GenerateMesh();
        }
    }

    public void OnTunnelExit()
    {
        StartCoroutine(FadeOut());
        IEnumerator FadeOut()
        {
            float total = 2;
            float t = total;
            while (t > 0)
            {
                TunnelVolume.weight = Mathf.Min( TunnelVolume.weight, t / total);
                yield return null;
                t -= Time.deltaTime;
            }
        }
    }
    
    public void OnTunnelEnter()
    {
        StartCoroutine(FadeIn());
        IEnumerator FadeIn()
        {
            float t = 0;
            float total = 2;
            while (t < total)
            {
                TunnelVolume.weight = Mathf.Max( TunnelVolume.weight, t / total);
                yield return null;
                t += Time.deltaTime;
            }
        }
    }

    // Walls
    //----------------------------------------------------------------------------------------------------
    void SpawnWalls()
    {
        Transform wallParent = SpawnContainer("Walls");
        Vector3 perpendicular;
        foreach(TunnelWallData wallData in WallElements)
        {
            if (wallData.Disabled) { continue; }
            
            Transform elementParent = SpawnContainer(wallData.Name, wallParent);

            int index;

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
                float distanceM = wallData.ZOffset / _tunnelLength;
                while(distanceM < 1)
                {
                    Spline.Spline.GetPointAtLinearDistance(distanceM, wallData.Spacing, out distanceM);

                    //
                    GetTunnelPositionAndDirection(distanceM, out Vector3 currentPosition, out Vector3 currentDirection,
                                                  out Quaternion currentRotation, out Vector3 up, out perpendicular);
                    // 
                    float width = GetTunnelWidth(distanceM);
                    Vector3 spawnPos = currentPosition + perpendicular * (sign * (width + wallData.Width));
                    SpriteRenderer wall = SpawnTunnelElement(wallData.WallPrefabs.ChooseRandom(), spawnPos, currentRotation, currentDirection, flip, distanceM);
                    elementParent.TakeChild(wall);
                    //Debug.Log("WIDTH:" + width);

                    index = (index + wallData.VerticalLoopIncrement) % (wallData.VerticalLoopLength + 1);
                    float indexM = index / (float)wallData.VerticalLoopLength;

                    wall.transform.position += perpendicular * (wallData.MinMaxPosition.GetValue(indexM) * sign);
                    wall.transform.position += up * wallData.HeightOffset.GetValue(indexM);
                    // Debug.DrawLine(currentPosition, currentPosition + perpendicular, Color.red, 30);

                    wall.transform.Rotate(0, 0, wallData.MinMaxAngle.GetValue(indexM) * sign + RNG.FloatSign() * wallData.RandomRotation);
                    //
                    wall.color = ColorGradient.Evaluate(distanceM);

                    if(side == TunnelSide.Right)
                    {
                        wall.flipX = true;
                    }

                    if(wallData.RandomFlipY) wall.flipY = Random.value > 0.5f;
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

        return;
        
        //--------------------------------------------------
        void GenerateWallFlats(TunnelSide side)
        {
            float sign = 1;
            if(side == TunnelSide.Right)
            {
                sign = -1;
            }

            //Spawn flats
            if(FlatWallPrefab == null)
            {
                // Debug.LogWarning($"[TunnelGenerator] {gameObject.name}; Flat wall element missing, skipping.", gameObject);
                return;
            }

            float dist = 0;
            GetTunnelPositionAndDirection(dist, out Vector3 lastPosition,
                                          out Vector3 forwardDir, out Quaternion rot, out Vector3 upDir, out perpendicular);
            lastPosition += perpendicular * (FlatWallOffset * sign);
            Transform flatsParent = SpawnContainer("Flats", wallParent);
            while(dist < 1)
            {
                Spline.Spline.GetPointAtLinearDistance(dist, FlatWallSpacing, out dist);

                //
                GetTunnelPositionAndDirection(dist, out Vector3 currentPosition, out Vector3 currentDirection, out Quaternion currentRotation, out Vector3 up, out perpendicular);

                // 
                Vector3 spawnPos = (currentPosition) + perpendicular * (FlatWallOffset * sign);
                Vector3 forward = (lastPosition - spawnPos).normalized;
                SpriteRenderer flatWall = SpawnTunnelElement(FlatWallPrefab, spawnPos, currentRotation, forward, false, dist);
                flatsParent.TakeChild(flatWall);

                flatWall.transform.localRotation *= Quaternion.Euler(0, 90, 90);
                flatWall.color = ColorGradient.Evaluate(dist);

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
        Transform surroundParent = SpawnContainer("Surrounds");
        foreach(TunnelSurroundData surroundData in SurroundElements)
        {
            if(surroundData.Disabled) {continue;}

            Transform elementParent = SpawnContainer(surroundData.Name, surroundParent);
            int index = 0;
            float currentAngleIncrement = 0;
            float distanceM = surroundData.ZOffset / _tunnelLength;
            while(distanceM < 1)
            {
                Spline.Spline.GetPointAtLinearDistance(distanceM, surroundData.Spacing, out distanceM);

                //
                GetTunnelPositionAndDirection(distanceM, out Vector3 currentPosition, out Vector3 currentDirection, out Quaternion currentRotation, out Vector3 up, out Vector3 perpendicular);

                //
                switch(surroundData.Pattern)
                {
                    case TunnelSurroundData.SurroundPattern.Solid:
                        while(currentAngleIncrement < 360)
                        {
                            if(surroundData.AngleRange.Contains(currentAngleIncrement))
                            {
                                Vector3 circleOffset = MathUtils.GetPointOnUnitCircleXY(currentAngleIncrement + 180, currentRotation);
                                Vector3 spawnPos = currentPosition + currentDirection * (0.0001f * currentAngleIncrement)
                                                 + up * surroundData.CenterHeight + circleOffset * surroundData.RadiusOffset
                                                 + circleOffset.MultiplyY(0.5f) * GetTunnelWidth(distanceM);

                                SpriteRenderer surroundLump = SpawnTunnelElement(surroundData.SurroundPrefabs.ChooseRandom(),
                                                                                 spawnPos,
                                                                                 currentRotation, currentDirection, false, distanceM);
                                elementParent.TakeChild(surroundLump);
                                surroundLump.transform.Rotate(0, 0, -currentAngleIncrement + 90);
                                surroundLump.color = ColorGradient.Evaluate(distanceM);
                                //
                                if(surroundData.RandomFlipX) surroundLump.flipX = Random.value > 0.5f;
                                if(surroundData.RandomFlipY) surroundLump.flipY = Random.value > 0.5f;
                            }
                            currentAngleIncrement += surroundData.AngleIncrement;
                        }
                        currentAngleIncrement -= 360;
                        break;
                    case TunnelSurroundData.SurroundPattern.Staggered:
                    case TunnelSurroundData.SurroundPattern.StaggeredOffset:
                        if(index % 2 == 1)
                        {
                            currentAngleIncrement += surroundData.AngleIncrement / 2;
                        }
                        else
                        {
                            currentAngleIncrement -= surroundData.AngleIncrement / 2;
                        }
                        if(surroundData.Pattern == TunnelSurroundData.SurroundPattern.StaggeredOffset) currentAngleIncrement -= surroundData.AngleIncrement / 2;
                        //
                        while(currentAngleIncrement < 360)
                        {
                            if(surroundData.AngleRange.Contains(currentAngleIncrement))
                            {
                                Vector3 circleOffset = MathUtils.GetPointOnUnitCircleXY(currentAngleIncrement + 180, currentRotation);
                                Vector3 spawnPos = currentPosition + currentDirection * (0.0001f * currentAngleIncrement)
                                                 + up * surroundData.CenterHeight + circleOffset * surroundData.RadiusOffset
                                                 + circleOffset.MultiplyY(0.5f) * GetTunnelWidth(distanceM);

                                SpriteRenderer surroundLump = SpawnTunnelElement(surroundData.SurroundPrefabs.ChooseRandom(), spawnPos,
                                                                                 currentRotation, currentDirection, circleOffset.x > 0, distanceM);
                                elementParent.TakeChild(surroundLump);
                                surroundLump.transform.Rotate(0, 0, -currentAngleIncrement + 90 + surroundData.RandomRotation * (-1 + Random.value * 2));
                                surroundLump.color = ColorGradient.Evaluate(distanceM);
                                //
                                if(surroundData.RandomFlipX) surroundLump.flipX = Random.value > 0.5f;
                                if(surroundData.RandomFlipY) surroundLump.flipY = Random.value > 0.5f;
                            }
                            currentAngleIncrement += surroundData.AngleIncrement;
                        }
                        currentAngleIncrement -= 360;
                        index++;
                        break;
                    case TunnelSurroundData.SurroundPattern.SpiralStaggered:
                    case TunnelSurroundData.SurroundPattern.Spiral:

                        if(!surroundData.AngleRange.Contains(currentAngleIncrement))
                        {
                            currentAngleIncrement += surroundData.AngleIncrement;
                        }
                        else
                        {
                            Vector3 circleOffset = MathUtils.GetPointOnUnitCircleXY(currentAngleIncrement + (surroundData.Pattern == TunnelSurroundData.SurroundPattern.SpiralStaggered ? surroundData.AngleIncrement / 2 : 0) + 180, currentRotation);
                            Vector3 spawnPos = currentPosition + currentDirection * (0.0001f * currentAngleIncrement)
                                             + up * surroundData.CenterHeight + circleOffset * surroundData.RadiusOffset
                                             + circleOffset.MultiplyY(0.5f) * GetTunnelWidth(distanceM);
                            SpriteRenderer surroundLump = SpawnTunnelElement(surroundData.SurroundPrefabs.ChooseRandom(), spawnPos, currentRotation, currentDirection, circleOffset.x > 0, distanceM);
                            elementParent.TakeChild(surroundLump);
                            surroundLump.transform.Rotate(0, 0, -currentAngleIncrement + 90 + surroundData.RandomRotation * (-1 + Random.value * 2));
                            surroundLump.color = ColorGradient.Evaluate(distanceM);
                            //
                            if(surroundData.RandomFlipX) surroundLump.flipX = Random.value > 0.5f;
                            if(surroundData.RandomFlipY) surroundLump.flipY = Random.value > 0.5f;
                            //
                        }
                        currentAngleIncrement += surroundData.AngleIncrement;
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
        Transform floorParent = SpawnContainer("Floors");
        Vector3 perpendicular;
        foreach(TunnelFloorData floorData in FloorElements)
        {
            if(floorData.Disabled) { continue;}

            Transform elementParent = SpawnContainer(floorData.Name, floorParent);
            float distanceM = floorData.ZOffset / _tunnelLength;
            while(distanceM < 1)
            {
                bool flip = true;
                if(floorData.RandomFlipX) flip = Random.value > 0.5f;

                Spline.Spline.GetPointAtLinearDistance(distanceM, floorData.Spacing, out distanceM);

                //
                GetTunnelPositionAndDirection(distanceM, out Vector3 currentPosition, out Vector3 currentDirection, out Quaternion currentRotation, out Vector3 up, out perpendicular);

                // 
                Vector3 spawnPos = currentPosition + perpendicular * (RNG.FloatSign() * floorData.RandomXPosition);
                SpriteRenderer floor = SpawnTunnelElement(floorData.FloorPrefabs.ChooseRandom(), spawnPos, currentRotation, currentDirection, flip, distanceM);
                elementParent.TakeChild(floor);

                floor.transform.position += perpendicular * floorData.MinMaxPosition.ChooseRandom();
                floor.transform.Rotate(floorData.XRotation, 0, floorData.MinMaxAngle.ChooseRandom());

                floor.color = ColorGradient.Evaluate(distanceM);
                floor.flipX = flip;
            }
        }

        //Spawn flats
        if(FlatFloorPrefab == null)
        {
            // Debug.LogWarning($"[TunnelGenerator] {gameObject.name}; Flat floor element missing, skipping.", gameObject);
            return;
        }

        float dist = 0;
        GetTunnelPositionAndDirection(dist, out Vector3 lastPosition,
                                      out Vector3 forwardDir, out Quaternion rot, out Vector3 upDir, out perpendicular);
        lastPosition += upDir * FlatFloorOffset;
        Transform flatsParent = SpawnContainer("Flats", floorParent);
        while(dist < 1)
        {
            Spline.Spline.GetPointAtLinearDistance(dist, FlatFloorSpacing, out dist);

            //
            GetTunnelPositionAndDirection(dist, out Vector3 currentPosition, out Vector3 currentDirection, out Quaternion currentRotation, out Vector3 up, out perpendicular);

            // 
            Vector3 spawnPos = (currentPosition) + up * FlatFloorOffset;
            Vector3 forward = (lastPosition - spawnPos).normalized;
            SpriteRenderer flatFloor = SpawnTunnelElement(FlatFloorPrefab, spawnPos, currentRotation, forward, false, dist);
            flatsParent.TakeChild(flatFloor);

            flatFloor.transform.localRotation *= Quaternion.Euler(90, 0, 0);

            flatFloor.color = ColorGradient.Evaluate(dist);

            // Floor flat planes need to be aligned exactly to avoid clipping with large tunnel height change 
            // So we need to rotate them towards the last plane, rather than just sample the current curve.
            lastPosition = spawnPos;
        }
    }

    // Ceilings
    //----------------------------------------------------------------------------------------------------
    void SpawnCeilings()
    {
        Transform ceilingsParent = SpawnContainer("Ceilings");
        Vector3 perpendicular;
        foreach(TunnelCeilingData ceilingData in CeilingElements)
        {
            if (ceilingData.Disabled) { continue; }

            Transform elementParent = SpawnContainer(ceilingData.Name, ceilingsParent);
            float distanceM = ceilingData.ZOffset / _tunnelLength;
            while(distanceM < 1)
            {
                bool flip = true;
                if(ceilingData.RandomFlipX) flip = Random.value > 0.5f;
                Spline.Spline.GetPointAtLinearDistance(distanceM, ceilingData.Spacing, out distanceM);

                //
                GetTunnelPositionAndDirection(distanceM, out Vector3 currentPosition, out Vector3 currentDirection, out Quaternion currentRotation, out Vector3 up, out perpendicular);
                // 
                Vector3 spawnPos = currentPosition + up * ceilingData.Height + perpendicular * (RNG.FloatSign() * ceilingData.RandomXPosition);
                SpriteRenderer ceiling = SpawnTunnelElement(ceilingData.CeilingPrefabs.ChooseRandom(), spawnPos, currentRotation, currentDirection, flip, distanceM);
                elementParent.TakeChild(ceiling);

                ceiling.transform.position += perpendicular * ceilingData.MinMaxPosition.ChooseRandom();
                ceiling.transform.Rotate(ceilingData.XRotation, 0, ceilingData.MinMaxAngle.ChooseRandom());
                //
                ceiling.color = ColorGradient.Evaluate(distanceM);
                ceiling.flipX = flip;
            }
        }

        //Spawn flats
        if(FlatCeilingPrefab == null)
        {
            // Debug.LogWarning($"[TunnelGenerator] {gameObject.name}; Flat ceiling element missing, skipping.", gameObject);
            return;
        }
        float dist = 0;
        GetTunnelPositionAndDirection(dist, out Vector3 lastPosition,
                                      out Vector3 forwardDir, out Quaternion rot, out Vector3 upDir, out perpendicular);
        lastPosition += upDir * FlatCeilingOffset;
        Transform flatsParent = SpawnContainer("Flats", ceilingsParent);
        while(dist < 1)
        {
            Spline.Spline.GetPointAtLinearDistance(dist, FlatCeilingSpacing, out dist);

            //
            GetTunnelPositionAndDirection(dist, out Vector3 currentPosition, out Vector3 currentDirection, out Quaternion currentRotation, out Vector3 up, out perpendicular);

            // 
            Vector3 spawnPos = (currentPosition) + up * FlatCeilingOffset;
            Vector3 forward = (lastPosition - spawnPos).normalized;
            SpriteRenderer flatCeiling = SpawnTunnelElement(FlatCeilingPrefab, spawnPos, currentRotation, forward, false, dist);
            flatsParent.TakeChild(flatCeiling);

            flatCeiling.transform.localRotation *= Quaternion.Euler(90, 0, 0);

            flatCeiling.color = ColorGradient.Evaluate(dist);

            // Floor flat planes need to be aligned exactly to avoid clipping with large tunnel height change 
            // So we need to rotate them towards the last plane, rather than just sample the current curve.
            lastPosition = spawnPos;
        }
    }

    // Particles
    //----------------------------------------------------------------------------------------------------
    void SpawnParticles()
    {
        if(Particles == null)
        {
            Debug.LogWarning($"[TunnelGenerator] {gameObject.name}; Missing particles prefab! Skipping...");
            return;
        }
        
        //Need to set the particle color on template instance, because prewarm spawns particles before we can set it in instance.
        if (_templateInstance == null)
        {
            _templateInstance = Instantiate(Particles);
        }

        Transform particleParent = SpawnContainer("Particles");
        Vector3 perpendicular;

        foreach(TunnelFloorData floorData in FloorElements)
        {
            float distanceM = floorData.ZOffset / _tunnelLength;
            while(distanceM < 1)
            {
                Spline.Spline.GetPointAtLinearDistance(distanceM, ParticleSpawnDistance, out distanceM);
                //
                GetTunnelPositionAndDirection(distanceM, out Vector3 currentPosition, out Vector3 currentDirection, out Quaternion currentRotation, out Vector3 up, out perpendicular);

                // 
                Vector3 spawnPos = currentPosition + perpendicular * (RNG.FloatSign() * floorData.RandomXPosition);

                Color particleColor = ColorGradient.Evaluate(distanceM);

                
               
                var templateMain = _templateInstance.main;
                
                templateMain.startColor = particleColor;

                ParticleSystem particles = SpawnParticleSystem(_templateInstance, spawnPos, currentRotation);
                particleParent.TakeChild(particles);

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
        
        Safe.Destroy(_templateInstance.gameObject);
    }

    // MonoBehaviour
    //----------------------------------------------------------------------------------------------------
    void Awake()
    {
        Generate();
    }

#if UNITY_EDITOR
    EditorCoroutine _onValidateRoutine;
    // Lame hack to get around unity being lame and spamming errors (Not used anymore, now a coroutine):
    // https://forum.unity.com/threads/sendmessage-cannot-be-called-during-awake-checkconsistency-or-onvalidate-can-we-suppress.537265/
    void OnValidate()
    {
        if(Application.isPlaying) { return; }
        if(_onValidateRoutine != null) {return;}

        //Determine context.Should run this if we are outside a broader level.
        if (!InWorldContext)
        {
            // Slowing down editor and causing errors. Just call Generated manually by pressing button.
            //_onValidateRoutine = EditorCoroutineUtility.StartCoroutine(OnValidateRoutine(), this);
        }
    }

    public bool InWorldContext => BackJoin != null || FrontJoin != null;
    

    IEnumerator OnValidateRoutine()
    {
        //Copy local spline to SO
        //CopySplinePoints();
        bool dirty = false;
        if (GenerationSettings.SceneData.Spline.Count == Spline.Spline.Count)
        {
            int i = 0;
            int j = 0;
            foreach (var soKnot in GenerationSettings.SceneData.Spline)
            {
                foreach (var knot in Spline.Spline)
                {
                    if (i == j)
                    {
                        bool3 rotEquals = soKnot.Rotation.Equals(knot.Rotation);
                        bool same = Vector3.Distance(soKnot.Position,knot.Position)< 0.01 && rotEquals is { x: true, y: true, z: true } &&
                            Vector3.Distance(soKnot.TangentIn,knot.TangentIn)< 0.01 && Vector3.Distance(soKnot.TangentOut,knot.TangentOut)< 0.01;

                        if (!same)
                        {
                            dirty = false;
                        }
                    }
                    j++;
                }
                i++;
            }
        }
        else
        {
            dirty = true;
        }

        if (dirty)
        {
            Debug.Log("<color=red>Local spline changes detected. Need to save to TunnelSettings?</color>");
        }
       
        
        if(this == null)
        {
            _onValidateRoutine = null;
            yield break;
        }
        yield return null;

        //Error can run while compilation is happening. Resulting in errors related to modifying prefab transforms.
        int attempts = 69;
        for(int retries = 0; retries < attempts; retries++) 
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                yield return null;
                if (retries == attempts - 1)
                {
                    Debug.LogError($"[TunnelGenerator] {gameObject.name} failed to generate OnValidate. Editor is busy. This is probably fine.");
                    _onValidateRoutine = null;
                    yield break;
                }
                continue;
            }
            break;
        }
        
        Generate();
        _onValidateRoutine = null;
    }
#endif
    public void GetEndData(out Vector3 lastTunnelPoint, out Quaternion lastTunnelRotation)
    {
        Spline.Evaluate(1, out float3 position, out float3 tangent, out float3 upVector);
        lastTunnelPoint = position;
        lastTunnelRotation = Quaternion.LookRotation(tangent, upVector);
    }
    
    public void GetLocalStartData(out Vector3 localTunnelStart, out Quaternion localTunnelRotation)
    {
        SplineUtility.Evaluate(Spline.Spline, 0, out float3 position, out float3 tangent, out float3 upVector);
        localTunnelStart = position;
        localTunnelRotation = Quaternion.LookRotation(tangent, upVector);
    }

    [Button("Save Spline")][UsedImplicitly]
    public void CopySplinePoints()
    {
        if (Spline.Spline.Count > 1)
        {
            GenerationSettings.SceneData.Spline = new Spline(Spline.Spline);
            GenerationSettings.Save();
            Debug.Log($"Saved spline data to {GenerationSettings.name}");
        }
    }
    
    [Button("Load Spline")][UsedImplicitly]
    public void SetSplinePointsFromSO()
    {
        if (UseSOData && GenerationSettings.SceneData.Spline != null)
        {
            if (GenerationSettings.SceneData.Spline.Count > 1 )
            {
                Spline.Spline = new Spline(GenerationSettings.SceneData.Spline);
                #if UNITY_EDITOR
                EditorUtility.SetDirty(Spline);
                PrefabUtility.RecordPrefabInstancePropertyModifications(gameObject);
                #endif
                //Debug.Log($"Loaded spline data from {GenerationSettings.name}");
                
                Generate();
            }
        }
    }

    private void OnDrawGizmos()
    {
        #if UNITY_EDITOR
        Color c = Handles.color;
        Handles.color =  ColorGradient.Evaluate(0.5f);
        Spline.Evaluate(0.5f, out float3 position, out float3 tangent, out float3 upVector);
        Handles.Label(position, InWorldContext?gameObject.name:UseSOData?GenerationSettings.name:gameObject.name);
        Handles.color = c;
        #endif
    }
}
