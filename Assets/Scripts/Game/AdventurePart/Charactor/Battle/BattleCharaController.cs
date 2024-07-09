using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleCharaController : MonoBehaviour
{
    public enum SousaChara
    {
        Apple,
    }

    public SousaChara Chara;

    [Space(8)]

    public float HP;
    public float MaxHP;

    [Space(8)]

    public AdventureController AC;

    private BattleCharaInputer BCI;
    private BattleCharaMover BCM;
    
    public HPBerController HPBer;

    public Animator CharaAni;

    public Rigidbody2D RB;
    public Transform TF;

    public AdventureCollider OnCollider;

    [System.NonSerialized]
    public GameObject ChakuchiCollider;

    [Space(5)]

    public GameObject RunEffectPre;
    public GameObject ChakuchiEffectPre;

    public GameObject Effect;

    public float NormalSpeed,AttackingSpeed,JumpSpeed,JumpGravity,DashSpeed,AirDashSpeed;

    [Space(10)]

    public bool CanMove;
    public bool CanAttack,CanDamage;

    private bool CharaStop;

    public int CanDashCount;

    [System.NonSerialized]
    public bool JumpKakou, Attacking, Jumping,Dashing,Dead;

    public bool Damaging;

    private GameObject AttackCollider;

    //音関係
    [System.NonSerialized]
    public AudioSource Audio;

    private List<string> SoundsAddress;
    private List<AudioClip> Sounds;

    //生成したオブジェクトの保管場所
    [SerializeField]
    private Dictionary<string, GameObject> Instantiated=new Dictionary<string, GameObject>();

    public Color PersonalColor;


    // Start is called before the first frame update
    void Start()
    {
        AC = FindObjectOfType<AdventureController>();

        CharaAni = GetComponent<Animator>();

        RB = GetComponent<Rigidbody2D>();

        TF = GetComponent<Transform>();

        BCI = gameObject.AddComponent<BattleCharaInputer>();
        BCM = gameObject.AddComponent<BattleCharaMover>();

        Audio = GetComponent<AudioSource>();

        ChakuchiCollider = transform.GetChild(0).gameObject;

        GetSoundsAddress();
    }

    // Update is called once per frame
    void Update()
    {
        //ダメージモーション中は何もできない
        if (Damaging)
        {
            return;
        }

        if (CanMove)
        {
            BCM.MoveCheck();
        }

        if (CanAttack)
        {
            BCI.AttackCheck();
        }

        if (Jumping)
        {
            BCM.JumpKakouChange();
        }

        if (OnCollider&&!AC.Iventing)
        {
            BCI.ButtonCheck();
        }
    }

    //ダメージモーションが終わった時
    public void DamageHukki()
    {
        Damaging = false;

        CanDamage = true;
    }

    //ダメージを食らう
    public void Damage(float Damage)
    {
        //無敵時間中ならダメージを食らわない
        if (!CanDamage||HP<=0)
        {
            return;
        }

        HP -= Damage;

        //ゲームオーバー
        if (HP<=0)
        {
            CharactorStop();
            CharaInitialize();

            Dead = true;

            AC.StartCoroutine("GameOver");
        }

        CanDamage = false;

        RB.velocity = new Vector2(0,0);

        if (!HPBer)
        {
            HPBer = FindObjectOfType<HPBerController>();
        }

        //HPバーを減らす
        HPBer.Shake(Damage,MaxHP);
        HPBer.HPChange(HP,MaxHP,true,false);

        //攻撃中なら止める
        Attacking = false;

        if (Jumping)
        {
            JumpKakou = true;
        }

        CharaAni.SetTrigger("DamageTrigger");

        GetComponent<SpriteRenderer>().color = new Color(1,1,1,1);
        GetComponent<SpriteRenderer>().DOColor(new Color(1,0,0,0),0.15f).SetEase(Ease.Flash,4);
    }

    //回復する
    public void Heal(float Heal)
    {
        HP += Heal;

        HP = Mathf.Clamp(HP,0,MaxHP);

        if (!HPBer)
        {
            HPBer = FindObjectOfType<HPBerController>();
        }

        //回復SEを鳴らす
        HPBer.Audio.PlayClips(1);

        //HPBerを変える
        HPBer.HPChange(HP,MaxHP,false,true);
    }

    //当たり判定作成
    public void MakeAttackCollider(string PrefabPath)
    {
        if (AttackCollider)
        {
            Destroy(AttackCollider);
        }

        GameObject Prefab = Resources.Load(PrefabPath) as GameObject;

        //当たり判定を作って名前を変える
        AttackCollider=Instantiate(Prefab,transform).gameObject;

        AttackCollider.name = "AttackCollider";
    }

    //当たり判定削除
    public void DestroyAttackCollider()
    {
        //当たり判定があったら破壊する
        if (AttackCollider)
        {
            Destroy(AttackCollider);
        }
    }

    //エフェクト作成
    public void MakeAttackEffect(string EffectPath)
    {
        //なければ新しく作る
        //エフェクトがもう生成されていたら使いまわす
        if (!Instantiated.ContainsKey(EffectPath))
        {
            GameObject Prefab = Resources.Load(EffectPath) as GameObject;

            //エフェクトを作る
            Effect = Instantiate(Prefab, transform);

            Instantiated[EffectPath] = Effect;
        }
        else
        {

            Effect = Instantiated[EffectPath];

            Instantiated[EffectPath].SetActive(true);

            Instantiated[EffectPath].GetComponent<Animator>().Play(EffectPath+"Anim", 0, 0f);
        }
    }

    //エフェクト削除
    public void DestroyAttackEffect()
    {
        if (Effect)
        {
            Effect.transform.GetChild(0).GetComponent<TrailRenderer>().time=0;
            Effect.transform.GetChild(1).GetComponent<TrailRenderer>().time=0;
            Effect.transform.GetChild(2).GetComponent<TrailRenderer>().time=0;
        }
    }

    //走ったり着地したりのエフェクトを作成
    public void MakeEffect(string Type)
    {
        Transform Effect;

        if (PersonalColor==new Color(0,0,0,0))
        {
            PersonalColorData Data = Resources.Load("DataBase/PersonalColors") as PersonalColorData;

            PersonalColor = Data.GetPersonalColor(Chara.ToString());
        }

        switch (Type)
        {
            case "Run":

                {
                    Effect = Instantiate(RunEffectPre).transform;

                    if (transform.localScale.x>0)
                    {

                        Effect.position = new Vector3(transform.position.x - 1.2f, -3.85f, 0);

                    }
                    else
                    {

                        Effect.position = new Vector3(transform.position.x + 1.2f, -3.85f, 0);
                        Effect.localScale = new Vector3(-Effect.transform.localScale.x,Effect.transform.localScale.y,1);
                    }

                    Effect.GetComponent<SpriteRenderer>().color = PersonalColor;
                }

                break;

            case "Chakuchi":

                {
                    Effect = Instantiate(ChakuchiEffectPre).transform;

                    Effect.position = new Vector3(transform.position.x, -1.25f,0);

                    Effect.GetChild(0).GetComponent<SpriteRenderer>().color = PersonalColor;
                    Effect.GetChild(1).GetComponent<SpriteRenderer>().color = PersonalColor;
                }

                break;
        }
    }

    public void CharaInitialize()
    {
        CharaAni = GetComponent<Animator>();

        CharaAni.SetBool("Running", false);

        BCM.Horizontal = 0;

        RB.velocity = new Vector2(0, 0);
    }

    //キャラクターの行動をストップ
    public void CharactorStop()
    {
        CanMove = false;
        CanAttack = false;

        CharaStop = true;
    }

    //キャラクターの行動を許す
    public void CharactorMove()
    {

        CanMove = true;
        CanAttack = true;

        CharaStop = false;
    }

    public void PlaySound(string Target)
    {
        int Index = SoundsAddress.IndexOf(Target);

        AudioClip Clip = Sounds[Index];

        Audio.clip = Clip;

        Audio.Play();
    }

    public void AttackStart()
    {
        Attacking = true;
    }

    public void AttackEnd()
    {
        if (!CharaStop)
        {
            CanAttack = true;
        }

        Attacking = false;
    }

    public void DashAttackCansel()
    {
        CanAttack = true;

        Attacking = false;

        DestroyAttackCollider();

        DestroyAttackEffect();
    }

    public void DashEnd()
    {
        Dashing = false;
        CanDamage = true;

        CharaAni.SetBool("Dashing", false);

        if (Jumping)
        {
            RB.constraints = RigidbodyConstraints2D.FreezeRotation;

            ChakuchiCollider.SetActive(true);

            CharaAni.ResetTrigger("KakouTrigger");
        }

        RB.drag = 0;

    }

    public void GetSoundsAddress()
    {
        string CharaName = Chara.ToString();

        BattleSoundsData Data = Resources.Load("DataBase/BattleSounds/"+CharaName+"BattleSounds") as BattleSoundsData;

        SoundsAddress = Data.GetAddress();
        Sounds = Data.GetSounds();
    }

    //ぶつかった時
    private void OnCollisionEnter2D(Collision2D collision)
    {
        /*
        //壁にぶつかっている場合
        if (collision.gameObject.tag=="Wall")
        {
            //左の壁か右の壁を判断する
            //右の壁の場合
            if (collision.gameObject.transform.position.x>transform.position.x)
            {
                WallContact = 1;
            }
            //右の壁の場合
            else
            {
                WallContact = -1;
            }
        }
        */

        if (Dead) return;

        AdventureCollider Collider;

        if (collision.gameObject.GetComponent<AdventureCollider>())
        {
            Collider = collision.gameObject.GetComponent<AdventureCollider>();
        }
        else
        {
            return;
        }

        if (Collider.Chara != AdventureCollider.CharaType.Battle)
        {
            return;
        }

        if (Collider.BattleingDont && AC.Battleing)
        {
            return;
        }

        ColliderEnter(Collider);


    }

    //すり抜けた時
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Dead) return;

        AdventureCollider Collider;

        if (collision.gameObject.GetComponent<AdventureCollider>())
        {
            Collider = collision.gameObject.GetComponent<AdventureCollider>();
        }
        else
        {
            return;
        }

        if (Collider.Chara != AdventureCollider.CharaType.Battle)
        {
            return;
        }

        if (Collider.BattleingDont && AC.Battleing)
        {
            return;
        }

        ColliderEnter(Collider);
    }

    //離れた時
    private void OnCollisionExit2D(Collision2D collision)
    {

    }

    //出た時
    private void OnTriggerExit2D(Collider2D collision)
    {

        if (Dead) return;

        AdventureCollider Collider;

        if (collision.gameObject.GetComponent<AdventureCollider>())
        {
            Collider = collision.gameObject.GetComponent<AdventureCollider>();
        }
        else
        {
            return;
        }

        if (Collider.Chara != AdventureCollider.CharaType.Battle)
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

    //入り続けているとき
    private void OnTriggerStay2D(Collider2D collision)
    {

        if (Dead) return;

        if (collision.gameObject.tag == "Bit")
        {
            Transform Bit = collision.transform.parent;
            BitDropParent Parent = Bit.parent.GetComponent<BitDropParent>();

            if (!Parent.Tracking)
            {
                return;
            }

            Parent.GetBit(Bit);

        }

    }



    private void ColliderEnter(AdventureCollider Collider)
    {
        AdventureIvent Ivent = AC.NowScene.Ivents.Find((ivent) => ivent.ID == Collider.IventID.ToString());

        if (Ivent==null)
        {
            return;
        }

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
}
