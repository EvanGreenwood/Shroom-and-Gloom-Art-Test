// #region usings
// using Framework;
// using UnityEditor;
// using UnityEngine;
// #endregion
//
// namespace FrameworkEditor
// {
// /// <summary> TubeRendererEditor </summary>
// [CustomEditor(typeof(TubeRenderer)), CanEditMultipleObjects]
// public class TubeRendererEditor: Editor
// {
//   private TubeRenderer _target;
//   private bool _showPosition = false;
//   private bool _showSegments = false;
//
//   private void OnEnable() {_target = (TubeRenderer)target;}
//   private void OnDisable() {}
//
//   // inspector
//   public override void OnInspectorGUI()
//   {
//     base.OnInspectorGUI();
//     if (_target == null)
//       return;
//
//     // Debug Toggles
//     GUILayout.BeginHorizontal();
//     GUI.color = _showPosition ? Color.green : Color.grey;
//     if (GUILayout.Button("Position Gizmos")) _showPosition = !_showPosition;
//     GUI.color = _showSegments ? Color.green : Color.grey;
//     if (GUILayout.Button("Segments")) _showSegments = !_showSegments;
//     GUI.color = Color.white;
//     GUILayout.EndHorizontal();
//
//     // Total Length of Line
//     float totalLength = 0f;
//     for (int i = 0; i < _target.positions.Length - 1; i++)
//     {
//       totalLength += Vector3.Distance(_target.positions[i], _target.positions[i + 1]);
//     }
//
//     // Tube Stats
//     Rect statsRect = InspectorUtil.GetRect(30);
//     EditorGUI.DrawRect(statsRect, Color.black);
//     GUI.Label(statsRect, "Verts " + _target.vertices.Count.ToString().Yellow() + "  Length " + totalLength.ToString("F2").Yellow(), GUIUtil.inspectorStyleBoldMiddle);
//   }
//
//   protected void OnSceneGUI()
//   {
//     if (_target == null)
//       return;
//
//     if (_showPosition)
//     {
//       Undo.RecordObject(_target, "Change TubeRenderer Positions");
//       for (int i = 0; i < _target.positions.Length; i++)
//       {
//         EditorGUI.BeginChangeCheck();
//         Vector3 p0 = _target.useWorldSpace ? _target.positions[i] : _target.transform.TransformPoint(_target.positions[i]);
//         Vector3 handlePos = Handles.PositionHandle(p0, Quaternion.identity);
//
//         if (EditorGUI.EndChangeCheck())
//         {
//           _target.positions[i] = handlePos;
//           _target.SetPositions(_target.positions);
//         }
//       }
//     }
//     if (_showSegments)
//     {
//       for (int i = 0; i < _target.positions.Length - 1; i++)
//       {
//         Vector3 p0 = _target.useWorldSpace ? _target.positions[i] : _target.transform.TransformPoint(_target.positions[i]);
//         Vector3 p1 = _target.useWorldSpace ? _target.positions[i + 1] : _target.transform.TransformPoint(_target.positions[i + 1]);
//         SceneUtil.DrawLine(p0, p1, 4, rgbf.Hue(i.ToFloat() / _target.positions.Length));
//       }
//     }
//   }
// }
// }






