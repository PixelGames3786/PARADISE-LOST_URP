using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AdventureTitleIn : MonoBehaviour
{
    public AdventureController AC;

    private NormalCharaController Normal;
    private BattleCharaController Battle;

    private AudioGeneral Audio;

    private RectTransform Line01, Line02, Line03, Title;

    private Image Fader;

    private Text TitleText;

    private string SceneName;

    public int NowCount,Special,Ivent;

    private bool Adding;
    public bool SpecialLoadSkip;

    // Start is called before the first frame update
    void Start()
    {
        Audio = GetComponent<AudioGeneral>();

        Line01 = transform.GetChild(0).GetComponent<RectTransform>();
        Line02 = transform.GetChild(1).GetComponent<RectTransform>();
        Line03 = transform.GetChild(2).GetComponent<RectTransform>();

        Title = transform.GetChild(3).GetComponent<RectTransform>();

        Fader = transform.GetChild(4).GetComponent<Image>();

        TitleText = Title.GetComponent<Text>();

        SceneName = AC.NowScene.Name[0];

        if (!SpecialLoadSkip)
        {

            Special = int.Parse(AC.NowScene.Name[1]);
            Ivent = int.Parse(AC.NowScene.Name[2]);
        }

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

        for (int i=0;i<NowCount;i++)
        {
            Addtext+=SceneName[i];

            Audio.PlayClips(0);
        }

        TitleText.text = Addtext;

        NowCount++;

        //タイトル表示が終わったら
        if (NowCount>SceneName.Length)
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


        if (AC.BattleMode)
        {
            AC.BC.MakeHPBer();
        }

        print(Special);

        //特殊系：タイトル表示スキップとか
        switch (Special)
        {
            //タイトル表示スキップ
            case 1:

                Fader.DOFade(0f, 1f).OnComplete(() =>
                {
                    FindObjectOfType<TargetController>().enabled = true;

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
                });

                return;

            //タイトル表示＆ターゲット表示スキップ
            case 2:

                Fader.DOFade(0f, 1f).OnComplete(() =>
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
                    }

                    if (BGMController.Controller.PlayAwake)
                    {
                        BGMController.Controller.BGMPlay(0);

                    }
                });

                return;

            //タイトル表示をスキップしてイベントがあったら
            case 3:

                Fader.DOFade(0f, 1f).OnComplete(() =>
                {
                    AC.IventStart(AC.NowScene.Ivents[Ivent]);
                });

                return;
        }

        Sequence sequence = DOTween.Sequence();

        Tween FaderTween = Fader.DOFade(0f, 0.8f);
        Tween BackLineTween1 = Line01.DOScaleX(1f, 0.6f);
        Tween BackLineTween2 = Line02.DOScaleX(1f, 0.6f);
        Tween BackLineTween3 = Line03.DOScaleX(1f, 0.6f);

        sequence.Append(FaderTween).Join(BackLineTween1).Join(BackLineTween2).Join(BackLineTween3).OnComplete(() =>
        {
            Adding = true;
        });

        sequence.Play();
    }

    public void AddNameEnd()
    {

        Adding = false;

        Line01.pivot = new Vector2(1,0.5f);
        Line02.pivot = new Vector2(0,0.5f);
        Line03.pivot = new Vector2(0,0.5f);

        Line01.localPosition = new Vector3(960,0,0);
        Line02.localPosition = new Vector3(-960,120,0);
        Line03.localPosition = new Vector3(-960,-120,0);

        Sequence sequence = DOTween.Sequence();

        Tween LineTween01 = Line01.DOScaleX(0f,0.6f);
        Tween LineTween02 = Line02.DOScaleX(0f,0.6f);
        Tween LineTween03 = Line03.DOScaleX(0f,0.6f);

        Tween TitleTween = Title.DOScaleY(0f,0.3f);

        sequence.Append(LineTween01).Join(LineTween02).Join(LineTween03).Join(TitleTween).OnComplete(() => 
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
                        }

                        BGMController.Controller.BGMPlay(0);
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

                if (SaveDataManager.DataManage.Data.CanDash)
                {
                    AC.BC.HPC.DashC.Gauges[0].GaugeCharge();
                }
            }

            BGMController.Controller.BGMPlay(0);

            FindObjectOfType<TargetController>().enabled = true;

        });

        sequence.Play();
    }
}
