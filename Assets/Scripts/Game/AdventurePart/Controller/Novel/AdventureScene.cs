using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventureScene 
{
    public string ID { get; private set; }

    //Title=
    public string[] Name { get; set; }

    public List<string> Lines { get; private set; } = new List<string>();

    public List<string> Targets { get; set; } = new List<string>();

    public List<AdventureIvent> Ivents = new List<AdventureIvent>();



    public AdventureScene(string id = "")
    {
        ID = id;
    }
}
