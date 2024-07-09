using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class NormalCharaController : MonoBehaviour
{
    public AdventureController AC;

    public Animator CharaAnimator;

    public Rigidbody2D RB;

    public Transform TF;

    private NormalCharaMover CM;
    private NormalCharaInputer CI;

    public AdventureCollider OnCollider;

    public float Speed;

    public bool CanMove;

    // Start is called before the first frame update
    void Start()
    {
        AC = FindObjectOfType<AdventureController>();

        CM = gameObject.AddComponent<NormalCharaMover>();
        CI = gameObject.AddComponent<NormalCharaInputer>();

        CharaAnimator = GetComponent<Animator>();

        RB = GetComponent<Rigidbody2D>();

        TF = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (CanMove)
        {
            CM.MoveCheck();
        }

        if (OnCollider && !AC.Iventing)
        {
            CI.ButtonCheck();
        }
    }

    public void CharaInitialize()
    {
        print("Initialize");

        CharaAnimator = GetComponent<Animator>();

        CharaAnimator.SetBool("Walking", false);

        CM.Horizontal = 0;

        RB.velocity = new Vector2(0,0);

        print("InitializeEnd");
    }

    //当たり判定系
    //ぶつかったとき
    private void OnCollisionEnter2D(Collision2D collision)
    {
        AdventureCollider Collider;

        if (collision.gameObject.GetComponent<AdventureCollider>())
        {
            Collider = collision.gameObject.GetComponent<AdventureCollider>();
        }
        else
        {
            return;
        }

        if (Collider.Chara!=AdventureCollider.CharaType.Normal)
        {
            return;
        }

        AdventureIvent Ivent = AC.NowScene.Ivents.Find((ivent) => ivent.ID == Collider.IventID.ToString());


        //イベントのタイプによって処理を変更する
        switch (Collider.Type)
        {
            //もしそのイベントが接触しただけで発生するとしたらイベントを開始する
            case AdventureCollider.IventType.OnContact:

                {
                    //イベントが行われる回数によって処理を変更する
                    switch (Collider.Kaisu)
                    {
                        //一度きりの場合
                        case AdventureCollider.IventKaisu.Once:

                            {
                                if (!Collider.OnceFlag)
                                {
                                    //対応したイベントを探し出し実行
                                    AC.IventStart(Ivent);

                                    Collider.OnceFlag = true;
                                }
                            }

                            break;

                        //無限回だったら
                        case AdventureCollider.IventKaisu.Repeat:

                            {
                                AC.IventStart(Ivent);
                            }

                            break;
                    }
                }

                break;

            case AdventureCollider.IventType.OnButton:

                {
                    //イベントが行われる回数によって処理を変更する
                    switch (Collider.Kaisu)
                    {
                        case AdventureCollider.IventKaisu.Once:

                            {
                                if (!Collider.OnceFlag)
                                {
                                    OnCollider = Collider;

                                    //対応するボタンがあるとそれをオンにする
                                    if (Collider.SupportButton)
                                    {
                                        Collider.SupportButton.SetActive(true);

                                        Collider.SupportButton.GetComponent<SupportButton>().FadeIn();
                                    }
                                }
                            }

                            break;

                        case AdventureCollider.IventKaisu.Repeat:

                            {
                                OnCollider = Collider;

                                //対応するボタンがあるとそれをオンにする
                                if (Collider.SupportButton)
                                {
                                    Collider.SupportButton.SetActive(true);

                                    Collider.SupportButton.GetComponent<SupportButton>().FadeIn();
                                }
                            }

                            break;
                    }
                }

                break;
        }
    }

    //すり抜けた時
    private void OnTriggerEnter2D(Collider2D collision)
    {
        AdventureCollider Collider = new AdventureCollider();

        if (collision.gameObject.GetComponent<AdventureCollider>())
        {
            Collider = collision.gameObject.GetComponent<AdventureCollider>();
        }
        else
        {
            return;
        }

        if (Collider.Chara != AdventureCollider.CharaType.Normal)
        {
            return;
        }

        AdventureIvent Ivent = AC.NowScene.Ivents.Find((ivent) => ivent.ID == Collider.IventID.ToString());

        //イベントのタイプによって処理を変更する
        switch (Collider.Type)
        {
            //もしそのイベントが接触しただけで発生するとしたらイベントを開始する
            case AdventureCollider.IventType.OnContact:

                {
                    //イベントが行われる回数によって処理を変更する
                    switch (Collider.Kaisu)
                    {
                        //一度きりの場合
                        case AdventureCollider.IventKaisu.Once:

                            {
                                if (!Collider.OnceFlag)
                                {
                                    //対応したイベントを探し出し実行
                                    AC.IventStart(Ivent);

                                    Collider.OnceFlag = true;
                                }
                            }

                            break;

                        //無限回だったら
                        case AdventureCollider.IventKaisu.Repeat:

                            {
                                AC.IventStart(Ivent);
                            }

                            break;
                    }
                    
                }

                break;

            case AdventureCollider.IventType.OnButton:

                {
                    //イベントが行われる回数によって処理を変更する
                    switch (Collider.Kaisu)
                    {
                        case AdventureCollider.IventKaisu.Once:

                            {
                                if (!Collider.OnceFlag)
                                {
                                    OnCollider = Collider;

                                    //対応するボタンがあるとそれをオンにする
                                    if (Collider.SupportButton)
                                    {
                                        Collider.SupportButton.SetActive(true);

                                        Collider.SupportButton.GetComponent<SupportButton>().FadeIn();
                                    }
                                }
                            }

                            break;

                        case AdventureCollider.IventKaisu.Repeat:

                            {
                                OnCollider = Collider;

                                //対応するボタンがあるとそれをオンにする
                                if (Collider.SupportButton)
                                {
                                    Collider.SupportButton.SetActive(true);

                                    Collider.SupportButton.GetComponent<SupportButton>().FadeIn();
                                }
                            }

                            break;
                    }

                }

                break;
        }
    }

    //出た時
    private void OnTriggerExit2D(Collider2D collision)
    {
        AdventureCollider Collider = new AdventureCollider();

        if (collision.gameObject.GetComponent<AdventureCollider>())
        {
            Collider = collision.gameObject.GetComponent<AdventureCollider>();
        }
        else
        {
            return;
        }

        if (Collider.Chara != AdventureCollider.CharaType.Normal)
        {
            return;
        }

        //イベントのタイプによって処理を変更する
        switch (Collider.Type)
        {
            case AdventureCollider.IventType.OnButton:

                {
                    switch (Collider.Kaisu)
                    {
                        case AdventureCollider.IventKaisu.Once:

                            {
                                if (!Collider.OnceFlag)
                                {
                                    OnCollider = null;

                                    //対応するボタンがあるとそれをオフにする
                                    if (Collider.SupportButton)
                                    {
                                        Collider.SupportButton.GetComponent<SupportButton>().FadeOut();
                                    }
                                }
                                else
                                {
                                    OnCollider = null;
                                }

                            }

                            break;

                        case AdventureCollider.IventKaisu.Repeat:

                            {
                                OnCollider = null;

                                //対応するボタンがあるとそれをオフにする
                                if (Collider.SupportButton)
                                {
                                    Collider.SupportButton.GetComponent<SupportButton>().FadeOut();
                                }
                            }

                            break;
                    }


                }

                break;
        }
    }
}
