using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class NovelItemWindow : MonoBehaviour
{
    private RectTransform Rect,ChildRect;

    public Vector2 SizeDelta;

    public void ScaleIn()
    {
        Rect = GetComponent<RectTransform>();
        ChildRect = transform.GetChild(0).GetComponent<RectTransform>();

        Rect.sizeDelta = SizeDelta;
        ChildRect.sizeDelta = new Vector2(SizeDelta.x-20,SizeDelta.y-20);

        Rect.DOScale(new Vector3(1,1,1),0.3f);
    }

    public void ScaleOut()
    {
        Rect.DOScale(new Vector3(0,0,0),0.3f).OnComplete(()=> 
        {
            Destroy(gameObject);
        });
    }
}
