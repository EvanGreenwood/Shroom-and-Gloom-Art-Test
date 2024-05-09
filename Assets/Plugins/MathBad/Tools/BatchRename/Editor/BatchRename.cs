#region
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MathBad;
using UnityEditor;
using UnityEngine;
using static MathBad_Editor.EDITOR_HELP;
using Object = UnityEngine.Object;
#endregion

namespace MathBad_Editor
{
public class BatchRename : EditorWindow
{
  const string WINDOW_TITLE = "BatchRename";
  const string MENU_ITEM = "Tools/" + WINDOW_TITLE;

  List<Object> _assets;
  List<Object> _gameObjects;
  int _numObjects;

  bool _override;
  bool _replace;
  bool _number;
  string _overrideInput;
  [SerializeField]
  List<StringPair> _replaceInputs;
  string _preffixInput, _suffixInput;
  [Serializable]
  class StringPair
  {
    public string a;
    public string b;
    public StringPair(string a, string b)
    {
      this.a = a;
      this.b = b;
    }
  }
  void OnEnable()
  {
    _assets = new List<Object>();
    _gameObjects = new List<Object>();
  }

  // Open Window
  //----------------------------------------------------------------------------------------------------
  [MenuItem(MENU_ITEM + " #r")]
  public static void OpenWindow()
  {
    BatchRename window = GetWindow<BatchRename>(false, WINDOW_TITLE, true);
    window.Show();
  }

  // GUI
  //----------------------------------------------------------------------------------------------------
  void OnGUI()
  {
    StepSelection();
    DrawHeader();

    if(_numObjects == 0)
      return;

    DrawSettings();

    return;
    //--------------------------------------------------
    void StepSelection()
    {
      _assets.Clear();
      _gameObjects.Clear();

      _assets.AddRange(EDITOR.GetProjectSelection());
      _gameObjects.AddRange(EDITOR.GetHeirarchySelection());
      _numObjects = _assets.Count + _gameObjects.Count;
    }
    void DrawHeader()
    {
      Color last = GUI.color;
      GUI.color = RGB.yellow;
      if(_assets.IsPopulated())
        DrawTitle($"Selected {_assets.Count} assets in the project.", RGB.black);
      if(_gameObjects.IsPopulated())
        DrawTitle($"Selected {_gameObjects.Count} game objects in the scene.", RGB.black);
      if(_numObjects == 0)
        DrawTitle($"Nothing selected.", RGB.black);

      GUI.color = last;
    }
  }

  // Draw Settings
  //----------------------------------------------------------------------------------------------------
  void DrawSettings()
  {
    LabelWidth(50, () =>
    {
      _preffixInput = EditorGUILayout.TextField("Preffix", _preffixInput);
      _suffixInput = EditorGUILayout.TextField("Suffix", _suffixInput);
      _number = EditorGUILayout.Toggle("Number", _number);
    });

    DisabledGroup(_replace, () =>
    {
      DrawToggleTitle("Override", ref _override, () =>
      {
        _overrideInput = EditorGUILayout.TextField(_overrideInput);
      });
    });
    DisabledGroup(_override, () =>
    {
      if(_replaceInputs.IsNullOrEmpty()) _replaceInputs = new List<StringPair>();
      DrawToggleTitle("Replace", ref _replace, () =>
      {
        for(int i = 0; i < _replaceInputs.Count; i++)
        {
          int index = i;
          Row(() =>
          {
            _replaceInputs[index].a = EditorGUILayout.TextField(_replaceInputs[index].a);
            _replaceInputs[index].b = EditorGUILayout.TextField(_replaceInputs[index].b);

            if(GUILayout.Button("x", GUILayout.Width(20)))
              _replaceInputs.RemoveAt(index);
          });
        }
        Button("+", () => { _replaceInputs.Add(new StringPair("", "")); });
      });
    });

    Space(10);
    Button($"Rename ({_numObjects})", () =>
    {
      Rename(_gameObjects, (obj, newName) =>
      {
        obj.name = newName;
      });
      Rename(_assets, (obj, newName) =>
      {
        string path = AssetDatabase.GetAssetPath(obj);
        PATH.RenameFile(path, newName);
      });

      EDITOR.SaveAndRefreshAssets();
    });
  }

  // Rename Selection
  //----------------------------------------------------------------------------------------------------
  void Rename(List<Object> objs, Action<Object, string> onRename)
  {
    for(int i = 0; i < objs.Count; i++)
    {
      Object obj = objs[i];
      Undo.RecordObject(obj, "Batch Renaming Object");
      string result = obj.name;
      if(_number)
      {
        result = RemoveNumberSuffix(result);
      }
      if(_override)
      {
        result = _overrideInput;
      }
      else
      {
        if(_replace && _replaceInputs.IsPopulated())
        {
          for(int j = 0; j < _replaceInputs.Count; j++)
          {
            result = result.Replace(_replaceInputs[j].a, _replaceInputs[j].b);
          }
        }
      }
      if(!_preffixInput.IsNullOrEmpty())
      {
        result = result.Insert(0, _preffixInput);
      }
      if(!_suffixInput.IsNullOrEmpty())
      {
        result += _suffixInput;
      }
      if(_number)
      {
        result += "_" + i;
      }
      onRename(obj, result);
    }

    Undo.SetCurrentGroupName("Batch Renamed Objects");
  }

  /// <summary>
  /// Removes numeric suffixes from a string.
  /// </summary>
  /// <param name="input">The input string from which to remove the numeric suffix.</param>
  /// <returns>A string without the numeric suffix.</returns>
  public static string RemoveNumberSuffix(string input)
  {
    string pattern = @"[\s\-_\(]*(\d+)[\)\s]*$";
    return Regex.Replace(input, pattern, "");
  }
}
}
