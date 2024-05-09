#region Usings
using UnityEngine;
using UnityEditor;
using MathBad;
#endregion

[CustomPropertyDrawer(typeof(PID))]
public class PIDDrawer : PropertyDrawer
{
  // GUI
  //----------------------------------------------------------------------------------------------------
  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
  {
    EditorGUI.BeginProperty(position, label, property);
    position = EditorGUI.PrefixLabel(position, label);

    int indent = EditorGUI.indentLevel;
    EditorGUI.indentLevel = 0;

    Rect[] split = position.SplitWidthEqually(3, 5f);

    SerializedProperty p = property.FindPropertyRelative("_kp");
    SerializedProperty i = property.FindPropertyRelative("_ki");
    SerializedProperty d = property.FindPropertyRelative("_kd");

    Rect[] p_rects = split[0].SplitFromLeft(15f);
    Rect[] i_rects = split[1].SplitFromLeft(15f);
    Rect[] d_rects = split[2].SplitFromLeft(15f);

    EditorGUI.LabelField(p_rects[0], "P");
    EditorGUI.PropertyField(p_rects[1], p, GUIContent.none);

    EditorGUI.LabelField(i_rects[0], "I");
    EditorGUI.PropertyField(i_rects[1], i, GUIContent.none);

    EditorGUI.LabelField(d_rects[0], "D");
    EditorGUI.PropertyField(d_rects[1], d, GUIContent.none);

    EditorGUI.indentLevel = indent;
    EditorGUI.EndProperty();
  }
}
