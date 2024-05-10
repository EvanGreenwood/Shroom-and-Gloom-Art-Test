#region
using Framework;
using MathBad;
using MathBad_Editor;
using UnityEditor;
using UnityEngine;
#endregion

namespace FrameworkEditor
{
[InitializeOnLoad]
public class SceneViewDebug
{
    static SceneViewDebug()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        SceneView.duringSceneGui += OnSceneGUI;
    }

    static void OnSceneGUI(SceneView sceneView)
    {
        Handles.BeginGUI();

        Vector2 buttonSize = new Vector2(75, 20);
        Vector2 padding = new Vector2(10f, 15f);
        Rect windowRect = sceneView.position.WithPosition(new Vector2(0f, -20));
        Rect rect = new Rect(windowRect.max - buttonSize - padding, buttonSize);
        rect.Draw(RGB.black);
        
        if(GUI.Button(rect, "RefreshAll"))
        {
            TunnelGenerator[] generators = Object.FindObjectsOfType<TunnelGenerator>();
            generators.Foreach(generator => generator.Generate());
        }
        if(GUI.Button(rect.MoveLeft(buttonSize.x + padding.x), "ValidatePPV"))
        {
            SceneData sceneData = Object.FindObjectOfType<SceneData>();
            Player.inst.SetPostProcessingProfile(sceneData.postProcessProfile);
        }
        
        Handles.EndGUI();
    }
}
}
