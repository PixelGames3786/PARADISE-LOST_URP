using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleController : MonoBehaviour
{
    [System.NonSerialized]
    public Transform UICanvas;

    public AdventureController AC;
    public BitCountController BitC;
    public HPBerController HPC;

    public GameObject HPBerPrefab,BitCountPrefab;
    public GameObject EnemyParent;

    public BattleCharaController BCC;
    public BattleSceneReader BSR;
    public BattleSceneHolder BSH;

    public BattleScene NowScene;
    public BattleIvent NowIvent;

    public int BattleNumber,Special;

    public bool MapChangeBool;

    private AdventureSpecialIvent ASI;
    private BattleNovelController BNC;

    private GameObject BattleWallParent;

    // Start is called before the first frame update
    void Start()
    {
        EnemyParent = GameObject.Find("EnemyParent");

        UICanvas = GameObject.Find("UICanvas").transform;

        AC = FindObjectOfType<AdventureController>();

        ASI = AC.gameObject.GetComponent<AdventureSpecialIvent>();

        BSH = new BattleSceneHolder(this);
        BSR = new BattleSceneReader(this);

        //今のシーンを取得
        NowScene = BSH.BattleScenes.Find((scene) => scene.ID == SceneManager.GetActiveScene().name);
    }

    // Update is called once per frame
    void Update()
    {
        BSR.GeneralWaiting();
    }

    public void MakeHPBer()
    {
        GameObject BattleParent = GameObject.Find("BattleParent");

        GameObject HPBer=Instantiate(HPBerPrefab,BattleParent.transform);

        HPC = HPBer.GetComponent<HPBerController>();

        HPC.BC = this;

        if (MapChangeBool)
        {
            HPC.MapChange = 1;
        }

        HPC.HPBerIn();
    }

    public void MakeBitCount()
    {
        Instantiate(BitCountPrefab, GameObject.Find("BattleParent").transform);
    }

    public void BattleStart()
    {

        //ビットカウントの作成
        /*
        if (!BitC)
        {
            MakeBitCount();
        }
        */

        //特殊な何か
        switch (Special)
        {
            //FirstDangeon : 操作説明を出す
            case 1:

                {
                    GameObject SetumeiPrefab = Resources.Load("Prefab/UI/Window/SousaSetumeiParent") as GameObject;

                    GameObject Setumei = Instantiate(SetumeiPrefab, UICanvas);

                    Setumei.GetComponent<WindowController>().Special = 1;
                }

                return;
        }

        //キャラクターが動けるようにする
        BCC = GameObject.Find("Charactor").GetComponent<BattleCharaController>();

        BCC.enabled = true;
        BCC.CanMove = true;
        BCC.CanAttack = true;

        NowIvent = NowScene.Ivents[BattleNumber-1];

        BSR.ReadLines(NowIvent);

    }

    public void BattleNovelStart(int StartID)
    {

        if (!BNC)
        {
            GameObject Prefab = Resources.Load("Prefab/Battle/BattleNovelParent") as GameObject;

            BNC = Instantiate(Prefab, GameObject.Find("UICanvas").transform).GetComponent<BattleNovelController>();
        }

        List<string> IventLine = BSH.BattleNIvents.Find((ivent)=>int.Parse(ivent.ID)==StartID).Lines;

        foreach (string line in IventLine)
        {
            BNC.Lines.Add(line);
        }

        if (BNC.SinkouEnd)
        {
            BNC.LineSinkou();
        }
    }

    public void BattleNovelForcedEnd(int Type)
    {
        BNC.ForcedEnd(Type);
    }

    //戦闘時の壁
    public void MakeBattleWall(string[] Content)
    {
        BattleWallParent = new GameObject();
        BattleWallParent.name = "BattleWallParent";

        GameObject CharaWallPrefab = Resources.Load("Prefab/Battle/Wall/BattleCharaWall")as GameObject;
        GameObject EnemyWallPrefab= Resources.Load("Prefab/Battle/Wall/BattleEnemyWall") as GameObject;

        for (int i=0;i<Content.Length;i++)
        {
            print(Content[i]);

            float Position = float.Parse(Content[i]);

            GameObject Wall=Instantiate(CharaWallPrefab, BattleWallParent.transform);

            Wall.transform.position = new Vector2(Position,Wall.transform.position.y);

            Wall = Instantiate(EnemyWallPrefab, BattleWallParent.transform);

            Wall.transform.position= new Vector2(Position, Wall.transform.position.y);
        }
    }

    public void DestroyBattleWall()
    {
        Destroy(BattleWallParent);
    }
}
