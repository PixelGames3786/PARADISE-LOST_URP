using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DeltaGNSword : MonoBehaviour
{
    private AudioGeneral Audio;

    public enum SwordAttackType
    {
        //突き
        Thrust,
        //斬り
        Slash
    }

    public DeltaGNWing GNW;

    [System.NonSerialized]
    public Vector2 OriginalPosi;
    [System.NonSerialized]
    public Vector3 OriginalQuater;

    public SwordAttackType AttackType;

    public Transform Chara;

    [Space(5)]

    public float[] ReturnHP;

    [Space(5)]

    public float LookAtConstant;

    private Sequence FollowSequence;

    [Space(5)]

    public float RandomPosiX;
    public float RandomPosiY;

    public float[] WaitTimeMinMax=new float[2];

    private float PosiX, PosiY,WaitTime, AttackKeika;

    private bool Following=true,Attacking,Returning,Breaking;

    private int BreakNum;

    private Vector2 SwordPosi,BreakPosi;

    private Rigidbody2D RB;
    private EnemyAttackCollider ACollider;

    [Space(10)]

    public Vector2 ThrustForce;

    // Start is called before the first frame update
    void Start()
    {
        Audio = GetComponent<AudioGeneral>();

        Following = true;
        Returning = false;

        Chara = GameObject.Find("Charactor").transform;

        RB = GetComponent<Rigidbody2D>();
        ACollider = GetComponent<EnemyAttackCollider>();

        FollowChara();
    }

    // Update is called once per frame
    void Update()
    {
        if (Following)
        {
            transform.LookAt2D(Chara,LookAtConstant);
        }

        if (Attacking)
        {
            AttackKeika += Time.deltaTime;

            if (AttackKeika>=2f)
            {
                RB.velocity = new Vector2(0, 0);
                RB.angularVelocity = 0f;

                AttackChange();
            }
        }

        //本体に戻るor破壊される
        if (GNW.GNC.HP<=ReturnHP[GNW.GNC.PurgeNumber-1]&&!Returning&&!Breaking)
        {
            if (GNW.GNC.PurgeNumber<=2)
            {
                ReturnOriginal();
            }
            //破壊される前のぐちゃぐちゃ動くモードに移行
            else
            {
                GNW.GNC.InInvisible();

                GetComponent<PolygonCollider2D>().enabled = false;

                BreakPosi = Chara.position;

                BreakNum = Random.Range(5,10);

                Attacking = false;
                Breaking = true;

                RB.velocity = new Vector2(0, 0);
                RB.angularVelocity = 0;

                Break();
            }
        }
    }

    private void FollowChara()
    {
        PosiX = Random.Range(Chara.position.x-RandomPosiX,Chara.position.x+RandomPosiX);
        PosiY = Random.Range(Chara.position.y+2,Chara.position.y+RandomPosiY);

        WaitTime = Random.Range(WaitTimeMinMax[0],WaitTimeMinMax[1]);

        SwordPosi = new Vector2(PosiX,PosiY);

        SwordPosi = transform.parent.InverseTransformPoint(SwordPosi);

        FollowSequence = DOTween.Sequence();

        FollowSequence.Append(transform.DOLocalMove(SwordPosi, 1.5f)).AppendInterval(WaitTime).OnComplete(() => Attack());

        FollowSequence.Play();

        //transform.DOLocalMove(SwordPosi, 1.5f).OnComplete(() => Attack());
    }

    private void Attack()
    {
        Following = false;
        Attacking = true;

        ACollider.enabled = true;

        switch (AttackType)
        {
            case SwordAttackType.Thrust:

                {
                    Vector2 CharaVelocity = Chara.GetComponent<Rigidbody2D>().velocity;
                    Vector3 CharaPosi = Chara.position;

                    Vector2 ToChara;

                    //偏差撃ち
                    //右
                    if (CharaVelocity.x>0)
                    {
                        CharaPosi = new Vector3(CharaPosi.x+5,CharaPosi.y);

                        ToChara = (CharaPosi - transform.position) * 5;

                    }
                    //左
                    else if (CharaVelocity.x<0)
                    {
                        CharaPosi = new Vector3(CharaPosi.x - 5, CharaPosi.y);

                        ToChara = (CharaPosi - transform.position) * 5;

                    }
                    //動いていない場合（ほとんどないと思うけど）
                    else
                    {
                        ToChara = (Chara.position - transform.position) * 5;

                    }


                    ToChara.x = Mathf.Clamp(ToChara.x,-20,20);
                    ToChara.y = Mathf.Clamp(ToChara.y,-20,20);

                    RB.AddForce(ToChara,ForceMode2D.Impulse);
                }

                break;

            case SwordAttackType.Slash:

                {
                    Vector2 ToChara = (Chara.position - transform.position) * 5;

                    ToChara.x = Mathf.Clamp(ToChara.x, -20, 20);
                    ToChara.y = Mathf.Clamp(ToChara.y, -20, 20);

                    RB.AddForce(ToChara, ForceMode2D.Impulse);

                    RB.AddTorque(400f);

                }

                break;
        }
    }

    private void AttackChange()
    {

        Attacking = false;
        Following = true;

        ACollider.enabled = false;

        AttackKeika = 0;

        switch (AttackType)
        {
            case SwordAttackType.Thrust:

                {
                    AttackType = SwordAttackType.Slash;

                    FollowChara();
                }

                break;

            case SwordAttackType.Slash:

                {
                    AttackType = SwordAttackType.Thrust;

                    FollowChara();
                }

                break;
        }
    }

    private void ReturnOriginal()
    {
        GNW.GNC.InInvisible();

        Following = false;
        Attacking = false;
        Returning = true;

        RB.velocity = new Vector2(0, 0);
        RB.angularVelocity = 0;

        if (FollowSequence.IsPlaying())
        {
            FollowSequence.Kill();
        }

        GNW.GNC.State = DeltaGNController.DeltaGNState.Wait;

        Vector2 TargetPosi = GNW.transform.TransformPoint(OriginalPosi);

        Sequence sequence = DOTween.Sequence();

        Tween Move = transform.DOMove(TargetPosi, 2f);
        Tween Rotate = transform.DORotate(OriginalQuater, 2f);
        Tween Fade = GetComponent<SpriteRenderer>().DOFade(0f, 0.5f);

        sequence.Append(Move).Join(Rotate).Append(Fade).OnComplete(() => GNW.SwordReturn());

        if (GNW.GNC.transform.localScale.x<=0f)
        {
            transform.DOScaleX(-1f,1f);
        }

    }

    private void Break()
    {
        BreakNum--;

        PosiX = Random.Range(BreakPosi.x - RandomPosiX, BreakPosi.x + RandomPosiX);
        PosiY = Random.Range(BreakPosi.y, BreakPosi.y + RandomPosiY);

        SwordPosi = new Vector2(PosiX, PosiY);

        SwordPosi = transform.parent.InverseTransformPoint(SwordPosi);

        transform.DOLocalMove(SwordPosi, 0.2f).OnComplete(() => BreakNext());
    }

    private void BreakNext()
    {
        if (BreakNum>0)
        {
            Break();
        }
        else
        {
            Audio.PlayClips(0);

            GameObject Effect = Instantiate(GNW.GNC.SwordBreakEffectPre);

            Effect.transform.position = transform.position;

            GNW.GNC.GetAngry();

            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag=="Ground"&&!Returning)
        {
            RB.velocity = new Vector2(0,0);
            RB.angularVelocity = 0;

            AttackChange();
        }
    }
}
