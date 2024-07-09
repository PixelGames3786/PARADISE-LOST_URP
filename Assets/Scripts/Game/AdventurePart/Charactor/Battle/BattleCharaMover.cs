using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ParticleSystemJobs;

public class BattleCharaMover : InputComponent
{
    private BattleCharaController CC;

    public float XSpeed, Horizontal;
    private float OriginalGravity;

    // Start is called before the first frame update
    void Start()
    {
        CC = GetComponent<BattleCharaController>();
    }

    public void MoveCheck()
    {
        //ジャンプ
        if (!CC.Jumping&&!CC.Attacking&&!CC.Dashing)
        {
            if (InputManager.IsController)
            {
                if (GetKeyDown(Button: XboxControllerButtons.A))
                {
                    Jump();
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.L))
                {
                    Jump();
                }
            }
        }

        //ダッシュ
        if (!CC.Dashing)
        {
            if (InputManager.IsController)
            {
                if (GetKeyDown(Button:XboxControllerButtons.X))
                {
                    Dash();
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.H))
                {
                    Dash();
                }
            }
        }


        //左右移動
        if (Horizontal!=0&&!CC.Dashing)
        {
            Move();
        }
        else
        {
            CC.CharaAni.SetBool("Running", false);

            XSpeed = 0;

        }

        if (!CC.Dashing)
        {
            CC.RB.velocity = new Vector3(XSpeed, CC.RB.velocity.y, 0);
        }

        //コントローラーの入力取得
        if (InputManager.IsController)
        {
            Horizontal = (GetKeyAxis(Axes: XboxControllerAxes.DpadHorizontal) * -1) + GetKeyAxis(Axes: XboxControllerAxes.LeftstickHorizontal);
        }
        else
        {
            Horizontal = GetKeyAxis(Set: KeybordSets.Horizontal);
        }
    }

    //左右移動処理
    public void Move()
    {
        //右向き
        if (Horizontal > 0)
        {
            if (CC.TF.localScale.x==-2&&CC.Attacking)
            {
                return;
            }

            CC.TF.localScale = new Vector3(2, 2, 1);

            CC.CharaAni.SetBool("Running", true);

            //もし攻撃中ならゆっくり動かす
            if (CC.Attacking)
            {
                XSpeed = CC.AttackingSpeed;
            }
            else
            {
                XSpeed = CC.NormalSpeed;
            }

            /*

            //右の壁に接触しているとき右に行かない
            if (CC.WallContact==1)
            {
                XSpeed = 0;
            }

    */
        }
        //左向き
        else if (Horizontal < 0)
        {
            if (CC.TF.localScale.x == 2 && CC.Attacking)
            {
                return;
            }

            CC.TF.localScale = new Vector3(-2, 2, 1);

            CC.CharaAni.SetBool("Running", true);

            if (CC.Attacking)
            {
                XSpeed = -CC.AttackingSpeed;
            }
            else
            {
                XSpeed = -CC.NormalSpeed;
            }

            /*
            //左の壁に接触しているとき左に行かない
            if (CC.WallContact==-1)
            {
                XSpeed = 0;
            }

    */
        }

    }

    //ジャンプ処理
    public void Jump()
    {

        CC.CharaAni.SetTrigger("JumpTrigger");
        CC.CharaAni.ResetTrigger("KakouTrigger");

        CC.CharaAni.SetBool("Jumping",true);

        CC.RB.AddForce(new Vector3(0, CC.JumpSpeed, 0), ForceMode2D.Impulse);

        OriginalGravity = CC.RB.gravityScale;
        CC.RB.gravityScale = CC.JumpGravity;

        CC.Jumping = true;
        CC.JumpKakou = false;
    }

    public void JumpKakouChange()
    {
        if (CC.RB.velocity.y<=0.2f&&!CC.JumpKakou)
        {
            CC.JumpKakou = true;

            CC.CharaAni.SetTrigger("KakouTrigger");
        }
    }

    public void JumpChakuchi()
    {
        if (CC.Dashing)
        {
            CC.DashEnd();
        }

        CC.CharaAni.SetTrigger("ChakuchiTrigger");
        CC.CharaAni.ResetTrigger("KakouTrigger");
        CC.CharaAni.ResetTrigger("JumpTrigger");

        CC.CharaAni.SetBool("Jumping", false);

        CC.Jumping = false;
        CC.JumpKakou = false;

        if (CC.Damaging)
        {
            CC.DamageHukki();
        }

        CC.RB.gravityScale = OriginalGravity;

        CC.MakeEffect("Chakuchi");
    }

    //ダッシュ処理
    public void Dash()
    {
        if (!SaveDataManager.DataManage.Data.CanDash||CC.CanDashCount<=0||CC.Damaging)
        {
            return;
        }

        if (!CC.HPBer)
        {
            CC.HPBer = FindObjectOfType<HPBerController>();
        }

        CC.HPBer.DashC.DecreaseGauge();

        //ダッシュできる回数を減らす
        CC.CanDashCount--;

        CC.CharaAni.SetBool("Dashing",true);

        string DashAddress;


        if (CC.Jumping)
        {
            DashAddress = "Prefab/Battle/Dash/" + CC.Chara.ToString() + "DashInAirParticle";

            //アニメーションをセット
            CC.CharaAni.SetTrigger("DashAirTrigger");

            CC.RB.constraints = RigidbodyConstraints2D.FreezePositionY;

            CC.RB.drag = 1.8f;

            CC.ChakuchiCollider.SetActive(false);
        }
        else
        {
            //残像パーティクルを作成
            DashAddress = "Prefab/Battle/Dash/" + CC.Chara.ToString() + "DashParticle";

            //アニメーションをセット
            CC.CharaAni.SetTrigger("DashTrigger");
        }

        GameObject DashPrefab = Resources.Load(DashAddress) as GameObject;

        GameObject Particle=Instantiate(DashPrefab);

        //ジャンプしてたら位置を微調整
        if (!CC.Jumping)
        {
            Particle.transform.position = new Vector3(CC.transform.position.x, CC.transform.position.y - 0.615f);

        }
        else
        {
            Particle.transform.position = new Vector3(CC.transform.position.x, CC.transform.position.y);
        }

        Particle.GetComponent<DashEffectFollow>().Target = CC.transform;

        //右向きの場合
        if (CC.TF.localScale.x > 0)
        {
            CC.RB.velocity = new Vector2(0,0);

            if (CC.Attacking)
            {
                CC.RB.AddForce(new Vector2(CC.DashSpeed, 0), ForceMode2D.Impulse);

                CC.DashAttackCansel();
            }
            else
            {
                CC.RB.AddForce(new Vector2(CC.DashSpeed, 0), ForceMode2D.Impulse);

            }
        }
        //左向きの場合
        else if (CC.TF.localScale.x < 0)
        {
            CC.RB.velocity = new Vector2(0, 0);

            if (CC.Attacking)
            {
                CC.RB.AddForce(new Vector2(-CC.DashSpeed, 0), ForceMode2D.Impulse);

                CC.DashAttackCansel();
            }
            else
            {
                CC.RB.AddForce(new Vector2(-CC.DashSpeed, 0), ForceMode2D.Impulse);
            }

            Particle.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
            Particle.GetComponent<ParticleSystemRenderer>().flip = new Vector3(1, 0, 0);
        }

        CC.Dashing = true;
        CC.CanDamage = false;


        /*
        
        if (CC.Attacking)
        {
            //右向き
            if (CC.TF.localScale.x > 0)
            {
                CC.RB.AddForce(new Vector2(CC.DashSpeed + CC.NormalSpeed - 5, 0), ForceMode2D.Impulse);

                CC.DashAttackCansel();

            }
            //左向き
            else
            {

                CC.RB.AddForce(new Vector2(-CC.DashSpeed - CC.NormalSpeed + 5, 0), ForceMode2D.Impulse);

                Particle.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                Particle.GetComponent<ParticleSystemRenderer>().flip = new Vector3(1, 0, 0);

                CC.DashAttackCansel();

            }
            
            CC.Dashing = true;
            CC.CanDamage = false;

            return;
        }

        //右向きの場合
        if (Horizontal > 0)
        {
            CC.RB.AddForce(new Vector2(CC.DashSpeed, 0), ForceMode2D.Impulse);

        }
        //左向きの場合
        else if (Horizontal < 0)
        {
            CC.RB.AddForce(new Vector2(-CC.DashSpeed, 0), ForceMode2D.Impulse);

            Particle.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
            Particle.GetComponent<ParticleSystemRenderer>().flip = new Vector3(1, 0, 0);
        }
        //どちらにも動いていない場合はキャラの向いている向きで判断
        else if (Horizontal==0)
        {
            //右向き
            if (CC.TF.localScale.x>0)
            {
                CC.RB.AddForce(new Vector2(CC.DashSpeed+CC.NormalSpeed-5, 0), ForceMode2D.Impulse);
            }
            //左向き
            else
            {

                CC.RB.AddForce(new Vector2(-CC.DashSpeed -CC.NormalSpeed+5, 0), ForceMode2D.Impulse);

                Particle.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                Particle.GetComponent<ParticleSystemRenderer>().flip = new Vector3(1, 0, 0);
            }
        }

        CC.Dashing = true;
        CC.CanDamage = false;

    */
    }

    //継承
    public override float GetKeyAxis(XboxControllerAxes Axes = XboxControllerAxes.None, KeybordSets Set = KeybordSets.None)
    {
        return base.GetKeyAxis(Axes, Set);
    }
    public override bool GetKeyDown(KeyCode Key = KeyCode.None, XboxControllerButtons Button = XboxControllerButtons.None, KeybordSets Set = KeybordSets.None)
    {
        return base.GetKeyDown(Key, Button, Set);
    }
}
