using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.Tilemaps;

public class AdventureController : MonoBehaviour
{
    [System.NonSerialized]
    public Transform UICanvas;

    public AdventureTextController ATC;

    public NormalCharaController CC;
    public BattleController BC;

    public TargetController TargetC;

    public AdventureSceneHolder ASH;
    public AdventureIventReader AIR;
    public AdventureSpecialIvent ASI;

    public AdventureScene NowScene;
    public AdventureIvent NowIvent;

    public RectTransform TextParent;

    public AudioGeneral Audio;

    public bool Iventing,Battleing,BattleMode;

    public TextAsset IventTextAsset,BattleTextAsset,BattleNovelAsset;

    private List<string> MapPositions;

    public MapChangeData MapChangeDatas;

    [System.NonSerialized]
    public Transform Fukidashi;

    public bool SpecialLoadSkip;


    // Start is called before the first frame update
    void Awake()
    {
        Random.InitState(System.DateTime.Now.Second * System.DateTime.Now.Millisecond * System.DateTime.Now.Minute);

        CC = FindObjectOfType<NormalCharaController>();
        ATC = TextParent.GetComponent<AdventureTextController>();

        TargetC = FindObjectOfType<TargetController>();

        ASH = new AdventureSceneHolder(this);
        AIR = new AdventureIventReader(this,ATC);

        ASI = GetComponent<AdventureSpecialIvent>();

        ATC.AC = this;

        if (GetComponent<AudioGeneral>())
        {
            Audio = GetComponent<AudioGeneral>();
        }

        UICanvas = GameObject.Find("UICanvas").transform;

        //今のシーンを取得
        NowScene = ASH.AdventureScenes.Find((scene) => scene.ID == SceneManager.GetActiveScene().name);

        SaveDataManager.DataManage.Data.AdventureScene = SceneManager.GetActiveScene().name;

        if (MapChangeDatas)
        {
            MapPositions = MapChangeDatas.GetData();
        }
    }

    void Update()
    {
        AIR.GeneralWaiting();
    }

    //最初のシーンをセットするよ
    public void SetScene()
    {
        AdventureTitleController TitleC = FindObjectOfType<AdventureTitleController>();

        TitleC.AC = this;

        TitleC.SceneName = NowScene.Name[0];

        if (!SpecialLoadSkip)
        {
            TitleC.Special = int.Parse(NowScene.Name[1]);
            TitleC.Ivent = int.Parse(NowScene.Name[2]);
        }


        TitleC.AddNameIn();

        return;
    }

    //イベント系
    public void IventStart(AdventureIvent Ivent)
    {
        NowIvent = Ivent;

        Iventing = true;

        AIR.ReadLines(Ivent);

        if (BattleMode)
        {
            //もしキャラクターがなんらかのイベントコライダーに触れていた場合
            if (BC.BCC.OnCollider&&BC.BCC.OnCollider.SupportButton)
            {
                BC.BCC.OnCollider.SupportButton.GetComponent<SupportButton>().FadeOut();
            }
        }
        else
        {
            //もしキャラクターがなんらかのイベントコライダーに触れていた場合
            if (CC.OnCollider && CC.OnCollider.SupportButton)
            {
                CC.OnCollider.SupportButton.GetComponent<SupportButton>().FadeOut();
            }
        }
    }

