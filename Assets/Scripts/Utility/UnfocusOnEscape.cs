using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class UnfocusOnEscape : MonoBehaviour
{
    void Update()
    {
        if (Application.isPlaying && Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_EDITOR
            EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");
#endif
        }
    }
}
