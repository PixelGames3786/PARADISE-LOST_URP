using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager inputmanager
    {
        get { return _inputmanage ?? (_inputmanage = FindObjectOfType<InputManager>()); }
    }

    static InputManager _inputmanage;

    public static bool IsController;

    void Awake()
    {
        // 自身がインスタンスでなければ自滅
        if (this != inputmanager)
        {
            Destroy(gameObject);
            return;
        }

        IsController =ControllerCheck();

        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        IsController=ControllerCheck();
    }

    public static bool ControllerCheck()
    {
        var ControllerName = Input.GetJoystickNames();

        int Count = new int();

        bool Return=new bool();

        //抜き差しすると空白のコントローラーが生まれるから正しくカウントする
        for (int i = 0; i < ControllerName.Length; i++)
        {
            if (ControllerName[i] != "")
            {
                Count++;
            }
        }

        if (Count > 0)
        {
            Return = true;
        }
        else
        {
            Return = false;
        }

        return Return;
    }

    void OnDestroy()
    {

        // ※破棄時に、登録した実体の解除を行なっている

        // 自身がインスタンスなら登録を解除
        if (this == inputmanager) _inputmanage = null;

    }
}
