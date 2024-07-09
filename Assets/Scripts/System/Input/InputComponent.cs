using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputComponent : MonoBehaviour
{
    public enum XboxControllerButtons
    {
        None,
        A,
        B,
        X,
        Y,
        LeftStick,
        RightStick,
        Back,
        Menu,
        LeftBumper,
        RightBumper,
    }

    public enum XboxControllerAxes
    {
        None,
        LeftstickHorizontal,
        LeftstickVertical,
        DpadHorizontal,
        DpadVertical,
        RightstickHorizontal,
        RightstickVertical,
        LeftTrigger,
        RightTrigger,
    }

    public enum KeybordSets
    {
        None,
        Decision,
        Cancel,
        UpArrow,
        DownArrow,
        LeftArrow,
        RightArrow,
        Horizontal,
        Vertical
    }

    [System.NonSerialized]
    public KeyCode Key;

    [System.NonSerialized]
    public XboxControllerButtons ControllerButton;

    private float ReturnFloat;

    protected static readonly Dictionary<int, string> ButtonToName = new Dictionary<int, string>
        {
            {(int)XboxControllerButtons.A,"A" },
            {(int)XboxControllerButtons.B, "B"},
            {(int)XboxControllerButtons.X, "X"},
            {(int)XboxControllerButtons.Y, "Y"},
            {(int)XboxControllerButtons.LeftStick, "Leftstick"},
            {(int)XboxControllerButtons.RightStick, "Rightstick"},
            {(int)XboxControllerButtons.Back, "View"},
            {(int)XboxControllerButtons.Menu, "Menu"},
            {(int)XboxControllerButtons.LeftBumper, "Left Bumper"},
            {(int)XboxControllerButtons.RightBumper, "Right Bumper"},
        };

    protected static readonly Dictionary<int, string> AxesToName = new Dictionary<int, string>
    {
        { (int)XboxControllerAxes.LeftstickHorizontal,"Leftstick Horizontal"},
        { (int)XboxControllerAxes.LeftstickVertical,"Leftstick Vertical"},
        { (int)XboxControllerAxes.DpadHorizontal,"Dpad Horizontal"},
        { (int)XboxControllerAxes.DpadVertical,"Dpad Vertical"},
        { (int)XboxControllerAxes.RightstickHorizontal,"Rightstick Horizontal"},
        { (int)XboxControllerAxes.RightstickVertical,"Rightstick Vertical"},
        { (int)XboxControllerAxes.LeftTrigger,"Left Trigger"},
        { (int)XboxControllerAxes.RightTrigger,"Right Trigger"},
    };

    public virtual bool GetKeyDown(KeyCode Key, XboxControllerButtons ControllerButton, KeybordSets Set)
    {
        bool ReturnBool = false;

        if (Set != KeybordSets.None)
        {
            switch (Set)
            {
                case KeybordSets.Decision:

                    {
                        ReturnBool = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return);
                    }

                    break;

                case KeybordSets.Cancel:

                    {
                        ReturnBool = Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Backspace);
                    }

                    break;

                case KeybordSets.UpArrow:

                    {
                        ReturnBool = Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W);
                    }

                    break;

                case KeybordSets.DownArrow:

                    {
                        ReturnBool = Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S);
                    }

                    break;

                case KeybordSets.LeftArrow:

                    {
                        ReturnBool = Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A);
                    }

                    break;

                case KeybordSets.RightArrow:

                    {
                        ReturnBool = Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D);
                    }

                    break;
            }

            if (ReturnBool)
            {
                return ReturnBool;
            }
        }

        //キーボード
        if (Key != KeyCode.None)
        {
            ReturnBool = Input.GetKeyDown(Key);

            if (ReturnBool)
            {
                return ReturnBool;
            }
        }

        //コントローラー
        if (ControllerButton != XboxControllerButtons.None)
        {
            ReturnBool = Input.GetButtonDown(ButtonToName[(int)ControllerButton]);

            if (ReturnBool)
            {
                return ReturnBool;
            }
        }

        return ReturnBool;
    }

    public virtual bool GetKeyUp(KeyCode Key, XboxControllerButtons ControllerButton, KeybordSets Set)
    {
        bool ReturnBool = false;

        if (Set != KeybordSets.None)
        {
            switch (Set)
            {
                case KeybordSets.Decision:

                    {
                        ReturnBool = Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.Return);
                    }

                    break;

                case KeybordSets.Cancel:

                    {
                        ReturnBool = Input.GetKeyUp(KeyCode.X) || Input.GetKeyUp(KeyCode.Backspace);
                    }

                    break;

                case KeybordSets.UpArrow:

                    {
                        ReturnBool = Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W);
                    }

                    break;

                case KeybordSets.DownArrow:

                    {
                        ReturnBool = Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S);
                    }

                    break;

                case KeybordSets.LeftArrow:

                    {
                        ReturnBool = Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A);
                    }

                    break;

                case KeybordSets.RightArrow:

                    {
                        ReturnBool = Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D);
                    }

                    break;
            }
        }
        else
        {
            //キーボード
            if (Key != KeyCode.None)
            {
                ReturnBool = Input.GetKeyUp(Key);
            }

            //コントローラー
            if (ControllerButton != XboxControllerButtons.None)
            {
                ReturnBool = Input.GetButtonUp(ButtonToName[(int)ControllerButton]);
            }
        }

        return ReturnBool;
    }

    public virtual float GetKeyAxis(XboxControllerAxes Axes, KeybordSets Set)
    {
        switch (Set)
        {
            case KeybordSets.Horizontal:

                {
                    ReturnFloat = Input.GetAxis("Horizontal");
                }

                break;

            case KeybordSets.Vertical:

                {
                    ReturnFloat = Input.GetAxis("Vertical");
                }

                break;
        }

        if (Axes!=XboxControllerAxes.None)
        {
            ReturnFloat = Input.GetAxis(AxesToName[(int)Axes]);

        }

        return ReturnFloat;
    }
}
