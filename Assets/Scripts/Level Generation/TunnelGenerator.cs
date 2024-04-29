
using Framework;
  
using System.Collections;
using System.Collections.Generic;
 
using UnityEngine;
using UnityEngine.UIElements;

public class TunnelGenerator : MonoBehaviour
{
    [System.Serializable]
    public class TunnelWallData
    {
        public string name;
        public SpriteRenderer[] wallPrefabs;
        public float zOffset = 0;
        public float spacing = 0.5f;
        public float width = 1.5f;
        public int verticalLoopLength = 4;
        public int verticalLoopIncrement = 2;
        public FloatRange minMaxAngle = new FloatRange(-20, 20);
        public FloatRange minMaxPosition = new FloatRange(0.6f, 2.6f);
        public float randomRotation = 5f;
        public bool randomFlipY = true;
    }
    //
    [System.Serializable]
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
    [System.Serializable]
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
    [System.Serializable]
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
    //
    //
    //
    [SerializeField] private TunnelWallData[] wallElements;
    [SerializeField] private TunnelSurroundData[] surroundElements;
    [SerializeField] private TunnelFloorData[] floorElements;
    [SerializeField] private TunnelCeilingData[] ceilingElements;
    [SerializeField] private Transform _destination;
    [SerializeField] private Gradient _colorGradient;
    [SerializeField] private AnimationCurve _ySlopeCurve;
    [SerializeField] private AnimationCurve _sidewaysCurve;
    //
    [SerializeField] private float _baseTunnelWidth = 1f;
    [SerializeField] private float _lumpyWidth = 0.5f;
    [SerializeField] private float _clearingWidth = 1f;
     private float _clearingDepth = 2.33f;
    private float _tunnelLength = 1;
    //
    public Vector3 TunnelForward => _forward;
    private Vector3 _forward;
    private Line3 _tunnelLine; 
    void Start()
    {
        Generate(); 
    }

