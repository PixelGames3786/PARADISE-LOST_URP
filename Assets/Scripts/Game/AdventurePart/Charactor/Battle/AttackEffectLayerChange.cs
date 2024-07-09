using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEffectLayerChange : MonoBehaviour
{

    public void LayerChange(string LayerName)
    {
        for (int i=0;i<transform.childCount;i++)
        {
            TrailRenderer Trail = transform.GetChild(i).GetComponent<TrailRenderer>();

            Trail.sortingLayerName = LayerName;
        }
    }
}
