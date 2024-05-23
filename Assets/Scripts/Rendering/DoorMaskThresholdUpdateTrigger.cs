using UnityEngine;

[RequireComponent(typeof(DoorMask))]
public class DoorMaskThresholdUpdateTrigger : MonoBehaviour
{
    private DoorMask _mask;

    public void Awake()
    {
        _mask = GetComponent<DoorMask>();
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger enter: " + other.gameObject.name);
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log($"[DoorMaskThresholdUpdateTrigger] Updating door mask threshold to: {_mask.MaskRef}");
            Shader.SetGlobalInt(RenderingManager.MaskThresholdIndex, _mask.MaskRef);
        }
    }
}