    public void NovelStart(string[] SpeakerSet)
    {
        print("aaaaaaaaaa");
        print(CC);

        if (!BattleMode)
        {
            CC.CanMove = false;
            CC.CharaInitialize();
        }
        else
        {
            BC.BCC.CharactorStop();
            BC.BCC.CharaInitialize();
        }

        print("abbbbbbbbbbb");


        TextParent.gameObject.SetActive(true);
        TextParent.GetComponent<AdventureTextController>().enabled = true;

        TextParent.GetComponent<AdventureTextController>().NowMoji = 0;

        if (SpeakerSet[0]=="None")
        {
            TextParent.DOLocalMoveY(0f, 0.3f).OnComplete(() =>
            {
                ATC.CanText = true;

                AIR.ReadLines(NowIvent);
            });

            return;
        }

        TextParent.GetChild(2).gameObject.SetActive(true);

        print("accccccccaaa");


        //スピーカーのセットがあったら
        switch (SpeakerSet.Length)
        {

            //スピーカーが一つあったら
            case 1:

                {

                    RectTransform SpeakerRect = TextParent.GetChild(2).GetChild(0).GetComponent<RectTransform>();

                    SpeakerRect.GetComponentInChildren<Text>().text = SpeakerSet[0];

                    Tween SpeakerTween= SpeakerRect.DOLocalMoveX(-710, 0.3f);

                    Sequence Sequence = DOTween.Sequence();

                    Sequence.Append(TextParent.DOLocalMoveY(0f, 0.3f)).Append(SpeakerTween).OnComplete(() =>
                     {
                         ATC.CanText = true;

                         AIR.ReadLines(NowIvent);
                     });
                }

                break;

            //スピーカーが二つあったら
            case 2:

                {
                    RectTransform SpeakerRect1 = TextParent.GetChild(2).GetChild(0).GetComponent<RectTransform>();

                    SpeakerRect1.GetComponentInChildren<Text>().text = SpeakerSet[0];

                    Tween SpeakerTween1 = SpeakerRect1.DOLocalMoveX(-710, 0.3f);

                    RectTransform SpeakerRect2 = TextParent.GetChild(2).GetChild(1).GetComponent<RectTransform>();

                    SpeakerRect2.GetComponentInChildren<Text>().text = SpeakerSet[1];

                    Tween SpeakerTween2 = SpeakerRect2.DOLocalMoveX(710, 0.3f);



                    Sequence Sequence = DOTween.Sequence();

                    Sequence.Append(TextParent.DOLocalMoveY(0f, 0.3f)).Append(SpeakerTween1).Join(SpeakerTween2).OnComplete(() =>
                    {

                        ATC.CanText = true;

                        AIR.ReadLines(NowIvent);
                    });
                }

                break;
        }

        print("adddddddddddddd");


    }

    public void NovelEnd(int DoingNext)
    {
        TextParent.GetComponent<AdventureTextController>().enabled=false;

        foreach (NovelCharactor Chara in ATC.Charactor)
        {
            Chara.FadeOut();
        }

        if (ATC.ItemWindow)
        {
            ATC.ItemWindow.ScaleOut();
        }

        if (ATC.SkipObject)
        {
            Destroy(ATC.SkipObject);

            ATC.Skipping = false;
        }

        //スピーカーを初期化
        ATC.SpeakerInitialize();

        TextParent.DOLocalMoveY(-500f,0.3f).OnComplete(()=> 
        {
            switch (DoingNext)
            {
                case 0:

                    {
                        TextParent.gameObject.SetActive(false);

                        if (BattleMode)
                        {
                            BC.BCC.CharactorMove();
                        }
                        else
                        {
                            CC.CanMove = true;
                        }

                        Iventing = false;

                        ATC.Text.text = "";
                        ATC.SkipHandan = false;
                    }

                    break;

                case 1:
                case 2:

                    {
                        TextParent.gameObject.SetActive(false);

                        ATC.Text.text = "";
                        ATC.SkipHandan = false;

                        AIR.ReadLines(NowIvent);
                    }

                    break;
            }

        });
    }

    public void MakeFukidashi(string[] Content)
    {
        GameObject Prefab = Resources.Load("Prefab/UI/Window/FukidashiParent") as GameObject;

        Fukidashi = Instantiate(Prefab).GetComponent<Transform>();

        Vector2 Pos = new Vector2(float.Parse(Content[0]),float.Parse(Content[1]));

        Fukidashi.position = Pos;

        if (float.Parse(Content[2])<0)
        {
            Fukidashi.GetChild(0).localScale = new Vector2(-0.4f,0.4f);
        }

        Fukidashi.DOScale(new Vector2(float.Parse(Content[2]),5f),0.5f).OnComplete(()=> 
        {
            Fukidashi.GetComponent<Animator>().SetTrigger(Content[3]);
        });
    }

    public void FukidashiOut()
    {
        Fukidashi.DOScale(new Vector2(0,0),0.5f).OnComplete(()=> 
        {
            Destroy(Fukidashi.gameObject);
        });
    }

