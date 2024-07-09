using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DeltaGNController : MonoBehaviour
{
    private AudioGeneral Audio;

    public enum DeltaGNState
    {
        Wait,
        Avoid,
        Sword,
        Angry
    }

    public DeltaGNState State;

    public float HP;

    [Space(7)]

    public float AvoidSpeed;
    public float AvoidDistance,AvoidCharaDistance;

    [Space(7)]

    public float SwordSpeed;
    public float SwordDistance, SwordCharaDistance;

    [Space(7)]

    public float AngrySpeed;
    public float AngryDistance, AngryCharaDistance;

    public bool InvisibleFlag, PurgedFlag,Beaming;

    public Transform Chara;
    public BattleController BC;

    [System.NonSerialized]
    public Transform BitParent;

    private Rigidbody2D RB;
    private Animator Anim;

    private DeltaGNWing[] Wings = new DeltaGNWing[3];

    public int PurgeNumber;

    [Space(10)]

    public float[] IdouMinMax;
    public float[] AngryWaitMinMax;

    private float AngryWait,AngryKeika;

    public GameObject BitBeamPrefab,BeamPrefab, RiseEffectPre,BitBreakEffectPre,SwordBreakEffectPre;
    public GameObject BeamChargePre,BakusanPre01,BakusanPre02;
    private GameObject Beam,BeamCharge;

    [Space(5)]

    public GameObject DamagePrefab;
    public Vector3 DamageEffectScale;

    private Tween BeamLoopTween,HeadTween;

    private Transform Head;
    private float HeadTime=1f,HeadCount;

    private bool HeadLook,Dying,Angrying;

    [System.NonSerialized]
    public bool[] KaiwaFlags = new bool[6];

    [System.NonSerialized]
    public int Special;

    // Start is called before the first frame update
    void Start()
    {

        Chara = GameObject.Find("Charactor").transform;
        BC = FindObjectOfType<BattleController>();

        RB = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();

        Audio = GetComponent<AudioGeneral>();

        for (int i=1;i<4;i++)
        {
            Wings[i - 1] = transform.GetChild(i).GetComponent<DeltaGNWing>();
        }

        MakeBitParent();

        InInvisible();
        ChildFade(0.5f);

        State = DeltaGNState.Avoid;
    }

    // Update is called once per frame
    void Update()
    {
        StateChecker();
    }

    public void StateChecker()
    {
        switch (State)
        {
            case DeltaGNState.Avoid:

                {
                    AvoidDoing();
                }

                break;

            case DeltaGNState.Sword:

                {
                    SwordDoing();
                }

                break;

            case DeltaGNState.Angry:

                {
                    AngryDoing();
                }

                break;
        }
    }

    public void StateChange(DeltaGNState ChangeState)
    {
        State = ChangeState;

        switch (State)
        {
            case DeltaGNState.Avoid:

                {
                    Audio.PlayClips(0);

                    Wings[PurgeNumber].BitPurge();

                    InInvisible();
                    ChildFade(0.5f);
                }

                break;

            case DeltaGNState.Sword:

                {
                    //BattleNovel表示
                    if (PurgeNumber==0)
                    {
                        BC.BattleNovelStart(3);
                    }
                    else if (PurgeNumber==2)
                    {
                        //BC.BattleNovelStart(4);
                    }

                    MakeSword();
                }

                break;

            case DeltaGNState.Angry:

                {
                }

                break;
        }
    }

    public void LookAtChara()
    {
        //false 左 true 右
        bool LookDirection = false;

        if (transform.localScale.x>=0)
        {
            LookDirection = false;
        }
        else
        {
            LookDirection = true;
        }

        //右を向くぜ
        if (Chara.position.x>=transform.position.x)
        {
            //もともと左を向いていたら？
            if (!LookDirection)
            {
                transform.DOScaleX(-1.5f,0.5f);
            }
        }
        //左を向くぜ
        else
        {
            //もともと右を向いていたら？
            if (LookDirection)
            {
                transform.DOScaleX(1.5f,0.5f);
            }
        }
    }
    
    public void MakeSword()
    {
        PurgeNumber++;

        for (int i=0;i<PurgeNumber;i++)
        {
            Wings[i].SwordPurge();
        }

        OutInvisible();
        ChildFade(1f);

    }

    public void GetAngry()
    {
        if (!Angrying)
        {
            Angrying = true;

            BC.BattleNovelStart(5);

            Sequence sequence = DOTween.Sequence();

            sequence.AppendInterval(1f).Append(transform.DOShakePosition(3f, 0.2f, 20, 90, false, false)).OnComplete(() =>
            {
                StateChange(DeltaGNState.Angry);

                RB.velocity = new Vector2(0, 0);

                BC.BattleNovelStart(6);

                BeamStart();
            });

            sequence.Play();
        }

    }

    //各ステートにおいての行動
    private void SwordDoing()
    {
        //-1 左 1 右
        int MoveDirection=0;

        float CharaX = Chara.position.x;
        float EnemyX = transform.position.x;

        //左に行こうね
        if ((CharaX<EnemyX)&&(EnemyX-CharaX>=SwordCharaDistance))
        {
            MoveDirection = -1;
        }
        //右に行こうね
        else if ((CharaX>=EnemyX)&&(CharaX-EnemyX>=SwordCharaDistance))
        {
            MoveDirection = 1;
        }

        if (MoveDirection == 1)
        {
            RB.velocity = new Vector2(SwordSpeed, 0);
        }
        else if (MoveDirection == -1)
        {
            RB.velocity = new Vector2(-SwordSpeed, 0);
        }
        else if (MoveDirection == 0)
        {
            RB.velocity = new Vector2(0, 0);
        }

        LookAtChara();
    }
    
    private void AvoidDoing()
    {
        //-1 左 1 右
        int MoveDirection;

        MoveDirection = 0;

        float CharaX = Chara.position.x;
        float EnemyX = transform.position.x;

        //左に行く
        if ((CharaX >= EnemyX) && (CharaX - EnemyX <= AvoidCharaDistance))
        {
            MoveDirection = -1;
        }
        //右に行く
        else if ((CharaX < EnemyX) && (EnemyX - CharaX <= AvoidCharaDistance))
        {
            MoveDirection = 1;
        }

        //もしDeltaが左壁に近づきすぎたら
        if (EnemyX - IdouMinMax[0] < AvoidDistance)
        {
            MoveDirection = 0;

            if (CharaX < EnemyX)
            {
                MoveDirection = 1;
            }
        }
        //もしDeltaが右壁に近づきすぎたら
        else if (IdouMinMax[1] - EnemyX < AvoidDistance)
        {
            MoveDirection = 0;

            if (CharaX > EnemyX)
            {
                MoveDirection = -1;
            }
        }

        if (MoveDirection == 1)
        {
            RB.velocity = new Vector2(AvoidSpeed, 0);
        }
        else if (MoveDirection == -1)
        {
            RB.velocity = new Vector2(-AvoidSpeed, 0);
        }
        else if (MoveDirection == 0)
        {
            RB.velocity = new Vector2(0, 0);
        }

        LookAtChara();
    }

    private void AngryDoing()
    {
        if (Beaming)
        {
            LookAtChara();

            return;
        }

        AngryKeika += Time.deltaTime;

        if (AngryKeika>=AngryWait)
        {
            AngryKeika = 0;
            RB.velocity = new Vector2(0, 0);

            BeamStart();

            return;
        }

        //-1 左 1 右
        int MoveDirection;

        MoveDirection = 0;

        float CharaX = Chara.position.x;
        float EnemyX = transform.position.x;

        //左に行く
        if ((CharaX >= EnemyX) && (CharaX - EnemyX <= AngryCharaDistance))
        {
            MoveDirection = -1;
        }
        //右に行く
        else if ((CharaX < EnemyX) && (EnemyX - CharaX <= AngryCharaDistance))
        {
            MoveDirection = 1;
        }

        //もしDeltaが左壁に近づきすぎたら
        if (EnemyX - IdouMinMax[0] < AngryDistance)
        {
            MoveDirection = 0;

            if (CharaX < EnemyX)
            {
                MoveDirection = 1;
            }
        }
        //もしDeltaが右壁に近づきすぎたら
        else if (IdouMinMax[1] - EnemyX < AngryDistance)
        {
            MoveDirection = 0;

            if (CharaX > EnemyX)
            {
                MoveDirection = -1;
            }
        }

        if (MoveDirection == 1)
        {
            RB.velocity = new Vector2(AngrySpeed, 0);
        }
        else if (MoveDirection == -1)
        {
            RB.velocity = new Vector2(-AngrySpeed, 0);
        }
        else if (MoveDirection == 0)
        {
            RB.velocity = new Vector2(0, 0);
        }

        LookAtChara();
    }

    public void MakeBitParent()
    {
        BitParent = new GameObject().transform;
        BitParent.name = "BitParent";
        BitParent.transform.position = transform.position;
        BitParent.transform.localScale = transform.localScale;

        Audio.PlayClips(0);

        Wings[PurgeNumber].BitPurge();
    }

    //ビーム関連
    public void BeamStart()
    {
        Anim.SetTrigger("Charge01");

        Beaming = true;
        InvisibleFlag = true;

        AngryWait = Random.Range(AngryWaitMinMax[0],AngryWaitMinMax[1]);
    }

    public void MakeBeamCharge()
    {

        BeamCharge = Instantiate(BeamChargePre,transform);

        BeamCharge.transform.DOScale(new Vector3(1,1,1),0.2f);
    }

    public void MakeBeam()
    {
        Audio.PlayClips(2);

        Beam = Instantiate(BeamPrefab,transform);

        Beam.transform.DOScaleY(2, 0.1f).OnComplete(()=> 
        {
            BeamLoopTween=Beam.transform.DOScaleY(2.2f, 0.1f).SetLoops(-1, LoopType.Yoyo);
        });
    }

    public void DestoryBeam()
    {
        BeamLoopTween.Kill();

        Beam.transform.DOScaleY(0, 0.5f).OnComplete(() => 
        {
            Destroy(Beam);

            BeamCharge.transform.DOScale(new Vector3(0,0),0.5f).OnComplete(()=> 
            {
                Destroy(BeamCharge);
            });
        });
    }

    public void BeamEnd()
    {
        Beaming = false;

        InvisibleFlag = false;
    }

    //攻撃を受けた時のエフェクト作成
    public void MakeDamageEffect(float Rotate)
    {
        switch (Chara.GetComponent<BattleCharaController>().Chara)
        {
            case BattleCharaController.SousaChara.Apple:

                {
                    Tween ScaleIn, ScaleOut;

                    Transform Effect01 = Instantiate(DamagePrefab).transform;
                    Transform Effect02 = Effect01.GetChild(0);

                    Effect01.localScale = new Vector2(DamageEffectScale.x, 0);

                    if (Chara.position.x >= transform.position.x)
                    {
                        Effect01.position = new Vector2(transform.position.x - 0.5f, transform.position.y);

                        Effect01.localRotation = Quaternion.Euler(new Vector3(0, 0, -Rotate));
                        Effect02.localRotation = Quaternion.Euler(new Vector3(-90 + Rotate, 90, -90));
                    }
                    else
                    {
                        Effect01.position = new Vector2(transform.position.x + 0.5f, transform.position.y);

                        Effect01.localRotation = Quaternion.Euler(new Vector3(0, 0, Rotate));
                        Effect02.localRotation = Quaternion.Euler(new Vector3(-90 - Rotate, 90, -90));
                    }

                    ScaleIn = Effect01.DOScale(new Vector2(DamageEffectScale.x, DamageEffectScale.y), 0.1f);
                    ScaleOut = Effect01.DOScale(new Vector2(DamageEffectScale.x, 0), 0.1f);

                    Sequence sequence = DOTween.Sequence();

                    sequence.Append(ScaleIn).Append(ScaleOut).OnComplete(() =>
                    {
                        Destroy(Effect01.gameObject);
                    });
                }

                break;
        }
    }


    //ダメージ処理
    public void Damage(float Damage)
    {
        HP -= Damage;
        
        //死亡したら
        if (HP<=0&&!Dying)
        {
            Dying = true;

            StartCoroutine("Dead");
        }
    }

    IEnumerator Dead()
    {
        State = DeltaGNState.Wait;
        RB.velocity = new Vector2(0, 0);

        //ストーリー上で死ぬとき(最初のダンジョンのボス)
        if (Special==1)
        {
            BGMController.Controller.BGMFadeOut();

            BC.BattleNovelForcedEnd(0);

            Chara.GetComponent<BattleCharaController>().CharactorStop();
            Chara.GetComponent<BattleCharaController>().CharaInitialize();

            Time.timeScale = 0.5f;

            Camera.main.DOShakePosition(1f, 1f, 20);

            yield return new WaitForSeconds(1f);

            Time.timeScale = 1f;

            Anim.enabled = false;

            for (int i = 1; i < 5; i++)
            {
                Transform Tag = transform.GetChild(0).GetChild(i);

                Tag.DOShakePosition(2, 0.5f, 25).OnComplete(() =>
                {
                    Tag.DORotate(new Vector3(0, 0, Random.Range(0, 360)), 1.5f);
                    Tag.DOLocalMoveY(-4.5f, 1.5f);
                    Tag.GetComponent<SpriteRenderer>().DOColor(new Color(0, 0, 0, 1), 1.5f).OnComplete(() => Destroy(Tag.gameObject));
                });
            }

            yield return new WaitForSeconds(3f);

            Head = transform.GetChild(0).GetChild(0);

            HeadLook = true;

            HeadTweenLoop();

            yield return new WaitForSeconds(7.3f);

            Audio.PlayClips(2);

            Transform BakusanEfe01, BakusanEfe02;

            BakusanEfe01 = Instantiate(BakusanPre01).transform;

            BakusanEfe01.position = Head.transform.position;

            BakusanEfe01.DOScale(new Vector3(5, 5, 1), 0.2f);
            BakusanEfe01.DORotate(new Vector3(0, 0, 360), 0.5f, RotateMode.FastBeyond360).OnComplete(() =>
            {
                float Factor = Mathf.Pow(2, 40);
                BakusanEfe01.GetComponent<SpriteRenderer>().material.SetColor("_Color", new Color(1f * Factor, 1f * Factor, 1f * Factor));
            });

            yield return new WaitForSeconds(0.6f);

            BakusanEfe02 = Instantiate(BakusanPre02).transform;

            BakusanEfe02.position = Head.transform.position;

            Camera.main.DOShakePosition(1f, 1.3f, 20);

            Destroy(BakusanEfe01.gameObject);

            Destroy(Head.gameObject);

            yield return new WaitForSeconds(3f);

            AdventureIvent Ivent = BC.AC.NowScene.Ivents.Find((ivent) => ivent.ID == "4");

            BC.AC.IventStart(Ivent);

            Destroy(gameObject);

            yield return null;
        }

    }

    public void HeadTweenLoop()
    {
        if (HeadCount>=10)
        {
            Head.DOMove(new Vector3(Chara.position.x, Chara.position.y + 3), 0.1f);
            Head.DORotate(new Vector3(0,0,-62),0.1f);

            return;
        }

        float PosiY, PosiX;

        PosiY = Random.Range(Chara.position.y + 1, Chara.position.y + 4);
        PosiX = Random.Range(Chara.position.x - 3, Chara.position.x + 3);

        Head.DORotate(new Vector3(0,0,Random.Range(0,360)),HeadTime);
        HeadTween = Head.DOMove(new Vector3(PosiX, PosiY), HeadTime).OnComplete(()=> 
        {
            if (HeadTime > 0.1f)
            {
                HeadTime -= 0.1f;

            }
            else
            {
                HeadCount += 1;

            }

            HeadTweenLoop();
        });

    }

    //無敵状態になろう！
    public void InInvisible()
    {
        InvisibleFlag = true;
    }

    public void OutInvisible()
    {
        InvisibleFlag = false;
    }

    //子オブジェクトのフェード
    public void ChildFade(float Target)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform Child = transform.GetChild(i);

            if (Child.GetComponent<SpriteRenderer>())
            {
                Child.GetComponent<SpriteRenderer>().DOColor(new Color(1f, 1f, 1f, Target), 0.5f);
            }

            for (int u = 0; u < Child.childCount; u++)
            {
                Transform GrandChild = Child.GetChild(u);

                if (GrandChild.GetComponent<SpriteRenderer>())
                {
                    GrandChild.GetComponent<SpriteRenderer>().DOColor(new Color(1f, 1f, 1f, Target), 0.5f);
                }
            }
        }
    }

    //当たり判定
    public void OnTriggerEnter2D(Collider2D collision)
    {

        //自キャラの攻撃だったら
        if (collision.gameObject.GetComponent<CharactorBattleCollider>())
        {
            //もし無敵状態だったら
            if (InvisibleFlag)
            {
                if (!KaiwaFlags[0])
                {
                    KaiwaFlags[0] = true;

                    BC.BattleNovelStart(2);
                }

                return;
            }

            CharactorBattleCollider Collider = collision.gameObject.GetComponent<CharactorBattleCollider>();

            if (Collider.ColliderType == CharactorBattleCollider.Type.Attack)
            {

                //エフェクトを発生
                //GameObject Effect = Instantiate(StateManage.DamagePrefab, transform);

                //Effect.transform.position = Point;

                float EffectRotate = Random.Range(Collider.EffectRotateMinMax[0], Collider.EffectRotateMinMax[1]);

                MakeDamageEffect(EffectRotate);

                //ダメージ処理
                Damage(Collider.Damage);
            }
        }
    }
}
