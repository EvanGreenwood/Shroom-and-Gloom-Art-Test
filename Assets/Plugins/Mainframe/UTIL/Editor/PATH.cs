#region
using System.Collections.Generic;
using System.IO;
using Mainframe;
using UnityEditor;
using UnityEngine;
#endregion

namespace Mainframe_Editor
{
public static class PATH
{
  public const string ASSET = ".asset", PREFAB = ".prefab", SCENE = ".unity", PNG = ".png", MAT = ".mat";
  public const string CS = ".cs", COMPUTE = ".compute", SHADER = ".shader", CGINC = ".cginc", TXT = ".txt";
  public static readonly string DataPath = Application.dataPath.CleanPath();

  public static string GetAssetsPath(this Object obj) => AssetDatabase.GetAssetPath(obj);

  public static bool IsDirectory(Object asset)
  {
    string assetPath = AssetDatabase.GetAssetPath(asset);
    if(AssetDatabase.IsValidFolder(assetPath))
      return true;
    return false;
  }

  public static string CleanPath(string path) => path.Replace('\\', '/').Trim('/');

  public static string CleanAndValidatePath(string path)
  {
    path = CleanPath(path);
    if(!Directory.Exists(path))
      Directory.CreateDirectory(path);
    return path;
  }

  public static string Combine(string a, string b, params string[] c)
  {
    string result = Path.Combine(a, b);
    for(int i = 0; i < c.Length; i++)
    {
      result = Path.Combine(result, c[i]);
    }

    return result.CleanPath();
  }

  // Get Path
  //----------------------------------------------------------------------------------------------------
  public static string GetFullPath(string path) => Path.GetFullPath(path);
  public static string GetPathOnly(string fullPath) => Path.GetDirectoryName(fullPath);

  public static string GetFileNameWithExtension(string fullPath) => Path.GetFileName(fullPath);
  public static string GetFileNameWithoutExtension(string fullPath) => Path.GetFileNameWithoutExtension(fullPath);

  public static string GetUniqueAssetNameWithExtension(string path, string fileNameWithExtension)
  {
    string fullPath = Path.Combine(path, fileNameWithExtension);
    string uniquePath = AssetDatabase.GenerateUniqueAssetPath(fullPath);

    string name = GetFileNameWithExtension(uniquePath);
    return name;
  }

  public static string ToAssetsPath(string path)
  {
    string processed = path.CleanPath();

    if(!processed.StartsWith(DataPath))
    {
      Debug.LogError($"{processed} does not lead to the project.");
      return null;
    }

    return $"Assets{processed.Substring(DataPath.Length)}";
  }

  public static string ToResourcesPath(string path)
  {
    path = CleanPath(path);

    int index = path.IndexOf("/Resources/");
    if(index == -1)
    {
      return string.Empty;
    }

    string resourcePath = path.Substring(index + "/Resources/".Length);

    int extensionIndex = resourcePath.LastIndexOf('.');
    if(extensionIndex != -1)
    {
      resourcePath = resourcePath.Substring(0, extensionIndex);
    }

    return resourcePath;
  }

  public static List<Object> GetFoldersAtPath(string fullPath)
  {
    string[] folderPaths = Directory.GetDirectories(fullPath);

    List<Object> folders = new List<Object>();
    for(int i = 0; i < folderPaths.Length; i++)
    {
      string assetsPath = ToAssetsPath(folderPaths[i]);
      Object folder = AssetDatabase.LoadAssetAtPath<Object>(assetsPath);
      folders.Add(folder);
    }
    return folders;
  }

  /// <summary>
  /// Renames a file within the Unity project.
  /// </summary>
  /// <param name="filePath">The relative path to the file to rename, starting from the Assets folder.</param>
  /// <param name="newFileName">The new name for the file, including extension.</param>
  public static void RenameFile(string filePath, string newFileName)
  {
    if(!filePath.StartsWith("Assets/"))
    {
      Debug.LogError("The old file path must start with 'Assets/'.");
      return;
    }

    string directoryPath = Path.GetDirectoryName(filePath);
    string newFilePath = Combine(directoryPath, newFileName);
    string error = AssetDatabase.RenameAsset(filePath, newFileName);
    if(!string.IsNullOrEmpty(error))
    {
      Debug.LogError("Error renaming file: " + error);
    }
  }

  /// <summary>
  /// Gets the asset path of the currently selected object in the editor.
  /// </summary>
  /// <returns>The asset path of the selected object or "Assets" if nothing is selected.</returns>
  public static string GetActiveObjectPath()
  {
    Object obj = Selection.activeObject;
    string path;

    path = obj == null ? "Assets" : AssetDatabase.GetAssetPath(obj.GetInstanceID());

    if(path.Length == 0)
      return string.Empty;

    if(File.Exists(path))
      path = GetPathOnly(path);

    return CleanPath(path);
  }

  /// <summary>
  /// Get the path of the folder containing the specified assembly definition file.
  /// </summary>
  /// <param name="asmdefName">The name of the assembly definition file (without extension).</param>
  /// <returns>The directory path containing the asmdef file or null if not found.</returns>
  public static string GetAssemblyDefinitionPath(string asmdefName)
  {
    string[] guids = AssetDatabase.FindAssets(asmdefName + " t:asmdef");

    if(guids.Length > 0)
    {
      string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
      string directoryPath = Path.GetDirectoryName(assetPath);

      return directoryPath;
    }
    else
    {
      Debug.LogError("Asmdef file not found.");
      return null;
    }
  }
}
}
