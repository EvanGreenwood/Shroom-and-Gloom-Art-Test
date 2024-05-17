using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityAnimation : MonoBehaviour
{
    public AnimationClip BeginIdle;
    public AnimationClip BeginToEnd;
    public AnimationClip EndIdle;

    [Range(0,1)]
    public float DisableChance;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
