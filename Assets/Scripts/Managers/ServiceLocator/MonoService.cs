using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Base class for a service. Singleton but better.
public class MonoService : MonoBehaviour
{
   
    protected void OnEnable()
    {
        ServiceLocator.Register(this);
    }

    
    protected void OnDisable()
    {
        ServiceLocator.UnRegister(this);
    }
}
