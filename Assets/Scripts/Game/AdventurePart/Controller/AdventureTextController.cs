using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AdventureTextController : InputComponent
{
    private AudioGeneral Audio;

    //[System.NonSerialized]
    public int SENumber;

    [Space(5)]

    public List<NovelCharactor> Charactor = new List<NovelCharactor>();

    [Space(5)]

    public GameObject SkipPrefab;

    public NovelItemWindow ItemWindow;

    public AdventureController AC;
    public Text Text;

    public GameObject SkipObject;

    public Transform TatieParent,SpeakerParent;

    public bool CanText,Texting,Skipping,SkipHandan,Iventing;

    public string HyoujiText;

    private float Keika,SkipKeika;

    public int NowMoji;
    private int LeftCount,RightCount;

    // Start is called before the first frame update
    void Start()
    {
        Audio = GetComponent<AudioGeneral>();

        TatieParent = AC.TextParent.Find("TatieParent");
        SpeakerParent= AC.TextParent.Find("SpeakerParent");

        Text = GetComponentInChildren<Text>();

        Skipping = false;
        NowMoji = 0;

    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Texting)
        {
            if (Skipping)
            {
                SkipTextSinkou();
            }
            else
            {
                TextSinkou();
            }
        }
        else
        {
            //スキップ中だとボタンを押さなくても次の行を読み込む
            if (Skipping)
            {
                AC.AIR.ReadLines(AC.NowIvent);
            }
        }

        /*

        //スキップするかどうか判断
        if (SkipHandan)
        {
            SkipKeika += Time.deltaTime;

            if (SkipKeika>=1f)
            {
                MakeSkip();
            }
        }

    */

        if (CanText)
        {
            if (GetKeyDown(Set: KeybordSets.Decision) || GetKeyDown(Button: XboxControllerButtons.B))
            {
                SetNextProcess();

                SkipHandan = true;
            }

        }

        //スキップ終わり
        if (GetKeyUp(Set: KeybordSets.Decision) || GetKeyUp(Button: XboxControllerButtons.B))
        {
            if (Skipping)
            {
                Skipping = false;
                SkipHandan = false;
                Texting = false;

                Destroy(SkipObject);

                NowMoji = 0;

                SetNextProcess();

            }

            if (SkipHandan)
            {
                SkipHandan = false;

                SkipKeika = 0;
            }

        }
    }

    //クリックされたときの処理
    public void SetNextProcess()
    {
        if (Iventing)
        {
            return;
        }

        if (Texting)
        {
            SetText(HyoujiText);
        }
        else if(!Texting&&!Iventing)
        {
            AC.AIR.ReadLines(AC.NowIvent);
        }
    }

    //テキストを進行させる　各Updateごとに呼ぶ
    public void TextSinkou()
    {
        if (Keika >= SaveDataManager.DataManage.TextSpeed)
        {
            //最後まで表示されたら
            if (NowMoji == HyoujiText.Length)
            {
                Texting = false;

                NowMoji = 0;
                Keika = 0;

                return;
            }

            Text.text += HyoujiText[NowMoji];

            char Moji = HyoujiText[NowMoji];

            if (Moji != '「' || Moji != '」'|| Moji != '…'||Moji!='？'||Moji!='！')
            {
                Audio.PlayClips(SENumber);

            }

            NowMoji++;

            Keika = 0;

        }

        Keika += Time.deltaTime;

    }

    public void SkipTextSinkou()
    {
        Text.text = HyoujiText;

        Keika += Time.deltaTime;

        if (Keika>=0.15f)
        {
            Texting = false;

            Keika = 0;
        }
    }

    public void MakeSkip()
    {
        SkipObject = Instantiate(SkipPrefab, transform);

        Skipping = true;

        SkipHandan = false;

        SkipKeika = 0;
    }


    //セット系

    //全テキスト表示
    public void SetText(string text)
    {
        if (Texting)
        {
            Text.text = text;

            Texting = false;

            NowMoji = 0;
        }
        else
        {
            Texting = true;
        }
    }

    //新しい立ち絵を作る
    public void AddCharactor(string Address,string ImageName,string Position,string CharaName,string XPosi=null)
    {
        if (!TatieParent)
        {
            TatieParent = AC.TextParent.Find("TatieParent");
        }

        var prefab = Resources.Load("Prefab/Novel/CharactorTatie") as GameObject;

        var tatieobject = Instantiate(prefab,TatieParent);

        var charactor = tatieobject.GetComponent<NovelCharactor>();
        var rect = tatieobject.GetComponent<RectTransform>();

        tatieobject.name = CharaName;

        charactor.CharactorName = CharaName;

        Charactor.Add(charactor);
        charactor.MakeNew(Address,ImageName,this);

        switch (Position)
        {
            case "Left":

                {
                    LeftCount++;

                    if (XPosi!=null)
                    {
                        rect.localPosition = new Vector3(float.Parse(XPosi), 0, 0);

                    }
                    else
                    {
                        rect.localPosition = new Vector3(-660, 0, 0);

                    }

                    rect.localScale = new Vector3(1, 1, 1);

                    charactor.Position = "Left";
                }

                break;

            case "Right":

                {
                    RightCount++;

                    if (XPosi!=null)
                    {
                        rect.localPosition = new Vector3(float.Parse(XPosi), 0, 0);

                    }
                    else
                    {
                        rect.localPosition = new Vector3(660, 0, 0);

                    }

                    rect.localScale = new Vector3(-1,1,1);

                    charactor.Position = "Right";

                }

                break;
        }


    }

    //立ち絵を変える
    public void ChangeCharactor(string CharaName,string ImageName)
    {
        var charactor = Charactor.Find((chara)=>chara.CharactorName==CharaName);

        charactor.SetImage(ImageName);
    }

    //立ち絵を暗くする
    public void CharactorDim(string CharaName)
    {
        var charactor= Charactor.Find((chara) => chara.CharactorName == CharaName);

        charactor.FadeDim();
    }

    //立ち絵を明るくする
    public void CharactorBright(string CharaName)
    {
        var charactor = Charactor.Find((chara) => chara.CharactorName == CharaName);

        charactor.FadeBright();
    }

    //立ち絵フェード
    public void CharactorFade(string CharaName,float EndValue)
    {
        var charactor = Charactor.Find((chara) => chara.CharactorName == CharaName);

        charactor.Fade(EndValue);
    }

    //立ち絵移動
    public void CharactorMove(string CharaName,string EndPosi,string Duration)
    {
        var charactor = Charactor.Find((chara) => chara.CharactorName == CharaName);

        charactor.transform.DOLocalMoveX(float.Parse(EndPosi), float.Parse(Duration));
    }

    //立ち絵方向チェンジ
    public void CharactorScaleChange(string CharaName,float EndScaleX,float Duration)
    {
        var charactor = Charactor.Find((chara) => chara.CharactorName == CharaName);

        charactor.ScaleChange(EndScaleX,Duration);
    }

    //立ち絵削除
    public void CharactorDestroy(string CharaName)
    {
        Destroy(Charactor.Find((chara) => chara.CharactorName == CharaName).gameObject);
    }

    //アイテムウィンドウを作る
    public void AddItemWindow(string[] Content)
    {
        var prefab = Resources.Load("Prefab/Novel/ItemWindow")as GameObject;

        var sprite = Resources.Load<Sprite>(Content[0]);

        var Window = Instantiate(prefab,AC.TextParent).transform;

        Window.GetChild(0).GetComponent<Image>().sprite = sprite;

        ItemWindow = Window.GetComponent<NovelItemWindow>();

        ItemWindow.SizeDelta = new Vector2(float.Parse(Content[1]),float.Parse(Content[2]));

        ItemWindow.ScaleIn();
    }

    //スピーカーウィンドウを入れる
    public void SpeakerWindowIn(string Direction,string Name)
    {
        SpeakerParent.gameObject.SetActive(true);

        RectTransform SpeakerWindow;

        switch (Direction)
        {
            case "Left":

                {
                    SpeakerWindow = SpeakerParent.GetChild(0).GetComponent<RectTransform>();

                    SpeakerWindow.GetComponentInChildren<Text>().text = Name;

                    SpeakerWindow.DOLocalMoveX(-710,0.5f);
                }

                break;

            case "Right":

                {
                    SpeakerWindow = SpeakerParent.GetChild(1).GetComponent<RectTransform>();

                    SpeakerWindow.GetComponentInChildren<Text>().text = Name;

                    SpeakerWindow.DOLocalMoveX(710, 0.5f);
                }

                break;
        }
    }

    //スピーカーウィンドウを出す
    public void SpeakerWindowOut(string Direction)
    {
        RectTransform SpeakerWindow;

        switch (Direction)
        {
            case "Left":

                {
                    SpeakerWindow = SpeakerParent.GetChild(0).GetComponent<RectTransform>();

                    SpeakerWindow.DOLocalMoveX(-1210, 0.5f);
                }

                break;

            case "Right":

                {
                    SpeakerWindow = SpeakerParent.GetChild(1).GetComponent<RectTransform>();

                    SpeakerWindow.DOLocalMoveX(1210, 0.5f);
                }

                break;
        }
    }

    //スピーカーウィンドウの名前を変える
    public void SpeakerWindowChange(string Direction,string Name)
    {
        RectTransform SpeakerWindow;

        switch (Direction)
        {
            case "Left":

                {
                    SpeakerWindow = SpeakerParent.GetChild(0).GetComponent<RectTransform>();

                    SpeakerWindow.GetComponentInChildren<Text>().text = Name;
                }

                break;

            case "Right":

                {
                    SpeakerWindow = SpeakerParent.GetChild(1).GetComponent<RectTransform>();

                    SpeakerWindow.GetComponentInChildren<Text>().text = Name;
                }

                break;
        }
    }

    public void SpeakerInitialize()
    {
        SpeakerParent.GetChild(0).GetComponent<RectTransform>().localPosition = new Vector3(-1210,-90,0);
        SpeakerParent.GetChild(1).GetComponent<RectTransform>().localPosition = new Vector3(1210,-90,0);

        SpeakerParent.gameObject.SetActive(false);
    }


    //継承
    public override bool GetKeyDown(KeyCode Key = KeyCode.None, XboxControllerButtons Button = XboxControllerButtons.None, KeybordSets Set = KeybordSets.None)
    {
        return base.GetKeyDown(Key, Button, Set);
    }

    public override bool GetKeyUp(KeyCode Key = KeyCode.None, XboxControllerButtons Button = XboxControllerButtons.None, KeybordSets Set = KeybordSets.None)
    {
        return base.GetKeyUp(Key, Button, Set);
    }
}
