using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "BattleSounds", menuName = "CreateDataBase/BattleSounds")]
public class BattleSoundsData : ScriptableObject
{

    public List<string> SoundsAddress;

    public List<AudioClip> Sounds;

    public List<string> GetAddress()
    {
        return SoundsAddress;
    }

    public List<AudioClip> GetSounds()
    {
        return Sounds;
    }
}
