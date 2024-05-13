using System.Collections;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using Unity.EditorCoroutines.Editor;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

/// <summary>
/// This script runs two compute shader kernels: GenerateSDF and GenerateNormalMap.
/// The first kernel generates a signed distance field (SDF) from the input texture, and the second kernel uses the SDF to generate a normal map texture.
/// </summary>
public class NormalsGenerator : EditorWindow
{
    private static ComputeShader computeShader;
    private static int sdfKernelHandle;
    private static bool isProcessing;

    private static string SDFKernel = "GenerateSDF";
    private static int WidthId = Shader.PropertyToID("_Width");
    private static int HeightId = Shader.PropertyToID("_Height");
    private static int ColorTexId = Shader.PropertyToID("_ColorTex");
    private static int SDFTexId = Shader.PropertyToID("_SDFTex");

    [MenuItem("Assets/S&G/Generate Normal Maps", false, 0)]
    private static void GenerateNormalMapsFromSelection()
    {
        string[] guids = AssetDatabase.FindAssets("NormalMapGenerator");
        Debug.Assert(guids.Length > 0, "[NormalsGenerator] NormalMapGenerator.compute is missing!");
        string computePath = AssetDatabase.GUIDToAssetPath(guids[0]);
        
        computeShader = AssetDatabase.LoadAssetAtPath<ComputeShader>(computePath);
        sdfKernelHandle = computeShader.FindKernel(SDFKernel);
        if (isProcessing)
        {
            Debug.LogError("Already generating normal maps");
            return;
        }
        
        isProcessing = true;
        Texture2D[] toProcess = Selection.GetFiltered<Texture2D>(SelectionMode.DeepAssets);
        EditorCoroutineUtility.StartCoroutineOwnerless(ProcessTextures(toProcess));
    }

    private static IEnumerator ProcessTextures(Texture2D[] toProcess)
    {
        List<Texture2D> processedTextures = new List<Texture2D>();
        List<string> processedPaths = new List<string>();

        SetReadWriteEnabled(toProcess, true);
        
        for (int i = 0; i < toProcess.Length; i++)
        {
            Texture2D texture = toProcess[i];
            string path = AssetDatabase.GetAssetPath(texture);
            string processedPath = Path.GetDirectoryName(path) + "/" + Path.GetFileNameWithoutExtension(path) + "_Normal.png";
            
            EditorUtility.DisplayProgressBar("Generating Normal Maps", $"{Path.GetFileNameWithoutExtension(path)}", 
                (float)(i + 1) / toProcess.Length);
            
            ComputeBuffer colorBuffer = new (texture.width * texture.height, sizeof(float) * 4);
            colorBuffer.SetData(texture.GetPixels());
            ComputeBuffer sdfBuffer = new (texture.width * texture.height, sizeof(float) * 4);
         
            computeShader.SetInt(WidthId, texture.width);
            computeShader.SetInt(HeightId, texture.height);
            
            
            computeShader.SetBuffer(sdfKernelHandle, ColorTexId, colorBuffer);
            computeShader.SetBuffer(sdfKernelHandle, SDFTexId, sdfBuffer);
            computeShader.Dispatch(sdfKernelHandle, Mathf.CeilToInt(texture.width / 8.0f),
                Mathf.CeilToInt(texture.height / 8.0f), 1);
            
            Texture2D processedTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBAFloat, false);

            var textureData = processedTexture.GetRawTextureData();
            sdfBuffer.GetData(textureData);
            processedTexture.SetPixelData(textureData, 0);
            processedTexture.Apply();
            
            //Writing texture
            byte[] bytes = processedTexture.EncodeToPNG();
            File.WriteAllBytes(processedPath, bytes);

            processedTextures.Add(processedTexture);
            processedPaths.Add(processedPath);
            
            // The graphics fence has passed and the compute shader has finished running.
            sdfBuffer.Dispose();
            //normalMapBuffer.Dispose();
            yield return null;
        }

        for (int i = 0; i < processedPaths.Count; i++)
        {
            string path = processedPaths[i];
            EditorUtility.DisplayProgressBar("Importing",
                $"{Path.GetFileNameWithoutExtension(path)}...", (float)(i + 1) / processedPaths.Count);
            AssetDatabase.ImportAsset(path);
            SetAsNormalMap(path);
        }
        
