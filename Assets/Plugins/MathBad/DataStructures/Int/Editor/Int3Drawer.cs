#region
using MathBad;
using UnityEditor;
using UnityEngine;
#endregion

namespace MathBad_Editor
{
[CustomPropertyDrawer(typeof(Int3))]
public class Int3Drawer : PropertyDrawer
{
  // GUI
  //----------------------------------------------------------------------------------------------------
  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
  {
    EditorGUI.BeginProperty(position, label, property);
    position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

    SerializedProperty x = property.FindPropertyRelative("x");
    SerializedProperty y = property.FindPropertyRelative("y");
    SerializedProperty z = property.FindPropertyRelative("z");

    Rect[] rects = position.SplitWidthEqually(3, 10f);

    float fieldWidth = EditorGUIUtility.labelWidth;
    EditorGUIUtility.labelWidth = 15;
    x.intValue = EditorGUI.IntField(rects[0], "X", x.intValue);
    y.intValue = EditorGUI.IntField(rects[1], "Y", y.intValue);
    y.intValue = EditorGUI.IntField(rects[2], "Z", z.intValue);
    EditorGUIUtility.labelWidth = fieldWidth;

    EditorGUI.EndProperty();
  }
}
}
