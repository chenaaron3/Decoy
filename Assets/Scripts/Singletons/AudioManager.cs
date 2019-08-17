using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioClip[] clips;
    private AudioSource[] sources;

    private void Awake()
    {
        instance = this;

        sources = new AudioSource[clips.Length];
        // creates an audio source for each clip
        for (int j = 0; j < clips.Length; j++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            sources[j] = source;
            sources[j].clip = clips[j];
        }
    }

    // plays a sound based on the clip name
    public void PlaySound(string name)
    {
        for (int j = 0; j < clips.Length; j++)
        {
            if (clips[j].name.Equals(name))
            {
                sources[j].Play();
            }
        }
    }
}
