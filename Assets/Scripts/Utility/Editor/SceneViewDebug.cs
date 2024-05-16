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
        if(Application.isPlaying)
            return;
        
        Handles.BeginGUI();

        Vector2 buttonSize = new Vector2(75, 20);
        Vector2 padding = new Vector2(10f, 15f);
        Rect windowRect = sceneView.position.WithPosition(new Vector2(0f, -20));
        Rect rect = new Rect(windowRect.max - buttonSize - padding, buttonSize);
        rect.Draw(RGB.black);

        if(GUI.Button(rect, "Regenerate"))
        {
            TunnelGenerator[] generators = Object.FindObjectsOfType<TunnelGenerator>();
            generators.Foreach(generator => generator.Generate());
        }

        rect = rect.WithWidth(rect.width + 15f);
        if(GUI.Button(rect.MoveLeft(rect.width + padding.x), "Update PPV"))
        {
            SceneData sceneData = Object.FindObjectOfType<SceneData>();
            if(ServiceLocator.Has<Player>())
            {
                ServiceLocator.Get<Player>().SetScenePostProcessProfile(sceneData.postProcessProfile);
            }
            else
            {
                Debug.LogWarning("[SceneViewDebug] Player not ready.");
            }
          
        }

        Handles.EndGUI();
    }
}
}
