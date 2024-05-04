using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class ShroomLight : MonoBehaviour
{
   [Header("Point Light")]
   public float OuterRadius = 10;
   public float InnerRadius = 2;
   public float Intensity = 1;
   public Color Color = Color.white;
   
   private Service<ShroomLightingService> _lightService;
   public void OnEnable()
   {
      if (!_lightService.Exists)
      {
         Debug.LogWarning("[ShroomLight] Shroom light exists in a scene or prefab without access " +
            "to the LightingService. Not a problem, but wont do anything.");
         return;
      }
      _lightService.Value.RegisterLight(this);
   }

   public void OnDisable()
   {
      if (!_lightService.Exists)
      {
         return;
      }
      _lightService.Value.UnRegisterLight(this);
   }

   public void OnDrawGizmos()
   {
      Color before = Gizmos.color;
      Gizmos.color = Color;
      
      Gizmos.DrawWireSphere(transform.position, InnerRadius);
      Gizmos.color = new Color(Color.r, Color.g, Color.b, 0.5f);
      Gizmos.DrawWireSphere(transform.position, OuterRadius);
      
      Gizmos.color = before;
   }
}
