using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;

    public bool loop = false;
    public bool playOnStartup = false;
    [Range(0, 1)] public float volume;

    [Range(.1f, 3)] public float pitch = 1;

    [HideInInspector] public AudioSource source;
}
