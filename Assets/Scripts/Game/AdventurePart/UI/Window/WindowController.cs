using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class WindowController : InputComponent
{
    //ページ数
    public int PageNumber,NowPage,Special;

    public AudioGeneral Audio;

    private RectTransform WindowRect,ContentRect,TitleRect;
    private Transform ArrowParent;

    private Material LeftArrow, RightArrow;

    private bool CanOut,PageChangeing;

    private float Horizontal;

    // Start is called before the first frame update
    void Start()
    {
        Audio = GetComponent<AudioGeneral>();

        WindowRect = transform.Find("WindowImage").GetComponent<RectTransform>();
        ContentRect=WindowRect.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
        TitleRect=WindowRect.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<RectTransform>();

        ArrowParent = WindowRect.GetChild(3);
        LeftArrow = ArrowParent.GetChild(0).GetComponent<Image>().material;
        RightArrow = ArrowParent.GetChild(1).GetComponent<Image>().material;

        if (PageNumber==0)
        {
            ArrowParent.gameObject.SetActive(false);
        }
        else
        {
            LeftArrow.SetFloat("_Alpha", 0f);
            RightArrow.SetFloat("_Alpha", 1f);
        }
    }

    // Update is called once per frame
    void Update()
    {

        //閉じる:スペースとエンター
        if (GetKeyDown(Key:KeyCode.Space,Button:XboxControllerButtons.RightBumper))
        {
            if (CanOut)
            {
                CanOut = false;

                WindowOut();
            }
        }

        //数ページある場合は
        if (PageNumber>0&&!PageChangeing)
        {
            //左に行く
            if (Horizontal<0&&NowPage!=0)
            {
                NowPage--;

                ContentRect.DOLocalMoveX(ContentRect.localPosition.x + 1400, 0.5f).OnComplete(() => PageChangeing = false);
                TitleRect.DOLocalMoveX(ContentRect.localPosition.x + 1400, 0.5f);

                Audio.PlayClips(2);

                ArrowCheck();

                PageChangeing = true;
            }

            //右に行く
            if (Horizontal>0&&NowPage!=PageNumber)
            {
                NowPage++;

                ContentRect.DOLocalMoveX(ContentRect.localPosition.x - 1400, 0.5f).OnComplete(() => PageChangeing = false);
                TitleRect.DOLocalMoveX(ContentRect.localPosition.x - 1400, 0.5f);

                Audio.PlayClips(2);

                ArrowCheck();

                PageChangeing = true;

            }
        }

        //コントローラーの入力取得
        if (InputManager.IsController)
        {
            Horizontal = (GetKeyAxis(Axes: XboxControllerAxes.DpadHorizontal) * -1) + GetKeyAxis(Axes: XboxControllerAxes.LeftstickHorizontal);
        }
        else
        {
            Horizontal = GetKeyAxis(Set: KeybordSets.Horizontal);
        }
    }

    public void WindowIn()
    {
        Audio.PlayClips(0);

        WindowRect.DOScale(new Vector2(1f, 1f), 0.3f).OnComplete(()=> 
        {
            CanOut = true;

            //WindowInSupportを消す
            for (int i=1;i<5;i++)
            {
                Destroy(transform.Find("WindowInSupport0" + i).gameObject);
            }
        });
    }

    public void WindowOut()
    {
        Audio.PlayClips(1);

        WindowRect.DOScale(new Vector2(0f, 0f), 0.3f).OnComplete(() =>
        {
            OutSpecialIvent();

            Destroy(gameObject);
        });
    }

    public void OutSpecialIvent()
    {
        switch (Special)
        {
            //BattleCharaを動かせるようにする
            case 1:

                {
                    BattleController Controller = FindObjectOfType<BattleController>();

                    Controller.BattleNumber = 1;
                    Controller.Special = 0;

                    Controller.BattleStart();

                    //BGMを変える
                    BGMController.Controller.BGMPlay(1);
                }

                break;

            //ダッシュを可能にする
            case 2:

                {
                    SaveDataManager.DataManage.Data.CanBit = true;
                    SaveDataManager.DataManage.Data.CanDash = true;

                    BattleController Controller= FindObjectOfType<BattleController>();

                    Controller.HPC.MakeDashGauge();
                    Controller.HPC.FirstCharge = true;

                    Controller.BCC.CharactorMove();
                }

                break;
        }
    }


    public void ArrowCheck()
    {
        if (NowPage==0)
        {
            LeftArrow.DOFloat(0f,"_Alpha",0.5f);
            RightArrow.DOFloat(1f,"_Alpha",0.5f);
        }
        else if (NowPage==PageNumber)
        {
            LeftArrow.DOFloat(1f,"_Alpha", 0.5f);
            RightArrow.DOFloat(0f,"_Alpha", 0.5f);
        }
        else
        {
            LeftArrow.DOFloat(1f,"_Alpha", 0.5f);
            RightArrow.DOFloat(1f,"_Alpha", 0.5f);
        }
    }

    //継承
    public override float GetKeyAxis(XboxControllerAxes Axes = XboxControllerAxes.None, KeybordSets Set = KeybordSets.None)
    {
        return base.GetKeyAxis(Axes, Set);
    }
    public override bool GetKeyDown(KeyCode Key = KeyCode.None, XboxControllerButtons Button = XboxControllerButtons.None, KeybordSets Set = KeybordSets.None)
    {
        return base.GetKeyDown(Key, Button, Set);
    }
}
