#region Usings
using System;
using MathBad;
using UnityEngine;
using UnityEditor;
using MathBad_Editor;
using static MathBad_Editor.EDITOR_HELP;
using Object = UnityEngine.Object;
#endregion

[CustomEditor(typeof(Ramp))]
public class RampExtendedEditor : ExtendedEditor<Ramp>
{
    const int WIDTH = 64;
    const int SINGLE_HEIGHT = 16;

    protected override void OnEnable()
    {
        base.OnEnable();
        ValidateName();
    }

    string NextTextureName() => $"{_target.name}";
    Int2 NextTextureSize()
    {
        if(_target.useFlags
           is Ramp.RampUseFlags.AB_TopBottom
           or Ramp.RampUseFlags.AB_HorizontalVertical)
            return new Int2(WIDTH, WIDTH);
        return new Int2(WIDTH, SINGLE_HEIGHT);
    }

    public override void OnInspectorGUI()
    {
        ValidateName();

        EditorGUI.BeginChangeCheck();

        DrawInspector();

        if(_target.alwaysUpdate && EditorGUI.EndChangeCheck())
        {
            UpdateRamp();
        }

        Space(5f);
        Row(() =>
        {
            Button("Update", UpdateRamp);
            Button("Clear", ClearTextures);
            Button("SavePNG", () =>
            {
                string path = AssetDatabase.GetAssetPath(_target);
                path = path.Replace(_target.name + PATH.ASSET, "");
                SaveAsset.PNG(_target.texture, true, path, _target.texture.name,
                              TextureImporterType.Default,
                              _target.alphaIsTransparency, _target.sRGB);
            });
        });

        Space(5f);
        Rect alwaysUpdateRect = GetRect(25);

        if(GUI.Button(alwaysUpdateRect, "Always Update Changes"))
        {
            _target.alwaysUpdate = !_target.alwaysUpdate;
            if(_target.alwaysUpdate) UpdateRamp();
        }
        if(_target.alwaysUpdate)
        {
            alwaysUpdateRect.Draw(RGB.darkGrey);
            alwaysUpdateRect.DrawBorder(1f, RGB.cyan.WithValue(0.75f));
            GUI.Label(alwaysUpdateRect, "Always Updating Changes", EDITOR_LIB.boldLabel_middle);
        }
        Space(10f);
        DrawRampPreview();
    }

    void DrawInspector()
    {
        _target.useFlags = (Ramp.RampUseFlags)EditorGUILayout.EnumPopup("UseFlags", _target.useFlags);

        Space(5);
        bool showGradientA = _target.useFlags != Ramp.RampUseFlags.B_Horizontal;
        bool showGradientB = _target.useFlags == Ramp.RampUseFlags.B_Horizontal
                          || _target.useFlags == Ramp.RampUseFlags.AB_TopBottom
                          || _target.useFlags == Ramp.RampUseFlags.AB_HorizontalVertical;

        if(showGradientA)
        {
            DrawTitle("Gradient A");
            _target.gradientA = EditorGUILayout.GradientField("GradientA", _target.gradientA);
            Row(" ", () =>
            {
                if(GUILayout.Button("Flip All")) _target.gradientA.ReverseAllKeys();
                if(GUILayout.Button("Flip Color")) _target.gradientA.ReverseColorKeys();
                if(GUILayout.Button("Flip Alpha")) _target.gradientA.ReverseAlphaKeys();
            });
        }

        if(showGradientB)
        {
            DrawTitle("Gradient B");
            _target.gradientB = EditorGUILayout.GradientField("GradientB", _target.gradientB);
            Row(" ", () =>
            {
                if(GUILayout.Button("Flip All")) _target.gradientB.ReverseAllKeys();
                if(GUILayout.Button("Flip Color")) _target.gradientB.ReverseColorKeys();
                if(GUILayout.Button("Flip Alpha")) _target.gradientB.ReverseAlphaKeys();
            });
        }

        DrawTitle("Evaluation");
        _target.easeType = (EaseType)EditorGUILayout.EnumPopup("EaseType", _target.easeType);

        Space(10);
        DrawTitle("Texture Settings");
        Row(() =>
        {
            _target.sRGB = EditorGUILayout.Toggle("sRGB", _target.sRGB);
            _target.alphaIsTransparency = EditorGUILayout.Toggle("AlphaIsTransparency", _target.alphaIsTransparency);
        });

        _target.wrapMode = (TextureWrapMode)EditorGUILayout.EnumPopup("WrapMode", _target.wrapMode);
        _target.filterMode = (FilterMode)EditorGUILayout.EnumPopup("FilterMode", _target.filterMode);
    }

