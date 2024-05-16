#if UNITY_EDITOR
    using UnityEditor;
#endif

using UnityEngine;

public static class SafeTime
{
    public static float DeltaTime
    {
        get
        {
            if (Application.isPlaying)
            {
                return UnityEngine.Time.deltaTime;
            }
            else
            {
                return 1/30f; //Estimate for editor
            }
        }
    }
    
    public static float Time
    {
        get
        {
            if (Application.isPlaying)
            {
                return UnityEngine.Time.time;
            }
            else
            {
                #if UNITY_EDITOR
                return (float)EditorApplication.timeSinceStartup;
                #endif
                return 0;
            }
        }
    }
}
