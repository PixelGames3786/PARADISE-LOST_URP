using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WindowArrowLoop : MonoBehaviour
{
    public enum ArrowType
    {
        Right,
        Left
    }

    public ArrowType Arrow;

    private RectTransform Rect;

    // Start is called before the first frame update
    void Start()
    {
        Rect = GetComponent<RectTransform>();

        Sequence sequence = DOTween.Sequence();
        Tween InTween, OutTween;

        if (Arrow==ArrowType.Left)
        {
            InTween = Rect.DOLocalMoveX(-750,0.5f);
            OutTween = Rect.DOLocalMoveX(-740,0.5f);

            sequence.Append(InTween).Append(OutTween).SetLoops(-1,LoopType.Restart);
        }
        else if (Arrow==ArrowType.Right)
        {
            InTween = Rect.DOLocalMoveX(750, 0.5f);
            OutTween = Rect.DOLocalMoveX(740, 0.5f);

            sequence.Append(InTween).Append(OutTween).SetLoops(-1, LoopType.Restart);
        }

        sequence.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
