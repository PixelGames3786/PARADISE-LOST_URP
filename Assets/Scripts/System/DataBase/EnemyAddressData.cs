using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "EnemyNames", menuName = "CreateDataBase/EnemyName")]
public class EnemyAddressData : ScriptableObject
{
    public List<string> EnemyAddress;

    public List<string> GetAddress()
    {
        return EnemyAddress;
    }
}
