using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyColliderTrigger : MonoBehaviour
{
    public enum ColliderType
    {
        ChaseRange,
        AttackRange,
        Body
    }

    public ColliderType Type;

    private EnemyStateAbstract StateManage;

    // Start is called before the first frame update
    void Start()
    {
        StateManage = transform.parent.GetComponent<EnemyStateAbstract>();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Vector3 Point = collision.ClosestPoint(transform.position);

        //コライダーのタイプで処理変更
        switch (Type)
        {
            case ColliderType.ChaseRange:

                {
                    if (collision.tag=="Charactor")
                    {
                        StateManage.StateChange(EnemyStateAbstract.EnemyState.Chase, collision.transform);
                    }

                }

                break;

            case ColliderType.AttackRange:

                {
                    if (collision.tag == "Charactor")
                    {
                        StateManage.StateChange(EnemyStateAbstract.EnemyState.Attack,collision.transform);
                    }
                }

                break;

            case ColliderType.Body:

                {

                    //壁だったら方向転換
                    if (collision.tag == "Wall"||collision.tag=="EnemyWall")
                    {

                        StateManage.MoveDirection = !StateManage.MoveDirection;
                        
                        StateManage.MoveTimeReset();
                    }

                    //自キャラの攻撃だったら
                    if (collision.gameObject.GetComponent<CharactorBattleCollider>())
                    {
                        CharactorBattleCollider Collider = collision.gameObject.GetComponent<CharactorBattleCollider>();

                        if (Collider.ColliderType == CharactorBattleCollider.Type.Attack)
                        {

                            if (StateManage.HP<=0)
                            {
                                return;
                            }

                            //エフェクトを発生
                            //GameObject Effect = Instantiate(StateManage.DamagePrefab, transform);

                            //Effect.transform.position = Point;

                            float EffectRotate = Random.Range(Collider.EffectRotateMinMax[0],Collider.EffectRotateMinMax[1]);

                            StateManage.MakeDamageEffect(EffectRotate);

                            //ダメージ処理
                            StateManage.Damage(Collider.Damage);
                        }
                    }
                }

                break;
                
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        //コライダーのタイプで処理変更
        switch (Type)
        {
            case ColliderType.ChaseRange:

                {
                    if (collision.tag=="Charactor")
                    {
                        StateManage.StateChange(EnemyStateAbstract.EnemyState.Move);

                    }
                }

                break;

            case ColliderType.AttackRange:

                {
                    if (collision.tag == "Charactor")
                    {
                        StateManage.StateChange(EnemyStateAbstract.EnemyState.Chase, collision.transform);
                    }
                }

                break;
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        Vector3 Point = collision.ClosestPoint(transform.position);

        //コライダーのタイプで処理変更
        switch (Type)
        {
            case ColliderType.ChaseRange:

                {
                    if (collision.tag == "Charactor"&& StateManage.State != EnemyStateAbstract.EnemyState.Chase)
                    {
                        if (StateManage.State!=EnemyStateAbstract.EnemyState.Attack)
                        {
                            StateManage.StateChange(EnemyStateAbstract.EnemyState.Chase, collision.transform);

                        }
                    }

                }

                break;

            case ColliderType.AttackRange:

                {
                    if (collision.tag == "Charactor"&& StateManage.State != EnemyStateAbstract.EnemyState.Attack)
                    {
                        StateManage.StateChange(EnemyStateAbstract.EnemyState.Attack, collision.transform);
                    }
                }

                break;
        }
        }
}
