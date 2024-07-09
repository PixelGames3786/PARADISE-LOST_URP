using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DeltaGNBit : MonoBehaviour
{
    public DeltaGNWing GNW;

    public float BitHP;

    public Transform Chara;

    private Vector3 BitPosi;
    public float LookAtConstant;

    public bool Following,Charging;

    public float[] FollowMinMax=new float[2];

    [Space(5)]

    public float[] RandomPositionYMinMax = new float[2];

    [Space(5)]

    public float RandomPosiX;
    private float RandomPosiY;

    private float FollowTime,KeikaTime;

    [Space(5)]

    public float BeamDamage;

    //攻撃系   
    public int AttackCount, Count;

    [Space(10)]

    public Vector2 BeamPosition;
    public Vector3 BeamRotation;

    public GameObject DamagePrefab;
    public Vector3 DamageEffectScale;

    private GameObject ChargeEffect;

    private Transform BeamTrans;

    private AudioGeneral Audio;

    // Start is called before the first frame update
    void Start()
    {
        Chara = GameObject.Find("Charactor").transform;

        Audio = GetComponent<AudioGeneral>();

        Following = true;

        FollowTime = Random.Range(FollowMinMax[0],FollowMinMax[1]);
        RandomPosiY = Random.Range(RandomPositionYMinMax[0],RandomPositionYMinMax[1]);
        
        float PosiX = Random.Range(Chara.position.x-RandomPosiX,Chara.position.x+RandomPosiX);
        float PosiY = Random.Range(Chara.position.y,Chara.position.y+RandomPosiY);

        AttackCount = Random.Range(3,5);
        
        PosiX = Mathf.Clamp(PosiX,-8,53);

        BitPosi = new Vector2(PosiX,PosiY);

        BitPosi = transform.InverseTransformPoint(BitPosi);

        transform.DOLocalMove(BitPosi, 1f).OnComplete(() => Following = false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Following)
        {
            transform.LookAt2D(Chara, LookAtConstant);
        }

        KeikaTime += Time.deltaTime;

        if (KeikaTime>=FollowTime)
        {
            FollowUpdate();
        }
    }

    public void FollowUpdate()
    {
        if (BitHP<=0)
        {
            return;
        }

        Following = true;

        FollowTime = Random.Range(FollowMinMax[0], FollowMinMax[1]);
        RandomPosiY = Random.Range(RandomPositionYMinMax[0], RandomPositionYMinMax[1]);

        KeikaTime = 0;

        float PosiX = Random.Range(Chara.position.x - RandomPosiX, Chara.position.x + RandomPosiX);
        float PosiY = Random.Range(Chara.position.y, Chara.position.y + RandomPosiY);

        PosiY = Mathf.Clamp(PosiY, -5, 1.7f);
        PosiX = Mathf.Clamp(PosiX, -8, 53);

        BitPosi = new Vector2(PosiX, PosiY);

        BitPosi = transform.parent.InverseTransformPoint(BitPosi);

        transform.DOLocalMove(BitPosi, 0.25f).OnComplete(() => 
        {
            Following = false;

            //攻撃判定
            Count++;

            //チャージ
            if (Count==AttackCount-1)
            {
                Audio.PlayClips(0);

                ChargeEffect=Instantiate(GNW.GNC.RiseEffectPre,transform);

                ChargeEffect.transform.localPosition = BeamPosition;

                ChargeEffect.transform.localScale = new Vector3(0.5f,0.5f,1f);

                Charging = true;
            }

            if (Count>=AttackCount)
            {
                Audio.PlayClips(1);

                MakeBeam();

                Destroy(ChargeEffect);

                Count = 0;
                AttackCount = Random.Range(3, 5);

                Charging = false;
            }
        });

        Audio.PlayClips(2);

        //Vector3 direction = Chara.position - transform.parent.parent.TransformPoint(BitPosi);

        //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        //transform.DOLocalRotate(new Vector3(0,0,angle-LookAtConstant),1f);


    }

    public void MakeBeam()
    {
        BeamTrans = Instantiate(GNW.GNC.BitBeamPrefab,transform).transform;

        BeamTrans.localPosition = BeamPosition;
        BeamTrans.localRotation = Quaternion.Euler(BeamRotation);

        BeamTrans.GetComponent<EnemyAttackCollider>().Damage = BeamDamage;

        Sequence sequence = DOTween.Sequence();

        sequence.Append(BeamTrans.DOScaleY(10f,0.1f)).Join(BeamTrans.DOScaleX(0.2f,0.25f)).AppendInterval(FollowTime-KeikaTime-0.6f).Append(BeamTrans.DOScaleX(0f,0.25f)).OnComplete(()=> 
        {
            Destroy(BeamTrans.gameObject);
        });
    }

    public void BitDamage(float Damage)
    {
        BitHP -= Damage;

        if (BitHP<=0)
        {
            BitBreak();
        }
    }

    public void BitBreak()
    {
        GetComponent<TrailRenderer>().enabled = false;

        Transform BreakEffect=Instantiate(GNW.GNC.BitBreakEffectPre).transform;

        BreakEffect.position = transform.position;

        BreakEffect.DOMoveY(-4.5f, 1.5f).OnComplete(() => Destroy(BreakEffect.gameObject));

        if (ChargeEffect)
        {
            Destroy(ChargeEffect);
        }

        if (BeamTrans)
        {
            BeamTrans.DOScaleX(0f, 0.25f).OnComplete(() =>
            {
                Destroy(BeamTrans.gameObject);
            });
        }

        GetComponent<SpriteRenderer>().DOColor(new Color(0f, 0f, 0f, 0f), 1.5f);

        transform.DOMoveY(-4.5f, 1.5f).OnComplete(() => Destroy(gameObject));
    }

    public void MakeDamageEffect(float Rotate)
    {
        switch (Chara.GetComponent<BattleCharaController>().Chara)
        {
            case BattleCharaController.SousaChara.Apple:

                {
                    Tween ScaleIn, ScaleOut;

                    Transform Effect01 = Instantiate(DamagePrefab).transform;
                    Transform Effect02 = Effect01.GetChild(0);

                    Effect01.position = new Vector2(transform.position.x, transform.position.y);

                    Effect01.localScale = new Vector2(DamageEffectScale.x, 0);

                    if (Chara.position.x >= transform.position.x)
                    {
                        Effect01.localRotation = Quaternion.Euler(new Vector3(0, 0, -Rotate));
                        Effect02.localRotation = Quaternion.Euler(new Vector3(-90 + Rotate, 90, -90));

                    }
                    else
                    {

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

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (enabled==false)
        {
            return;
        }

        //自キャラの攻撃だったら
        if (collision.gameObject.GetComponent<CharactorBattleCollider>())
        {
            CharactorBattleCollider Collider = collision.gameObject.GetComponent<CharactorBattleCollider>();

            if (Collider.ColliderType == CharactorBattleCollider.Type.Attack)
            {

                if (BitHP<=0)
                {
                    return;
                }

                //エフェクトを発生
                //GameObject Effect = Instantiate(StateManage.DamagePrefab, transform);

                //Effect.transform.position = Point;

                float EffectRotate = Random.Range(Collider.EffectRotateMinMax[0], Collider.EffectRotateMinMax[1]);

                MakeDamageEffect(EffectRotate);

                //ダメージ処理
                BitDamage(Collider.Damage);
            }
        }
    }
}
