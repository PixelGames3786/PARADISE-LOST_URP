using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using DG.Tweening;

public class AudioButton : InputComponent
{
    public AudioMixer Mixer;

    public SaveDataManager Maneger;

    public OptionButton OptionButton;

    private AudioGeneral Audio;

    [System.NonSerialized]
    public TitleLineTween[] Lines;
    private OptionTumamiTween[] Tumamis = new OptionTumamiTween[3];

    private Transform[] OptionBack = new Transform[2];

    private float LineWidth = 950;
    private float Horizontal,Vertical;

    private bool VerticalStop,HorizontalStop;

    public int NowLine;
    
    private int[] NowTumamiPosi = new int[3];

    // Start is called before the first frame update
    void Start()
    {
        Audio = GetComponent<AudioGeneral>();

        Lines = new TitleLineTween[3];

        Lines[0] = transform.GetChild(0).GetChild(0).GetComponent<TitleLineTween>();
        Lines[1] = transform.GetChild(1).GetChild(0).GetComponent<TitleLineTween>();
        Lines[2] = transform.GetChild(2).GetChild(0).GetComponent<TitleLineTween>();

        Tumamis[0] = transform.GetChild(0).Find("SliderTumami").GetComponent<OptionTumamiTween>();
        Tumamis[1] = transform.GetChild(1).Find("SliderTumami").GetComponent<OptionTumamiTween>();
        Tumamis[2] = transform.GetChild(2).Find("SliderTumami").GetComponent<OptionTumamiTween>();

        OptionBack[0] = transform.parent.GetChild(0).GetChild(0).Find("OptionBack01");
        OptionBack[1] = transform.parent.GetChild(0).GetChild(0).Find("OptionBack02");

        //Masterの線を入れる
        Lines[0].LineTweenIn(LineWidth, 140, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        //上
        if (Vertical>0&&!VerticalStop)
        {
            StartCoroutine("StickVerticalStop");

            NowLine--;

            if (NowLine < 0)
            {
                NowLine = 0;
            }
            else
            {
                Lines[NowLine].LineTweenIn(LineWidth, 140, 0.5f);
                Lines[NowLine + 1].LineTweenOut(140,0.5f);

                Audio.PlayClips(1);

            }
        }

        //下
        if (Vertical < 0 && !VerticalStop)
        {
            StartCoroutine("StickVerticalStop");

            NowLine++;

            if (NowLine > 2)
            {
                NowLine = 2;
            }
            else
            {
                Lines[NowLine].LineTweenIn(LineWidth, 140, 0.5f);
                Lines[NowLine + -1].LineTweenOut(140,0.5f);

                Audio.PlayClips(1);

            }
        }

        //左
        if (Horizontal < 0 && !HorizontalStop)
        {
            StartCoroutine("StickHorizontalStop");

            //もう左端にあったら処理しない
            if (Maneger.OptionData.Volume[NowLine]<1||VerticalStop)
            {
                return;
            }

            Maneger.OptionData.Volume[NowLine]--;
            Tumamis[NowLine].TumamiTween(new Vector2(45 + (Maneger.OptionData.Volume[NowLine] * 120), 370));

            //今どのラインにいるのかで処理が変わる
            switch (NowLine)
            {
                //Master
                case 0:

                    {
                        Mixer.SetFloat("MasterVolume", -28 + (Maneger.OptionData.Volume[0] * 2));
                    }

                    break;

                //SE
                case 1:

                    {
                        Mixer.SetFloat("SEVolume", -3 + (Maneger.OptionData.Volume[1] * 1));
                    }

                    break;

                //BGM
                case 2:

                    {
                        Mixer.SetFloat("BGMVolume", -3 + (Maneger.OptionData.Volume[2] * 1));
                    }

                    break;
            }

            Maneger.OptionSave();

            Audio.PlayClips(0);

        }

        //右
        if (Horizontal > 0 && !HorizontalStop)
        {
            StartCoroutine("StickHorizontalStop");

            //もう右端にあったら処理しない
            if (Maneger.OptionData.Volume[NowLine] >5 || VerticalStop)
            {
                return;
            }

            Maneger.OptionData.Volume[NowLine]++;
            Tumamis[NowLine].TumamiTween(new Vector2(45 + (Maneger.OptionData.Volume[NowLine] * 120), 370));

            //今どのラインにいるのかで処理が変わる
            switch (NowLine)
            {
                //Master
                case 0:

                    {
                        Mixer.SetFloat("MasterVolume", -28 + (Maneger.OptionData.Volume[0] * 2));
                    }

                    break;

                //SE
                case 1:

                    {
                        Mixer.SetFloat("SEVolume", -3 + (Maneger.OptionData.Volume[1] * 1));
                    }

                    break;

                //BGM
                case 2:

                    {
                        Mixer.SetFloat("BGMVolume", -3 + (Maneger.OptionData.Volume[2] * 1));
                    }

                    break;
            }

            Maneger.OptionSave();

            Audio.PlayClips(0);

        }

        //キャンセル
        if (GetKeyDown(Set: KeybordSets.Cancel) || GetKeyDown(Button: XboxControllerButtons.A))
        {
            Audio.PlayClips(2);

            Color DarkColor = new Color32(0, 0, 100, 255);
            Color BrightColor = new Color32(180, 190, 255, 255);

            //背景の色を変える
            //AudioText選択は暗くする
            Tween BackDarkTween = OptionBack[1].GetComponent<Image>().DOColor(DarkColor, 0.5f);
            Tween BackBrightTween = OptionBack[0].GetComponent<Image>().DOColor(BrightColor, 0.5f);

            Sequence Sequence = DOTween.Sequence();

            enabled = false;

            //AudioCanvasのAlphaを下げる
            Tween AudioAlphaTween = gameObject.GetComponent<CanvasGroup>().DOFade(0f, 0.5f);

            //Lineを縮らせる
            Lines[NowLine].LineTweenOut(140,0.5f);

            Sequence.Append(BackDarkTween).Join(BackBrightTween).Join(AudioAlphaTween).OnComplete(() =>
            {
                //OptionButtonをオンにする
                OptionButton.enabled = true;

                //AudioCanvasをオフにする
                gameObject.SetActive(false);

            });

            Sequence.Play();
        }

        //コントローラーの入力取得
        if (InputManager.IsController)
        {
            Vertical = (Input.GetAxis("Dpad Vertical") * -1) + Input.GetAxis("Leftstick Vertical");
            Horizontal = (Input.GetAxis("Dpad Horizontal") * -1) + Input.GetAxis("Leftstick Horizontal");
        }
        else
        {
            Vertical = Input.GetAxis("Vertical");
            Horizontal = Input.GetAxis("Horizontal");
        }

    }

    public override bool GetKeyDown(KeyCode Key = KeyCode.None, XboxControllerButtons Button = XboxControllerButtons.None, KeybordSets Set = KeybordSets.None)
    {
        return base.GetKeyDown(Key, Button, Set);
    }

    IEnumerator StickVerticalStop()
    {
        VerticalStop = true;

        yield return new WaitForSeconds(0.4f);

        VerticalStop = false;
    }

    IEnumerator StickHorizontalStop()
    {
        HorizontalStop = true;

        yield return new WaitForSeconds(0.35f);

        HorizontalStop = false;
    }
}
