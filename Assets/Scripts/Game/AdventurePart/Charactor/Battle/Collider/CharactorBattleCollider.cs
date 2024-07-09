using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactorBattleCollider : MonoBehaviour
{
    public enum Type
    {
        Body,
        Attack,
        Chakuchi,
    }

    public BattleCharaController CC;
    private EnemyAttackCollider SlipCollider,AttackCollider;

    public Type ColliderType;

    private bool SlipDamaging;

    private float SlipWait;
    public float Damage,SlipDamage;

    public float[] EffectRotateMinMax=new float[2];

    // Start is called before the first frame update
    void Start()
    {
        CC = transform.parent.GetComponent<BattleCharaController>();
    }

    private void Update()
    {
        if (SlipDamaging)
        {
            SlipWait += Time.deltaTime;

            if (SlipWait>=0.3f)
            {
                SlipWait = 0;

                CC.Damaging = true;

                CC.Damage(SlipDamage);
            }
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        

        switch (ColliderType)
        {
            case Type.Body:

                {
                    if (collision.gameObject.GetComponent<EnemyAttackCollider>())
                    {
                        EnemyAttackCollider EnemyAttack = collision.gameObject.GetComponent<EnemyAttackCollider>();

                        if (SlipDamaging && EnemyAttack == SlipCollider)
                        {
                            SlipDamaging = false;

                            SlipWait = 0;
                        }

                        if (EnemyAttack==AttackCollider)
                        {
                            EnemyAttack.Hit = false;
                        }

                    }

                    
                }

                break;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Vector3 Point = collision.ClosestPoint(transform.position);

        switch (ColliderType)
        {
            case Type.Body:

                {
                    //敵の攻撃がぶつかったら
                    if (collision.gameObject.GetComponent<EnemyAttackCollider>())
                    {
                        EnemyAttackCollider EnemyAttack = collision.gameObject.GetComponent<EnemyAttackCollider>();

                        if (CC.Dashing||!EnemyAttack.enabled||EnemyAttack.Hit)
                        {
                            return;
                        }

                        if (EnemyAttack.Type==EnemyAttackCollider.AttackType.Slip)
                        {
                            SlipDamaging = true;

                            SlipDamage = EnemyAttack.Damage;

                            SlipCollider = EnemyAttack;
                        }

                        AttackCollider = EnemyAttack;

                        EnemyAttack.Hit = true;

                        CC.Damaging = true;

                        CC.Damage(EnemyAttack.Damage);

                        /*

                        //左から殴られたら右に飛ばす
                        if (transform.position.x>=collision.transform.position.x)
                        {
                            CC.RB.AddForce(new Vector3(5, 0, 0), ForceMode2D.Impulse);
                        }
                        //右から殴られたら左に飛ばす
                        else
                        {
                            CC.RB.AddForce(new Vector3(-5, 0, 0), ForceMode2D.Impulse);
                        }

    */
                    }
                }

                break;
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        switch (ColliderType)
        {
            case Type.Chakuchi:

                {
                    if (collision.gameObject.tag == "Ground")
                    {
                        //ジャンプの着地判定
                        if (CC.JumpKakou)
                        {
                            GetComponentInParent<BattleCharaMover>().JumpChakuchi();
                        }
                    }

                }

                break;
        }
    }
}
