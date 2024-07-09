using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName ="PersonalColors",menuName = "CreateDataBase/PersonalColor")]
public class PersonalColorData : ScriptableObject
{
    public List<string> CharaName;

    public List<Color> PersonalColor;

    public List<string> GetName()
    {
        return CharaName;
    }

    public List<Color> GetColor()
    {
        return PersonalColor;
    }

    public Color GetPersonalColor(string Name)
    {
        int Num=CharaName.IndexOf(Name);

        return PersonalColor[Num];
    }
}
