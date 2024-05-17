#region Usings
using UnityEngine;
using MathBad;
#endregion

public class ScaleGrow : MonoBehaviour
{
    [SerializeField] float _speed = 0.5f;
    Vector3 _startScale;

    void Awake()
    {
        _startScale = transform.localScale;
    }
    
    void Update()
    {
        transform.localScale += Vector3.one * (_speed * UnityEngine.Time.deltaTime);
    }

    void OnDisable() {transform.localScale = _startScale;}
}
