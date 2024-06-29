using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderWebNetwork : MonoBehaviour
{
    private SpiderWebNode[] _nodes;
    void Start()
    {
        _nodes = GetComponentsInChildren<SpiderWebNode>();
    }

    public SpiderWebNode FindNextNode(Vector3 fromPosition, float minX, float maxX, Vector3 direction, float range = 1.2f)
    {
        float nearestDist = range +0.01f;
        int nearestIndex = -1;
        for (int i = 0; i < _nodes.Length; i++)
        {
            if (!_nodes[i].IsOccupied && _nodes[i].transform.position.x > minX && _nodes[i].transform.position.x < maxX)
            {
                Vector3 diff = (_nodes[i].transform.position - fromPosition);
                if (diff.magnitude < nearestDist)
                {
                    if (Vector3.Dot(diff.normalized, direction.normalized) > 0.2f)
                    {
                        nearestDist = diff.magnitude;
                        nearestIndex = i;
                    }
                }
            }
        }

        if (nearestIndex >= 0)
        {
            return _nodes[nearestIndex];
        }
        return null;
    }
    void Update()
    {
        
    }
}
