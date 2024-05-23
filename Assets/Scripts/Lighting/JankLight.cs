#region Usings
using Framework;
using MathBad;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
#endregion

public class JankLight : MonoBehaviour
{
    [SerializeField] Color _color;
    [SerializeField] float _flareAlpha = 3f;

    Light _light;
    SpriteRenderer _flare;

    void Awake()
    {
        _light = GetComponent<Light>();
        _flare = transform.FindInChildren("Flare").GetComponent<SpriteRenderer>();
        _light.color = _color;
        _flare.color = _color.WithA(_flareAlpha / 100f);
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if(_light == null) _light = GetComponent<Light>();
        if(_flare == null) _flare = transform.FindInChildren("Flare").GetComponent<SpriteRenderer>();
    }

    public void GetLightColor()
    {
        _color = _light.color;
        SetColors();
    }

    public void SetColors()
    {
        OnValidate();

        _light.color = _color;
        _flare.color = _color.WithA(_flareAlpha / 100f);
        EditorUtility.SetDirty(_light);
        EditorUtility.SetDirty(_flare);
    }
#endif
}
