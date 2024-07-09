using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalCharaInputer : InputComponent
{
    private NormalCharaController CC;

    // Start is called before the first frame update
    void Start()
    {
        CC = GetComponent<NormalCharaController>();
    }

    public void ButtonCheck()
    {
        var ControllerButton = CC.OnCollider.ControllerButton;
        var KeyboardSet = CC.OnCollider.KeyboardSet;
        var Key = CC.OnCollider.Key;

        //対応したイベントを探し出し実行
        AdventureIvent Ivent = CC.AC.NowScene.Ivents.Find((ivent) => ivent.ID == CC.OnCollider.IventID.ToString());

        //コントローラーにつながっていたら
        if (InputManager.IsController)
        {
            if (GetKeyDown(Button:ControllerButton))
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
            if (GetKeyDown(Set:KeyboardSet,Key:Key))
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

    public override bool GetKeyDown(KeyCode Key = KeyCode.None, XboxControllerButtons Button = XboxControllerButtons.None, KeybordSets Set = KeybordSets.None)
    {
        return base.GetKeyDown(Key, Button, Set);
    }
}
