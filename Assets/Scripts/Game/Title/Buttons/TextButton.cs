using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TextButton : InputComponent
{
    public SaveDataManager Maneger;

    public OptionButton OptionButton;

    private AudioGeneral Audio;

    [System.NonSerialized]
    public TitleLineTween Line;
    private OptionTumamiTween Tumami;

    private Transform[] OptionBack = new Transform[2];

    private float LineWidth = 950;
    private float Horizontal;

    private bool HorizontalStop;

    private int[] NowTumamiPosi = new int[3];

    // Start is called before the first frame update
    void Start()
    {
        Audio=GetComponent<AudioGeneral>();

        Line = transform.GetChild(0).GetChild(0).GetComponent<TitleLineTween>();

        Tumami = transform.GetChild(0).Find("SliderTumami").GetComponent<OptionTumamiTween>();

        OptionBack[0] = transform.parent.GetChild(0).GetChild(0).Find("OptionBack01");
        OptionBack[1] = transform.parent.GetChild(0).GetChild(0).Find("OptionBack02");

        //Speedの線を入れる
        Line.LineTweenIn(LineWidth, 140, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {

        //左
        if (GetKeyDown(Set: KeybordSets.LeftArrow) || ((Horizontal < 0) && (!HorizontalStop)))
        {
            HorizontalStop = true;

            StartCoroutine("StickHorizontalStop");

            //もう左端にあったら処理しない
            if (Maneger.OptionData.TextSpeed < 1)
            {
                return;
            }

            Maneger.OptionData.TextSpeed--;
            Maneger.SpeedSet();

            Tumami.TumamiTween(new Vector2(45 + (Maneger.OptionData.TextSpeed * 120), 370));

            Maneger.OptionSave();

            Audio.PlayClips(0);

        }

        //右
        if (GetKeyDown(Set: KeybordSets.RightArrow) || ((Horizontal > 0) && (!HorizontalStop)))
        {
            HorizontalStop = true;

            StartCoroutine("StickHorizontalStop");

            //もう右端にあったら処理しない
            if (Maneger.OptionData.TextSpeed > 5)
            {
                return;
            }

            Maneger.OptionData.TextSpeed++;
            Maneger.SpeedSet();

            Tumami.TumamiTween(new Vector2(45 + (Maneger.OptionData.TextSpeed * 120), 370));

            Maneger.OptionSave();

            Audio.PlayClips(0);

        }

        //キャンセル
        if (GetKeyDown(Set: KeybordSets.Cancel) || GetKeyDown(Button: XboxControllerButtons.A))
        {
            Audio.PlayClips(1);

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
            Line.LineTweenOut(140, 0.5f);

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
        Horizontal = (Input.GetAxis("Dpad Horizontal") * -1) + Input.GetAxis("Leftstick Horizontal");
    }

    public override bool GetKeyDown(KeyCode Key = KeyCode.None, XboxControllerButtons Button = XboxControllerButtons.None, KeybordSets Set = KeybordSets.None)
    {
        return base.GetKeyDown(Key, Button, Set);
    }

    IEnumerator StickHorizontalStop()
    {
        yield return new WaitForSeconds(0.3f);

        HorizontalStop = false;
    }
}
