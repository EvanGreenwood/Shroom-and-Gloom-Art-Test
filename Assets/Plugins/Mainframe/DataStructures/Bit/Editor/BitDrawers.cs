#region
using Mainframe;
using Mainframe_Editor;
using UnityEditor;
using UnityEngine;
#endregion

namespace FrameworkEditor
{
// bit
//--------------------------------------------------------------------------------------------------//
[CustomPropertyDrawer(typeof(Bit))]
public class BitDrawer : PropertyDrawer
{
  // on gui
  //--------------------------------------------------------------------------------------------------//
  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
  {
    EditorGUI.BeginProperty(position, label, property);
    position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

    SerializedProperty x = property.FindPropertyRelative("_value");

    float bitWidth = 15;
    x.intValue = EditorGUI.Toggle(position.WithWidth(bitWidth), x.intValue == 1) ? 1 : 0;

    EditorGUI.EndProperty();
  }
}

// bit2
//--------------------------------------------------------------------------------------------------//
[CustomPropertyDrawer(typeof(Bit2))]
public class Bit2Drawer : PropertyDrawer
{
  // on gui
  //--------------------------------------------------------------------------------------------------//
  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
  {
    EditorGUI.BeginProperty(position, label, property);
    position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

    SerializedProperty x = property.FindPropertyRelative("_x");
    SerializedProperty y = property.FindPropertyRelative("_y");

    float bitWidth = 15f;
    float labelWidth = EDITOR.GetLabelWidth("0");
    float totalWidth = (bitWidth + labelWidth + 15f) * 2f;

    Rect[] split = position.WithWidth(totalWidth).SplitWidthEqually(2, 5f);
    Rect[] xSplit = split[0].SplitFromLeft(bitWidth, 5f);
    Rect[] ySplit = split[1].SplitFromLeft(bitWidth, 5f);

    // x
    SerializedProperty bitX = x.FindPropertyRelative("_value");
    bitX.intValue = EditorGUI.Toggle(xSplit[0], bitX.intValue == 1) ? 1 : 0;
    EditorGUI.LabelField(xSplit[1], x.displayName);

    // y
    SerializedProperty bitY = y.FindPropertyRelative("_value");
    bitY.intValue = EditorGUI.Toggle(ySplit[0], bitY.intValue == 1) ? 1 : 0;
    EditorGUI.LabelField(ySplit[1], y.displayName);

    EditorGUI.EndProperty();
  }
}

// bit3
//--------------------------------------------------------------------------------------------------//
[CustomPropertyDrawer(typeof(Bit3))]
public class Bit3Drawer : PropertyDrawer
{
  // on gui
  //--------------------------------------------------------------------------------------------------//
  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
  {
    EditorGUI.BeginProperty(position, label, property);
    position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

    SerializedProperty x = property.FindPropertyRelative("_x");
    SerializedProperty y = property.FindPropertyRelative("_y");
    SerializedProperty z = property.FindPropertyRelative("_z");

    float bitWidth = 15f;
    float labelWidth = EDITOR.GetLabelWidth("0");
    float totalWidth = (bitWidth + labelWidth + 15f) * 3f;

    Rect[] split = position.WithWidth(totalWidth).SplitWidthEqually(3, 5f);
    Rect[] xSplit = split[0].SplitFromLeft(bitWidth, 5f);
    Rect[] ySplit = split[1].SplitFromLeft(bitWidth, 5f);
    Rect[] zSplit = split[2].SplitFromLeft(bitWidth, 5f);

    // x
    SerializedProperty bitX = x.FindPropertyRelative("_value");
    bitX.intValue = EditorGUI.Toggle(xSplit[0], bitX.intValue == 1) ? 1 : 0;
    EditorGUI.LabelField(xSplit[1], x.displayName);

    // y
    SerializedProperty bitY = y.FindPropertyRelative("_value");
    bitY.intValue = EditorGUI.Toggle(ySplit[0], bitY.intValue == 1) ? 1 : 0;
    EditorGUI.LabelField(ySplit[1], y.displayName);

    // z
    SerializedProperty bitZ = z.FindPropertyRelative("_value");
    bitZ.intValue = EditorGUI.Toggle(zSplit[0], bitZ.intValue == 1) ? 1 : 0;
    EditorGUI.LabelField(zSplit[1], z.displayName);
    EditorGUI.EndProperty();
  }
}
}
