#region usings
using UnityEngine;
#endregion

public class HandyCam : MonoBehaviour
{
    [SerializeField] private float _positionIntensity = 1.0f;
    [SerializeField] private float _rotationIntensity = 1.0f;
    [SerializeField] private float _speed             = 1.0f;

    private Vector3    _initialPosition;
    private Quaternion _initialRotation;

    // monobehaviour
    private void Awake()
    {
        _initialPosition = transform.localPosition;
        _initialRotation = transform.localRotation;
    }

    // step
    //----------------------------------------------------------------------------------------------------//
    public void Update()
    {
        // position
        float x = Mathf.PerlinNoise(Time.time * _speed, 0) * 2.0f - 1.0f;
        float y = Mathf.PerlinNoise(0, Time.time * _speed) * 2.0f - 1.0f;
        float z = Mathf.PerlinNoise(Time.time * _speed, Time.time * _speed) * 2.0f - 1.0f;
        Vector3 move = new Vector3(x, y, z) * _positionIntensity;
        transform.localPosition = _initialPosition + move;

        // rotation
        x = Mathf.PerlinNoise(Time.time * _speed, 0.3f) * 2.0f - 1.0f;
        y = Mathf.PerlinNoise(0.3f, Time.time * _speed) * 2.0f - 1.0f;
        z = Mathf.PerlinNoise(Time.time * _speed, Time.time * _speed + 0.3f) * 2.0f - 1.0f;
        Vector3 rot = new Vector3(x, y, z) * _rotationIntensity;
        transform.localRotation = _initialRotation * Quaternion.Euler(rot);
    }
}
