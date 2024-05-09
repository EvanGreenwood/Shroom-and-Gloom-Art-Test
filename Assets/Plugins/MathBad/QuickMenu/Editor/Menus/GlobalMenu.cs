#region
using System;
using System.Diagnostics;
using MathBad;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using Debug = UnityEngine.Debug;
#endregion

namespace MathBad_Editor
{
// Global Menu
//----------------------------------------------------------------------------------------------------
[InitializeOnLoad]
public static class GlobalMenu
{
    static GlobalMenu()
    {
        QuickMenu menu = new QuickMenu("Global Menu",
                                       OnInput,
                                       typeof(GlobalMenuItems));
    }

    public static bool OnInput()
    {
        if(Application.isPlaying)
            return false;
        Event e = Event.current;

        // Check if the event is a key press event.
        return e.type == EventType.KeyDown && e.shift && e.keyCode == KeyCode.W;
    }
}

// Menu Items
//----------------------------------------------------------------------------------------------------
public static class GlobalMenuItems
{
    [MenuInvoke("Recompile", 10, "")]
    static void Recompile()
    {
        EDITOR.SaveAll();

        CompilationPipeline.RequestScriptCompilation();

        Debug.Log("Project Saved.");
    }

    [MenuInvoke("Memory/Free Memory", 30)]
    static void FreeMemory()
    {
        EditorUtility.UnloadUnusedAssetsImmediate();
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }

    [MenuInvoke("Open Data Path", 100)]
    static void DataPath()
    {
        string path = PATH.GetFullPath(Application.persistentDataPath);
        Process.Start("explorer.exe", string.Format("/select,\"{0}\"", path));
    }
}
}
