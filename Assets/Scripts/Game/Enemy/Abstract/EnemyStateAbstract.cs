using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

abstract public class EnemyStateAbstract : MonoBehaviour
{
    public enum EnemyState
    {
        Stop,
        Idle,
        Move,
        Chase,
        Attack,
    }

    public EnemyState State;

    public float HP;

    public Vector2 EnemySize;

    [Space(10)]

    public float RiseTime;
    public float DisappearTime;

    [Space(10)]

    public float MoveSpeed;
    public float[] MoveTimeMinMax;

    [Space(5)]
    public float[] SearchMinMax;

    [Space(5)]
    public float[] AttackMinMax;

    [Space(5)]
    public int[] BitMinMax;

    [Space(5)]
    public Vector2 DamageEffectScale;

    protected float MoveTime,MoveKeika;

    protected int DropBit;

    //移動方向 false:左 true:右
    public bool MoveDirection;

    protected bool Waiting,Attacking,Dead;

    protected Animator Animator;
    protected Rigidbody2D RB;

    [Space(10)]

    public Transform CharaTrans;

    protected AudioSource Audio;

    [Space(5)]

    public GameObject DamagePrefab;

    [Space(10)]

    public GameObject RiseEffect;
    public GameObject DisappearEffect;
    public GameObject BitDropEffect;

    [Space(5)]

    public AudioClip[] Clips;

    void Start()
    {
        Audio = GetComponent<AudioSource>();

        Animator = GetComponent<Animator>();
        RB = GetComponent<Rigidbody2D>();

        MoveTime = Random.Range(MoveTimeMinMax[0], MoveTimeMinMax[1]);

        DropBit = Random.Range(BitMinMax[0],BitMinMax[1]);

        ColliderSet();

        Rise(RiseTime);
    }

    public void StateDoing()
    {
        switch (State)
        {
            case EnemyState.Move:

                {
                    MoveChecker();
                }

                break;

            case EnemyState.Chase:

                {
                    ChaseChecker();
                }

                break;

            case EnemyState.Attack:

                {
                    AttackChecker();
                }

                break;
        }
    }

    abstract public void MoveChecker();

    abstract public void ChaseChecker();

    abstract public void AttackChecker();

    abstract public void StateChange(EnemyState ChangeState, Transform Chara = null);

    public void MakeDamageEffect(float Rotate)
    {
        switch (CharaTrans.GetComponent<BattleCharaController>().Chara)
        {
            case BattleCharaController.SousaChara.Apple:

                {
                    Tween ScaleIn,ScaleOut;

                    Transform Effect01 = Instantiate(DamagePrefab).transform;
                    Transform Effect02 = Effect01.GetChild(0);

                    Effect01.position = new Vector2(transform.position.x, transform.position.y);

                    Effect01.localScale = new Vector2(DamageEffectScale.x, 0);

                    if (CharaTrans.position.x>=transform.position.x)
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

    public void Damage(float Damage)
    {
        if (Dead) return;

        HP -= Damage;

        if (HP<=0)
        {
            Dead = true;

            Disappear(DisappearTime);
        }
    }

    abstract public void Rise(float Time);

    abstract public void Disappear(float Time);

    abstract public void ColliderSet();

    public void MakeBitDrop()
    {
        if (SaveDataManager.DataManage.Data.CanBit)
        {

            GameObject BitEffect = Instantiate(BitDropEffect);

            BitEffect.transform.position = transform.position;

            BitEffect.GetComponent<BitDropParent>().BitNumber = DropBit;

            BitEffect.GetComponent<BitDropParent>().EachBit = 1;
        }
    }

    public void MoveTimeReset()
    {
        MoveTime = Random.Range(MoveTimeMinMax[0], MoveTimeMinMax[1]);

        MoveKeika = 0;

        RB.velocity = new Vector2(0,0);
    }

    public void PlayAudio(int Pointer)
    {
        Audio.clip = Clips[Pointer];

        Audio.Play();
    }
}
