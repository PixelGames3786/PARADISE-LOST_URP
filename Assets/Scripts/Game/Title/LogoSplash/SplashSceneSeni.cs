using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SplashSceneSeni : InputComponent
{
    private AudioSource Audio;

    private Animator Animator;

    public AudioClip[] Clips;

    private bool FirstStartUp;

    void Start()
    {
        //マウスカーソル非表示
        Cursor.visible = false;

        Audio = GetComponent<AudioSource>();
        Animator = GetComponent<Animator>();

        //初回起動かどうか判断
        FirstStartUp=PlayerPrefs.HasKey("FirstStartUp");
    }

    void Update()
    {
        if (FirstStartUp)
        {
            
            if (GetKeyDown(KeyCode.Space, Button: XboxControllerButtons.B, Set: KeybordSets.Decision))
            {
                Animator.enabled = false;

                GameObject.Find("Fader").GetComponent<Image>().DOFade(1f,0.5f).OnComplete(()=>TitleSeni());
            }
        }
    }

    public void RingSE(int Pointer)
    {
        Audio.clip = Clips[Pointer];

        Audio.Play();
    }

    public void TitleSeni()
    {
        PlayerPrefs.SetString("FirstStartUp", "");

        SceneManager.LoadScene("Title");
    }

    public override bool GetKeyDown(KeyCode Key = KeyCode.None, XboxControllerButtons Button = XboxControllerButtons.None, KeybordSets Set = KeybordSets.None)
    {
        return base.GetKeyDown(Key, Button, Set);
    }
}
