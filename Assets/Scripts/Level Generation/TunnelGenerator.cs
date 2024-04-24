
using Framework;
 
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
    [SerializeField] private TunnelWallData[] wallElements;
    [SerializeField] private TunnelFloorData[] floorElements;
    [SerializeField] private TunnelCeilingData[] ceilingElements;
    [SerializeField] private Transform _destination;
    [SerializeField] private Gradient _colorGradient;
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
    //
    public void Generate()
    {

        Vector3 diff = _destination.position - transform.position;
        Vector3 direction = diff.normalized;
        _forward = direction;
        _tunnelLine = new Line3(transform.position, _destination.position);
        Vector3 perpendicular = new Vector3(-direction.z, 0, direction.x);
        float dist = diff.magnitude;
        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
        //
        foreach (TunnelWallData wallData in wallElements)
        {
            int index = 0;
            float distanceGenerated = wallData.zOffset;
            while (distanceGenerated < dist)
            {
                distanceGenerated += wallData.spacing;
                float distanceM = distanceGenerated / dist;
                SpriteRenderer wall = Instantiate(wallData.wallPrefabs[Random.Range(0, wallData.wallPrefabs.Length)], transform.position + direction * distanceGenerated - perpendicular * wallData.width, rotation, transform);
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
            while (distanceGenerated < dist)
            {
                distanceGenerated += wallData.spacing;
                float distanceM = distanceGenerated / dist;
                SpriteRenderer wall = Instantiate(wallData.wallPrefabs[Random.Range(0, wallData.wallPrefabs.Length)], transform.position + direction * distanceGenerated + perpendicular * wallData.width, rotation, transform);
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
        foreach (TunnelFloorData floorData in floorElements)
        {
            float distanceGenerated = floorData.zOffset;
            while (distanceGenerated < dist)
            {
                distanceGenerated += floorData.spacing;
                float distanceM = distanceGenerated / dist;
                SpriteRenderer floor = Instantiate(floorData.floorPrefabs[Random.Range(0, floorData.floorPrefabs.Length)], transform.position + direction * distanceGenerated + perpendicular * (Random.value * 2 - 1) * floorData.randomXPosition, rotation, transform);
               
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
            while (distanceGenerated < dist)
            {
                distanceGenerated += ceilingData.spacing;
                float distanceM = distanceGenerated / dist;
                SpriteRenderer floor = Instantiate(ceilingData.ceilingPrefabs[Random.Range(0, ceilingData.ceilingPrefabs.Length)], transform.position + Vector3.up * ceilingData.height + direction * distanceGenerated + perpendicular * (Random.value * 2 - 1) * ceilingData.randomXPosition, rotation, transform);

                floor.transform.position = floor.transform.position.PlusY(ceilingData.minMaxPosition.ChooseRandom());
                floor.transform.Rotate(ceilingData.xRotation, 0, ceilingData.minMaxAngle.ChooseRandom());
                //
                floor.color = _colorGradient.Evaluate(distanceM);
                floor.flipX = true;
                if (ceilingData.randomFlipX) floor.flipX = Random.value > 0.5f;
            }
        }
    }
}
