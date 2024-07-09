using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalCharaMover : InputComponent
{
    private NormalCharaController CC;

    public float XSpeed,Horizontal;

    public void Start()
    {
        CC = GetComponent<NormalCharaController>();
    }

    public void MoveCheck()
    {
        //押されていたら
        if (Horizontal > 0)
        {
            CC.TF.localScale = new Vector3(2, 2, 1);

            CC.CharaAnimator.SetBool("Walking", true);

            XSpeed = CC.Speed;
        }
        else if (Horizontal < 0)
        {
            CC.TF.localScale = new Vector3(-2, 2, 1);

            CC.CharaAnimator.SetBool("Walking", true);

            XSpeed = -CC.Speed;

        }
        else
        {
            CC.CharaAnimator.SetBool("Walking", false);

            XSpeed = 0;
        }

        CC.RB.velocity = new Vector2(XSpeed, CC.RB.velocity.y);

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

    //継承
    public override float GetKeyAxis(XboxControllerAxes Axes = XboxControllerAxes.None, KeybordSets Set = KeybordSets.None)
    {
        return base.GetKeyAxis(Axes, Set);
    }
}
