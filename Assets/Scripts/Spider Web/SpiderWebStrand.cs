using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderWebStrand : MonoBehaviour
{
    [SerializeField] private Transform _startDummy;
    [SerializeField] private Transform _endDummy;

    [SerializeField] private int _spriteMaterialID;
    [SerializeField] private Texture _webTexture;
    private LineRenderer _lineRenderer;
    //
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();

           MaterialPropertyBlock theSprite   = new MaterialPropertyBlock();
        Material spriteMaterial = _lineRenderer.material;

        _lineRenderer.GetPropertyBlock(theSprite, _spriteMaterialID);

      int   spriteMainTextureID = Shader.PropertyToID("_MainTex");

        theSprite.SetTexture(spriteMainTextureID, _webTexture);

        _lineRenderer.SetPropertyBlock(theSprite, _spriteMaterialID);
        // _lineRenderer.SetPropertyBlock(
    }
    public void Setup(Transform start, Transform end)
    {
        _startDummy = start;
        _endDummy = end;
    }

    // Update is called once per frame
    void Update()
    {
        SetPositions();
    }
    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            SetPositions();
        }
    }
    private void SetPositions()
    {
        if (_lineRenderer == null) _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.SetPosition(0, _startDummy.position);
        _lineRenderer.SetPosition(1, _endDummy.position);
    }
}
