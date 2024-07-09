using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestory : MonoBehaviour
{
    public void EffectReset()
    {
        transform.GetChild(0).GetComponent<TrailRenderer>().time = 0.2f;
        transform.GetChild(1).GetComponent<TrailRenderer>().time = 0.2f;
        transform.GetChild(2).GetComponent<TrailRenderer>().time = 0.2f;
    }

    public void SetFalseMe()
    {
        gameObject.SetActive(false);
    }

    public void DestoryMe()
    {
        Destroy(gameObject);
    }
}
