using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventureCollider : MonoBehaviour
{
    public enum CharaType
    {
        Normal,
        Battle
    }

    public enum IventType
    {
        //接触したらイベント発生
        OnContact,
        //その上でボタンを押したらイベント発生
        OnButton,
    }

    public enum IventKaisu
    {
        //一度きり
        Once,
        //ずっと
        Repeat,
    }

    public CharaType Chara;
    public IventType Type;
    public IventKaisu Kaisu;

    public InputComponent.XboxControllerButtons ControllerButton;
    public KeyCode Key;
    public InputComponent.KeybordSets KeyboardSet;

    public int IventID;

    public GameObject SupportButton;

    public bool OnceFlag;

    public bool BattleingDont;

    // Start is called before the first frame update
    void Start()
    {
        
    }
}
