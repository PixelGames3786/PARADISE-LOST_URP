using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class OptionTumamiTween : MonoBehaviour
{
    private RectTransform TumamiRect;

    // Start is called before the first frame update
    void Start()
    {
        TumamiRect = GetComponent<RectTransform>();
    }

    public void TumamiTween(Vector2 EndPosition)
    {
        TumamiRect.DOLocalMove(EndPosition,0.25f);
    }
}
