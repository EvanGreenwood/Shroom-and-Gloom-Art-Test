using System;
using UnityEngine;

[ExecuteInEditMode]
public class SceneViewShaderHelper : MonoBehaviour
{
#if UNITY_EDITOR
    public void OnEnable()
    {
        Camera.onPreRender += SetIfSceneViewCamera;
    }

    public void OnDisable()
    {
        Camera.onPreRender -= SetIfSceneViewCamera;
    }

    private void SetIfSceneViewCamera(Camera cam)
    {
        // Scene View camera is named "SceneCamera"
        if (cam.gameObject.name == "SceneCamera" || cam.gameObject.name == "Preview Scene Camera")
        {
            Shader.EnableKeyword("_SCENE_VIEW");
        }
        else
        {
            Shader.DisableKeyword("_SCENE_VIEW");
        }

        // You can double check the camera names if something breaks in the future
        // Debug.Log(cam);
    }

#endif

}
