using UnityEngine;

public class RotationWobble : MonoBehaviour
{
    [SerializeField] private float _speed = 2;
    [SerializeField] private float _amount = 4;
    private Quaternion _originalLocalRotation;
    private float _counter = 0;
    [SerializeField] private float _offsetFrameYM = 0;
    void Start()
    {
        _counter = transform.position.z + transform.position.y * _offsetFrameYM;
        _originalLocalRotation = transform.localRotation;
    }

    // 
    void Update()
    {
        _counter += UnityEngine.Time.deltaTime * _speed;
        transform.localRotation = _originalLocalRotation * Quaternion.Euler(0, 0, Mathf.Sin(_counter) * _amount);
    }
}
