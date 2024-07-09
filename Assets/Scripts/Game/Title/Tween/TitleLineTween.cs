using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TitleLineTween : MonoBehaviour
{

    //縮んだり伸びたり
    public void LineTweenIn(float Width,float height,float Time)
    {
        RectTransform RectTran = GetComponent<RectTransform>();

        RectTran.DOSizeDelta(new Vector2(Width, height), Time).Play();
    }

    public void LineTweenOut(float height,float Time)
    {
        RectTransform RectTran = GetComponent<RectTransform>();

        RectTran.DOSizeDelta(new Vector2(0, height), Time).Play();
    }
}
