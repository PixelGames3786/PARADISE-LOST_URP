using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class SaveData
{
    public enum SceneType
    {
        Novel,
        Adventure,
    }

    public enum CharaType
    {
        Normal,
        Battle
    }

    public SceneType Scene;
    public CharaType Chara;
    public BattleCharaController.SousaChara Sousa;

    public int NovelScene;
    public int NovelLine;

    public string AdventureScene;

    public int TargetNumber;

    public float PositionX, PositionY;

    public int DashGaugeMax;
    public int Bit;
    public bool CanDash,CanBit;

    //セーブした場所
    public string SavePlace;
    //ゲームの経過時間
    public int GameHour, GameMinute;

    [SerializeField]
    public Dictionary<string, bool> GameFlags;

    [SerializeField]
    public Dictionary<string, CharactorData> CharactorDatas;
}
