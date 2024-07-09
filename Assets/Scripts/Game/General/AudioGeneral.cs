using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioGeneral : MonoBehaviour
{
    private AudioSource Audio;

    public AudioClip[] Clips;

    // Start is called before the first frame update
    void Awake()
    {
        Audio = GetComponent<AudioSource>();
    }

    public void PlayClips(int Pointer)
    {
        Audio.clip = Clips[Pointer];

        Audio.Play();
    }

    public void Stop()
    {
        Audio.Stop();
    }
}
