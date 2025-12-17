#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

[CustomEditor(typeof(SfxSO))]
public class SfxSOEditor : Editor
{
    private void OnEnable()
    {
        ref SfxList[] sfxList = ref ((SfxSO)target).sfx;
        
        if (sfxList == null) return;
        
        string[] names = Enum.GetNames(typeof(SfxType));
        bool differentSize = names.Length != sfxList.Length;
        Dictionary<string, SfxList> sfx = new();

        if (differentSize)
        {
            for (int i = 0; i < sfxList.Length; i++)
            {
                sfx.Add(sfxList[i].name, sfxList[i]);
            }
        }
        
        Array.Resize(ref sfxList, names.Length);
        for (int i = 0; i < sfxList.Length; i++)
        {
            string currName = names[i];
            sfxList[i].name = currName;
        
            if (differentSize)
            {
                if (sfx.TryGetValue(currName, out var curr))
                    UpdateElement(ref sfxList[i], curr.volume, curr.sfx, curr.mixerGroup);
                else
                    UpdateElement(ref sfxList[i], 1, Array.Empty<AudioClip>(), null);

                static void UpdateElement(ref SfxList element, float volume, AudioClip[] sfx, AudioMixerGroup mixerGroup)
                {
                    element.volume = volume;
                    element.sfx = sfx;
                    element.mixerGroup = mixerGroup;
                }
            }
        }
    }
}
#endif
