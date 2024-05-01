using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Base class for a service. Singleton but better.
public class MonoService : MonoBehaviour
{
   
    void OnEnable()
    {
        ServiceLocator.Register(this);
    }

    
    void OnDisable()
    {
        ServiceLocator.UnRegister(this);
    }
}
