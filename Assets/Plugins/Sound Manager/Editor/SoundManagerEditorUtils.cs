using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;

/// <summary>
/// Robbie should write a comment here.
/// </summary>
public static class SoundManagerEditorUtils
{



    [MenuItem("Assets/Create/Sound Bank/From Selected Clips")]
    private static void CreateSoundBankFromClips()
    {
        List<AudioClip> clips = new List<AudioClip>();
        for (int i = 0; i < Selection.objects.Length; i++)
        {
            AudioClip clip = Selection.objects[i] as AudioClip;
            if (clip != null)
            {
                clips.Add(clip);
            }
        }

        clips.Sort((x, y) => x.name.CompareTo(y.name));

        if (clips.Count > 0)
        {
            EffectSoundBank bank = ScriptableObject.CreateInstance<EffectSoundBank>();
            bank.SetClips(clips.ToArray());

            string path = AssetDatabase.GetAssetPath(clips[0]);
            path = path.Substring(0, path.LastIndexOf("/"));
            path += "/" + clips[0].name + ".asset";

            AssetDatabase.CreateAsset(bank, AssetDatabase.GenerateUniqueAssetPath(path));
            AssetDatabase.SaveAssets();

            Selection.activeObject = bank;
        }

    }

    [MenuItem("Assets/Create/Sound Bank/Create Multiple for each Selected Clip")]
    private static void CreateMultipleSoundBankFromClips()
    {
        List<AudioClip> clips = new List<AudioClip>();
        for (int i = 0; i < Selection.objects.Length; i++)
        {
            AudioClip clip = Selection.objects[i] as AudioClip;
            if (clip != null)
            {
                clips.Add(clip);
            }
        }

        clips.Sort((x, y) => x.name.CompareTo(y.name));

        foreach (AudioClip clip in clips)
        {
            EffectSoundBank bank = ScriptableObject.CreateInstance<EffectSoundBank>();

            bank.SetClips(new AudioClip[] { clip });

            string path = AssetDatabase.GetAssetPath(clip);
            path = path.Substring(0, path.LastIndexOf("/"));
            path += "/" + clip.name + ".asset";

            AssetDatabase.CreateAsset(bank, AssetDatabase.GenerateUniqueAssetPath(path));
            AssetDatabase.SaveAssets();

            Selection.activeObject = bank;
        }
    }

    public static SoundBank CreateNewSoundBank(Type type, string name)
    {
        SoundBank bank = ScriptableObject.CreateInstance(type) as SoundBank;
        AssetDatabase.CreateAsset(bank, AssetDatabase.GenerateUniqueAssetPath("Assets/" + name + ".asset"));
        AssetDatabase.SaveAssets();

        return bank;
    }

}
