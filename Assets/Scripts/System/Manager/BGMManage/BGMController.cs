using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BGMController : MonoBehaviour
{
    public bool PlayAwake;

    private AudioSource Audio;

    public AudioClip[] Clips;

    public static BGMController Controller
    {
        get
        {
            if (controller != null)
                return controller;

            controller = FindObjectOfType<BGMController>();

            return controller;
        }
    }

    protected static BGMController controller;

    // Start is called before the first frame update
    void Start()
    {
        Audio = GetComponent<AudioSource>();
    }

    public void BGMPlay(int Pointer)
    {
        Audio.clip = Clips[Pointer];

        Audio.Play();

        BGMFadeIn();
    }

    public void BGMChange(int Pointer)
    {
        Audio.DOFade(1f, 2f).OnComplete(()=> 
        {
            Audio.clip= Clips[Pointer];

            Audio.Play();

            BGMFadeIn();
        });
    }

    public void BGMTeleportation(int Pointer)
    {
        Audio.clip = Clips[Pointer];

        Audio.Play();
    }

    public void BGMFadeIn()
    {
        Audio.DOFade(1f,2f);
    }

    public void BGMFadeOut()
    {
        Audio.DOFade(0f,2f);
    }

    public void BGMPause()
    {
        Audio.Pause();
    }

    public void BGMUnPause()
    {
        Audio.UnPause();
    }

    public void VolumeChange(float Volume)
    {
        Audio.DOFade(Volume,2f);
    }
}
