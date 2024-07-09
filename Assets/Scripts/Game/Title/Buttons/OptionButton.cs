using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using DG.Tweening;

public class OptionButton : InputComponent
{
    public SaveDataManager Maneger;

    private AudioGeneral Audio;

    private TitleLineTween[] Lines = new TitleLineTween[2];

    private Transform[] OptionBack=new Transform[2];

    public GameObject TitleCanvas;

    public Volume volume;

    public float[] LineWidth = new float[2];

    private float Vertical, CAValue, GrainValue;

    private bool VolumeChange,VerticalStop;

    private int NowLine;

    // Start is called before the first frame update
    void Start()
    {
        Audio = GetComponent<AudioGeneral>();

        Lines[0] = transform.GetChild(0).GetChild(1).GetComponent<TitleLineTween>();
        Lines[1] = transform.GetChild(1).GetChild(1).GetComponent<TitleLineTween>();

        OptionBack[0]= transform.parent.GetChild(0).Find("OptionBack01");
        OptionBack[1]= transform.parent.GetChild(0).Find("OptionBack02");

        //Audioの線を入れる Audio:510 Text:490

        Lines[0].LineTweenIn(510f, 20, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        //上
        if (Vertical > 0&&!VerticalStop)
        {
            StartCoroutine("VerticalStopChange");

            Vertical = 0f;

            NowLine--;

            if (NowLine < 0)
            {
                NowLine = 0;
            }
            else
            {
                Lines[NowLine].LineTweenIn(LineWidth[NowLine], 20, 0.3f);
                Lines[NowLine + 1].LineTweenOut(20,0.3f);

                Audio.PlayClips(1);
            }
        }

        //下
        if (Vertical < 0 && !VerticalStop)
        {
            StartCoroutine("VerticalStopChange");

            Vertical = 0f;

            NowLine++;

            if (NowLine > 1)
            {
                NowLine = 1;
            }
            else
            {
                Lines[NowLine].LineTweenIn(LineWidth[NowLine], 20, 0.3f);
                Lines[NowLine + -1].LineTweenOut(20,0.3f);

                Audio.PlayClips(1);

            }
        }

        //決定
        if (GetKeyDown(Set: KeybordSets.Decision) || GetKeyDown(Button: XboxControllerButtons.B))
        {
            Audio.PlayClips(0);

            Color DarkColor = new Color32(0, 0, 100, 255);
            Color BrightColor = new Color32(180, 190, 255, 255);

            //背景の色を変える
            //AudioText選択は暗くする
            Tween BackDarkTween = OptionBack[0].GetComponent<Image>().DOColor(DarkColor, 0.5f);
            Tween BackBrightTween = OptionBack[1].GetComponent<Image>().DOColor(BrightColor, 0.5f);

            //今選択している場所によって処理変更
            switch (NowLine)
            {
                //Audio
                case 0:

                    {
                        Transform TargetCanvas = transform.parent.parent.Find("AudioCanvas");

                        Sequence Sequence =DOTween.Sequence();

                        //AudioCanvasが既にtrueだったら重複を防ぐために処理をしない
                        if (TargetCanvas.gameObject.activeSelf)
                        {
                            return;
                        }

                        //AudioCanvasのAlphaを上げる
                        Tween AudioAlphaTween = TargetCanvas.GetComponent<CanvasGroup>().DOFade(1f,0.5f);

                        TargetCanvas.gameObject.SetActive(true);

                        //つまみの位置の初期設定
                        for (int i = 0; i < 3; i++)
                        {
                            RectTransform TumamiRect = TargetCanvas.GetChild(i).Find("SliderTumami").GetComponent<RectTransform>();

                            Vector2 TumamiPosition = new Vector2(45 + (Maneger.OptionData.Volume[i] * 120), 370);

                            TumamiRect.localPosition = TumamiPosition;
                        }

                        Sequence.Append(BackDarkTween).Join(BackBrightTween).Join(AudioAlphaTween).OnComplete(() =>
                        {
                            AudioButton AudioButton = TargetCanvas.GetComponent<AudioButton>();

                            //AudioButtonをオンにする
                            AudioButton.enabled = true;

                            //MasterLineを入れる
                            if (AudioButton.Lines!=null)
                            {
                                AudioButton.Lines[NowLine].LineTweenIn(950, 140, 0.5f);
                            }

                        });

                        Sequence.Play();

                        //OptionButtonをオフにする
                        enabled = false;
                    }

                    break;

                //Text
                case 1:

                    {
                        Transform TargetCanvas = transform.parent.parent.Find("TextCanvas");

                        Sequence Sequence = DOTween.Sequence();

                        //TextCanvasが既にtrueだったら重複を防ぐために処理をしない
                        if (TargetCanvas.gameObject.activeSelf)
                        {
                            return;
                        }

                        //AudioCanvasのAlphaを上げる
                        Tween AudioAlphaTween = TargetCanvas.GetComponent<CanvasGroup>().DOFade(1f, 0.5f);

                        TargetCanvas.gameObject.SetActive(true);

                        //つまみの位置の初期設定
                        RectTransform TumamiRect = TargetCanvas.GetChild(0).Find("SliderTumami").GetComponent<RectTransform>();

                        Vector2 TumamiPosition = new Vector2(45 + (Maneger.OptionData.TextSpeed * 120), 370);

                        TumamiRect.localPosition = TumamiPosition;


                        Sequence.Append(BackDarkTween).Join(BackBrightTween).Join(AudioAlphaTween).OnComplete(() =>
                        {
                            TextButton TextButton = TargetCanvas.GetComponent<TextButton>();

                            //TextButtonをオンにする
                            TextButton.enabled = true;

                            //SpeedLineをいれる
                            if (TextButton.Line)
                            {
                                TextButton.Line.LineTweenIn(950,140,0.5f);
                            }

                        });

                        Sequence.Play();

                        //OptionButtonをオフにする
                        enabled = false;
                    }

                    break;
            }

        }

        //キャンセル
        if (GetKeyDown(Set:KeybordSets.Cancel)||GetKeyDown(Button:XboxControllerButtons.A))
        {
            Audio.PlayClips(2);

            //TitleCanvasをオンに
            TitleCanvas.SetActive(true);

            Transform OptionCanvasParent = transform.parent.parent.transform;

            Tween OptionOut01 = OptionCanvasParent.DOScale(new Vector3(1.1f, 0.1f, 1f), 0.3f);
            Tween OptionOut02 = OptionCanvasParent.DOScale(new Vector3(0f, 0f, 0f), 0.5f);

            Sequence Sequence = DOTween.Sequence();

            Sequence.Append(OptionOut01).Append(OptionOut02).OnComplete(() =>
            {
                VolumeChange = false;

                GrainValue = 0f;
                CAValue = 0f;

                //TitleButtonをオンに
                TitleCanvas.transform.GetChild(1).GetComponent<TitleButton>().enabled = true;

                //OptionCanvasをオフにする
                transform.parent.gameObject.SetActive(false);
            });

            VolumeChange = true;

            Sequence.Play();

            enabled = false;
        }


        //HDRPVolumeの変更
        if (VolumeChange)
        {
            GrainValue -= Time.deltaTime;
            CAValue -= Time.deltaTime;

            GrainValue = Mathf.Clamp(GrainValue, 0f, 0.8f);
            CAValue = Mathf.Clamp(CAValue, 0.15f, 0.3f);


            if (volume.profile.TryGet(out ChromaticAberration Chroma))
            {
                ChromaticAberration CA = Chroma;

                CA.intensity.SetValue(new ClampedFloatParameter(CAValue, 0f, 1f, true));
            }

            if (volume.profile.TryGet(out FilmGrain Grain))
            {
                FilmGrain Gra = Grain;

                Grain.intensity.SetValue(new ClampedFloatParameter(GrainValue, 0f, 1f, true));
            }
        }

        //コントローラーの入力取得
        if (InputManager.IsController)
        {
            Vertical = (Input.GetAxis("Dpad Vertical") * -1) + Input.GetAxis("Leftstick Vertical");
        }
        else
        {
            Vertical = Input.GetAxis("Vertical");
        }
    }

    public override bool GetKeyDown(KeyCode Key = KeyCode.None, XboxControllerButtons Button = XboxControllerButtons.None, KeybordSets Set = KeybordSets.None)
    {
        return base.GetKeyDown(Key, Button, Set);
    }

    IEnumerator VerticalStopChange()
    {
        VerticalStop = true;

        yield return new WaitForSeconds(0.25f);

        VerticalStop = false;

    }
}
