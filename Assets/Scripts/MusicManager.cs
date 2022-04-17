using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : CSingletonMono<MusicManager>
{
    public class AudioBuffer
    {
        public string name;
        public float time;
    }

    [SerializeField] AudioSource source;
    [SerializeField] AudioSource sfxAudioSource;
    [SerializeField] AudioClip phrase1;
    [SerializeField] AudioClip phrase2;
    [SerializeField] AudioClip confirm_pick_up;
    [SerializeField] AudioClip unconfirm_put_back;
    [SerializeField] AudioClip all_confirm;
    [SerializeField] AudioClip congrats;
    [SerializeField] AudioClip Incorrect;
    [SerializeField] AudioClip fiveClock;

    public float bufferTime;
    public List<AudioBuffer> audioBuffers = new List<AudioBuffer>();

    void Update()
    {
        if (audioBuffers.Capacity > 0)
        {
            foreach (AudioBuffer b in audioBuffers)
            {
                b.time -= Time.deltaTime;
            }
        }
    }

    public void PlayPhrase2()
    {
        source.clip = phrase2;
        source.Play();
    }

    public void Play_confirm_pick_up()
    {
        PlayClip(confirm_pick_up);
    }

    public void Play_unconfirm_put_back()
    {
        PlayClip(unconfirm_put_back);
    }

    public void Play_all_confirm()
    {
        PlayClip(all_confirm);
    }

    public void Play_congrats()
    {
        PlayClip(congrats);
    }

    public void Play_Incorrect()
    {
        PlayClip(Incorrect);
    }

    public void Play_fiveClock()
    {
        PlayClip(fiveClock);
    }


    public void PlayClip(AudioClip clip, float delay = 0)
    {
        if (clip == null)
        {
            Debug.Log("Null Clip");
            return;
        }

        if (audioBuffers.Capacity > 0)
        {
            foreach (AudioBuffer b in audioBuffers)
            {
                if (clip.name == b.name && b.time > 0)
                {
                    return;
                }
                else if (clip.name == b.name)
                {
                    b.time = bufferTime;
                }
            }
        }

        if (delay > 0)
        {
            sfxAudioSource.clip = clip;
            sfxAudioSource.PlayDelayed(delay);
        }
        else
        {
            sfxAudioSource.PlayOneShot(clip);
        }

        AudioBuffer buffer = new AudioBuffer();
        buffer.name = clip.name;
        buffer.time = bufferTime;
        audioBuffers.Add(buffer);
    }
}
