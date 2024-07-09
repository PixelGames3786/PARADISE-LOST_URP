using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AdventureTitleController : MonoBehaviour
{
    public AdventureController AC;

    private NormalCharaController Normal;
    private BattleCharaController Battle;

    private AudioGeneral Audio;

    private RectTransform TitleWindow,Line01,Line02, Title;

    private Image Fader;

    private Text TitleText;

    public string SceneName;

    public int NowCount, Special, Ivent;

    private bool Adding;

    // Start is called before the first frame update
    void Start()
    {
        Audio = GetComponent<AudioGeneral>();

        TitleWindow= transform.GetChild(0).GetComponent<RectTransform>();

        Line01 = TitleWindow.GetChild(0).GetComponent<RectTransform>();
        Line02 = TitleWindow.GetChild(1).GetComponent<RectTransform>();

        Title = transform.GetChild(1).GetComponent<RectTransform>();

        Fader = transform.GetChild(2).GetComponent<Image>();

        TitleText = Title.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Adding)
        {
            StartCoroutine("AddName");
        }
    }

    IEnumerator AddName()
    {
        Adding = false;

        TitleText.text += "_";

        yield return new WaitForSeconds(0.2f);

        string Addtext = "";

        for (int i = 0; i < NowCount; i++)
        {
            Addtext += SceneName[i];

            Audio.PlayClips(0);
        }

        TitleText.text = Addtext;

        NowCount++;

        //タイトル表示が終わったら
        if (NowCount > SceneName.Length)
        {
            yield return new WaitForSeconds(1f);

            AddNameEnd();
        }
        else
        {
            Adding = true;
        }
    }

    public void AddNameIn()
    {
        Normal = FindObjectOfType<NormalCharaController>();
        Battle = FindObjectOfType<BattleCharaController>();

        print(Special);

        //特殊系：タイトル表示スキップとか
        switch (Special)
        {
            //タイトル表示スキップ
            case 1:

                Fader.DOFade(0f, 1f).OnComplete(() =>
                {
                    FindObjectOfType<TargetController>().enabled = true;

                    if (AC.BattleMode)
                    {
                        AC.BC.MakeHPBer();
                    }

                    if (Normal)
                    {
                        Normal.enabled = true;
                        Normal.CanMove = true;
                    }
                    else
                    {
                        Battle.enabled = true;
                        Battle.CharactorMove();

                        /*
                        if (SaveDataManager.DataManage.Data.CanDash)
                        {
                            print("1");

                            AC.BC.HPC.DashC.Gauges[0].GaugeCharge();

                            print("2");

                        }

    */
                    }

                    if (BGMController.Controller.PlayAwake)
                    {
                        BGMController.Controller.BGMPlay(0);
                    }
                });

                return;

            //タイトル表示＆ターゲット表示スキップ
            case 2:

                Fader.DOFade(0f, 1f).OnComplete(() =>
                {

                    if (AC.BattleMode)
                    {
                        AC.BC.MakeHPBer();
                    }

                    if (Normal)
                    {
                        Normal.enabled = true;
                        Normal.CanMove = true;
                    }
                    else
                    {
                        Battle.enabled = true;
                        Battle.CharactorMove();

                        /*
                        if (SaveDataManager.DataManage.Data.CanDash)
                        {
                            AC.BC.HPC.DashC.Gauges[0].GaugeCharge();
                        }
                        */
                    }

                    if (BGMController.Controller.PlayAwake)
                    {
                        BGMController.Controller.BGMPlay(0);
                    }
                });

                return;

            //タイトル表示をスキップしてイベントがあったら
            case 3:

                Normal.enabled = true;

                Fader.DOFade(0f, 1f).OnComplete(() =>
                {
                    AC.IventStart(AC.NowScene.Ivents[Ivent]);
                });

                return;
        }

        Sequence sequence = DOTween.Sequence();

        Tween FaderTween = Fader.DOFade(0f, 0.8f);
        Tween WindowTween = TitleWindow.DOSizeDelta(new Vector2(120,130),0.6f);
        Tween BackLineTween1 = Line01.DOScaleX(1f, 0.3f);
        Tween BackLineTween2 = Line02.DOScaleX(1f, 0.3f);

        sequence.Append(FaderTween).Append(WindowTween).Append(BackLineTween2).Join(BackLineTween1).OnComplete(() =>
        {
            Adding = true;
        });

        sequence.Play();
    }

    public void AddNameEnd()
    {

        Adding = false;

        Line01.pivot = new Vector2(1, 0.5f);
        Line02.pivot = new Vector2(0, 0.5f);

        Line01.localPosition = new Vector3(100,-105, 0);
        Line02.localPosition = new Vector3(-100, 105, 0);

        Sequence sequence = DOTween.Sequence();

        Tween LineTween01 = Line01.DOScaleX(0f, 0.3f);
        Tween LineTween02 = Line02.DOScaleX(0f, 0.3f);

        Tween TitleTween = Title.DOScaleY(0f, 0.3f);

        Tween WindowTween = TitleWindow.DOScaleY(0f,0.5f);

        sequence.Append(LineTween01).Join(LineTween02).Join(TitleTween).Append(WindowTween).OnComplete(() =>
        {
            switch (Special)
            {
                //ターゲット表示スキップ
                case 4:

                    {
                        if (Normal)
                        {
                            Normal.enabled = true;
                            Normal.CanMove = true;
                        }
                        else
                        {
                            Battle.enabled = true;
                            Battle.CharactorMove();

                            /*
                            if (SaveDataManager.DataManage.Data.CanDash)
                            {
                                if (AC.BC.BCC.CanDashCount >= 1)
                                {
                                    AC.BC.HPC.DashC.SetCharge(AC.BC.BCC.CanDashCount - 1);
                                }
                                else
                                {
                                    AC.BC.HPC.DashC.Gauges[0].GaugeCharge();

                                }
                            }
                            */
                        }

                        if (BGMController.Controller.PlayAwake)
                        {
                            BGMController.Controller.BGMPlay(0);
                        }
                    }

                    break;

                //なんらかのイベントを行う場合
                case 5:

                    {
                        AC.IventStart(AC.NowScene.Ivents[Ivent]);
                    }

                    break;
            }

            if (Normal)
            {
                Normal.enabled = true;
                Normal.CanMove = true;
            }
            else
            {
                Battle.enabled = true;
                Battle.CharactorMove();

                /*
                if (SaveDataManager.DataManage.Data.CanDash)
                {
                    AC.BC.HPC.DashC.Gauges[0].GaugeCharge();
                }
                */
            }

            if (BGMController.Controller.PlayAwake)
            {
                BGMController.Controller.BGMPlay(0);
            }

            FindObjectOfType<TargetController>().enabled = true;

        });

        sequence.Play();
    }
}