    public void DestroyFukidashi()
    {
        Fukidashi.DOScale(new Vector2(0, 0), 0.5f).OnComplete(() => Destroy(Fukidashi.gameObject));
    }


    public void BattleStart(string[] BattleSet)
    {
        GameObject BCPrefab = Resources.Load("Prefab/Battle/BattleController") as GameObject;

        BC = Instantiate(BCPrefab).GetComponent<BattleController>();

        BC.BattleNumber = int.Parse(BattleSet[0]);
        BC.Special = int.Parse(BattleSet[1]);

        //HPバーの作成
        if (!BC.HPC)
        {
            BC.MakeHPBer();
        }

        Battleing = true;
    }

    public void BTIventStart(string[] BattleSet)
    {
        BC.BattleNumber = int.Parse(BattleSet[0]);
        BC.Special = int.Parse(BattleSet[1]);

        Battleing = true;

        if (BC.HPC)
        {
            BC.BattleStart();
        }
    }

    //通常状態から戦闘状態へ移行
    public void NormalBattleChange()
    {
        Vector3 CharaPosi = GameObject.Find("Charactor").transform.position;

        GameObject BattlePrefab = Resources.Load("Prefab/Charactor/BattleCharactor") as GameObject;

        Destroy(GameObject.Find("Charactor"));

        GameObject BattleChara = Instantiate(BattlePrefab);

        BattleChara.transform.position = CharaPosi;

        BattleChara.name = "Charactor";

        Camera.main.GetComponent<CameraFollower>().CharaTrans = BattleChara.transform;

        BattleMode = true;

    }

    //データをセーブ
    public void Save()
    {
        GameObject CanvasParent = Resources.Load("Prefab/UI/Save/SaveBackCanvas") as GameObject;

        Instantiate(CanvasParent).GetComponent<SaveBackCanvas>().ac=this;

        SaveData data = SaveDataManager.DataManage.Data;

        //キャラクター止めるマン
        if (BattleMode)
        {
            CharactorData chara = data.CharactorDatas[BC.BCC.Chara.ToString()];

            BC.BCC.CharactorStop();
            BC.BCC.CharaInitialize();

            data.PositionX = BC.BCC.transform.position.x;
            data.PositionY = BC.BCC.transform.position.y;

            data.Chara = SaveData.CharaType.Battle;
            data.Sousa = BC.BCC.Chara;

            if (SaveDataManager.DataManage.Data.CanDash)
            {
                BC.HPC.DashC.DashGaugePause(true);
            }


            chara.CharaHP = BC.BCC.HP;
            chara.CharaHPMax = BC.BCC.MaxHP;

            chara.DashCount = BC.BCC.CanDashCount;
        }
        else
        {
            CC.CharaInitialize();
            CC.CanMove = false;

            data.Chara = SaveData.CharaType.Normal;

            data.PositionX = CC.transform.position.x;
            data.PositionY = CC.transform.position.y;
        }


        data.Scene = SaveData.SceneType.Adventure;
        data.AdventureScene = SceneManager.GetActiveScene().name;

        SaveDataManager.DataManage.Save();
    }

    public void SaveEnd()
    {
        if (BattleMode)
        {
            BC.BCC.CharactorMove();
            BC.BCC.CharaInitialize();

            if (SaveDataManager.DataManage.Data.CanDash)
            {
                BC.HPC.DashC.DashGaugePause(false);
            }
        }
        else
        {
            CC.CharaInitialize();
            CC.CanMove = true;
        }

        AIR.ReadLines(NowIvent);
    }

    //ゲームオーバー
    public IEnumerator GameOver()
    {
        Time.timeScale = 0.5f;

        BGMController.Controller.BGMPause();

        Camera.main.GetComponent<CameraFollower>().enabled = false;

        Camera.main.DOShakePosition(3f,1,20);

        yield return new WaitForSeconds(1f);

        FilterController.Controller.ChangeFilter("GameOver");

        yield return new WaitForSeconds(3.5f);

        SceneChange("Title",SceneFader.FadeType.GameOver);

        yield return null;
    }


    //シーンチェンジ
    public void SceneChange(string scenename, SceneFader.FadeType fadetype)
    {
        SceneController.Controller.Transition(scenename, fadetype);
    }

