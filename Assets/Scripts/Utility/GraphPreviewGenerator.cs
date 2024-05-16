using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
#if UNITY_EDITOR
using Unity.EditorCoroutines.Editor;
using Unity.VisualScripting;
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Networking;

public class GraphPreviewGenerator
{
    public static void RequestGraphAsync(Action<bool, string, Texture2D> onComplete, string graphName, List<(string, string)> connections, List<(string, List<(string, string)>)> subStates = null)
    {
        StringBuilder codeBuilder = new("%%{init: {'theme':'dark'}}%%\nstateDiagram-v2\n");
        //codeBuilder.Append($"    direction LR\n");
        foreach ((string from, string to) in connections)
        {
            codeBuilder.Append($"    {from}-->{to}\n");
        }

        if (subStates != null)
        {
            foreach ((string from, List<(string,string)> subConnections) in subStates)
            {
                codeBuilder.Append("    state "+from+"{\n");
                foreach ((string subFrom, string subTo)  in subConnections)
                {
                    codeBuilder.Append($"        {subFrom}-->{subTo}\n");
                }
                codeBuilder.Append("    }\n");
            }
        }
        
        string url = CreateMermaidLiveUrl(codeBuilder.ToString());
        DownloadAndSaveImage(onComplete, graphName, url);
    }

    public static bool FileExists(string graphName)
    {
        string path = Path.Combine(Path.Combine(Application.temporaryCachePath, "GraphPreviews"), graphName+".png");
        return File.Exists(path);
    }

    private static void DownloadAndSaveImage(Action<bool, string, Texture2D> onComplete, string graphName, string url)
    {
        #if UNITY_EDITOR
        
        Debug.Assert(!graphName.IsNullOrEmpty());
        
        EditorCoroutineUtility.StartCoroutineOwnerless(GetAndDisplayImage());

        IEnumerator GetAndDisplayImage()
        {
            Task<Texture2D> getTexTask = GetRemoteTexture(url);

            while (!getTexTask.IsCompleted && !getTexTask.IsCanceled && !getTexTask.IsFaulted)
            {
                yield return null;
            }

            if (getTexTask.IsCompletedSuccessfully)
            {
                Texture2D result = getTexTask.Result;
                try
                {
                    string path = GetPath(graphName);
                    if (FileExists(graphName))
                    {
                        File.Delete(path);
                    }
                    string dir = Path.GetDirectoryName(path) ?? string.Empty;

                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                  
                    File.WriteAllBytes(path, result.EncodeToPNG());
                    onComplete?.Invoke(true, path, result);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);

                    throw;
                }
            }
            else
            {
                Debug.LogError("Error showing graph");
            }
            
            #endif
            
            onComplete?.Invoke(false, string.Empty, null);
        }

        async Task<Texture2D> GetRemoteTexture(string url)
        {
            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
            {
                // begin request:
                UnityWebRequestAsyncOperation asyncOp = www.SendWebRequest();

                // await until it's done: 
                while (asyncOp.isDone == false)
                {
                    await Task.Delay(1000 / 30); //30 hertz
                }

                // read results:
                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log($"{www.error}, URL:{www.url}");

                    return null;
                }

                // return valid results:
                return DownloadHandlerTexture.GetContent(www);
            }
        }
    }
    
    public static string CreateMermaidLiveUrl(string mermaidCode)
    {
        byte[] mermaidBytes = Encoding.UTF8.GetBytes(mermaidCode);
        string base64Encoded = Convert.ToBase64String(mermaidBytes);

        return "https://mermaid.ink/img/" + base64Encoded + "?bgColor=383838";
    }

    public static string GetPath(string graphName)
    {
        return Path.Combine(Path.Combine(Application.temporaryCachePath, "GraphPreviews"), graphName + ".png");
    }
}
