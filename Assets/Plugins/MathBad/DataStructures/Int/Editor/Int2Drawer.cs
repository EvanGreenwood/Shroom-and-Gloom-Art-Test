#region
using MathBad;
using UnityEditor;
using UnityEngine;
#endregion

namespace MathBad_Editor
{
[CustomPropertyDrawer(typeof(Int2))]
public class Int2Drawer : PropertyDrawer
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

    SerializedProperty a = property.FindPropertyRelative("x");
    SerializedProperty b = property.FindPropertyRelative("y");

    Rect[] a_rects = split[0].SplitFromLeft(15f);
    Rect[] b_rects = split[1].SplitFromLeft(15f);

    EditorGUI.LabelField(a_rects[0], "X");
    a.intValue = EditorGUI.IntField(a_rects[1], GUIContent.none, a.intValue);

    EditorGUI.LabelField(b_rects[0], "Y");
    b.intValue = EditorGUI.IntField(b_rects[1], GUIContent.none, b.intValue);

    EditorGUI.indentLevel = indent;
    EditorGUI.EndProperty();
  }
}
}
