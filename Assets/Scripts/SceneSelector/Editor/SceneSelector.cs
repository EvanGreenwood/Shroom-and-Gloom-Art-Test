using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityToolbarExtender;

[InitializeOnLoad]
public class SceneSelector
{
    public const string EDITOR_LOAD_SCENE_KEY = "START_SCENE_NAME";
    public const string DEFAULT_SCENE = "NONE";

    static SceneSelector()
    {
        EditorCoroutineUtility.StartCoroutineOwnerless(AddAfterDelay());
        IEnumerator AddAfterDelay()
        {
            yield return null;
            ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
        }
    }

    private static void OnToolbarGUI()
    {
        GUILayout.FlexibleSpace();

        if (Application.isPlaying) { }
        else //editor
        {
            List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>();
            List<string> scenesNames = new List<string>();
            foreach(EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled)
                {
                    scenes.Add(scene);
                    scenesNames.Add(Path.GetFileName(scene.path).Replace(".unity", ""));
                }
            }
            if (scenes.Count == 0)
            {
                EditorGUILayout.LabelField("No scenes in build settings");
            }
            
            string currentSceneName = EditorPrefs.GetString(EDITOR_LOAD_SCENE_KEY, DEFAULT_SCENE);

            int currentSceneIndex = 0;
            for (int i = 0; i < scenes.Count; i++)
            {
                if (currentSceneName == scenesNames[i])
                {
                    currentSceneIndex = i;
                }
            }

            EditorGUI.BeginDisabledGroup(Application.isPlaying);

            int chosenIndex = EditorGUILayout.Popup(currentSceneIndex,
                scenesNames.ToArray(),
                GUILayout.ExpandHeight(true));
            EditorGUI.EndDisabledGroup();

            EditorPrefs.SetString(EDITOR_LOAD_SCENE_KEY, scenesNames[chosenIndex]);
            if (chosenIndex != currentSceneIndex)
            {
                EditorSceneManager.OpenScene(scenes[chosenIndex].path);
            }
        }
    }
}