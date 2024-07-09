using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneFader : MonoBehaviour
{
    public enum FadeType
    {
        Black, Loading, GameOver,GetUp
    }

    static SceneFader scenefader;

    private static CanvasGroup canvasGroup;

    private static bool Fading;

    public CanvasGroup faderCanvasGroup;
    public CanvasGroup loadingCanvasGroup;
    public CanvasGroup gameOverCanvasGroup;
    public CanvasGroup getupCanvasGroup;
    public float fadeDuration = 1f;

    private static FadeType Type;

    private static RectTransform LoadingRect;
    private static Image LoadingBack;

    public static Color32 BackColor;

    private string NowSceneName;

    public static SceneFader Fader
    {
        get { return scenefader ?? (scenefader = FindObjectOfType<SceneFader>()); }
    }


    void Awake()
    {
        // 自身がインスタンスでなければ自滅
        if (this != Fader)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        NowSceneName = SceneManager.GetActiveScene().name;
    }

    void Update()
    {
        if (Fading)
        {
            if (NowSceneName!=SceneManager.GetActiveScene().name)
            {

                FadeSceneIn();

                NowSceneName = SceneManager.GetActiveScene().name;

                SceneController.Controller.SceneLoad.allowSceneActivation = false;

                return;
            }

            float LoadProgress = SceneController.Controller.SceneLoad.progress;

            //次のシーンがロードできたなら
            if (LoadProgress>=0.9f)
            {

                SceneController.Controller.SceneLoad.allowSceneActivation = true;

                LoadProgress = 0;
            }
        }
    }

    public static void FadeSceneIn()
    {
        //canvasGroup.gameObject.SetActive(true);

        Fading = false;

        Sequence Sequence = DOTween.Sequence();

        switch (Type)
        {
            case FadeType.Black:

                {
                    Tween FadeTween = canvasGroup.DOFade(0f, 0.35f);

                    //黒い幕に完全に覆われたら
                    Sequence.Append(FadeTween).OnComplete(()=> 
                    {
                        canvasGroup.gameObject.SetActive(false);

                        switch (SaveDataManager.DataManage.Data.Scene)
                        {
                            //ノベルパートだったら
                            case SaveData.SceneType.Novel:

                                {
                                    NovelController NC = FindObjectOfType<NovelController>();
                                    TextController TC = NC.TC;

                                    Sequence sequence = DOTween.Sequence();

                                    //黒い幕を透明にする
                                    Tween FaderTween = GameObject.Find("BlackFader").GetComponent<Image>().DOFade(0f, 0.8f);

                                    sequence.Append(FadeTween).OnComplete(() =>
                                    {
                                        //コンティニューしてたら
                                        if (SceneController.Controller.ContinueFlag)
                                        {
                                            TC.LoadNovelData();
                                        }
                                        else
                                        {
                                            NC.SetFirstScene();
                                        }

                                    });
                                }

                                break;

                            //アドベンチャーパートだったら
                            case SaveData.SceneType.Adventure:

                                {
                                    //特殊な状況
                                    if (SceneController.Controller.SpecialNum != 0)
                                    {
                                        SpecialJudge();
                                    }
                                    else
                                    {

                                        AdventureController ac = FindObjectOfType<AdventureController>();

                                        if (SceneController.Controller.MapTrans)
                                        {
                                            ac.SetStartPosition(SceneController.Controller.PositionNum);

                                            SceneController.Controller.MapTrans = false;
                                        }

                                        print("SetScene");

                                        ac.SetScene();
                                    }

                                }

                                break;
                        }

                    });

                    Sequence.Play();

                }

                break;

            case FadeType.Loading:

                {
                    LoadingRect.pivot = new Vector2(1, 0.5f);
                    LoadingRect.localPosition = new Vector2(960, 0);

                    //Tween LoadingTween = LoadingRect.DOSizeDelta(new Vector2(0, 1080), 0.4f);
                    Tween MaterialTween = LoadingRect.GetComponent<Image>().material.DOFloat(-5, "_Sinkou", 0.4f);

                    //Tween LoadingTween = LoadingRect.DOSizeDelta(new Vector2(0, 320), 0.4f);
                    Tween FadeTween = canvasGroup.DOFade(0f, 0.35f);

                    Sequence.Append(MaterialTween).Append(FadeTween).OnComplete(() =>
                    {
                        canvasGroup.gameObject.SetActive(false);

                        print(SaveDataManager.DataManage.Data.Scene);
                        print(SceneController.Controller.ContinueFlag);

                        switch (SaveDataManager.DataManage.Data.Scene)
                        {
                            //ノベルパートだったら
                            case SaveData.SceneType.Novel:

                                {
                                    NovelController NC = FindObjectOfType<NovelController>();
                                    TextController TC = NC.TC;

                                    Sequence sequence = DOTween.Sequence();

                                    Tween FaderTween = GameObject.Find("BlackFader").GetComponent<Image>().DOFade(0f, 0.6f);

                                    sequence.Append(FadeTween).OnComplete(() =>
                                    {
                                        //コンティニューしてたら
                                        if (SceneController.Controller.ContinueFlag)
                                        {
                                            TC.LoadNovelData();
                                        }
                                        else
                                        {
                                            NC.SetFirstScene();
                                        }
                                    });
                                }

                                break;

                            //アドベンチャーパートだったら
                            case SaveData.SceneType.Adventure:

                                {
                                    //特殊な状況
                                    if (SceneController.Controller.SpecialNum!=0)
                                    {
                                        SpecialJudge();
                                    }
                                    else
                                    {
                                        AdventureController ac = FindObjectOfType<AdventureController>();

                                        if (SceneController.Controller.MapTrans)
                                        {
                                            ac.SetStartPosition(SceneController.Controller.PositionNum);

                                            SceneController.Controller.MapTrans = false;
                                        }

                                        print(SceneController.Controller.PositionNum);

                                        ac.SetScene();
                                    }

                                }

                                break;
                        }
                    });

                    Sequence.Play();
                }

                break;

            case FadeType.GameOver:

                {
                    Tween FadeTween = canvasGroup.DOFade(0f, 0.6f);

                    Sequence.Append(FadeTween).OnComplete(()=> 
                    {
                        canvasGroup.gameObject.SetActive(false);

                        SaveDataManager.DataManage.AllLoad();

                    });
                }

                break;

            case FadeType.GetUp:

                {

                    Sequence.AppendInterval(1f).OnComplete(()=> 
                    {
                        Sequence sequence = DOTween.Sequence();

                        canvasGroup.transform.GetChild(1).GetComponent<Animator>().SetTrigger("GetUpTrigger");

                        sequence.AppendInterval(1f).Append(canvasGroup.DOFade(0f, 0.1f)).OnComplete(() =>
                        {
                            canvasGroup.gameObject.SetActive(false);

                            //特殊な状況
                            if (SceneController.Controller.SpecialNum != 0)
                            {
                                SpecialJudge();
                            }
                            else
                            {
                                AdventureController ac = FindObjectOfType<AdventureController>();

                                if (SceneController.Controller.MapTrans)
                                {
                                    ac.SetStartPosition(SceneController.Controller.PositionNum);

                                    SceneController.Controller.MapTrans = false;
                                }

                                print(SceneController.Controller.PositionNum);

                                ac.SetScene();
                            }
                        });
                    });
                }

                break;
        }
    }

    public static void FadeSceneOut(FadeType fadeType = FadeType.Black)
    {
        Type = fadeType;

        switch (fadeType)
        {
            case FadeType.Black:
                canvasGroup = Fader.faderCanvasGroup;
                break;
            case FadeType.GameOver:
                canvasGroup = Fader.gameOverCanvasGroup;
                break;
            case FadeType.Loading:
                canvasGroup = Fader.loadingCanvasGroup;

                LoadingRect = canvasGroup.transform.GetChild(1).GetComponent<RectTransform>();
                LoadingBack = canvasGroup.transform.GetChild(0).GetComponent<Image>();

                break;
            case FadeType.GetUp:

                canvasGroup = Fader.getupCanvasGroup;
                break;
        }

        //Canvasをオンに
        canvasGroup.gameObject.SetActive(true);

        Sequence Sequence = DOTween.Sequence();

        Tween FadeTween = canvasGroup.DOFade(1f, 0.35f);

        switch (fadeType)
        {
            case FadeType.Black:

                Sequence.Append(FadeTween).OnComplete(()=> 
                {
                    Fading = true;

                    SceneController.Controller.StartSceneLoad();
                });

                break;

            case FadeType.Loading:

                {
                    LoadingBack.color = BackColor;

                    LoadingRect.pivot = new Vector2(0f, 0.5f);
                    LoadingRect.localPosition = new Vector2(-960, 0);

                    LoadingRect.GetComponent<Image>().material.SetFloat("_Sinkou",6f);

                    //Tween LoadingTween = LoadingRect.DOSizeDelta(new Vector2(1400, 320), 0.4f);
                    //Tween LoadingTween = LoadingRect.DOSizeDelta(new Vector2(1920, 1080), 0.4f);
                    Tween MaterialTween = LoadingRect.GetComponent<Image>().material.DOFloat(0,"_Sinkou",0.4f);

                    Sequence.Append(FadeTween).Append(MaterialTween).OnComplete(() => 
                    {

                        if (!Fading)
                        {
                            SceneController.Controller.StartSceneLoad();
                        }

                        Fading = true;
                    });

                }

                break;

            case FadeType.GameOver:

                {
                    canvasGroup.transform.GetChild(1).GetComponent<Animator>().SetTrigger("GameOver");

                    canvasGroup.alpha = 1;

                    Time.timeScale = 1f;

                    Fading = true;

                    Sequence.AppendInterval(2f).OnComplete(() =>
                    {
                        SceneController.Controller.StartSceneLoad();
                    });
                }

                break;

            case FadeType.GetUp:

                {
                    Sequence.Append(FadeTween).OnComplete(() =>
                    {
                        Fading = true;

                        SceneController.Controller.StartSceneLoad();
                    });
                }

                break;
        }


        Sequence.Play();


    }

    public static void SpecialJudge()
    {
        switch (SceneController.Controller.SpecialNum)
        {
            //SceneFaderを明るくせずにイベント起動
            case 1:

                {
                    AdventureController ac = FindObjectOfType<AdventureController>();

                    if (SceneController.Controller.MapTrans)
                    {
                        ac.SetStartPosition(SceneController.Controller.PositionNum);

                        SceneController.Controller.MapTrans = false;
                    }

                    if (SceneController.Controller.Status==SceneController.CharaStatus.Battle)
                    {
                        GameObject.Find("Charactor").GetComponent<BattleCharaController>().enabled = true;
                    }
                    else
                    {
                        GameObject.Find("Charactor").GetComponent<NormalCharaController>().enabled = true;
                    }

                    print(ac.NowScene.Ivents.Count);

                    AdventureIvent Ivent = ac.NowScene.Ivents.Find((ivent) => ivent.ID == SceneController.Controller.IventNum.ToString());

                    ac.IventStart(Ivent);

                    Tween FaderTween = GameObject.Find("Fader").GetComponent<Image>().DOFade(0f, 0.6f);

                }

                break;

            //TitleInの後に発生するイベントを設定する
            case 2:

                {
                    AdventureController ac = FindObjectOfType<AdventureController>();

                    if (SceneController.Controller.MapTrans)
                    {
                        ac.SetStartPosition(SceneController.Controller.PositionNum);

                        SceneController.Controller.MapTrans = false;
                    }

                    AdventureTitleController Script = FindObjectOfType<AdventureTitleController>();

                    //AdventureTitleIn Script = FindObjectOfType<AdventureTitleIn>();

                    Script.Special = SceneController.Controller.IventNum;
                    Script.Ivent = int.Parse(SceneController.Controller.OtherElements[0]);
                    
                    ac.SpecialLoadSkip = true;

                    ac.SetScene();
                }

                break;
        }
    }

    void OnDestroy()
    {

        // ※破棄時に、登録した実体の解除を行なっている

        // 自身がインスタンスなら登録を解除
        if (this == Fader) scenefader = null;

    }
}
