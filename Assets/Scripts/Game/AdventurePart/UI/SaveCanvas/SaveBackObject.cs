using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SaveBackObject : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        In();
    }

    public void In()
    {
        Tween InTween = transform.DOScale(new Vector3(1, 1), 0.2f);

        InTween.SetEase(Ease.InOutBack);

        InTween.Play();
    }

    public void Out()
    {
        Tween OutTween = transform.DOScale(new Vector3(0, 0), 0.2f);

        OutTween.SetEase(Ease.InOutBack);

        OutTween.OnComplete(() => { Destroy(gameObject); });

        OutTween.Play();
    }
}
