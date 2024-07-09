using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventureIvent
{
    public string ID;

    public List<string> Lines = new List<string>();

    public Dictionary<string, int> IventScene = new Dictionary<string, int>();

    public int Index;

    public AdventureIvent(string id="")
    {
        ID = id;
    }
}
