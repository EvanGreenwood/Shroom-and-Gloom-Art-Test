
using Framework;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
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
    //
    [SerializeField] private TunnelWallData[] wallElements;
    [SerializeField] private Transform _destination;
    void Start()
    {
        Generate();
    }

    //
    public void Generate()
    {
      
        Vector3 diff = _destination.position - transform.position;
        Vector3 direction = diff.normalized;
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
                SpriteRenderer wall = Instantiate(wallData.wallPrefabs[Random.Range(0, wallData.wallPrefabs.Length)], transform.position + direction * distanceGenerated - perpendicular * wallData.width, rotation);
                index = (index + wallData.verticalLoopIncrement) % (wallData.verticalLoopLength + 1);
                float m = index / (float)wallData.verticalLoopLength;
                wall.transform.position = wall.transform.position.PlusY(wallData.minMaxPosition.GetValue(m));
                wall.transform.Rotate(  0,0, wallData.minMaxAngle.GetValue(m) + (Random.value * 2 - 1) * wallData.randomRotation);
                //
                wall.flipX = true;
                if (wallData.randomFlipY)   wall.flipY = Random.value > 0.5f;
            }
            //
            index = 0;
            distanceGenerated = wallData.zOffset;
            while (distanceGenerated < dist)
            {
                distanceGenerated += wallData.spacing;
                SpriteRenderer wall = Instantiate(wallData.wallPrefabs[Random.Range(0, wallData.wallPrefabs.Length)], transform.position + direction * distanceGenerated + perpendicular * wallData.width, rotation);
                index = (index + wallData.verticalLoopIncrement) % (wallData.verticalLoopLength + 1);
                float m = index / (float)wallData.verticalLoopLength;
                wall.transform.position = wall.transform.position.PlusY(wallData.minMaxPosition.GetValue(m));
                wall.transform.Rotate(0, 0, wallData.minMaxAngle.GetValue(   m) * -1 + (Random.value * 2 - 1) * wallData.randomRotation);
                //
              
                if (wallData.randomFlipY) wall.flipY = Random.value > 0.5f;
            }
        }
    }
}
