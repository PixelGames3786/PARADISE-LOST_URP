using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleNovelIvent 
{
    public string ID;

    public List<string> Lines=new List<string>();

    public int Index;

    public BattleNovelIvent(string id="")
    {
        ID = id;
    }
}
