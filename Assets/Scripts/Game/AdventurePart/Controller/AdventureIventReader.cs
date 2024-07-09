using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AdventureIventReader
{
    private enum AnimeWait
    {
        Seni,
        AnimeEnd
    }

    private AnimeWait Waiter;

    private bool AnimeWaiting,FadeDoing,SecondWaiting,FadeWaiting;

    private int NowState,WaitState,FadeDoLines;

    private float WaitTime,WaitTargetTime,FaderWaitTargetTime;

    private Animator WaitAnimator;

    private Image BlackFader;



    private AdventureController AC;

    private AdventureTextController ATC;

    private AdventureSpecialIvent ASI;

    public AdventureIventReader(AdventureController ac,AdventureTextController atc)
    {
        AC = ac;

        ATC = atc;

        ASI = ac.gameObject.GetComponent<AdventureSpecialIvent>();

        ASI.AIR = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    //何かを待つときにAdventureControllerから呼び出してもらうやつ
    public void GeneralWaiting()
    {
        if (AnimeWaiting)
        {
            AnimeWaitCheck();
        }

        if (SecondWaiting)
        {
            SecondWaitCheck();
        }

        if (FadeWaiting)
        {
            FadeWaitCheck();
        }
    }

    private void FadeWaitCheck()
    {
        WaitTime += Time.deltaTime;

        if (WaitTime>=FaderWaitTargetTime)
        {
            FadeWaiting = false;

            WaitTime = 0;
            FaderWaitTargetTime = 0;

            BlackFader.DOFade(0, 0.5f).OnComplete(() =>
            {
                ReadLines(AC.NowIvent);
            });
        }
    }

    private void AnimeWaitCheck()
    {
        if (WaitState==0)
        {
            if (NowState!=WaitAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash)
            {
                WaitState = WaitAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash;
            }
        }
        else
        {
            switch (Waiter)
            {
                case AnimeWait.Seni:

                    {
                        if (WaitState != WaitAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash)
                        {
                            AnimeWaiting = false;

                            ReadLines(AC.NowIvent);

                            WaitState = 0;
                        }
                    }

                    break;

                case AnimeWait.AnimeEnd:

                    {
                        if (WaitAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                        {
                            AnimeWaiting = false;

                            ReadLines(AC.NowIvent);

                            WaitState = 0;
                        }
                    }

                    break;
            }
        }

    }

    private void SecondWaitCheck()
    {
        WaitTime += Time.deltaTime;

        if (WaitTime>=WaitTargetTime)
        {

            SecondWaiting = false;

            WaitTime = 0;

            ReadLines(AC.NowIvent);

        }
    }

    public void ReadLines(AdventureIvent Ivent)
    {
        if (Ivent.Index >= Ivent.Lines.Count) return;
        if (AnimeWaiting||FadeWaiting) return;

        int Count=0;

        var line = Ivent.Lines[Ivent.Index];
        var text = "";


        if (line.Contains("#"))
        {
            while (true)
            {
                Debug.Log(line);

                if (!line.Contains("#")) break;

                line = line.Replace("#","");
                line = line.Replace("\t", "");

                //いったん中止
                if (line.Contains("IventStop"))
                {
                    return;
                }

                //会話を開始
                else if (line.Contains("NovelStart"))
                {
                    Ivent.Index++;

                    line = line.Replace("NovelStart=", "");

                    var Content = line.Split(',');

                    AC.NovelStart(Content);

                    break;
                }
                //会話を終了
                else if (line.Contains("NovelEnd"))
                {
                    Ivent.Index++;

                    line = line.Replace("NovelEnd=", "");

                    AC.NovelEnd(int.Parse(line));

                    ATC.CanText = false;

                    //直後がイベント終了だったら
                    if (Ivent.Lines[Ivent.Index].Contains("Iventend"))
                    {
                        Ivent.Index = 0;

                        return;
                    }

                    //イベントを続ける時
                    if (line == "2")
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }

                }
                //中央ぞろえとか変更
                else if (line.Contains("AlignmentChange"))
                {
                    line = line.Replace("AlignmentChange=","");

                    ATC.Text.alignment = (TextAnchor)Enum.Parse(typeof(TextAnchor), line);
                }

                //新立ち絵作成
                else if (line.Contains("AddCharaImage"))
                {
                    line = line.Replace("AddCharaImage=", "");

                    var content = line.Split(',');

                    if (content.Length==6)
                    {
                        ATC.AddCharactor(content[0], content[1], content[2], content[3], content[5]);

                    }
                    else
                    {
                        ATC.AddCharactor(content[0], content[1], content[2], content[3]);

                    }


                    if (content[4] == "0")
                    {
                        Ivent.Index++;

                        break;
                    }
                    else if (content[4] == "1")
                    {

                    }
                }
                //立ち絵変更
                else if (line.Contains("CharaImageChange"))
                {
                    line = line.Replace("CharaImageChange=", "");

                    var content = line.Split(',');

                    ATC.ChangeCharactor(content[0], content[1]);

                    /*

                    if (content[2] == "0")
                    {
                        Ivent.Index++;

                        break;
                    }

    */
                }
                //立ち絵を暗くする
                else if (line.Contains("CharaImageDim"))
                {
                    line = line.Replace("CharaImageDim=", "");

                    var Content = line.Split(',');

                    ATC.CharactorDim(Content[0]);
                }
                //立ち絵を明るくする
                else if (line.Contains("CharaImageBright"))
                {
                    line = line.Replace("CharaImageBright=", "");

                    var Content = line.Split(',');

                    ATC.CharactorBright(Content[0]);
                }
                //立ち絵フェードアウト
                else if (line.Contains("CharaImageFadeOut"))
                {
                    line = line.Replace("CharaImageFadeOut=", "");

                    ATC.CharactorFade(line, 0);
                }
                //立ち絵フェードイン
                else if (line.Contains("CharaImageFadeIn"))
                {
                    line = line.Replace("CharaImageFadeIn=", "");

                    ATC.CharactorFade(line, 1);
                }
                //立ち絵アルファ値指定フェード
                else if (line.Contains("CharaImageFadeShitei"))
                {
                    line = line.Replace("CharaImageFadeShitei=","");

                    var content = line.Split(',');

                    ATC.CharactorFade(content[0], float.Parse(content[1]));
                }
                //立ち絵動かし
                else if (line.Contains("CharaImageMove"))
                {
                    line = line.Replace("CharaImageMove=", "");

                    var content = line.Split(',');

                    ATC.CharactorMove(content[0], content[1], content[2]);
                }
                //立ち絵けし
                else if (line.Contains("CharaImageDestroy"))
                {
                    line = line.Replace("CharaImageDestroy=", "");

                    ATC.CharactorDestroy(line);
                }
                //立ち絵方向チェンジ
                else if (line.Contains("CharaScaleChange"))
                {
                    line = line.Replace("CharaScaleChange=", "");

                    var content = line.Split(',');

                    ATC.CharactorScaleChange(content[0], float.Parse(content[1]), float.Parse(content[2]));
                }



                //喋るときのSEを変える
                else if (line.Contains("SENumChange"))
                {
                    line = line.Replace("SENumChange=", "");

                    ATC.SENumber = int.Parse(line);
                }

                //小さい吹き出しを出す
                else if (line.Contains("FukidashiIn"))
                {
                    line = line.Replace("FukidashiIn=", "");

                    var content = line.Split(',');

                    AC.MakeFukidashi(content);
                }
                //小さい吹き出しを小さくしてから破壊する
                else if (line.Contains("FukidashiOut"))
                {
                    AC.FukidashiOut();
                }
                //吹き出しを消す
                else if (line.Contains("FukidashiDestroy"))
                {
                    AC.DestroyFukidashi();
                }

                //BGMを流す
                else if (line.Contains("BGMStart"))
                {
                    line = line.Replace("BGMStart=", "");

                    BGMController.Controller.BGMPlay(int.Parse(line));
                }
                //BGMをフェードイン
                else if (line.Contains("BGMFadeIn"))
                {
                    BGMController.Controller.BGMFadeIn();

                }
                //BGMをフェードアウト
                else if (line.Contains("BGMFadeOut"))
                {
                    BGMController.Controller.BGMFadeOut();
                }
                //BGMのボリュームを変える
                else if (line.Contains("BGMVolume"))
                {
                    line = line.Replace("BGMVolume=", "");

                    BGMController.Controller.VolumeChange(float.Parse(line));
                }
                //BGMを止める
                else if (line.Contains("BGMPause"))
                {
                    BGMController.Controller.BGMPause();
                }
                //一度止めたBGMを再生する
                else if (line.Contains("BGMResume"))
                {
                    BGMController.Controller.BGMUnPause();
                }


                //スピーカーウィンドウを入れる
                else if (line.Contains("SpeakerWindowIn"))
                {
                    line = line.Replace("SpeakerWindowIn=", "");

                    var Content = line.Split(',');

                    ATC.SpeakerWindowIn(Content[0], Content[1]);

                    if (Content[2] == "0")
                    {
                        Ivent.Index++;

                        break;
                    }
                }
                //スピーカーウィンドウを出す
                else if (line.Contains("SpeakerWindowOut"))
                {
                    line = line.Replace("SpeakerWindowOut=","");

                    var content = line.Split(',');

                    ATC.SpeakerWindowOut(content[0]);

                    if (content[1] == "0")
                    {
                        Ivent.Index++;

                        break;
                    }
                }
                //スピーカーウィンドウの名前を変える
                else if (line.Contains("SpeakerWindowChange"))
                {
                    line = line.Replace("SpeakerWindowChange=", "");

                    var Content = line.Split(',');

                    ATC.SpeakerWindowChange(Content[0], Content[1]);
                }


                //カメラの追従をオフにする
                else if (line.Contains("MovieIn"))
                {
                    line = line.Replace("MovieIn=", "");

                    var Content = line.Split(',');

                    Vector3 CameraPosi = new Vector3();

                    if (Content[0] != "Chara")
                    {
                        CameraPosi = new Vector3(float.Parse(Content[0]), float.Parse(Content[1]), -10);
                    }
                    else
                    {
                        float CharaX = GameObject.Find("Charactor").transform.position.x;

                        CameraPosi = new Vector3(CharaX, float.Parse(Content[1]), -10);
                    }

                    if (Camera.main.GetComponent<CameraFollower>())
                    {
                        float[] ClampX = new float[2];

                        ClampX[0] = Camera.main.GetComponent<CameraFollower>().MiniPosition.x;
                        ClampX[1] = Camera.main.GetComponent<CameraFollower>().MaxPosition.x;

                        CameraPosi = new Vector3(Mathf.Clamp(CameraPosi.x, ClampX[0], ClampX[1]), CameraPosi.y, CameraPosi.z);
                    }

                    Camera.main.GetComponent<CameraFollower>().enabled = false;
                    Camera.main.transform.position = CameraPosi;
                }
                //カメラの追従をオンにする
                else if (line.Contains("MovieOut"))
                {
                    line = line.Replace("MovieOut=", "");

                    Vector3 CameraPosi = new Vector3();

                    var Content = line.Split(',');

                    if (Content[0] != "Chara")
                    {
                        CameraPosi = new Vector3(float.Parse(Content[0]), float.Parse(Content[1]), -10);
                    }
                    else
                    {
                        float CharaX = GameObject.Find("Charactor").transform.position.x;

                        CameraPosi = new Vector3(CharaX, float.Parse(Content[1]), -10);
                    }

                    Camera.main.GetComponent<CameraFollower>().enabled = true;
                    Camera.main.transform.position = CameraPosi;
                }
                //カメラを移動させる
                else if (line.Contains("CameraMove"))
                {
                    line = line.Replace("CameraMove=", "");

                    var Content = line.Split(',');

                    Vector3 EndPosition = new Vector3();

                    if (Content[0] == "Chara")
                    {
                        EndPosition = new Vector3(GameObject.Find("Charactor").transform.position.x, float.Parse(Content[1]), float.Parse(Content[2]));
                    }
                    else
                    {
                        EndPosition = new Vector3(float.Parse(Content[0]), float.Parse(Content[1]), float.Parse(Content[2]));
                    }

                    if (Camera.main.GetComponent<CameraFollower>())
                    {
                        float[] ClampX=new float[2];

                        ClampX[0] = Camera.main.GetComponent<CameraFollower>().MiniPosition.x;
                        ClampX[1] = Camera.main.GetComponent<CameraFollower>().MaxPosition.x;

                        EndPosition = new Vector3(Mathf.Clamp(EndPosition.x,ClampX[0],ClampX[1]),EndPosition.y,EndPosition.z);
                    }

                    Camera.main.transform.DOMove(EndPosition, float.Parse(Content[3]));
                }

                //オブジェクトの移動
                else if (line.Contains("ObjectSetPosi"))
                {
                    line = line.Replace("ObjectSetPosi=","");

                    var content = line.Split(',');

                    GameObject Target = GameObject.Find(content[0]);

                    Vector3 TargetPosi = new Vector3(float.Parse(content[2]),float.Parse(content[3]));

                    if (content[1]=="World")
                    {
                        Target.transform.position = TargetPosi;
                    }
                    //Local
                    else
                    {
                        Target.transform.localPosition = TargetPosi;
                    }
                }


                //アイテムウィンドウを作成
                else if (line.Contains("AddItemImage"))
                {
                    line = line.Replace("AddItemImage=", "");

                    var content = line.Split(',');

                    ATC.AddItemWindow(content);

                    if (content[3] == "0")
                    {
                        Ivent.Index++;

                        break;
                    }
                }
                //アイテムウィンドウを消す
                else if (line.Contains("ItemImageClose"))
                {
                    ATC.ItemWindow.ScaleOut();

                    line = line.Replace("ItemImageClose=", "");

                    if (line == "0")
                    {
                        Ivent.Index++;

                        break;
                    }
                }

                //戦闘関連
                else if (line.Contains("BattleStart"))
                {
                    line = line.Replace("BattleStart=", "");

                    var content = line.Split(',');

                    AC.BattleStart(content);
                }

                //通常状態から戦闘状態へチェンジ
                else if (line.Contains("NormalBattleChange"))
                {
                    AC.NormalBattleChange();
                }

                else if (line.Contains("BTIventStart"))
                {
                    line = line.Replace("BTIventStart=", "");

                    var content = line.Split(',');

                    AC.BTIventStart(content);
                }

                //キャラを動かせるようにする
                else if (line.Contains("CharaCanMove"))
                {
                    //通常状態
                    if (!AC.BattleMode)
                    {
                        AC.CC.CanMove = true;
                    }
                    //戦闘状態
                    else
                    {
                        AC.BC.BCC.CharactorMove();
                    }
                }
                //キャラを止める
                else if (line.Contains("CharaStop"))
                {
                    //通常状態
                    if (!AC.BattleMode)
                    {
                        AC.CC.CanMove = false;

                        AC.CC.CharaInitialize();
                    }
                    //戦闘状態
                    else
                    {
                        AC.BC.BCC.CharactorStop();
                    }
                }
                //キャラのアニメを設定する
                else if (line.Contains("CharaInitialize"))
                {
                    //通常状態
                    if (!AC.BattleMode)
                    {
                        AC.CC.CharaAnimator.SetBool("Walking",false);
                    }
                    //戦闘状態
                    else
                    {
                        AC.BC.BCC.CharaInitialize();
                    }
                }
                //キャラのスクリプトのEnable設定
                else if (line.Contains("CharaEnable"))
                {
                    line = line.Replace("CharaEnable=","");

                    var Enable = Boolean.Parse(line);

                    //通常状態
                    if (!AC.BattleMode)
                    {
                        AC.CC.enabled = Enable;
                    }
                    //戦闘状態
                    else
                    {
                        AC.BC.enabled = Enable;
                    }
                }

                //アニメ関連
                //アニメの終わり待ち
                else if (line.Contains("AnimeWait"))
                {
                    Ivent.Index++;

                    line = line.Replace("AnimeWait=", "");

                    var Content = line.Split(',');

                    WaitAnimator = GameObject.Find(Content[0]).GetComponent<Animator>();

                    NowState = WaitAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash;

                    Enum.TryParse(Content[1], out Waiter);

                    AnimeWaiting = true;

                    return;
                }
                //アニメーションTriggerとかBoolセット
                else if (line.Contains("AnimeSet"))
                {
                    line = line.Replace("AnimeSet=", "");

                    var Content = line.Split(',');

                    //Animator取得
                    var Animator = GameObject.Find(Content[0]).GetComponent<Animator>();

                    Animator.enabled = true;

                    //TriggerかBoolか
                    switch (Content[1])
                    {
                        case "Trigger":

                            {
                                Animator.SetTrigger(Content[2]);
                            }

                            break;
                    }
                }
                //アニメーターをセット
                else if (line.Contains("AnimatorSet"))
                {
                    line = line.Replace("AnimatorSet=", "");

                    var Content = line.Split(',');

                    //変える場所のアニメーター取得
                    var ChangeAnimator = GameObject.Find(Content[0]).GetComponent<Animator>();

                    //変える用のアニメーター取得
                    var Animator = Resources.Load<RuntimeAnimatorController>(Content[1]);

                    ChangeAnimator.runtimeAnimatorController = Animator;
                }

                //フラグ関連
                //フラグ追加
                else if (line.Contains("FlagAdd"))
                {
                    line = line.Replace("FlagAdd=","");

                    var Content = line.Split(',');

                    Debug.Log(Content[0]);

                    //もうそのフラグがなかったら追加する あったら中身を変える
                    if (!SaveDataManager.DataManage.Data.GameFlags.ContainsKey(Content[0]))
                    {
                        SaveDataManager.DataManage.Data.GameFlags.Add(Content[0], Convert.ToBoolean(Content[1]));
                    }
                    else
                    {
                        SaveDataManager.DataManage.Data.GameFlags[Content[0]] = Convert.ToBoolean(Content[1]);
                    }

                }
                //フラグ削除
                else if (line.Contains("FlagDelete"))
                {
                    line = line.Replace("FlagDelete=","");

                    //そのフラグを削除する
                    if (SaveDataManager.DataManage.Data.GameFlags.ContainsKey(line))
                    {
                        SaveDataManager.DataManage.Data.GameFlags.Remove(line);
                    }
                }
                //フラグ真偽確認
                else if (line.Contains("FlagCheck"))
                {
                    line = line.Replace("FlagCheck=", "");

                    //0:フラグ名前 1:true時移動 2:false時移動
                    var Content = line.Split(',');

                    //(0の場合は移動なし)

                    //そのキーが存在していたら
                    if (SaveDataManager.DataManage.Data.GameFlags.ContainsKey(Content[0]))
                    {
                        //true時
                        if (SaveDataManager.DataManage.Data.GameFlags[Content[0]])
                        {
                            if (Content[1] != "0")
                            {
                                Ivent.Index = Ivent.IventScene[Content[1]];
                            }
                        }
                        //false時
                        else
                        {
                            if (Content[2] != "0")
                            {
                                Ivent.Index = Ivent.IventScene[Content[2]];

                            }
                        }
                    }
                    //存在していない場合はfalse扱いにする
                    else
                    {
                        if (Content[2] != "0")
                        {
                            Ivent.Index = Ivent.IventScene[Content[2]];
                        }
                    }



                }

                //イベントシーンチェンジ
                else if (line.Contains("next"))
                {
                    var ivscene = line.Replace("next=","");

                    Ivent.Index = Ivent.IventScene[ivscene];
                }

                //暗転する
                else if (line.Contains("BlackFade"))
                {
                    Ivent.Index++;

                    line = line.Replace("BlackFade=","");

                    var Content = line.Split(',');

                    //黒い幕を探す
                    BlackFader = GameObject.Find(Content[0]).GetComponent<Image>();

                    Tween FadeIn = BlackFader.DOFade(1,1f);

                    //待ち時間を取得
                    float Wait = float.Parse(Content[1]);

                    //黒幕を暗転する
                    FadeIn.OnComplete(() =>
                    {
                        //暗転中に行うことが何もなかったら
                        if (Content[2]=="0")
                        {
                            //もし待ち時間があったら
                            if (Wait>0)
                            {
                                FadeWaiting = true;

                                FaderWaitTargetTime = Wait;
                            }
                            else
                            {
                                Tween FadeOut = BlackFader.DOFade(0, 0.5f);

                                FadeOut.OnComplete(() =>
                                {
                                    ReadLines(AC.NowIvent);
                                });

                                FadeOut.Play();
                            }

                        }
                        else
                        {
                            //もし待ち時間があったら
                            if (Wait>0)
                            {
                                FaderWaitTargetTime = Wait;
                            }

                            //暗転中に行うイベントの数を設定
                            FadeDoLines = int.Parse(Content[2]);

                            FadeDoing = true;

                            ReadLines(AC.NowIvent);
                        }

                    });

                    FadeIn.Play();

                    return;
                }

                //指定時間分待つ
                else if (line.Contains("WaitSeconds"))
                {
                    Ivent.Index++;

                    line = line.Replace("WaitSeconds=","");

                    WaitTargetTime = float.Parse(line);

                    SecondWaiting = true;

                    WaitTime = 0;

                    return;
                }

                //シーンチェンジ 今はあんまりつかってない
                else if (line.Contains("SceneChange"))
                {
                    line = line.Replace("SceneChange=","");

                    var SceneChange = line.Split(',');

                    SaveDataManager.DataManage.Data.AdventureScene = SceneChange[0];

                    if (SceneChange[1] == "Adventure")
                    {
                        SaveDataManager.DataManage.Data.Scene = SaveData.SceneType.Adventure;
                    }
                    else if (SceneChange[1] == "Novel")
                    {
                        SaveDataManager.DataManage.Data.Scene = SaveData.SceneType.Novel;
                    }

                    SceneFader.FadeType FadeType;

                    Enum.TryParse(SceneChange[2], out FadeType);

                    AC.SceneChange(SceneChange[0], FadeType);

                    return;
                }
                //他のシーンのマップにチェンジ
                else if (line.Contains("AnotherMapChange"))
                {
                    line = line.Replace("AnotherMapChange=","");

                    var Content = line.Split(',');

                    SaveDataManager.DataManage.Data.AdventureScene = Content[0];
                    SaveDataManager.DataManage.Data.Scene = SaveData.SceneType.Adventure;

                    SceneController.CharaStatus Status;
                    SceneFader.FadeType FadeType;

                    Enum.TryParse(Content[2], out Status);
                    Enum.TryParse(Content[3], out FadeType);

                    bool Trans = bool.Parse(Content[6]);

                    List<string> Others=new List<string>();

                    Debug.Log(Content.Length);

                    if (Content.Length>7)
                    {
                        for (int i=7;i<Content.Length;i++)
                        {
                            Others.Add(Content[i]);
                        }
                    }

                    AC.MapChange(Content[0], Status, FadeType, int.Parse(Content[1]),int.Parse(Content[4]),int.Parse(Content[5]),Trans,Others);

                }


                ///フェードしたり？いろいろ///


                //UIを隠す
                else if (line.Contains("HideUI"))
                {
                    CanvasGroup UICanvas = GameObject.Find("UICanvas").GetComponent<CanvasGroup>();

                    UICanvas.alpha = 0;
                }
                //UIを現す
                else if (line.Contains("RiseUI"))
                {
                    CanvasGroup UICanvas = GameObject.Find("UICanvas").GetComponent<CanvasGroup>();

                    UICanvas.alpha = 1;
                }
                //UIをフェードさせる
                else if (line.Contains("FadeUI"))
                {
                    line = line.Replace("FadeUI=","");

                    CanvasGroup UICanvas = GameObject.Find("UICanvas").GetComponent<CanvasGroup>();

                    UICanvas.DOFade(float.Parse(line),1.5f);
                }
                //Charaを隠す
                else if (line.Contains("HideChara"))
                {
                    GameObject.Find("Charactor").GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
                }
                //Charaを現す
                else if (line.Contains("RiseChara"))
                {
                    GameObject.Find("Charactor").GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                }

                //HPBerをフェードイン
                else if (line.Contains("HPBerFadeIn"))
                {
                    AC.BC.HPC.HPBerFadeIn();

                    if (AC.BC.HPC.DashC)
                    {
                        AC.BC.HPC.DashC.DashGaugeFadeIn();

                    }
                }
                //HPBerをフェードアウト
                else if (line.Contains("HPBerFadeOut"))
                {
                    AC.BC.HPC.HPBerFadeOut();

                    if (AC.BC.HPC.DashC)
                    {
                        AC.BC.HPC.DashC.DashGaugeFadeOut();

                    }
                }
                //HPBerを作る
                else if (line.Contains("MakeHPBer"))
                {
                    AC.BC.MakeHPBer();
                }


                //あるオブジェクトの子オブジェクトを全部フェードする
                else if (line.Contains("ChildrenAllFade"))
                {
                    line = line.Replace("ChildrenAllFade=","");

                    var content = line.Split(',');

                    Transform Parent = GameObject.Find(content[0]).transform;

                    float FadeTime = float.Parse(content[2]);
                    float Target = float.Parse(content[3]);

                    foreach (Transform child in Parent)
                    {
                        if (content[1]=="Sprite")
                        {
                            child.GetComponent<SpriteRenderer>().DOFade(Target,FadeTime);
                        }
                    }
                }

                //目標のセット
                else if (line.Contains("TargetSet"))
                {
                    AC.TargetC.enabled = true;
                }
                //目標の進行
                else if (line.Contains("TargetSinkou"))
                {
                    //もしもう目標が出ていたなら
                    if (AC.TargetC.enabled)
                    {
                        SaveDataManager.DataManage.Data.TargetNumber++;

                        AC.TargetC.TargetSet();

                    }
                    else
                    {
                        AC.TargetC.enabled = true;
                    }
                }
                //目標を好きな位置にセット
                else if (line.Contains("TargetDetail"))
                {
                    line = line.Replace("TargetDetail=","");

                    SaveDataManager.DataManage.Data.TargetNumber = int.Parse(line);

                    //もしもう目標が出ていたなら
                    if (AC.TargetC.enabled)
                    {
                        AC.TargetC.TargetSet();
                    }
                    else
                    {
                        AC.TargetC.enabled = true;
                    }
                }
                //目標を隠す
                else if (line.Contains("TargetHide"))
                {
                    AC.TargetC.TargetHide();
                }
                //目標を出す
                else if (line.Contains("TargetRise"))
                {
                    AC.TargetC.TargetRise();
                }

                //ゲームオブジェクトを破壊
                else if (line.Contains("DestroyObject"))
                {
                    line = line.Replace("DestroyObject=","");

                    UnityEngine.Object.Destroy(GameObject.Find(line));
                }
                //ゲームオブジェクトのActive設定をする
                else if (line.Contains("ObjectSetActive"))
                {
                    line = line.Replace("ObjectSetActive=", "");

                    var content = line.Split(',');

                    GameObject.Find(content[0]).SetActive(bool.Parse(content[1]));
                }
                //ゲームオブジェクトの位置設定
                else if (line.Contains("ObjectPosition"))
                {

                }

                //データをセーブ
                else if (line.Contains("DataSave"))
                {
                    line = line.Replace("DataSave=","");

                    SaveDataManager.DataManage.Data.SavePlace = line;

                    Ivent.Index++;

                    AC.Save();

                    return;
                }

                //回復
                else if (line.Contains("Heal"))
                {
                    line = line.Replace("Heal=","");

                    if (line=="Full")
                    {
                        AC.BC.BCC.Heal(AC.BC.BCC.MaxHP);
                    }
                    else
                    {
                        float HealNum = float.Parse(line);

                        AC.BC.BCC.Heal(HealNum);
                    }
                }

                //画面にかかるフィルターの変化
                else if (line.Contains("FilterChange"))
                {
                    line = line.Replace("FilterChange=","");

                    FilterController.Controller.ChangeFilter(line);
                }


                //特殊な操作が行われるイベント(他に応用があまりできない系)
                else if (line.Contains("SpecialIvent"))
                {

                    line = line.Replace("SpecialIvent=","");

                    var content= line.Split(',');

                    ASI.IventScene = content[0];
                    ASI.IventNumber = int.Parse(content[1]);

                    if (content.Length>3)
                    {
                        ASI.Others = content[3];
                    }

                    ASI.StartCoroutine("SpecialIvent");

                    if (content[2]=="0")
                    {
                        ATC.Iventing = true;

                        Ivent.Index++;

                        break;
                    }
                }

                //テキストの進行を一旦止める
                else if (line.Contains("IventingStart"))
                {
                    AC.ATC.Iventing = true;
                }
                //テキストの進行を再開する
                else if (line.Contains("IventingEnd"))
                {
                    AC.ATC.Iventing = false;
                }
                //テキストの内容を指定
                else if (line.Contains("TextShitei"))
                {
                    line = line.Replace("TextShitei=","");

                    ATC.Text.text = line;
                }

                else if (line.Contains("Iventend"))
                {
                    AC.Iventing = false;
                    Ivent.Index = 0;

                    return;
                }

                Ivent.Index++;

                Count++;

                //もし暗転中にやる事やったら
                if (FadeDoing&&FadeDoLines==Count)
                {
                    FadeDoing = false;

                    if (FaderWaitTargetTime>0)
                    {
                        FadeWaiting = true;

                        return;
                    }

                    //黒幕を明るくする
                    Tween FadeOut = BlackFader.DOFade(0,1f);

                    FadeOut.OnComplete(() =>
                    {
                        ReadLines(AC.NowIvent);
                    });

                    FadeOut.Play();

                    return;
                }

                if (Ivent.Index >= Ivent.Lines.Count) break;

                line = Ivent.Lines[Ivent.Index];
            }
        }

        if (line.Contains("{"))
        {
            if (!ATC.CanText) return;

            line=line.Replace("{","");
            line = line.Replace("\t", "");

            while (true)
            {
                if (line.Contains("}"))
                {
                    line = line.Replace("}", "");

                    text += line;

                    Ivent.Index++;

                    break;
                }
                else
                {
                    text += line;
                }

                Ivent.Index++;

                if (Ivent.Index >= Ivent.Lines.Count) break;

                line = Ivent.Lines[Ivent.Index];

            }

            ATC.Texting = true;
            ATC.Text.text = "";

            ATC.HyoujiText = text;
        }
    }
}
