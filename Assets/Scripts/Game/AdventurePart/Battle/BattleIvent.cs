using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleIvent
{
    //ivent=のID
    public string ID;

    //そのイベントの中身
    public List<string> Lines = new List<string>();

    //そのイベントの中でさらに小分けされたイベントシーン置き場
    public Dictionary<string, int> BattleIventScenes = new Dictionary<string, int>();

    //戦闘中に出てくるメッセージ
    public List<string> BattleMessages = new List<string>();

    //現在どこまで進んでいるのか
    public int Index;

    
    public BattleIvent(string id="")
    {
        ID = id;
    }
}
