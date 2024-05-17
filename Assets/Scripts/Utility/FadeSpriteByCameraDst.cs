#region Usings
using Framework;
using UnityEngine;
using MathBad;
#endregion
[RequireComponent(typeof(SpriteRenderer))]
public class FadeSpriteByCameraDst : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private float _fadeRange = 8f;
    private float _startAlpha;

    // MonoBehaviour
    //----------------------------------------------------------------------------------------------------
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _startAlpha = _spriteRenderer.color.a;
    }
    private void Update()
    {
        if(CAMERA.main == null)
            return;

        float cameraDst = (CAMERA.main.transform.position - transform.position).magnitude;
        float fadeFactor = Mathf.InverseLerp(0f, _fadeRange, cameraDst);

        _spriteRenderer.color = _spriteRenderer.color.WithA(_startAlpha * fadeFactor);
    }
}
