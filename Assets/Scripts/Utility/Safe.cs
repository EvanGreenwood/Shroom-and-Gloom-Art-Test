using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Ross.EditorRuntimeCombatibility
{
        public static class Safe
        {
                public static void Destroy(Object toDestroy)
                {
                        if (Application.isPlaying)
                        {
                                Object.Destroy(toDestroy);
                        }
                        else
                        {
                                Object.DestroyImmediate(toDestroy);
                        }
                }
                
                #region Time
                
#if UNITY_EDITOR
                [InitializeOnLoad]
#endif
                public static class Time
                {
                        static Time()
                        {
#if UNITY_EDITOR
                                EditorApplication.update += EditorUpdate;
#endif
                        }

                        private static float _editorLastTime;
                        private static float _editorDt;

                        private static void EditorUpdate()
                        {
#if UNITY_EDITOR
                                float currentTime = (float)EditorApplication.timeSinceStartup;
                                _editorDt = Mathf.Max(0, currentTime - _editorLastTime);
                                _editorLastTime = currentTime;
#endif
                        }

                        public static float deltaTime
                        {
                                get
                                {
                                        if (Application.isPlaying)
                                        {
                                                return UnityEngine.Time.deltaTime;
                                        }

                                        if (_editorDt > 0)
                                        {
                                                return _editorDt;
                                        }

                                        //fallback dt for eg. before first editor update. Estimate for editor.
                                        return 1 / 30f;
                                }
                        }

                        public static float time
                        {
                                get
                                {
                                        if (Application.isPlaying)
                                        {
                                                return UnityEngine.Time.time;
                                        }

#if UNITY_EDITOR
                                        return (float)EditorApplication.timeSinceStartup;
#endif
                                        return 0;
                                }
                        }
                }
                #endregion
        }
}