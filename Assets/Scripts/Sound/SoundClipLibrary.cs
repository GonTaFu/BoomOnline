using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Audio/Sound Clip Library")]
public class SoundClipLibrary : ScriptableObject
{
    public List<SoundClipPair> clips;

    public AudioClip GetClip(SoundType type)
    {
        foreach (var pair in clips)
        {
            if (pair.type == type) return pair.clip;
        }
        return null;
    }
}

[System.Serializable]
public class SoundClipPair
{
    public SoundType type;
    public AudioClip clip;
}