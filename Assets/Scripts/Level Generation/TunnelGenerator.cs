
using Framework;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TunnelGenerator : MonoBehaviour
{
    [System.Serializable]
    public class TunnelWallElement
    {
        public string name;
        public SpriteRenderer[] wallPrefabs;
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
    [SerializeField] private TunnelWallElement[] wallElements;
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
        foreach (TunnelWallElement wallElement in wallElements)
        {
            int index = 0;
            float distanceGenerated = 0;
            while (distanceGenerated < dist)
            {
                distanceGenerated += wallElement.spacing;
                SpriteRenderer wall = Instantiate(wallElement.wallPrefabs[Random.Range(0, wallElement.wallPrefabs.Length)], transform.position + direction * distanceGenerated - perpendicular * wallElement.width, rotation);
                index = (index + wallElement.verticalLoopIncrement) % (wallElement.verticalLoopLength + 1);
                float m = index / (float)wallElement.verticalLoopLength;
                wall.transform.position = wall.transform.position.PlusY(wallElement.minMaxPosition.GetValue(m));
                wall.transform.Rotate(  0,0, wallElement.minMaxAngle.GetValue(m) + (Random.value * 2 - 1) * wallElement.randomRotation);
                //
                if (wallElement.randomFlipY)   wall.flipY = Random.value > 0.5f;
            }
            //
            index = 0;
            distanceGenerated = 0.15f;
            while (distanceGenerated < dist)
            {
                distanceGenerated += wallElement.spacing;
                SpriteRenderer wall = Instantiate(wallElement.wallPrefabs[Random.Range(0, wallElement.wallPrefabs.Length)], transform.position + direction * distanceGenerated + perpendicular * wallElement.width, rotation);
                index = (index + wallElement.verticalLoopIncrement) % (wallElement.verticalLoopLength + 1);
                float m = index / (float)wallElement.verticalLoopLength;
                wall.transform.position = wall.transform.position.PlusY(wallElement.minMaxPosition.GetValue(m));
                wall.transform.Rotate(0, 0, wallElement.minMaxAngle.GetValue(   m) * -1 + (Random.value * 2 - 1) * wallElement.randomRotation);
                //
                wall.flipX = true;
                if (wallElement.randomFlipY) wall.flipY = Random.value > 0.5f;
            }
        }
    }
}
