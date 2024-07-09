using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleScene 
{
    //SchoolZone,FirstDangeonなどの場所の名前
    //BattleScene=のところ
    public string ID { get; set; }

    public List<BattleIvent> Ivents = new List<BattleIvent>();


    public BattleScene(string id="")
    {
        ID = id;
    }
}
