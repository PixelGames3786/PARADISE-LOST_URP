using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCharaInputer : InputComponent
{
    private BattleCharaController CC;

    // Start is called before the first frame update
    void Start()
    {
        CC = GetComponent<BattleCharaController>();
    }

    public void AttackCheck()
    {
        //ジャンプ攻撃
        if (CC.Jumping)
        {

        }
        //地上攻撃
        else
        {
            //コントローラーにつながっていたら
            if (InputManager.IsController)
            {
                //弱攻撃
                if (GetKeyDown(Button: XboxControllerButtons.B))
                {
                    CC.CharaAni.SetTrigger("JakuTrigger");

                    CC.Attacking = true;
                }
                //強攻撃
                if (GetKeyDown(Button: XboxControllerButtons.Y))
                {
                    CC.CharaAni.SetTrigger("KyouTrigger");

                    CC.Attacking = true;
                }
            }
            //キーボード操作だったら
            else
            {
                //弱攻撃
                if (Input.GetKeyDown(KeyCode.J))
                {
                    CC.CharaAni.SetTrigger("JakuTrigger");

                    //CC.CanAttack = false;

                    CC.Attacking = true;
                }
                //強攻撃
                if (Input.GetKeyDown(KeyCode.K))
                {
                    CC.CharaAni.SetTrigger("KyouTrigger");

                    CC.Attacking = true;
                }
            }
        }
    }

    public void ButtonCheck()
    {
        var ControllerButton = CC.OnCollider.ControllerButton;
        var KeyboardSet = CC.OnCollider.KeyboardSet;

        //対応したイベントを探し出し実行
        AdventureIvent Ivent = CC.AC.NowScene.Ivents.Find((ivent) => ivent.ID == CC.OnCollider.IventID.ToString());

        //コントローラーにつながっていたら
        if (InputManager.IsController)
        {
            if (GetKeyDown(Button: ControllerButton))
            {
                CC.AC.IventStart(Ivent);

                if (CC.OnCollider.Kaisu == AdventureCollider.IventKaisu.Once)
                {
                    CC.OnCollider.OnceFlag = true;

                    CC.OnCollider.SupportButton.GetComponent<SupportButton>().FadeOut();
                }
            }
        }
        else
        {
            if (GetKeyDown(Set: KeyboardSet))
            {
                CC.AC.IventStart(Ivent);

                if (CC.OnCollider.Kaisu == AdventureCollider.IventKaisu.Once)
                {
                    CC.OnCollider.OnceFlag = true;

                    CC.OnCollider.SupportButton.GetComponent<SupportButton>().FadeOut();
                }

            }
        }
    }

    //継承
    public override bool GetKeyDown(KeyCode Key = KeyCode.None, XboxControllerButtons Button = XboxControllerButtons.None, KeybordSets Set = KeybordSets.None)
    {
        return base.GetKeyDown(Key, Button, Set);
    }

}