        SetNormalsAsSecondaryTexture(toProcess, processedPaths);
        SetReadWriteEnabled(toProcess, false);
        
        EditorUtility.ClearProgressBar();
        isProcessing = false;
    }
    
    private static void SetAsNormalMap(string path)
    {
        TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

        if (textureImporter != null)
        {
            // Enable read/write
            textureImporter.textureType = TextureImporterType.NormalMap;
            textureImporter.normalmapFilter = TextureImporterNormalFilter.Sobel;
            textureImporter.convertToNormalmap = true;
            textureImporter.mipMapsPreserveCoverage = true;
            textureImporter.mipmapEnabled = true;
            
            AssetDatabase.ImportAsset(path);
        }
        else
        {
            Debug.LogError("Failed to get TextureImporter for texture at path: " + path);
        }
    }
    
    private static void SetReadWriteEnabled(Texture2D[] textures, bool enabled)
    {
        List<string> paths = new List<string>();

        for (int i = 0; i < textures.Length; i++)
        {
            Texture2D tex = textures[i];

            // Load the texture from the project
            string path = AssetDatabase.GetAssetPath(tex);
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

            if (textureImporter != null)
            {
                // Enable read/write
                textureImporter.isReadable = enabled;
                paths.Add(path);
            }
            else
            {
                Debug.LogError("Failed to get TextureImporter for texture at path: " + path);
            }

            EditorUtility.DisplayProgressBar("Setting write flags",
                $"{Path.GetFileNameWithoutExtension(path)}...",
                (float)(i + 1) / paths.Count);
        }

        // Apply changes
        for (int i = 0; i < paths.Count; i++)
        {
            string path = paths[i];
            EditorUtility.DisplayProgressBar("Applying write flags",
                $"{Path.GetFileNameWithoutExtension(path)}...",
                (float)(i + 1) / paths.Count);
            AssetDatabase.ImportAsset(path);
        }
        AssetDatabase.Refresh();
    }
    
    private static void SetNormalsAsSecondaryTexture(Texture2D[] textures, List<string> normalPaths)
    {
        List<string> paths = new List<string>();
        Debug.Assert(textures.Length == normalPaths.Count);
        for (int i = 0; i < textures.Length; i++)
        {
            Texture2D tex = textures[i];

            // Load the texture from the project
            string path = AssetDatabase.GetAssetPath(tex);
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

            if (textureImporter != null)
            {
                // Enable read/write
                List<SecondarySpriteTexture> secondaryTextures= new List<SecondarySpriteTexture>(textureImporter.secondarySpriteTextures);

                Texture2D normalTex = AssetDatabase.LoadAssetAtPath<Texture2D>(normalPaths[i]);

                bool exists = false;
                for (int j = 0; j < secondaryTextures.Count; j++)
                {
                    SecondarySpriteTexture secondaryTex = secondaryTextures[j];

                    if (secondaryTex.name == "_Normal")
                    {
                        exists = true;
                        secondaryTex.texture = normalTex;
                    }

                    secondaryTextures[j] = secondaryTex;
                }

                if (!exists)
                {
                    secondaryTextures.Add(new SecondarySpriteTexture(){name = "_Normal", texture = normalTex });
                }

                textureImporter.secondarySpriteTextures = secondaryTextures.ToArray();
                
                paths.Add(path);
            }
            else
            {
                Debug.LogError("Failed to get TextureImporter for texture at path: " + path);
            }

            EditorUtility.DisplayProgressBar("Setting secondary normal texture",
                $"{Path.GetFileNameWithoutExtension(path)}...",
                (float)(i + 1) / paths.Count);
        }

        // Apply changes
        for (int i = 0; i < paths.Count; i++)
        {
            string path = paths[i];
            EditorUtility.DisplayProgressBar("Applying secondary normal textures",
                $"{Path.GetFileNameWithoutExtension(path)}...",
                (float)(i + 1) / paths.Count);
            AssetDatabase.ImportAsset(path);
        }
        AssetDatabase.Refresh();
    }
}