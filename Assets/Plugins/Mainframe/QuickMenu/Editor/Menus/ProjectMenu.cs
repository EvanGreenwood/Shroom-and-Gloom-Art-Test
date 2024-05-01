#region
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Mainframe;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
#endregion

namespace Mainframe_Editor
{
// Project Menu
//----------------------------------------------------------------------------------------------------
[InitializeOnLoad]
public static class ProjectMenu
{
  static ProjectMenu()
  {
    QuickMenu menu = new QuickMenu("Project Menu",
                                   OnInput,
                                   typeof(ProjectMenuItems),
                                   typeof(ScriptTemplates));
  }

  public static bool OnInput()
  {
    if(EDITOR.IsMouseOverEditorWindow(UnityEditorWindowType.ProjectBrowser))
    {
      if(Application.isPlaying)
        return false;

      return EditorInput.shift && EditorInput.keyDown(KeyCode.A);
    }
    return false;
  }
}

// Menu Items
//----------------------------------------------------------------------------------------------------
public static class ProjectMenuItems
{
  const int PRIORITY = 0;

  [MenuItem("Assets/Open In Explorer #f", false, 0)] static void OpenExplorerHotkey() {OpenExplorer();}
  // [MenuInvoke("Open In Explorer", PRIORITY - 5000)]
  static void OpenExplorer()
  {
    Object obj = Selection.activeObject;
    if(obj == null)
    {
      Debug.Log($"{"GlobalMenu".Yellow()}: Nothing selected.");
      return;
    }
    string assetPath = AssetDatabase.GetAssetPath(obj);
    string filePath = Path.GetFullPath(assetPath);

    if(Directory.Exists(filePath))
    {
      Process.Start(new ProcessStartInfo
                    {
                      FileName = filePath,
                      UseShellExecute = true,
                      Verb = "open"
                    });
    }
    else
    {
      Process.Start("explorer.exe", string.Format("/select,\"{0}\"", filePath));
    }
  }

  // Create new assets
  //----------------------------------------------------------------------------------------------------
#region Assets
  [MenuInvoke("Folder", PRIORITY - 50, "")]
  static void CreateFolder()
  {
    ProjectWindowUtil.CreateFolder();
  }

  [MenuInvoke("Editor Folder", PRIORITY - 40)]
  static void CreateEditorFolder()
  {
    string path = EDITOR.GetSelectedAssetPath();
    string folderPath = AssetDatabase.GenerateUniqueAssetPath(path + "/Editor");
    if(!AssetDatabase.IsValidFolder(folderPath))
    {
      AssetDatabase.CreateFolder(path, "Editor");
      AssetDatabase.Refresh();
    }
  }

  [MenuInvoke("Prefab", PRIORITY - 35, "")]
  static void CreatePrefab()
  {
    string path = EDITOR.GetSelectedAssetPath();
    string name = PATH.GetUniqueAssetNameWithExtension(path, "New Prefab" + PATH.PREFAB);

    GameObject prefab = new GameObject(name);
    PrefabUtility.SaveAsPrefabAsset(prefab, PATH.Combine(path, name), out bool success);
    if(!success) { Debug.LogError($"Failed to create new GameObject prefab at {path}/{name}."); }

    Object.DestroyImmediate(prefab);
    EDITOR.SaveAndRefreshAssets();
  }
  [MenuInvoke("Scene", PRIORITY - 30, "")]
  static void CreateScene()
  {
    if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
    {
      string path = EDITOR.GetSelectedAssetPath();
      Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
      SaveAsset.Scene(scene, false, path, "New Scene");
      EDITOR.SaveAndRefreshAssets();
    }
  }

  [MenuInvoke("Material", PRIORITY)]
  static void CreateMaterial()
  {
    string shaderName;

    shaderName = RENDER.GetStandardShaderName();

    if(EDITOR.TryGetSelectedAsset(out Shader result))
    {
      shaderName = result.name;
    }

    string path = EDITOR.GetSelectedAssetPath();
    string fileName = "New Material";
    if(string.IsNullOrEmpty(path))
      path = "Assets";

    Material material = new Material(Shader.Find(shaderName));
    string unique = PATH.GetUniqueAssetNameWithExtension(path, fileName);
    ProjectWindowUtil.CreateAsset(material, unique + PATH.MAT);
    EDITOR.SaveAndRefreshAssets();
  }

  // [MenuInvoke("Texture Wizard", PRIORITY + 20, "")]
  // static void OpenTextureWizard()
  // {
  //   TextureWizard.Open();
  // }

