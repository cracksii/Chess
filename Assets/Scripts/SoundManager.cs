using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager INSTANCE;
    public Sound[] sounds;


    void Awake()
    {
        if(INSTANCE == null)
            INSTANCE = this;
        else
            Destroy(gameObject);
    }

    public void Init()
    {
        for(int i = 0; i < sounds.Length; i++)
        {
            Sound s = sounds[i];
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;

            if(s.playOnStartup)
                s.source.Play();
        }
    }

    public void Play(string _name)
    {
        Sound s = Array.Find(sounds, s => s.name == _name);
        s.source.Play();
    }

    public void Stop(string _name)
    {
        Sound s = Array.Find(sounds, s => s.name == _name);
        s.source.Stop();
    }
}
