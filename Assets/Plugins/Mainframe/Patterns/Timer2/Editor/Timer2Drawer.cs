#region
using Mainframe;
using Mainframe_Editor;
using UnityEditor;
using UnityEngine;
using static Mainframe_Editor.EDITOR_HELP;
#endregion

[CustomPropertyDrawer(typeof(Timer2))]
public class Timer2Drawer : PropertyDrawer
{
  // GUI
  //----------------------------------------------------------------------------------------------------
  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
  {
    EditorGUI.BeginProperty(position, label, property);
    position = EditorGUI.PrefixLabel(position, label);

    int indent = EditorGUI.indentLevel;
    EditorGUI.indentLevel = 0;

    Rect[] split = position.SplitWidthPercent(0.5f, 10f);

    SerializedProperty a = property.FindPropertyRelative("_baseTarget");
    SerializedProperty b = property.FindPropertyRelative("_targetVariance");

    Rect[] a_rects = split[0].SplitFromLeft(30f);
    Rect[] b_rects = split[1].SplitFromLeft(30f);

    EditorGUI.LabelField(a_rects[0], "T");
    EditorGUI.PropertyField(a_rects[1], a, GUIContent.none);

    EditorGUI.LabelField(b_rects[0], "+-");
    EditorGUI.PropertyField(b_rects[1], b, GUIContent.none);

    EditorGUI.indentLevel = indent;
    EditorGUI.EndProperty();
  }
}
