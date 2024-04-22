using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Framework;

[CustomPropertyDrawer(typeof(AmbienceSoundBank.EffectData))]
public class AmbienceBankEffectDataDrawer : PropertyDrawer
{

    private bool _isExpanded;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        if (!property.FindPropertyRelative("_initialized").boolValue)
        {
            //DefaultSoundBankValues.EnsureInstanceExists();

            property.FindPropertyRelative("Sound").SetValue(new ClipOrBank(true));
            property.FindPropertyRelative("SpawnMode").SetValue(DefaultSoundBankValues.Instance.AmbienceBank.EffectSpawnMode);
            property.FindPropertyRelative("SpawnDistance").SetValue(DefaultSoundBankValues.Instance.AmbienceBank.EffectSpawnDistance);
            property.FindPropertyRelative("Cooldown").SetValue(DefaultSoundBankValues.Instance.AmbienceBank.EffectCooldown);

            property.FindPropertyRelative("_initialized").boolValue = true;
        }

        _isExpanded = property.isExpanded;
        bool isBank = property.FindPropertyRelative("Sound").FindPropertyRelative("_isBank").boolValue;
        string name = "Empty";
        Object value = property.FindPropertyRelative("Sound").FindPropertyRelative(isBank ? "_soundBank" : "_audioClip").objectReferenceValue;

        if (value != null)
        {
            name = value.name;
        }

        EditorGUILayout.PropertyField(property, new GUIContent(name), false);
        property.ForEachChildProperty(DrawProperty);

    }

    void DrawProperty(SerializedProperty property)
    {
        if (!_isExpanded) return;
        if (property.name == "_initialized") return;
        EditorGUILayout.PropertyField(property, true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) - EditorGUIUtility.singleLineHeight;
    }

}