  [MenuInvoke("Texture", PRIORITY + 30, "")]
  static void CreateTexture()
  {
    string path = EDITOR.GetSelectedAssetPath();
    Texture2D tex = TEX.Create(32, 32, FilterMode.Point);
    tex.Fill(RGB32.grey);
    SaveAsset.PNG(tex, false, path, "New Texture", TextureImporterType.Default, true, true);
  }

  [MenuInvoke("Sprite", PRIORITY + 40)]
  static void CreateSprite()
  {
    string path = EDITOR.GetSelectedAssetPath();
    Texture2D tex = TEX.Create(32, 32, FilterMode.Point);
    tex.Fill(RGB32.grey);
    SaveAsset.PNG(tex, false, path, "New Sprite", TextureImporterType.Sprite, true, true);
  }
#endregion

  // Duplicate Script
  //----------------------------------------------------------------------------------------------------//
  [MenuInvoke("Code/DuplicateScript", -2500)]
  static void DuplicateScript()
  {
    MonoScript source = Selection.activeObject as MonoScript;

    if(source == null)
      return;

    string sourcePath = AssetDatabase.GetAssetPath(source);
    string sourceName = Path.GetFileNameWithoutExtension(sourcePath);

    CreateAssetInCurrentFolder<ScriptableObject>(sourceName, ".cs", OnAssetCreated, OnAssetCanceled);

    void OnAssetCreated(ScriptableObject asset)
    {
      string scriptName = asset.name;
      string script = source.text.Replace(sourceName, scriptName);

      string path = AssetDatabase.GetAssetPath(asset);
      File.WriteAllText(path, script);
      AssetDatabase.Refresh();
    }

    void OnAssetCanceled() { }

    AssetDatabase.Refresh();
  }
  public static void CreateAssetInCurrentFolder<T>(string initialAssetName, string extension = PATH.ASSET,
                                                   Action<T> created = null, Action canceled = null) where T : ScriptableObject
  {
    // Process the asset name:
    if(string.IsNullOrWhiteSpace(initialAssetName))
      initialAssetName = "New" + ObjectNames.NicifyVariableName(typeof(T).Name);

    if(!initialAssetName.EndsWith(extension, StringComparison.InvariantCultureIgnoreCase))
      initialAssetName += extension;

    // Set up the end name edit action callback object:
    AssetCreatorEndNameEditAction endNameEditAction = ScriptableObject.CreateInstance<AssetCreatorEndNameEditAction>();

    endNameEditAction.canceledCallback = canceled;

    if(created != null)
      endNameEditAction.createdCallback = (instance) => created((T)instance);

    // Create the asset:
    T asset = ScriptableObject.CreateInstance<T>();
    ProjectWindowUtil.StartNameEditingIfProjectWindowExists(asset.GetInstanceID(), endNameEditAction, initialAssetName,
                                                            AssetPreview.GetMiniThumbnail(asset), null);
    Selection.objects = null;
  }

  internal class AssetCreatorEndNameEditAction : EndNameEditAction
  {
    public Action<Object> createdCallback;
    public Action canceledCallback;

    public override void Action(int instanceId, string pathName, string resourceFile)
    {
      Object asset = EditorUtility.InstanceIDToObject(instanceId);
      AssetDatabase.CreateAsset(asset, AssetDatabase.GenerateUniqueAssetPath(pathName));
      createdCallback?.Invoke(asset);
    }

    public override void Cancelled(int instanceId, string pathName, string resourceFile)
    {
      Selection.activeObject = null;
      canceledCallback?.Invoke();
    }
  }

  // Insert ASCII Header
  //----------------------------------------------------------------------------------------------------//
  [MenuInvoke("Code/Add Ascii Header", -2400)]
  static void AddScriptNameAsAsciiHeader()
  {
    List<MonoScript> scripts = new List<MonoScript>();
    foreach(Object obj in Selection.objects)
    {
      if(obj is MonoScript ms)
        scripts.Add(ms);
    }

    if(scripts.IsNullOrEmpty())
      return;

    foreach(MonoScript script in scripts)
    {
      string path = AssetDatabase.GetAssetPath(script);
      string name = PATH.GetFileNameWithoutExtension(path);

      string contents = script.text;
      string asciiHeader = AsciiUtil.ToAsciiCommentHeader(name);
      contents = contents.Insert(0, asciiHeader + "\n");

      File.WriteAllText(AssetDatabase.GetAssetPath(script), contents);
    }

    AssetDatabase.Refresh();
  }
}
}
