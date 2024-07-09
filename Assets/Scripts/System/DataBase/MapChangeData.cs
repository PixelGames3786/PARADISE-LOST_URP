using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "MapChangeDatas", menuName = "CreateDataBase/MapChange")]
public class MapChangeData : ScriptableObject
{
    //X座標,Y座標
    public List<string> MapChangeDatas;

    public List<string> GetData()
    {
        return MapChangeDatas;
    }
}
