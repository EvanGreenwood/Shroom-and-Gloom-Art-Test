#region
using MathBad;
using UnityEditor;
using UnityEngine;
#endregion

namespace MathBad_Editor
{
public static class EDITOR_LIB
{
    static EDITOR_LIB()
    {
        guiError = new GUIStyle(EditorStyles.boldLabel);
        guiError.normal.textColor = RGB.black;
        guiError.normal.background = Texture2D.redTexture;

        label_left = new GUIStyle(EditorStyles.label);
        label_left.alignment = TextAnchor.MiddleLeft;
        label_middle = new GUIStyle(EditorStyles.label);
        label_middle.alignment = TextAnchor.MiddleCenter;
        label_right = new GUIStyle(EditorStyles.label);
        label_right.alignment = TextAnchor.MiddleRight;

        boldLabel_left = new GUIStyle(EditorStyles.boldLabel);
        boldLabel_left.alignment = TextAnchor.MiddleLeft;
        boldLabel_middle = new GUIStyle(EditorStyles.boldLabel);
        boldLabel_middle.alignment = TextAnchor.MiddleCenter;
        boldLabel_right = new GUIStyle(EditorStyles.boldLabel);
        boldLabel_right.alignment = TextAnchor.MiddleRight;

        gradient_white_lr = Resources.Load<Texture2D>("MathBad/Textures/Gradient_White_LR");
    }

    public static readonly GUIStyle guiError;
    public static readonly GUIStyle label_left, label_middle, label_right;
    public static readonly GUIStyle boldLabel_left, boldLabel_middle, boldLabel_right;

    public static readonly Texture2D gradient_white_lr;
}
}
