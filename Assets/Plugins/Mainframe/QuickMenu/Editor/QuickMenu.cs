#region
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Mainframe;
using UnityEditor;
using UnityEngine;
#endregion

namespace Mainframe_Editor
{
public class QuickMenu
{
  readonly Type[] _types;

  GenericMenu _menu;

  List<CreateAssetData> _createAssetDatas;
  List<MenuObject> _menuObjects;
  Func<bool> _inputFunc;

  public QuickMenu(string name, Func<bool> inputFunc, params Type[] types)
  {
    _types = types;

    _menu = new GenericMenu();
    // _menu.AddDisabledItem(new GUIContent(name));

    _createAssetDatas = new List<CreateAssetData>();
    _menuObjects = new List<MenuObject>();

    _inputFunc = inputFunc ?? throw new ArgumentNullException($"{nameof(inputFunc)} cannot be null.");

    ProcessCreatable();
    ProcessInvokable();

    PopulateMenu();

    EditorInput.onInput += OnEditorInput;
  }

  // Dispose
  //------------------------------------------------------------------------------------------------------
  public void Dispose() {EditorInput.onInput -= OnEditorInput;}

  // Input callback
  //----------------------------------------------------------------------------------------------------
  void OnEditorInput()
  {
    if(Application.isPlaying)
      return;

    if(_inputFunc())
    {
      EDITOR.evnt.Use();
      ShowMenu();
    }
  }

  // Show menu
  //------------------------------------------------------------------------------------------------------
  void ShowMenu()
  {
    _menu.ShowAsContext();
  }

  // Process Types with MenuCreate attributes
  //----------------------------------------------------------------------------------------------------
  void ProcessCreatable()
  {
    _types.Foreach(type =>
    {
      Attribute[] attributes = Attribute.GetCustomAttributes(type, typeof(MenuCreateAttribute));
      attributes.Foreach(attribute =>
      {
        if(attribute is MenuCreateAttribute menuCreate)
        {
          _createAssetDatas.Add(new CreateAssetData(type, menuCreate));
          _menuObjects.Add(new MenuObject(menuCreate, OnCreateAssetClicked, _createAssetDatas.Last()));
        }
      });
    });
  }

  // Process Types with MenuInvoke attributes
  //----------------------------------------------------------------------------------------------------
  void ProcessInvokable()
  {
    BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.NonPublic;
    List<MethodInfo> methodTypes = REFLECTION.GetMethodsWithAttribute<MenuInvokeAttribute>(bindingFlags, _types);

    if(methodTypes.IsNullOrEmpty())
      return;

    methodTypes.Foreach(method =>
    {
      MenuInvokeAttribute invokeAttribute = method.GetCustomAttribute<MenuInvokeAttribute>();
      if(invokeAttribute != null)
      {
        _menuObjects.Add(new MenuObject(invokeAttribute, () => REFLECTION.InvokeMethod(method)));
      }
    });
  }

  // Populate menu
  //----------------------------------------------------------------------------------------------------
  void PopulateMenu()
  {
    if(_menuObjects.IsNullOrEmpty())
      return;

    List<MenuObject> sorted = GetMenuObjectsSorted();

    sorted.Foreach(menuObject =>
    {
      string fullPath = PATH.CleanPath(menuObject.attribute.menuPath);
      string seperator = menuObject.attribute.separator;

      // MenuInvokeAttribute
      if(menuObject.function1 != null)
      {
        if(seperator != null)
        {
          AddSeperator(fullPath, seperator);
        }
        _menu.AddItem(new GUIContent(fullPath), false, menuObject.function1);
      }
      // MenuCreateAttribute
      else if(menuObject.function2 != null)
      {
        _menu.AddItem(new GUIContent(fullPath), false, menuObject.function2, menuObject.userData);
      }
    });

    return;
    //--------------------------------------------------
    void AddSeperator(string fullPath, string seperator)
    {
      string path = string.Empty;
      if(fullPath.Contains('/'))
      {
        int index = fullPath.LastIndexOf('/');
        path = fullPath.Substring(0, index);
      }
      // NOTE: Don't use Path.Combine, the seperator can be empty or white space.
      string result = path.IsEmpty() ? seperator : path + '/' + seperator;
      GUIContent content = new GUIContent(result);
      _menu.AddDisabledItem(content, false);
    }
  }

  // Sort menu objects
  //------------------------------------------------------------------------------------------------------
  List<MenuObject> GetMenuObjectsSorted()
  {
    List<MenuObject> results = new List<MenuObject>();
    results.AddRange(_menuObjects);
    results.Sort((a, b) =>
    {
      int idComparison = (b.attribute.menuPath != null).CompareTo(a.attribute.menuPath != null);
      if(idComparison != 0)
      {
        return idComparison;
      }
      return a.attribute.priority.CompareTo(b.attribute.priority);
    });

    results.Sort((a, b) => a.attribute.priority.CompareTo(b.attribute.priority));
    return results;
  }

  // Create asset on menu item clicked
  //------------------------------------------------------------------------------------------------------
  void OnCreateAssetClicked(object item)
  {
    if(item is CreateAssetData menuObject)
    {
      string path = PATH.GetActiveObjectPath();

      string name = menuObject.attribute.menuPath;
      if(name.Contains('/'))
      {
        int index = name.LastIndexOf('/');
        name = name.Substring(index);
      }

      if(!Directory.Exists(path))
        Directory.CreateDirectory(path);

      ScriptableObject asset = ScriptableObject.CreateInstance(menuObject.type);
      string filePath = PATH.Combine(path, name + PATH.ASSET);
      ProjectWindowUtil.CreateAsset(asset, filePath);

      AssetDatabase.SaveAssets();

      EditorUtility.FocusProjectWindow();
      Selection.activeObject = asset;
    }
  }

  // Data
  //----------------------------------------------------------------------------------------------------
  public struct MenuObject
  {
    public IMenuAttribute attribute;
    public GenericMenu.MenuFunction function1;

    public GenericMenu.MenuFunction2 function2;
    public object userData;

    public MenuObject(IMenuAttribute attribute, GenericMenu.MenuFunction function1)
    {
      this.attribute = attribute;
      this.function1 = function1;
      function2 = null;
      userData = null;
    }

    public MenuObject(IMenuAttribute attribute, GenericMenu.MenuFunction2 function2, object userData)
    {
      this.attribute = attribute;
      this.function2 = function2;
      this.userData = userData;

      function1 = null;
    }
  }

  public struct CreateAssetData
  {
    public Type type;
    public MenuCreateAttribute attribute;
    public CreateAssetData(Type type, MenuCreateAttribute attribute)
    {
      this.type = type;
      this.attribute = attribute;
    }
  }
}
}
