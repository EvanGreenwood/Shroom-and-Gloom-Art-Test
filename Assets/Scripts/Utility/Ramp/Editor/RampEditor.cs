#region Usings
using MathBad;
using UnityEngine;
using UnityEditor;
using MathBad_Editor;
using static MathBad_Editor.EDITOR_HELP;
#endregion

[CustomEditor(typeof(Ramp))]
public class RampEditor : ExtendedEditor<Ramp>
{
    const int WIDTH = 64;
    const int SINGLE_HEIGHT = 16;

    void OnEnable()
    {
        ValidateName();
    }

    string NextTextureName() => $"{target.name}";
    Int2 NextTextureSize()
    {
        if(target.useFlags == Ramp.RampUseFlags.NorthSouth
        || target.useFlags == Ramp.RampUseFlags.AB_EastWest
        || target.useFlags == Ramp.RampUseFlags.AB_HorizontalVertical)
            return new Int2(WIDTH, WIDTH);
        return new Int2(WIDTH, SINGLE_HEIGHT);
    }

    public override void OnInspectorGUI()
    {
        ValidateName();

        EditorGUI.BeginChangeCheck();

        DrawInspector();

        if(target.alwaysUpdate && EditorGUI.EndChangeCheck())
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
                string path = AssetDatabase.GetAssetPath(target);
                path = path.Replace(target.name + PATH.ASSET, "");
                SaveAsset.PNG(target.texture, true, path, target.texture.name,
                              TextureImporterType.Default,
                              target.alphaIsTransparency, target.sRGB);
            });
        });

        Space(5f);
        Rect alwaysUpdateRect = GetRect(25);

        if(GUI.Button(alwaysUpdateRect, "Always Update Changes"))
        {
            target.alwaysUpdate = !target.alwaysUpdate;
            if(target.alwaysUpdate) UpdateRamp();
        }
        if(target.alwaysUpdate)
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
        target.useFlags = (Ramp.RampUseFlags)EditorGUILayout.EnumPopup("UseFlags", target.useFlags);

        Space(5);
        bool showGradientA = target.useFlags != Ramp.RampUseFlags.B_Horizontal;
        bool showGradientB = target.useFlags == Ramp.RampUseFlags.B_Horizontal
                          || target.useFlags == Ramp.RampUseFlags.NorthSouth
                          || target.useFlags == Ramp.RampUseFlags.AB_EastWest
                          || target.useFlags == Ramp.RampUseFlags.AB_HorizontalVertical;

        if(showGradientA)
        {
            DrawTitle("Gradient A");
            target.gradientA = EditorGUILayout.GradientField("GradientA", target.gradientA);
            Row(" ", () =>
            {
                if(GUILayout.Button("Flip All")) target.gradientA.ReverseAllKeys();
                if(GUILayout.Button("Flip Color")) target.gradientA.ReverseColorKeys();
                if(GUILayout.Button("Flip Alpha")) target.gradientA.ReverseAlphaKeys();
            });
        }

        if(showGradientB)
        {
            DrawTitle("Gradient B");
            target.gradientB = EditorGUILayout.GradientField("GradientB", target.gradientB);
            Row(" ", () =>
            {
                if(GUILayout.Button("Flip All")) target.gradientB.ReverseAllKeys();
                if(GUILayout.Button("Flip Color")) target.gradientB.ReverseColorKeys();
                if(GUILayout.Button("Flip Alpha")) target.gradientB.ReverseAlphaKeys();
            });
        }

        DrawTitle("Evaluation");
        target.easeType = (EaseType)EditorGUILayout.EnumPopup("EaseType", target.easeType);

        Space(10);
        DrawTitle("Texture Settings");
        Row(() =>
        {
            target.sRGB = EditorGUILayout.Toggle("sRGB", target.sRGB);
            target.alphaIsTransparency = EditorGUILayout.Toggle("AlphaIsTransparency", target.alphaIsTransparency);
        });

        target.wrapMode = (TextureWrapMode)EditorGUILayout.EnumPopup("WrapMode", target.wrapMode);
        target.filterMode = (FilterMode)EditorGUILayout.EnumPopup("FilterMode", target.filterMode);
    }

    public void UpdateRamp()
    {
        ValidateTex();

        switch(target.useFlags)
        {
            case Ramp.RampUseFlags.A_Horizontal:
                target.texture.GadientFill(target.gradientA, target.easeType);
                break;
            case Ramp.RampUseFlags.B_Horizontal:
                target.texture.GadientFill(target.gradientB, target.easeType);
                break;
            case Ramp.RampUseFlags.NorthSouth:
                target.texture.GadientFillNorthSouth(target.gradientA, target.gradientB, target.easeType);
                break;
            case Ramp.RampUseFlags.AB_EastWest:
                target.texture.GadientFillEastWest(target.gradientA, target.gradientB, target.easeType);
                break;
            case Ramp.RampUseFlags.AB_HorizontalVertical:
                target.texture.GadientFillHV(target.gradientA, target.gradientB, target.easeType);
                break;
            default:
                target.texture.GadientFill(target.gradientA, target.easeType);
                break;
        }

        EditorUtility.SetDirty(target.texture);
        EditorUtility.SetDirty(target.icon);
        EditorUtility.SetDirty(target);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    void ValidateName()
    {
        string tex_name = NextTextureName();
        if(target.texture != null)
        {
            if(target.texture.name != tex_name)
            {
                string tex_path = AssetDatabase.GetAssetPath(target.texture);
                AssetDatabase.RenameAsset(tex_path, tex_name);
                target.texture.name = tex_name;
                target.icon.name = $"{tex_name}_Icon";

                EditorUtility.SetDirty(target.texture);
                EditorUtility.SetDirty(target.icon);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }

    void ValidateTex()
    {
        Int2 size = NextTextureSize();

        if(target.texture != null)
        {
            if(target.texture.Size() != size)
            {
                target.texture.Reinitialize(size.x, size.y, TextureFormat.RGBA32, false);
                target.texture.Apply();

                target.children.Remove(target.icon);
                AssetDatabase.RemoveObjectFromAsset(target.icon);
                CreateIcon(target.texture);
            }
        }
        else
        {
            CreateTexture(size, NextTextureName());
            CreateIcon(target.texture);
        }
    }

    void CreateTexture(Int2 size, string name)
    {
        Texture2D texture = TEX.Create(size.x, size.y, target.filterMode, TextureFormat.RGBA32, false);
        texture.name = name;
        texture.wrapMode = target.wrapMode;

        AssetDatabase.AddObjectToAsset(texture, target);
        target.texture = texture;
        target.children.Add(texture);
    }

    void CreateIcon(Texture2D texture)
    {
        Rect spriteRect = new Rect(0, 0, texture.width, texture.height);
        Sprite icon = Sprite.Create(texture, spriteRect, new Vector2(0.5f, 0.5f), 128);
        icon.name = $"{texture.name}_Icon";

        AssetDatabase.AddObjectToAsset(icon, target);

        target.icon = icon;
        target.children.Add(icon);
    }

    void ClearTextures()
    {
        for(int i = target.children.Count - 1; i >= 0; i--)
        {
            AssetDatabase.RemoveObjectFromAsset(target.children[i]);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        target.texture = null;
        target.icon = null;

        target.children.Clear();

        EditorUtility.SetDirty(target);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    // Preview
    //----------------------------------------------------------------------------------------------------
    void DrawRampPreview()
    {
        Int2 size = target.texture != null ? target.texture.Size() : NextTextureSize();
        Rect rect = GetRect();

        float windowWidth = rect.width;

        float widthRatio = size.x._float() / size.y;
        float height = windowWidth * (size.y._float() / size.x);
        float width = height * widthRatio - 4f;

        rect.height = height;
        rect.width = width;
        rect.x = windowWidth.Half() - width.Half() + 18f;

        if(target.alphaIsTransparency)
        {
            rect.Draw(TEX.checkerTex, RGB.white, false, ScaleMode.ScaleAndCrop);
        }

        if(target.texture != null)
        {
            rect.Draw(target.texture, RGB.white, target.alphaIsTransparency, ScaleMode.StretchToFill);
        }

        rect.DrawBorder(1f, RGB.Value(0.15f));
    }
}
