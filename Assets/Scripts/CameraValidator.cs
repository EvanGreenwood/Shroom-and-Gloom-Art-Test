#region Usings
using UnityEngine;
using Framework;
#endregion

public class CameraValidator : MonoBehaviour
{
  [SerializeField] Camera _camera;
  [SerializeField] Transform _tunnelDestination;
  
  // MonoBehaviour
  //----------------------------------------------------------------------------------------------------
  void Awake() { }
  void Update()
  {
    float endDst = Vector3.Distance(transform.position, _tunnelDestination.position);
    
  }
}
