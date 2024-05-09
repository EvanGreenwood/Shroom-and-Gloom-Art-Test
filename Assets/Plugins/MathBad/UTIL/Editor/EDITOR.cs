#region
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;
#endregion

namespace MathBad_Editor
{
public enum UnityEditorWindowType { None = 0, ProjectBrowser, InspectorWindow, SceneView, SceneHierarchyWindow, ConsoleWindow, GameView, }
public static class EDITOR
{
    // Event
    //----------------------------------------------------------------------------------------------------
    public static Event evnt => Event.current;
    public static float GetLabelWidth(string label, float padding = 5f) => GetLabelWidth(label, EditorStyles.label, padding);
    public static float GetLabelWidth(string label, GUIStyle style, float padding = 5f) => style.CalcSize(new GUIContent(label)).x + padding;

    // Asset Database
    //----------------------------------------------------------------------------------------------------
    public static void SaveAll()
    {
        EditorSceneManager.SaveOpenScenes();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Assets and all open scenes saved.");
    }

    public static void Recompile()
    {
        CompilationPipeline.RequestScriptCompilation();
    }

    public static void SaveAndRefreshAssets()
    {
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    // Assets
    //----------------------------------------------------------------------------------------------------
    public static List<T> LoadScriptableObjects<T>() where T : ScriptableObject
    {
        Type type = typeof(T);
        List<T> results = new List<T>();

        string[] guids = AssetDatabase.FindAssets($"t:{type.Name}");
        foreach(string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            T obj = AssetDatabase.LoadAssetAtPath<T>(assetPath);

            // Check if obj is not null and of the correct derived type
            if((T)obj != null)
            {
                results.Add(obj);
            }
        }

        return results;
    }

    // Window
    //----------------------------------------------------------------------------------------------------
    public static EditorWindow GetEditorWindow(UnityEditorWindowType type)
    {
        string windowName = $"UnityEditor.{type.ToString()}";
        return EditorWindow.GetWindow(typeof(EditorWindow).Assembly.GetType(windowName));
    }

    public static bool IsMouseOverEditorWindow(UnityEditorWindowType type)
    {
        if(type == UnityEditorWindowType.None)
            return false;
        if(EditorWindow.mouseOverWindow == null)
            return false;
        return EditorWindow.mouseOverWindow.ToString().Contains($"UnityEditor.{type.ToString()}");
    }

    // Selection
    //----------------------------------------------------------------------------------------------------
    public static string GetSelectedAssetPath()
    {
        string path;
        Object obj = Selection.activeObject;

        if(obj == null) path = "Assets";
        else path = AssetDatabase.GetAssetPath(obj.GetInstanceID());

        if(path.Length > 0)
        {
            if(Directory.Exists(path))
                return path;
            return PATH.GetPathOnly(path);
        }

        return "Assets";
    }

    public static bool TryGetSelectedAsset<T>(out T result) where T : Object
    {
        if(Selection.activeObject is T selectedAsset)
        {
            string objectPath = AssetDatabase.GetAssetPath(selectedAsset);
            if(!string.IsNullOrEmpty(objectPath) && File.Exists(objectPath))
            {
                result = selectedAsset;
                return true;
            }
        }

        result = null;
        return false;
    }

    public static List<Object> GetProjectSelection()
    {
        List<Object> results = new List<Object>();
        foreach(Object obj in Selection.objects)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            if(!string.IsNullOrEmpty(path))
            {
                results.Add(obj);
            }
        }
        return results;
    }
    public static List<GameObject> GetHeirarchySelection()
    {
        List<GameObject> results = new List<GameObject>(Selection.gameObjects);
        return results;
    }
}
}
