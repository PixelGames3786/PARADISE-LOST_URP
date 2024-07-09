using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DashGauge : MonoBehaviour
{
    public DashGaugeController Parent;

    public RectTransform ChargeImage;

    private GameObject EmissionObject;

    public bool ChargedFlag;

    // Start is called before the first frame update
    void Start()
    {
        //Effect = transform.GetChild(2).GetComponent<DashGaugeEffect>();

        EmissionObject = transform.GetChild(1).gameObject;
        ChargeImage = transform.GetChild(0).GetComponent<RectTransform>();

        if (ChargedFlag) ChargeSkip();

    }

    public void FadeOut()
    {
        ChargeImage.DOPause();

        ChargeImage.GetComponent<Image>().DOFade(0f, 1.5f);
        gameObject.GetComponent<Image>().DOFade(0f, 1.5f);

        EmissionObject.GetComponent<Image>().material.DOFloat(0f, "_Alpha", 1.5f);
    }

    public void PauseUnPause(bool UnDoorDo)
    {
        if (UnDoorDo)
        {
            ChargeImage.DOPause();
        }
        else
        {
            ChargeImage.DOPlay();
        }

    }

    public void FadeIn()
    {
        ChargeImage.GetComponent<Image>().DOFade(1f, 1.5f);
        gameObject.GetComponent<Image>().DOFade(1f, 1.5f);

        EmissionObject.GetComponent<Image>().material.DOFloat(1f, "_Alpha", 1.5f).OnComplete(() =>
        {
            ChargeImage.DOPlay();
        });
    }

    public void GaugeCharge()
    {
        if (!ChargeImage)
        {
            ChargeImage = transform.GetChild(0).GetComponent<RectTransform>();
        }

        ChargeImage.transform.localPosition = new Vector3(-40f, 0f);

        ChargeImage.DOLocalMoveX(0f, 2.5f).SetEase(Ease.Linear).OnComplete(() =>
         {
             ChargeEnd();
         });
    }

    public void ChargeSkip()
    {
        ChargeImage.transform.localPosition = new Vector3(0f, 0f);

        EmissionObject.SetActive(true);
    }

    private void ChargeEnd()
    {
        EmissionObject.SetActive(true);

        //キャラクターコントローラーのダッシュできる回数を増やす
        Parent.CharaCon.CanDashCount++;

        Parent.NextCharge();
    }

    public void Decrease()
    {
        ChargeImage.DOKill();

        EmissionObject.SetActive(false);

        ChargeImage.transform.localPosition = new Vector3(-40f, 0f);
    }
}