using UnityEngine;
using UnityEngine.Serialization;

[ExecuteAlways]
[RequireComponent(typeof(Renderer))]
public class DoorMask : MonoBehaviour
{
    [FormerlySerializedAs("MaskRef")] [SerializeField]private int _maskRef;

    public int MaskRef
    {
        get => _maskRef;
        set
        {
            _maskRef = value;
            Refresh();
        }
    }

    private Service<RenderingManager> _renderingManager;
    private Renderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    private void Refresh()
    {
        _renderingManager.Value.RemoveDoorMask(_renderer);

        if (gameObject.activeSelf)
        {
            _renderingManager.Value.AddDoorMask(_renderer, _maskRef);
        }
    }

    void OnEnable()
    {
        _renderingManager.Value.AddDoorMask(_renderer, _maskRef);
    }
    
    void OnDisable()
    {
        if (_renderingManager.Exists)
        {
            _renderingManager.Value.RemoveDoorMask(_renderer);
        }
    }
}
