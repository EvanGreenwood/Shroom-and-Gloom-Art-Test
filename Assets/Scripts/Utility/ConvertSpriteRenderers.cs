#region Usings
using System.Collections;
using UnityEngine;
using Framework;
using MathBad;
using NaughtyAttributes;
#endregion

public class ConvertSpriteRenderers : MonoBehaviour
{
    [SerializeField] bool _disabled;
    // MonoBehaviour
    //----------------------------------------------------------------------------------------------------
    IEnumerator Start()
    {
        if(_disabled)
            yield break;
        
        yield return WAIT.ForSeconds(1f);
        SpriteRenderer[] srs = FindObjectsOfType<SpriteRenderer>();
        for(int i = srs.Length - 1; i >= 0; i--)
        {
            SpriteRenderer sr = srs[i];
            if(sr == null || !sr.gameObject.activeInHierarchy)
                continue;

            Material mat = new Material(Shader.Find("SpriteToMesh"));
            mat.SetTexture("_MainTex", sr.sprite.texture);
            mat.SetColor("_Color", sr.color);

            Mesh mesh = sr.sprite.GetSpriteMesh();
            GameObject go = sr.gameObject;
            if(sr.flipX) { go.transform.localScale = Vector3.Scale(go.transform.localScale, new Vector3(-1f, 1f, 1f)); }
            if(sr.flipY) { go.transform.localScale = Vector3.Scale(go.transform.localScale, new Vector3(1f, -1f, 1f)); }

            Destroy(sr);

            yield return null;

            MeshFilter mf = go.AddComponent<MeshFilter>();
            mf.mesh = mesh;

            MeshRenderer mr = go.AddComponent<MeshRenderer>();
            mr.material = mat;
        }
    }
}
