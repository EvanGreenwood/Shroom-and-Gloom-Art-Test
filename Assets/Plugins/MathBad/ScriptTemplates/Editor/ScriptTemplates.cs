//  ___           _        _    _____                    _        _                                   
// / __| __  _ _ (_) _ __ | |_ |_   _| ___  _ __   _ __ | | __ _ | |_  ___  ___                       
// \__ \/ _|| '_|| || '_ \|  _|  | |  / -_)| '  \ | '_ \| |/ _` ||  _|/ -_)(_-<                       
// |___/\__||_|  |_|| .__/ \__|  |_|  \___||_|_|_|| .__/|_|\__,_| \__|\___|/__/                       
//                  |_|                           |_|                                                 
//----------------------------------------------------------------------------------------------------

#region Usings
using System;
using System.Collections.Generic;
using System.IO;
using AmplifyShaderEditor;
using MathBad;
using UnityEditor;
using UnityEngine;
#endregion

namespace MathBad_Editor
{
public class ScriptTemplates
{
  static Dictionary<string, TextAsset> _templates;
  const int PRIORITY = -100;
  const string CPU_MENU_PATH = "Code (CPU)/";
  const string GPU_MENU_PATH = "Code (GPU)/";

  // init
  //----------------------------------------------------------------------------------------------------
  static bool _hasInit;
  static void Init()
  {
    _templates = new Dictionary<string, TextAsset>();

    string path = "Framework/ScriptTemplates/";
    TextAsset[] templates = Resources.LoadAll<TextAsset>(path);
    if(templates.Length == 0)
      throw new NullReferenceException($"Can't find any script templates in {path}..");

    foreach(TextAsset asset in templates)
      _templates.Add(asset.name, asset);

    _hasInit = true;
  }

  // create file
  //----------------------------------------------------------------------------------------------------
  static void CreateFileFromTemplate(string key, string extension)
  {
    if(!_hasInit)
      Init();

    if(_templates.ContainsKey(key) == false)
      throw new FileNotFoundException($"Can't find a script template for {key}.");

    string fullPath = AssetDatabase.GetAssetPath(_templates[key]);
    ProjectWindowUtil.CreateScriptAssetFromTemplateFile(fullPath, $"New{key}{extension}");
  }

  //  _____                    _        _                                                               //
  // |_   _| ___  _ __   _ __ | | __ _ | |_  ___  ___                                                   //
  //   | |  / -_)| '  \ | '_ \| |/ _` ||  _|/ -_)(_-<                                                   //
  //   |_|  \___||_|_|_|| .__/|_|\__,_| \__|\___|/__/                                                   //
  //                    |_|                                                                             //
  //----------------------------------------------------------------------------------------------------
  // MonoBehaviour
  //----------------------------------------------------------------------------------------------------
  [MenuInvoke(CPU_MENU_PATH + "MonoBehaviour", PRIORITY - 100)] static void CreateMonoBehaviour() {CreateFileFromTemplate("MonoBehaviour", PATH.CS);}
  [MenuInvoke(CPU_MENU_PATH + "MonoBehaviourUI", PRIORITY - 90)] static void CreateMonoBehaviourUI() {CreateFileFromTemplate("MonoBehaviourUI", PATH.CS);}

  // Singleton
  //----------------------------------------------------------------------------------------------------
  [MenuInvoke(CPU_MENU_PATH + "MonoSingleton", PRIORITY - 70, "")] static void CreateMonoSingleton() {CreateFileFromTemplate("MonoSingleton", PATH.CS);}
  [MenuInvoke(CPU_MENU_PATH + "MonoSingletonUI", PRIORITY - 65, "")] static void CreateMonoSingletonUI() {CreateFileFromTemplate("MonoSingletonUI", PATH.CS);}
  [MenuInvoke(CPU_MENU_PATH + "CanvasSingleton", PRIORITY - 60)] static void CreateCanvasSingleton() {CreateFileFromTemplate("CanvasSingleton", PATH.CS);}
  [MenuInvoke(CPU_MENU_PATH + "ScriptableSingleton", PRIORITY - 50)] static void CreateScriptableSingleton() {CreateFileFromTemplate("ScriptableSingleton", PATH.CS);}

