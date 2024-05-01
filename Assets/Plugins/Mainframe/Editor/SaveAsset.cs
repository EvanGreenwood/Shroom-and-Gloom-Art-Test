#region
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
#endregion

namespace Mainframe_Editor
{
public static class SaveAsset
{
  static string ProcessPath(string path, string name, string extension, bool overwriteExisting)
  {
    path = PATH.CleanAndValidatePath(path);

    string result;
    if(overwriteExisting) result = Path.Combine(path, name + extension);
    else
    {
      result = PATH.GetUniqueAssetNameWithExtension(path, name + extension);
      result = Path.Combine(path, result);
    }

    return result;
  }

  // Save Scene
  //----------------------------------------------------------------------------------------------------
  public static void Scene(Scene scene, bool overwriteExisting, string path, string name)
  {
    string filePath = ProcessPath(path, name, PATH.SCENE, overwriteExisting);
    EditorSceneManager.SaveScene(scene, filePath);
    EDITOR.SaveAndRefreshAssets();
  }

  // Texture2D to *.PNG
  //----------------------------------------------------------------------------------------------------
  public static void PNG(Texture2D texture, bool overwriteExisting,
                         string path, string name,
                         TextureImporterType importerFormat,
                         bool sRGB, bool alphaIsTransparency)
  {
    texture.Apply();

    string filePath = ProcessPath(path, name, PATH.PNG, overwriteExisting);

    byte[] bytes = texture.EncodeToPNG();
    File.WriteAllBytes(filePath, bytes);

    TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(filePath);
    if(importer)
    {
      importer.textureType = importerFormat;
      importer.textureCompression = TextureImporterCompression.Uncompressed;
      importer.crunchedCompression = false;
      importer.sRGBTexture = sRGB;

      importer.filterMode = texture.filterMode;
      importer.isReadable = true;
      importer.mipmapEnabled = false;
      importer.alphaIsTransparency = alphaIsTransparency;
      importer.compressionQuality = 0;

      importer.spriteImportMode = SpriteImportMode.Single;
      importer.spritePixelsPerUnit = 32;

      importer.wrapMode = texture.wrapMode;
      importer.npotScale = TextureImporterNPOTScale.None;
      TextureImporterPlatformSettings settings = new TextureImporterPlatformSettings();
      settings.format = TextureImporterFormat.RGBA32;

      importer.SetPlatformTextureSettings(settings);
      importer.SaveAndReimport();
    }

    EDITOR.SaveAndRefreshAssets();
  }
}
}
