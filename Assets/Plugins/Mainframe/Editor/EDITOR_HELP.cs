#region
using System;
using Mainframe;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
#endregion

namespace Mainframe_Editor
{
public static class EDITOR_HELP
{
  static float _lastLabelWidth;
  static GUIContent _tempContent = new GUIContent();

  const string INDENT = "  ";
  public static float SingleLineHeight => EditorGUIUtility.singleLineHeight;

  public static void Space(float space) => EditorGUILayout.Space(space);

  // Get Rect
  //----------------------------------------------------------------------------------------------------
  public static Rect GetRect() => GetRect(EditorGUIUtility.singleLineHeight);
  public static Rect GetRect(float height) => EditorGUILayout.GetControlRect(true, height, GUIStyle.none);
  public static Rect GetFullViewRect(float height) => new Rect(0, GetRect(height).y, EditorGUIUtility.currentViewWidth, height);

  // Title
  //----------------------------------------------------------------------------------------------------
  public static void DrawTitle(string text, Action content)
  {
    DrawTitle(text, RGB.darkGrey, content);
  }
  public static void DrawTitle(string text, Color rectColor, Action content)
  {
    DrawTitle(20f, text, rectColor);
    Indent(content);
  }
  public static Rect DrawTitle(string text) => DrawTitle(20f, text, RGB.darkGrey);
  public static Rect DrawTitle(string text, Color rectColor) => DrawTitle(20f, text, rectColor);

  public static Rect DrawTitle(float height, string text, Color rectColor)
  {
    Rect rect = GetFullViewRect(height);
    rect.Draw(rectColor);
    EditorGUI.LabelField(rect, $"{INDENT}{text}", EditorStyles.boldLabel);
    return rect;
  }

  public static void DrawTitle(float height, string text) {EditorGUI.LabelField(GetFullViewRect(height), $"{INDENT}{text}", EditorStyles.boldLabel);}
  public static void DrawTitle(Rect rect, string text) {EditorGUI.LabelField(rect, $"{INDENT}{text}", EditorStyles.boldLabel);}
  public static void DrawTitle(Rect rect, string text, Color rectColor)
  {
    rect.Draw(rectColor);
    EditorGUI.LabelField(rect.Shrink(5f), $"{INDENT}{text}", EditorStyles.boldLabel);
  }
  // ToggleTitle
  //----------------------------------------------------------------------------------------------------
  public static void DrawToggleTitle(string text, ref bool flag, Action content)
  {
    Rect rect = GetFullViewRect(20f);
    rect.Draw(flag ? RGB.nearBlack : RGB.darkGrey);
    Rect[] split = rect.SplitFromLeft(25f);
    DrawTitle(split[1], text);
    flag = EditorGUI.Toggle(split[0].MoveRight(5), flag);
    if(flag)
    {
      Indent(content);
    }
  }

  public static void Indent(Action content)
  {
    int indent = EditorGUI.indentLevel;
    EditorGUI.indentLevel++;
    content();
    EditorGUI.indentLevel = indent;
  }

  public static void SaveLabelWidth() => _lastLabelWidth = EditorGUIUtility.labelWidth;
  public static void SetLabelWidth(float labelWidth) => EditorGUIUtility.labelWidth = labelWidth;
  public static void RestoreLabelWidth() => EditorGUIUtility.labelWidth = _lastLabelWidth;

  public static void LabelWidth(float labelWidth, Action content)
  {
    float lastWidth = EditorGUIUtility.labelWidth;
    EditorGUIUtility.labelWidth = labelWidth;
    content();
    EditorGUIUtility.labelWidth = lastWidth;
  }

  public static void DisabledGroup(bool isDisabled, Action content)
  {
    EditorGUI.BeginDisabledGroup(isDisabled);
    content();
    EditorGUI.EndDisabledGroup();
  }

  public static void ChangeCheck(Action content, Action action)
  {
    EditorGUI.BeginChangeCheck();
    content();
    if(EditorGUI.EndChangeCheck())
    {
      action();
    }
  }

  public static void Button(Rect rect, string label, Action content)
  {
    if(GUI.Button(rect, label)) { content(); }
  }

  public static void Button(string label, Action content)
  {
    if(GUILayout.Button(label)) { content(); }
  }

  public static void Label(string label) => EditorGUILayout.LabelField(label);
  public static void Label(string label, float width) => EditorGUILayout.LabelField(label, GUILayout.Width(width));
  public static void Label(string label, GUIStyle style) => EditorGUILayout.LabelField(label, style);
  public static void Label(Rect rect, string label) => EditorGUI.LabelField(rect, label);
  public static void Label(Rect rect, string label, GUIStyle style) => EditorGUI.LabelField(rect, label, style);

  public static void ProcessTargets<T>(Object[] targets, string undoName, Action content) where T : Component
  {
    foreach(Object obj in targets)
    {
      if(obj is T target)
      {
        Undo.RecordObject(target, undoName);

        content();
      }
    }
  }

  public static void SetTargetsDirty<T>(Object[] targets, Action content) where T : Component
  {
    foreach(Object obj in targets)
    {
      if(obj is T target)
      {
        EditorUtility.SetDirty(target);
      }
    }
  }

  // Layout
  //----------------------------------------------------------------------------------------------------
  public static void Row(Action content)
  {
    EditorGUILayout.BeginHorizontal();
    content();
    EditorGUILayout.EndHorizontal();
  }

  public static void Column(Action content)
  {
    EditorGUILayout.BeginVertical();
    content();
    EditorGUILayout.EndVertical();
  }

  public static void DrawGUIError(string error)
  {
    EditorGUILayout.LabelField($"  ERROR: {error}", EDITOR_LIB.guiError);
  }
}
}
