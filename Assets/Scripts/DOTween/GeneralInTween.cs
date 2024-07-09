using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GeneralInTween : MonoBehaviour
{
    public enum TweenType
    {
        ImageIn,
        SpriteIn,
        TextIn,
    }

    public TweenType Type;
    public float TweenTime;
    public float TweenFinal;

    private Sequence Sequence;
    private Tween InTween;

    // Start is called before the first frame update
    void Start()
    {

        switch (Type)
        {
            case TweenType.ImageIn:

                {
                    var Target = GetComponent<Image>();

                    InTween = Target.DOFade(TweenFinal,TweenTime);
                }

                break;

            case TweenType.TextIn:

                {
                    var Target = GetComponent<Text>();

                    InTween = Target.DOFade(TweenFinal,TweenTime);
                }

                break;
        }

        Sequence.Append(InTween).OnComplete(() =>
        {
            Destroy(this);
        });
    }

}
