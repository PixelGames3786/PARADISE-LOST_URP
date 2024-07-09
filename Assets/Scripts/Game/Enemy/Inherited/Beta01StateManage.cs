using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Beta01StateManage : EnemyStateAbstract
{
    [Space(5)]
    public float ChaseSpeed;

    private GameObject AttackCollider;

    public GameObject[] AttackCoPrefabs;

    [Space(10)]

    public float WaitTime;

    public float WaitDelta;

    // Update is called once per frame
    void Update()
    {
        StateDoing();
    }

    public override void AttackChecker()
    {
        if (Waiting)
        {
            WaitDelta += Time.deltaTime;

            if (WaitDelta >= WaitTime)
            {
                Waiting = false;

                State = EnemyState.Idle;

                RB.velocity = new Vector2(0, 0);

                Attacking = true;

                PlayAudio(0);

                Animator.SetTrigger("AttackTrigger");

                if (HP <= 6)
                {
                    Animator.SetTrigger("Attack02");
                }
                else
                {
                    Animator.SetTrigger("Attack01");
                }
            }
        }
    }

    public void AttackEnd()
    {
        if (CharaTrans)
        {
            StateChange(EnemyState.Chase,CharaTrans);
        }
        else
        {
            StateChange(EnemyState.Move);
        }

        Waiting = true;
        Attacking = false;

        WaitDelta = 0.05f;
    }

    public void MakeAttackCo(int Num)
    {
        AttackCollider = Instantiate(AttackCoPrefabs[Num],transform);
    }

    public void DestroyAttackCo()
    {
        Destroy(AttackCollider);
    }

    public void AttackStart()
    {
        PlayAudio(1);

        if (HP <= 6)
        {
            Animator.SetTrigger("Attack02");
        }
        else
        {
            Animator.SetTrigger("Attack01");
        }
    }

    public override void ChaseChecker()
    {
        //キャラクターが左右どちらにいるのか判断

        if (!CharaTrans)
        {
            StateChange(EnemyState.Move);
        }

        //左にいる場合
        if (CharaTrans.position.x <= transform.position.x)
        {
            MoveDirection = false;
        }
        //右にいる場合
        else
        {
            MoveDirection = true;
        }

        //左行き
        if (!MoveDirection)
        {
            transform.localScale = new Vector3(EnemySize.x, EnemySize.y);

            //一定距離以上近づかない
            if (Mathf.Abs(transform.position.x) - Mathf.Abs(CharaTrans.position.x) >= 2f)
            {
                RB.velocity = new Vector2(-MoveSpeed, RB.velocity.y);
            }
        }
        //右行き
        else
        {
            transform.localScale = new Vector3(-EnemySize.x, EnemySize.y);

            //一定距離以上近づかない
            if (Mathf.Abs(CharaTrans.position.x) - Mathf.Abs(transform.position.x) >= 2f)
            {
                RB.velocity = new Vector2(MoveSpeed, RB.velocity.y);
            }

        }
    }

    public override void MoveChecker()
    {
        MoveKeika += Time.deltaTime;

        if (MoveKeika >= MoveTime)
        {
            //方向転換
            MoveDirection = !MoveDirection;

            MoveKeika = 0;
        }

        //左に行く場合
        if (!MoveDirection)
        {
            transform.localScale = new Vector3(EnemySize.x, EnemySize.y);

            RB.velocity = new Vector2(-MoveSpeed, RB.velocity.y);
        }
        //右に行く場合
        else
        {
            transform.localScale = new Vector3(-EnemySize.x, EnemySize.y);

            RB.velocity = new Vector2(MoveSpeed, RB.velocity.y);
        }
    }

    public override void Rise(float Time)
    {
        //エフェクト作成
        if (RiseEffect)
        {
            GameObject Effect = Instantiate(RiseEffect);

            Effect.transform.position = gameObject.transform.position;
        }

        float TargetSize;

        //右向き
        if (MoveDirection)
        {
            TargetSize = EnemySize.x;
        }
        else
        {
            TargetSize = -EnemySize.x;
        }

        transform.DOScaleX(TargetSize, Time).OnComplete(() =>
        {
            if (State != EnemyState.Stop)
            {
                State = EnemyState.Move;
            }
        });
    }

    public override void Disappear(float Time)
    {
        //エフェクト作成
        if (DisappearEffect)
        {
            GameObject Effect = Instantiate(DisappearEffect);

            Effect.transform.position = gameObject.transform.position;
        }

        MakeBitDrop();

        StateChange(EnemyState.Stop);

        GetComponent<SpriteRenderer>().DOFade(0, Time).OnComplete(() => Destroy(gameObject));
    }

    public override void ColliderSet()
    {
        float Size;

        Size = Random.Range(SearchMinMax[0], SearchMinMax[1]);

        transform.GetChild(0).GetComponent<CircleCollider2D>().radius = Size;
    }

    public override void StateChange(EnemyState ChangeState, Transform Chara = null)
    {
        if (State == EnemyState.Stop)
        {
            return;
        }


        switch (ChangeState)
        {
            case EnemyState.Idle:

                {
                    
                }

                break;

            case EnemyState.Move:

                {
                    MoveTime = Random.Range(MoveTimeMinMax[0], MoveTimeMinMax[1]);

                    MoveKeika = 0;

                    //もし左向きだったら
                    if (transform.localScale.x == -EnemySize.x)
                    {
                        MoveDirection = true;
                    }
                }

                break;

            case EnemyState.Chase:

                {
                    CharaTrans = Chara;
                }

                break;

            case EnemyState.Attack:

                {
                    if (Waiting)
                    {
                        State = ChangeState;

                        return;
                    }

                    if (WaitDelta==0f)
                    {

                        State = EnemyState.Idle;

                        RB.velocity = new Vector2(0, 0);

                        Animator.SetTrigger("AttackTrigger");

                        PlayAudio(0);
                    }


                    if (CharaTrans == null)
                    {
                        CharaTrans = Chara;
                    }
                }

                break;
        }

        State = ChangeState;


    }
}
