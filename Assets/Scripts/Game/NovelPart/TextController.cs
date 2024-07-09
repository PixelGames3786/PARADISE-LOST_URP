using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TextController : InputComponent
{
    public NovelController NC;

    private NovelSceneHolder NSH;
    private NovelSceneReader NSR;

    private NovelScene NowScene;

    public Color BackColor;

    private Tween BGTween;

    public string HyoujiText,AllText;
    public int NowMoji;

    private float Keika;

    public bool Texting,CanText,BGTweening;

    public int SENumber;

    public TextController(NovelController NC)
    {
        this.NC = NC;

        NSH = new NovelSceneHolder(this);
        NSR = new NovelSceneReader(this);
    }

    //クリック待ち
    public void WaitButton()
    {
        if (GetKeyDown(Set:KeybordSets.Decision) || GetKeyDown(Button: XboxControllerButtons.B))
        {
            if (CanText)
            {
                SetNextProcess();

            }
        }
    }

    //クリックされたときの処理
    public void SetNextProcess()
    {
        if (BGTweening)
        {
            BGTween.Complete();

            BGTweening = false;
        }

        if (Texting)
        {
            SetText(HyoujiText);
        }
        else
        {
            NSR.ReadLines(NowScene);
        }
    }

    //テキストを進行させる　各Updateごとに呼ぶ
    public void TextSinkou()
    {
        if (Texting)
        {
            Keika += Time.deltaTime;

            if (Keika>=NC.SaveManege.TextSpeed)
            {
                NC.Audio.PlayClips(SENumber);

                //最後まで表示されたら
                if (NowMoji==HyoujiText.Length)
                {
                    Texting = false;

                    NowMoji = 0;
                    Keika = 0;

                    return;
                }

                NC.Text.text += HyoujiText[NowMoji];

                NowMoji++;

                Keika = 0;
            }

        }
    }


    //データをロードするとき
    public void LoadNovelData()
    {
        SaveData Data = NC.SaveManege.Data;

        NowScene = NSH.NovelScenes.Find(s => s.ID == Data.NovelScene.ToString());
        NowScene.Index = Data.NovelLine;

        List<string> IventLines = new List<string>();

        //#が前についてるイベントテキストを取得
        for (int i=NowScene.Index;i>-1;i--)
        {
            string line = NowScene.Lines[i];

            if (line.Contains("#"))
            {
                IventLines.Add(line);
            }
        }

        bool[] IventDone = new bool[2];

        //イベントをしたから一つずつやっていく
        for (int i=0;i<IventLines.Count;i++)
        {
            string target = IventLines[i];

            target = target.Replace("#", "");

            //しゃべっている人を設定
            if (target.Contains("speaker")&&!IventDone[0])
            {
                target = target.Replace("speaker=", "");

                SetSpeaker(target);

                //自分以外のスピーカーは行わない
                IventDone[0] = true;
            }
            //背景をセット
            else if (target.Contains("background") && !IventDone[1])
            {
                target = target.Replace("background=", "");

                SetBackGround(target);

                //自分以外の背景セットイベントは行わない
                IventDone[1] = true;

            }
        }

        //一行ずれるので修正
        NowScene.Index = Data.NovelLine-1;

        CanText = true;
        SetNextProcess();
    }

    //データをセーブするとき
    public void SaveNovelData()
    {
        NC.SaveManege.Data.NovelScene = int.Parse(NowScene.ID);
        NC.SaveManege.Data.NovelLine = NowScene.Index;

        NC.SaveManege.Save();
    }



    ////セット系////

    //Sceneをセット
    public void SetScene(string id)
    {
        NowScene = NSH.NovelScenes.Find(s => s.ID == id);

        NowScene = NowScene.Clone();
        if (NowScene == null) Debug.LogError("scenario not found");

        SetNextProcess();
    }

    //全テキスト表示
    public void SetText(string text)
    {
        if (Texting)
        {
            NC.Text.text = text;

            Texting = false;

            NowMoji = 0;
        }
        else
        {
            Texting = true;
        }
    }

    //しゃべっている人物の名前をセット
    public void SetSpeaker(string speaker)
    {
        if (speaker=="None")
        {
            NC.SpeakerText.transform.parent.gameObject.SetActive(false);

            return;
        }

        NC.SpeakerText.text = speaker;
    }

    //背景をセット
    public void SetBackGround(string bgname)
    {
        Sprite BG = Resources.Load<Sprite>("NovelSprites/BackGround/" + bgname);

        if (!BG) return;

        //最初に画像をセットするなら
        if (!NC.BGBefore.sprite)
        {
            NC.BGBefore.sprite = BG;

            NC.BGBefore.DOFade(1f,0.5f);

            return;
        }
        //画像を変える時
        else
        {
            BGTweening = true;

            NC.BGAfter.sprite = BG;

            BGTween = NC.BGAfter.DOFade(1f, 0.5f).OnComplete(() =>
              {
                  NC.BGBefore.sprite = BG;

                  NC.BGAfter.sprite = null;
                  NC.BGAfter.color = new Color(1, 1, 1, 0);

                  BGTweening = false;
              });
        }
    }

    //シーンチェンジ
    public void SetSceneChange(string scenename,SceneFader.FadeType fadetype)
    {
        SceneController.Controller.BackColor = BackColor;
        SceneController.Controller.Transition(scenename,fadetype);
    }

    public void ChangeToAdventure(string scenename,SceneFader.FadeType fadetype,SceneController.CharaStatus status,int pnum,int special=0,int specialivent=0,bool trans=true,List<string> Others=null)
    {
        SceneController.Controller.BackColor = BackColor;

        SceneController.Controller.MapTransition(scenename,status,fadetype,pnum,special,trans,specialivent,Others);
    }


    //継承
    public override bool GetKeyDown(KeyCode Key = KeyCode.None, XboxControllerButtons Button = XboxControllerButtons.None, KeybordSets Set = KeybordSets.None)
    {
        return base.GetKeyDown(Key, Button, Set);
    }
}
