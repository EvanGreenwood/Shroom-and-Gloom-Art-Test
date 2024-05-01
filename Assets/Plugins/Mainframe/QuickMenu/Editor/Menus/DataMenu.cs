#region
using System;
using System.Collections.Generic;
using Mainframe;
using UnityEditor;
using UnityEngine;
#endregion

namespace Mainframe_Editor
{
[InitializeOnLoad]
public static class DataMenu
{
  static DataMenu()
  {
    List<Type> data = REFLECTION.GetDerivedWithAttribute<ScriptableObject, MenuCreateAttribute>();
    QuickMenu menu = new QuickMenu("Data Menu", OnInput, data.ToArray());
  }

  public static bool OnInput()
  {
    if(EDITOR.IsMouseOverEditorWindow(UnityEditorWindowType.ProjectBrowser))
    {
      if(Application.isPlaying)
        return false;

      return EditorInput.shift && EditorInput.keyDown(KeyCode.D);
    }
    return false;
  }
}
}