    public void MapChange(string scenename,SceneController.CharaStatus status,SceneFader.FadeType fadetype,int num,int Special=0,int SpecialIvent=0,bool trans=true,List<string> other=null)
    {
        if (status==SceneController.CharaStatus.Battle)
        {

            //キャラの体力等を保存しておく
            string CharaName = BC.BCC.Chara.ToString();

            CharactorData Data = new CharactorData();

            Data.CharaHPMax = BC.BCC.MaxHP;
            Data.CharaHP = BC.BCC.HP;

            Data.DashCount = BC.BCC.CanDashCount;

            SaveDataManager.DataManage.Data.CharactorDatas[CharaName] = Data;
        }

        SaveDataManager.DataManage.Data.TargetNumber = 0;
        
        //マップ移行
        SceneController.Controller.MapTransition(scenename,status,fadetype,num,Special,trans,SpecialIvent,other);
    }

    public void SetStartPosition(int posi)
    {
        print("startposition");

        GameObject Chara;
        Vector2 CharaPosi;

        switch (SceneController.Controller.Status)
        {
            case SceneController.CharaStatus.Normal:

                {
                    if (posi != -1)
                    {
                        string[] PosiString = MapPositions[posi].Split(',');
                        CharaPosi = new Vector2(float.Parse(PosiString[0]), float.Parse(PosiString[1]));
                    }
                    else
                    {
                        CharaPosi = new Vector2(SaveDataManager.DataManage.Data.PositionX, SaveDataManager.DataManage.Data.PositionY);
                    }

                    GameObject Prefab = Resources.Load("Prefab/Charactor/NormalCharactor") as GameObject;

                    Chara = Instantiate(Prefab);
                    Chara.name = "Charactor";
                    Chara.transform.position = CharaPosi;

                    //Camera.main.transform.position = new Vector2(CharaPosi.x, 0);

                    CC = Chara.GetComponent<NormalCharaController>();

                    print("CCSettei");

                }

                break;

            case SceneController.CharaStatus.Battle:

                {
                    BattleMode = true;

                    if (posi != -1)
                    {
                        string[] PosiString = MapPositions[posi].Split(',');
                        CharaPosi = new Vector2(float.Parse(PosiString[0]), float.Parse(PosiString[1]));
                    }
                    else
                    {
                        CharaPosi = new Vector2(SaveDataManager.DataManage.Data.PositionX, SaveDataManager.DataManage.Data.PositionY);
                    }

                    GameObject Prefab = Resources.Load("Prefab/Charactor/BattleCharactor") as GameObject;

                    Chara=Instantiate(Prefab);
                    Chara.name = "Charactor";
                    Chara.transform.position = CharaPosi;

                    Camera.main.transform.position = new Vector2(CharaPosi.x,0);


                    //BattleControllerにCharaControllerをセット
                    BC.BCC = Chara.GetComponent<BattleCharaController>();

                    CharactorData Data = SaveDataManager.DataManage.Data.CharactorDatas[BC.BCC.Chara.ToString()];

                    BC.BCC.MaxHP = Data.CharaHPMax;
                    BC.BCC.HP = Data.CharaHP;

                    BC.BCC.CanDashCount = Data.DashCount;

                    BC.MapChangeBool = true;

                }

                break;
        }

        SetCondition();

    }