    public void UpdateRamp()
    {
        ValidateTex();

        switch(_target.useFlags)
        {
            case Ramp.RampUseFlags.A_Horizontal:
                _target.texture.GadientFill(_target.gradientA, _target.easeType);
                break;
            case Ramp.RampUseFlags.B_Horizontal:
                _target.texture.GadientFill(_target.gradientB, _target.easeType);
                break;
            case Ramp.RampUseFlags.AB_TopBottom:
                _target.texture.GadientFillTB(_target.gradientA, _target.gradientB, _target.easeType);
                break;
            case Ramp.RampUseFlags.AB_HorizontalVertical:
                _target.texture.GadientFillHV(_target.gradientA, _target.gradientB, _target.easeType);
                break;
            default:
                _target.texture.GadientFill(_target.gradientA, _target.easeType);
                break;
        }

        EditorUtility.SetDirty(_target.texture);
        EditorUtility.SetDirty(_target.icon);
        EditorUtility.SetDirty(_target);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    void ValidateName()
    {
        string tex_name = NextTextureName();
        if(_target.texture != null)
        {
            if(_target.texture.name != tex_name)
            {
                string tex_path = AssetDatabase.GetAssetPath(_target.texture);
                AssetDatabase.RenameAsset(tex_path, tex_name);
                _target.texture.name = tex_name;
                _target.icon.name = $"{tex_name}_Icon";

                EditorUtility.SetDirty(_target.texture);
                EditorUtility.SetDirty(_target.icon);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }

    void ValidateTex()
    {
        Int2 size = NextTextureSize();

        if(_target.texture != null)
        {
            if(_target.texture.Size() != size)
            {
                _target.texture.Reinitialize(size.x, size.y, TextureFormat.RGBA32, false);
                _target.texture.Apply();

                _target.children.Remove(_target.icon);
                AssetDatabase.RemoveObjectFromAsset(_target.icon);
                CreateIcon(_target.texture);
            }
        }
        else
        {
            CreateTexture(size, NextTextureName());
            CreateIcon(_target.texture);
        }
    }

    void CreateTexture(Int2 size, string name)
    {
        Texture2D texture = TEX.Create(size.x, size.y, _target.filterMode, TextureFormat.RGBA32, false);
        texture.name = name;
        texture.wrapMode = _target.wrapMode;

        AssetDatabase.AddObjectToAsset(texture, _target);
        _target.texture = texture;
        _target.children.Add(texture);
    }

    void CreateIcon(Texture2D texture)
    {
        Rect spriteRect = new Rect(0, 0, texture.width, texture.height);
        Sprite icon = Sprite.Create(texture, spriteRect, new Vector2(0.5f, 0.5f), 128);
        icon.name = $"{texture.name}_Icon";

        AssetDatabase.AddObjectToAsset(icon, _target);

        _target.icon = icon;
        _target.children.Add(icon);
    }

    void ClearTextures()
    {
        for(int i = _target.children.Count - 1; i >= 0; i--)
        {
            AssetDatabase.RemoveObjectFromAsset(_target.children[i]);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        _target.texture = null;
        _target.icon = null;

        _target.children.Clear();

        EditorUtility.SetDirty(_target);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    // Preview
    //----------------------------------------------------------------------------------------------------
    void DrawRampPreview()
    {
        Int2 size = _target.texture != null ? _target.texture.Size() : NextTextureSize();
        Rect rect = GetRect();

        float windowWidth = rect.width;

        float widthRatio = size.x._float() / size.y;
        float height = windowWidth * (size.y._float() / size.x);
        float width = height * widthRatio - 4f;

        rect.height = height;
        rect.width = width;
        rect.x = windowWidth.Half() - width.Half() + 18f;

        if(_target.alphaIsTransparency)
        {
            rect.Draw(TEX.checkerTex, RGB.white, false, ScaleMode.ScaleAndCrop);
        }

        if(_target.texture != null)
        {
            rect.Draw(_target.texture, RGB.white, _target.alphaIsTransparency, ScaleMode.StretchToFill);
        }

        rect.DrawBorder(1f, RGB.Value(0.15f));
    }
}