    public Vector3 GetClosestPoint(Vector3 point)
    {
        return MathUtils.ProjectPointOnRay( _tunnelLine.ToRay(),  point);
    }
    public float GetTunnelYHeight(Vector3 point)
    {
        return GetTunnelYHeight((GetClosestPoint(point) - transform.position).magnitude / _tunnelLength);
    }
    public float GetTunnelProportion(Vector3 point)
    {
        return (GetClosestPoint(point) - transform.position).magnitude / _tunnelLength;
    }
    public float GetTunnelYHeight(float proportion)
    {
        return Mathf.Lerp(_tunnelLine.Start.y, _tunnelLine.End.y, _ySlopeCurve.Evaluate(proportion));        
    }
    public Vector3 GetTunnelSidewaysOffset(float proportion)
    {
        return _sidewaysCurve.Evaluate(proportion) * _tunnelLine.PerpendicularHorizontal;  ;
    }
    //
    public void GetTunnelPosittionAndDirection(float proportion, out Vector3 postion, out Vector3 direction)
    {
      //  Debug.Log("proportion " + proportion);
        postion = _tunnelLine.EvaluateUnclamped(proportion).WithY(GetTunnelYHeight(proportion)) + GetTunnelSidewaysOffset(proportion);
        Vector3 forwardPosition = _tunnelLine.EvaluateUnclamped(proportion + 0.033f).WithY(GetTunnelYHeight(proportion + 0.02f)) + GetTunnelSidewaysOffset(proportion + 0.033f);
        direction =  (forwardPosition - postion).normalized;
    }
    public void GetTunnelPosittionAndDirection(float proportion, out Vector3 postion, out Vector3 direction, out Quaternion rotation, out Vector3 perpendicular)
    {
        //  Debug.Log("proportion " + proportion);
        postion = _tunnelLine.EvaluateUnclamped(proportion).WithY(GetTunnelYHeight(proportion)) + GetTunnelSidewaysOffset(proportion);
        Vector3 forwardPosition = _tunnelLine.EvaluateUnclamped(proportion + 0.033f).WithY(GetTunnelYHeight(proportion + 0.02f)) + GetTunnelSidewaysOffset(proportion + 0.033f);
        direction = (forwardPosition - postion).normalized;
        rotation = Quaternion.LookRotation(direction);
        perpendicular = new Vector3(-direction.z, 0, direction.x);
    }
    //
    public float GetTunnelWidth(Vector3 position)
    {
        return GetTunnelWidth((GetClosestPoint(position) - transform.position).magnitude / _tunnelLength) ;

    }
    public float GetTunnelWidth(float proportion)
    {
        float proximityToMiddle = Mathf.Abs(0.5f - proportion) * _tunnelLength;
        float clearingM = Mathf.Clamp01(_clearingDepth - proximityToMiddle) / _clearingDepth;
        return _baseTunnelWidth + (1 + Mathf.Sin(proportion * _tunnelLength * 0.66f)) /2f * _lumpyWidth + clearingM * _clearingWidth;
    }
    public void Generate()
    {

        Vector3 diff = _destination.position - transform.position;
        _tunnelLength = diff.magnitude;
        Vector3 direction = diff.normalized;
        _forward = direction;
        _tunnelLine = new Line3(transform.position, _destination.position);
        Vector3 perpendicular = new Vector3(-direction.z, 0, direction.x);
         
        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
        //
        foreach (TunnelWallData wallData in wallElements)
        {
            int index = 0;
            float distanceGenerated = wallData.zOffset;
            while (distanceGenerated < _tunnelLength)
            {
                distanceGenerated += wallData.spacing;
                float distanceM = distanceGenerated / _tunnelLength;

                //
                GetTunnelPosittionAndDirection(distanceM, out Vector3 currentPosition, out Vector3 currentDirection, out Quaternion currentRotation, out perpendicular);
                 
                //
                SpriteRenderer wall = Instantiate(wallData.wallPrefabs[Random.Range(0, wallData.wallPrefabs.Length)], currentPosition - perpendicular * (GetTunnelWidth (distanceGenerated / _tunnelLength) + wallData.width), currentRotation, transform);
                index = (index + wallData.verticalLoopIncrement) % (wallData.verticalLoopLength + 1);
                float indexM = index / (float)wallData.verticalLoopLength;
                wall.transform.position = wall.transform.position.PlusY(wallData.minMaxPosition.GetValue(indexM));
                wall.transform.Rotate(0, 0, wallData.minMaxAngle.GetValue(indexM) + (Random.value * 2 - 1) * wallData.randomRotation);
                //
                wall.color = _colorGradient.Evaluate(distanceM);
                wall.flipX = true;
                if (wallData.randomFlipY) wall.flipY = Random.value > 0.5f;
                //
                if (wall.TryGetComponent(out PropCoward propCoward))
                {
                    propCoward.Setup(false);
                }
            }
            //
            index = 0;
            distanceGenerated = wallData.zOffset;
            while (distanceGenerated < _tunnelLength)
            {
                distanceGenerated += wallData.spacing;
                float distanceM = distanceGenerated / _tunnelLength;
                //
                GetTunnelPosittionAndDirection(distanceM, out Vector3 currentPosition, out Vector3 currentDirection, out Quaternion currentRotation, out perpendicular); 
                // 
                SpriteRenderer wall = Instantiate(wallData.wallPrefabs[Random.Range(0, wallData.wallPrefabs.Length)],  currentPosition + perpendicular * (GetTunnelWidth(distanceGenerated / _tunnelLength) + wallData.width), currentRotation, transform);
                index = (index + wallData.verticalLoopIncrement) % (wallData.verticalLoopLength + 1);
                float indexM = index / (float)wallData.verticalLoopLength;
                wall.transform.position = wall.transform.position.PlusY(wallData.minMaxPosition.GetValue(indexM));
                wall.transform.Rotate(0, 0, wallData.minMaxAngle.GetValue(indexM) * -1 + (Random.value * 2 - 1) * wallData.randomRotation);
                //
                wall.color = _colorGradient.Evaluate(distanceM);
                if (wallData.randomFlipY) wall.flipY = Random.value > 0.5f;
                //
                if (wall.TryGetComponent(out PropCoward propCoward))
                {
                    propCoward.Setup(true);
                }
            }
        }
        //
        foreach (TunnelSurroundData surroundData in surroundElements)
        {
            int index = 0;
            float currentAngleIncrement = 0;
            float distanceGenerated = surroundData.zOffset;
            while (distanceGenerated < _tunnelLength)
            {
                distanceGenerated += surroundData.spacing;
                float distanceM = distanceGenerated / _tunnelLength;
                //
                GetTunnelPosittionAndDirection(distanceM, out Vector3 currentPosition, out Vector3 currentDirection, out Quaternion currentRotation, out perpendicular);

                //
                switch (surroundData.pattern)
                {
                    case TunnelSurroundData.SurroundPattern.Solid:
                        while (currentAngleIncrement < 360)
                        {
                            if (surroundData.angleRange.Contains(currentAngleIncrement))
                            {
                                SpriteRenderer surroundLump = Instantiate(surroundData.surroundPrefabs[Random.Range(0, surroundData.surroundPrefabs.Length)], currentPosition + currentDirection* 0.0001f * currentAngleIncrement + Vector3.up * surroundData.centerHeight + MathUtils.GetPointOnUnitCircleXY(currentAngleIncrement + 180, currentRotation) * (GetTunnelWidth(distanceM) + surroundData.radiusOffset), currentRotation, transform);
                                surroundLump.transform.Rotate(0, 0, -currentAngleIncrement + 90);
                                surroundLump.color = _colorGradient.Evaluate(distanceM);
                                //
                                if (surroundData.randomFlipX) surroundLump.flipX = Random.value > 0.5f;
                                if (surroundData.randomFlipY) surroundLump.flipY = Random.value > 0.5f;
                            }
                            currentAngleIncrement += surroundData.angleIncrement;
                        }
                        currentAngleIncrement -= 360;
                        break;
                    case TunnelSurroundData.SurroundPattern.Staggered:
                    case TunnelSurroundData.SurroundPattern.StaggeredOffset:
                        if (index % 2 == 1)
                        {
                            currentAngleIncrement += surroundData.angleIncrement / 2;
                        }
                        else
                        {
                            currentAngleIncrement -= surroundData.angleIncrement / 2;
                        }
                        if (surroundData.pattern == TunnelSurroundData.SurroundPattern.StaggeredOffset) currentAngleIncrement -= surroundData.angleIncrement / 2;
                        //
                        while (currentAngleIncrement < 360)
                        {
                            if (surroundData.angleRange.Contains(currentAngleIncrement))
                            {
                                SpriteRenderer surroundLump = Instantiate(surroundData.surroundPrefabs[Random.Range(0, surroundData.surroundPrefabs.Length)], currentPosition + currentDirection * 0.0001f * currentAngleIncrement + Vector3.up * surroundData.centerHeight + MathUtils.GetPointOnUnitCircleXY(currentAngleIncrement + 180, currentRotation) * (GetTunnelWidth(distanceM) + surroundData.radiusOffset), currentRotation, transform);
                                surroundLump.transform.Rotate(0, 0, -currentAngleIncrement + 90 + surroundData.randomRotation * (-1 + Random.value * 2));
                                surroundLump.color = _colorGradient.Evaluate(distanceM);
                                //
                                if (surroundData.randomFlipX) surroundLump.flipX = Random.value > 0.5f;
                                if (surroundData.randomFlipY) surroundLump.flipY = Random.value > 0.5f;
                            }
                            currentAngleIncrement += surroundData.angleIncrement;
                        }
                        currentAngleIncrement -= 360;
                        index++;
                        break;

                }
            }
        }
                //
        foreach (TunnelFloorData floorData in floorElements)
        {
            float distanceGenerated = floorData.zOffset;
            while (distanceGenerated < _tunnelLength)
            {
                distanceGenerated += floorData.spacing;
                float distanceM = distanceGenerated / _tunnelLength;
                //
                GetTunnelPosittionAndDirection(distanceM, out Vector3 currentPosition, out Vector3 currentDirection, out Quaternion currentRotation, out perpendicular); 
                // 
                SpriteRenderer floor = Instantiate(floorData.floorPrefabs[Random.Range(0, floorData.floorPrefabs.Length)], currentPosition + perpendicular * (Random.value * 2 - 1) * floorData.randomXPosition, currentRotation, transform);
               
                floor.transform.position = floor.transform.position.PlusY(floorData.minMaxPosition.ChooseRandom());
                floor.transform.Rotate(floorData.xRotation, 0, floorData.minMaxAngle.ChooseRandom( )  );
                //
                floor.color = _colorGradient.Evaluate(distanceM);
                floor.flipX = true;
                if (floorData.randomFlipX) floor.flipX = Random.value > 0.5f;
            }
        }
        //
        foreach (TunnelCeilingData ceilingData in ceilingElements)
        {
            float distanceGenerated = ceilingData.zOffset;
            while (distanceGenerated < _tunnelLength)
            {
                distanceGenerated += ceilingData.spacing;
                float distanceM = distanceGenerated / _tunnelLength;
                //
                GetTunnelPosittionAndDirection(distanceM, out Vector3 currentPosition, out Vector3 currentDirection, out Quaternion currentRotation, out perpendicular); 
                // 
                SpriteRenderer floor = Instantiate(ceilingData.ceilingPrefabs[Random.Range(0, ceilingData.ceilingPrefabs.Length)], currentPosition + Vector3.up * ceilingData.height  + perpendicular * (Random.value * 2 - 1) * ceilingData.randomXPosition, currentRotation, transform);

                floor.transform.position = floor.transform.position.PlusY(ceilingData.minMaxPosition.ChooseRandom());
                floor.transform.Rotate(ceilingData.xRotation, 0, ceilingData.minMaxAngle.ChooseRandom());
                //
                floor.color = _colorGradient.Evaluate(distanceM);
                floor.flipX = true;
                if (ceilingData.randomFlipX) floor.flipX = Random.value > 0.5f;
            }
        }
    }
    [SerializeField] private bool _debug;
    private void OnDrawGizmos()
    {
        //
        if(!_debug)
            return;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position , _destination.position  );
        //
        Vector3 diff = _destination.position - transform.position;
        
        Vector3 direction = diff.normalized;
       
        Vector3 perpendicular = new Vector3(-direction.z, 0, direction.x);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(_destination.position, _destination.position + perpendicular - direction);
        Gizmos.DrawLine(_destination.position, _destination.position - perpendicular - direction);
        //
        //DrawingUtils.DrawLine(transform.position + Vector3.up, _destination.position + Vector3.up, Color.green);
    }
}
