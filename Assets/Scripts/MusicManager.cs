using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : CSingletonMono<MusicManager>
{
    [SerializeField] AudioSource source;
    [SerializeField] AudioClip phrase1;
    [SerializeField] AudioClip phrase2;

    public void PlayPhrase2()
    {
        source.clip = phrase2;
        source.Play();
    }
}