    public void SetCondition()
    {
        Dictionary<string, bool> Flags = SaveDataManager.DataManage.Data.GameFlags;

        switch (SceneManager.GetActiveScene().name)
        {
            case "SchoolZone":

                {
                    if (Flags.ContainsKey("DeltaGNBreak"))
                    {
                        Destroy(GameObject.Find("MapChangeCollider01"));
                        Destroy(GameObject.Find("MapChangeCollider02"));
                        Destroy(GameObject.Find("MapChangeCollider03"));
                    }

                    if (!Flags.ContainsKey("FirstKitaku"))
                    {
                        Destroy(GameObject.Find("Portal"));
                        Destroy(GameObject.Find("FirstPortalNovel"));
                    }

                    if (!Flags.ContainsKey("SecondDangeonNovel"))
                    {
                        Destroy(GameObject.Find("SecondDangeonNovel"));
                    }

                    if (Flags.ContainsKey("GoldenAppleTrack"))
                    {
                        Transform GApple=GameObject.Find("GoldenAppleParent").transform;

                        GApple.position = new Vector2(CC.transform.position.x,GApple.position.y);

                        GApple.GetComponent<GoldenAppleTracking>().enabled=true;
                        GApple.GetComponent<GoldenAppleTracking>().Target=CC.transform;

                        GApple.GetChild(0).gameObject.SetActive(true);
                        //GApple.GetChild(1).gameObject.SetActive(true);
                    }

                    if (Flags.ContainsKey("FirstPortal"))
                    {
                        Destroy(GameObject.Find("FirstPortalNovel"));
                        //Destroy(GameObject.Find("Portal"));
                    }

                    if (Flags.ContainsKey("StoryFlag_1_1"))
                    {
                        GameObject Prefab = Resources.Load("Prefab/Charactor/NormalCharactor") as GameObject;
                        RuntimeAnimatorController BerryAnimator = Resources.Load("Animator/Berry/BerryNormalController") as RuntimeAnimatorController;

                        Transform Chara = Instantiate(Prefab).transform;

                        Chara.name = "BerryObject";
                        Chara.position = new Vector3(29.8f, -2.01f);
                        Chara.localScale = new Vector3(-2f,2f);

                        Chara.GetComponent<Animator>().runtimeAnimatorController = BerryAnimator;

                        GameObject.Find("Portal").transform.position=new Vector2(31.6f,-2.38f);
                    }
                }

                break;

            case "FirstDangeon_2":

                {
                    if (Flags.ContainsKey("FirstSave"))
                    {
                        Destroy(GameObject.Find("FirstSaveCollider"));
                    }

                    if (Flags.ContainsKey("TalkFlag01"))
                    {
                        Destroy(GameObject.Find("TalkCollider01"));

                    }

                    if (Flags.ContainsKey("BattleFlag01"))
                    {
                        Destroy(GameObject.Find("BattleCollider01"));
                    }
                }

                break;

            case "FirstDangeon_3":

                {
                    if (Flags.ContainsKey("FirstHeal"))
                    {
                        Destroy(GameObject.Find("TalkCollider01"));
                    }

                    if (Flags.ContainsKey("FD_3_TalkFlag02"))
                    {
                        Destroy(GameObject.Find("TalkCollider02"));

                    }

                    if (Flags.ContainsKey("FD_3_BattleFlag01"))
                    {
                        Destroy(GameObject.Find("BattleCollider01"));
                    }
                }

                break;

            case "ClassRoom":

                {
                    //廊下でセーブした場合
                    if (Flags.ContainsKey("ClassRoom_Rouka"))
                    {
                        //カメラの追尾設定オン
                        Camera.main.GetComponent<CameraFollower>().enabled = true;

                        Transform BackGroundParent = GameObject.Find("BackGroundParent").transform;

                        //外観の画像を消す
                        BackGroundParent.GetChild(2).gameObject.SetActive(false);

                        BackGroundParent.GetChild(0).gameObject.SetActive(true);

                        //教室の画像を一気に全部透明にする
                        foreach (Transform child in BackGroundParent.GetChild(1))
                        {
                            foreach (Transform grandchild in child)
                            {
                                if (grandchild.GetComponent<SpriteRenderer>())
                                {
                                    grandchild.GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f,0f);
                                }
                            }

                            if (child.GetComponent<SpriteRenderer>())
                            {
                                child.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
                            }
                        }

                        Destroy(GameObject.Find("Mob02"));
                        Destroy(GameObject.Find("Mob03"));
                        Destroy(GameObject.Find("Mob04"));

                        Destroy(GameObject.Find("Teacher"));

                        GameObject.Find("Chair05").GetComponent<SpriteRenderer>().sortingLayerName = "BackHaikei";

                        //タイルマップのチェンジ
                        Transform TileMapGrid = GameObject.Find("TileMapGrid").transform;

                        TileMapGrid.GetChild(1).gameObject.SetActive(false);
                        TileMapGrid.GetChild(0).GetComponent<Tilemap>().color=new Color(1f,1f,1f,0f);

                        TileMapGrid.GetChild(1).gameObject.SetActive(true);
                        TileMapGrid.GetChild(1).GetComponent<Tilemap>().color = new Color(1f,1f,1f,1f);

                        //廊下の画像を一気にフェードインさせる
                        foreach (Transform child in BackGroundParent.GetChild(0))
                        {
                            if (child.GetComponent<SpriteRenderer>())
                            {
                                child.GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f,1f);
                            }
                        }

                        BackGroundParent.GetChild(1).gameObject.SetActive(false);

                        //黄金の林檎追尾
                        Transform GoldenApple = GameObject.Find("GoldenAppleParent").transform;

                        GoldenApple.GetChild(0).gameObject.SetActive(true);
                        //GoldenApple.GetChild(1).gameObject.SetActive(true);

                        GoldenApple.position = new Vector2(CC.transform.position.x, -1.3f);

                        GoldenApple.GetComponent<GoldenAppleTracking>().enabled = true;
                        GoldenApple.GetComponent<GoldenAppleTracking>().Target = CC.transform;

                    }

                    //SchoolZoneから来た時の場合
                    if (Flags.ContainsKey("GotoRoukaFromSchoolZone"))
                    {
                        if (Flags.ContainsKey("ClassRoom_Rouka"))
                        {
                            if (Flags.ContainsKey("NeteruMobKesu"))
                            {
                                SaveDataManager.DataManage.Data.GameFlags.Add("NeteruMobKesu", true);

                            }

                            Destroy(GameObject.Find("Mob01"));

                        }
                        else
                        {
                            SaveDataManager.DataManage.Data.GameFlags.Add("NeteruMobKesu", true);

                            //カメラの追尾設定オン
                            Camera.main.GetComponent<CameraFollower>().enabled = true;

                            Transform BackGroundParent = GameObject.Find("BackGroundParent").transform;

                            //外観の画像を消す
                            BackGroundParent.GetChild(2).gameObject.SetActive(false);

                            BackGroundParent.GetChild(0).gameObject.SetActive(true);

                            //教室の画像を一気に全部透明にする
                            foreach (Transform child in BackGroundParent.GetChild(1))
                            {
                                foreach (Transform grandchild in child)
                                {
                                    if (grandchild.GetComponent<SpriteRenderer>())
                                    {
                                        grandchild.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
                                    }
                                }

                                if (child.GetComponent<SpriteRenderer>())
                                {
                                    child.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
                                }
                            }

                            Destroy(GameObject.Find("Mob01"));
                            Destroy(GameObject.Find("Mob02"));
                            Destroy(GameObject.Find("Mob03"));
                            Destroy(GameObject.Find("Mob04"));

                            Destroy(GameObject.Find("Teacher"));

                            GameObject.Find("Chair05").GetComponent<SpriteRenderer>().sortingLayerName = "BackHaikei";

                            //タイルマップのチェンジ
                            Transform TileMapGrid = GameObject.Find("TileMapGrid").transform;

                            TileMapGrid.GetChild(0).GetComponent<Tilemap>().color = new Color(1f, 1f, 1f, 0f);

                            TileMapGrid.GetChild(1).gameObject.SetActive(true);
                            TileMapGrid.GetChild(1).GetComponent<Tilemap>().color = new Color(1f, 1f, 1f, 1f);

                            //廊下の画像を一気にフェードインさせる
                            foreach (Transform child in BackGroundParent.GetChild(0))
                            {
                                if (child.GetComponent<SpriteRenderer>())
                                {
                                    child.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
                                }
                            }

                            BackGroundParent.GetChild(1).gameObject.SetActive(false);

                            //黄金の林檎追尾
                            Transform GoldenApple = GameObject.Find("GoldenAppleParent").transform;

                            GoldenApple.GetChild(0).gameObject.SetActive(true);
                            //GoldenApple.GetChild(1).gameObject.SetActive(true);

                            GoldenApple.position = new Vector2(CC.transform.position.x, -1.3f);

                            GoldenApple.GetComponent<GoldenAppleTracking>().enabled = true;
                            GoldenApple.GetComponent<GoldenAppleTracking>().Target = CC.transform;
                        }
                    }

                    if (Flags.ContainsKey("NeteruMobKesu")&&GameObject.Find("Mob01"))
                    {
                        Destroy(GameObject.Find("Mob01"));

                    }

                }

                break;
        }
    }
}
