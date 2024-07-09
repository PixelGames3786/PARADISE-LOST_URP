using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Alpha01StateManage : EnemyStateAbstract
{
    private GameObject StandbyEffect;

    [Space(10)]
    public GameObject BulletPrefab;
    public GameObject StandbyPrefab;

    private GameObject Bullet;

    public float BulletSpeed,BulletWait;

    public float WaitTime;

    // Update is called once per frame
    void Update()
    {
        StateDoing();
    }

    /// <summary>
    /// チェッカーシリーズ
    /// </summary>
    public override void MoveChecker()
    {
        MoveKeika += Time.deltaTime;

        if (MoveKeika>=MoveTime)
        {
            //方向転換
            MoveDirection = !MoveDirection;

            MoveKeika = 0;
        }

        //左に行く場合
        if (!MoveDirection)
        {
            transform.localScale = new Vector3(EnemySize.x,EnemySize.y);

            RB.velocity = new Vector2(-MoveSpeed,RB.velocity.y);
        }
        //右に行く場合
        else
        {
            transform.localScale = new Vector3(-EnemySize.x, EnemySize.y);

            RB.velocity = new Vector2(MoveSpeed, RB.velocity.y);
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

    public override void AttackChecker()
    {
        //向きをキャラのいる方へ向かせる
        //左にいる場合
        if (CharaTrans.position.x <= transform.position.x)
        {
            transform.localScale = new Vector3(EnemySize.x, EnemySize.y);
        }
        //右にいる場合
        else
        {
            transform.localScale = new Vector3(-EnemySize.x, EnemySize.y);
        }

        if (Waiting)
        {
            WaitTime += Time.deltaTime;

            if (WaitTime>=BulletWait)
            {
                Animator.SetTrigger("AttackTrigger");

                Waiting = false;
                Attacking = true;
            }
        }

        if (!Bullet&&!Waiting)
        {
            Waiting = true;

            WaitTime = 0.05f;
        }
    }



    public override void StateChange(EnemyState ChangeState, Transform Chara = null)
    {
        if (State==EnemyState.Stop)
        {
            return;
        }

        Waiting = false;

        switch (ChangeState)
        {
            case EnemyState.Idle:

                {
                    Animator.SetBool("Walking",false);
                }

                break;

            case EnemyState.Move:

                {
                    MoveTime = Random.Range(MoveTimeMinMax[0], MoveTimeMinMax[1]);

                    MoveKeika = 0;

                    Animator.SetBool("Walking",true);

                    //もし左向きだったら
                    if (transform.localScale.x==-EnemySize.x)
                    {
                        MoveDirection = true;
                    }
                }

                break;

            case EnemyState.Chase:

                {
                    if (Attacking)
                    {
                        return;
                    }

                    CharaTrans = Chara;

                    Animator.SetBool("Walking", true);

                }

                break;

            case EnemyState.Attack:

                {
                    //State = EnemyState.Idle;

                    RB.velocity = new Vector2(0, 0);

                    if (WaitTime==0)
                    {
                        Animator.SetTrigger("AttackTrigger");

                        Attacking = true;
                    }

                    Animator.SetBool("Walking",false);

                    if (CharaTrans==null)
                    {
                        CharaTrans = Chara;
                    }
                }

                break;
        }

        State = ChangeState;

    }

    public void MakeStandby()
    {
        if (StandbyEffect)
        {
            StandbyEffect.SetActive(true);
        }
        else
        {
            StandbyEffect=Instantiate(StandbyPrefab, transform);

        }
    }

    public void MakeBullet()
    {
        if (State != EnemyState.Attack&&State!=EnemyState.Stop)
        {
            State = EnemyState.Attack;
        }

        Attacking = false;

        PlayAudio(2);

        Bullet = Instantiate(BulletPrefab, transform);

        //右に向かって撃つとき
        if (CharaTrans.position.x > transform.position.x)
        {
            Bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(BulletSpeed, 0f);

            Bullet.transform.localRotation = Quaternion.Euler(0,180,180);
        }
        //左に向かって撃つとき
        else
        {
            Bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(-BulletSpeed, 0f);
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

        PlayAudio(0);

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

        transform.DOScaleX(TargetSize,Time).OnComplete(()=>
        {
            if (State!=EnemyState.Stop)
            {
                State = EnemyState.Move;

                Animator.SetBool("Walking", true);
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

        PlayAudio(1);

        StateChange(EnemyState.Stop);

        GetComponent<SpriteRenderer>().DOFade(0, Time).OnComplete(()=>Destroy(gameObject));
    }

    public override void ColliderSet()
    {
        float Size;

        Size = Random.Range(SearchMinMax[0],SearchMinMax[1]);

        transform.GetChild(0).GetComponent<CircleCollider2D>().radius = Size;

        Size = Random.Range(AttackMinMax[0],AttackMinMax[1]);

        transform.GetChild(1).GetComponent<BoxCollider2D>().size = new Vector2(Size*2,Size*2);
    }
}
