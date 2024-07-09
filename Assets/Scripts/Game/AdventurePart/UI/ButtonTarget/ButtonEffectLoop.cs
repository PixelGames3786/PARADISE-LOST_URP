using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ButtonEffectLoop : MonoBehaviour
{
    private RectTransform ThisRect;
    private Transform ThisTrans;

    private Material Material;

    public Vector3 TargetScale;

    void Awake()
    {
        if (GetComponent<RectTransform>())
        {
            ThisRect = GetComponent<RectTransform>();
            Material = GetComponent<Image>().material;
        }
        else
        {
            ThisTrans = GetComponent<Transform>();
            Material = GetComponent<SpriteRenderer>().material;
        }

    }

    // Start is called before the first frame update
    void Start()
    {

        if (ThisRect)
        {
            ThisRect.localScale = new Vector3(0, 0, 0);

        }
        else
        {
            ThisTrans.localScale = new Vector3(0,0,0);
        }

        Material.SetFloat("_Alpha", 1f);

        StartCoroutine("AlphaChange");
    }

    IEnumerator AlphaChange()
    {
        if (ThisRect)
        {
            ThisRect.DOScale(TargetScale, 1f);
            Material = GetComponent<Image>().material;

        }
        else
        {
            ThisTrans.DOScale(TargetScale,1f);
            Material = GetComponent<SpriteRenderer>().material;

        }

        Material.DOFloat(0f, "_Alpha", 1f);

        yield return new WaitForSeconds(1f);

        if (ThisRect)
        {
            ThisRect.localScale = new Vector3(0, 0, 0);
        }
        else
        {
            ThisTrans.localScale = new Vector3(0,0,0);
        }

        Material.SetFloat("_Alpha",1f);

        StartCoroutine("AlphaChange");
    }
}
