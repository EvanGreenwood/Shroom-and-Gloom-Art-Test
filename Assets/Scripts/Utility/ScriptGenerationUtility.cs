using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ScriptGenerationUtility : MonoBehaviour
{
   public static string CreateVarName(string rawName)
   {
      string name = rawName.Replace(" ", string.Empty)
         .Replace("-", "_");
      return name;
   }
   public static void Generate(string defaultPath, string scriptName, List<string> lines, bool reload = true)
   {
#if UNITY_EDITOR
      string[] guids = AssetDatabase.FindAssets($"t:Script {scriptName}");
      string path = $"{defaultPath}/{scriptName}.cs";
      if(guids.Length > 0)
      {
         //reuse existing path.
         path = AssetDatabase.GUIDToAssetPath(guids[0]);
      }
   
      var scriptDirectory = Path.GetDirectoryName(path);
      PathUtility.CreateDirectoryIfNeeded(scriptDirectory);
      using (StreamWriter writer = new (new FileStream(path, FileMode.Create)))
      {
         foreach (var line in lines)
         {
            writer.WriteLine(line);
         }
      }

      if (reload)
      {
         AssetDatabase.ImportAsset(path);
      }
      
#endif
   }
}