  // Data
  //----------------------------------------------------------------------------------------------------
  [MenuInvoke(CPU_MENU_PATH + "ScriptableObject", PRIORITY - 40, "")] static void CreateScriptableObject() {CreateFileFromTemplate("ScriptableObject", PATH.CS);}

  // Type
  //----------------------------------------------------------------------------------------------------
  [MenuInvoke(CPU_MENU_PATH + "Class", PRIORITY - 10)] static void CreateClass() {CreateFileFromTemplate("Class", PATH.CS);}

  // Editor
  //----------------------------------------------------------------------------------------------------
  [MenuInvoke(CPU_MENU_PATH + "PropertyDrawer", PRIORITY + 10, "")] static void CreatePropertyDrawer() {CreateFileFromTemplate("PropertyDrawer", PATH.CS);}
  [MenuInvoke(CPU_MENU_PATH + "Editor", PRIORITY + 20)] static void CreateEditor() {CreateFileFromTemplate("Editor", PATH.CS);}
  [MenuInvoke(CPU_MENU_PATH + "EditorWindow", PRIORITY + 30)] static void CreateEditorWindow() {CreateFileFromTemplate("EditorWindow", PATH.CS);}

  // Shader
  //----------------------------------------------------------------------------------------------------
  [MenuInvoke(GPU_MENU_PATH + "FragmentShader", PRIORITY + 40)] static void CreateShaderUnlit() {CreateFileFromTemplate("FragmentShader", PATH.SHADER);}
  [MenuInvoke(GPU_MENU_PATH + "ShaderInclude", PRIORITY + 50)] static void CreateShaderInclude() {CreateFileFromTemplate("ShaderInclude", PATH.CGINC);}
  [MenuInvoke(GPU_MENU_PATH + "ComputeDispatch", PRIORITY + 60, "")] static void CreateComputeDispatch() {CreateFileFromTemplate("ComputeDispatch", PATH.CS);}
  [MenuInvoke(GPU_MENU_PATH + "ComputeShader", PRIORITY + 70)] static void CreateComputeShader() {CreateFileFromTemplate("ComputeShader", PATH.COMPUTE);}

#if AMPLIFY_SHADER_EDITOR
  [MenuInvoke(GPU_MENU_PATH + "AmplifyShader", PRIORITY + 80, "")]
  static void NewAmplifyShader()
  {
    var endNameEditAction = ScriptableObject.CreateInstance<DoCreateStandardShader>();
    ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, endNameEditAction,
                                                            "NewAmplifyShader.shader" /*assetPathAndName*/,
                                                            AssetPreview.GetMiniTypeThumbnail(typeof(Shader)), null);
  }

  [MenuInvoke(GPU_MENU_PATH + "AmplifyShaderFunction", PRIORITY + 81)]
  static void NewAmplifyShaderFunction()
  {
    AmplifyShaderFunction asset = ScriptableObject.CreateInstance<AmplifyShaderFunction>();

    var endNameEditAction = ScriptableObject.CreateInstance<DoCreateFunction>();
    ProjectWindowUtil.StartNameEditingIfProjectWindowExists(asset.GetInstanceID(), endNameEditAction,
                                                            "NewShaderFunction.asset" /*assetPathAndName*/,
                                                            AssetPreview.GetMiniThumbnail(asset), null);
  }
#endif

  // Misc
  //----------------------------------------------------------------------------------------------------
  [MenuInvoke(CPU_MENU_PATH + "TextFile", PRIORITY + 80, "")] static void CreateTxt() {CreateFileFromTemplate("Text", PATH.TXT);}
}
}
