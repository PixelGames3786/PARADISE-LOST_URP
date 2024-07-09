using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BitCountController : MonoBehaviour
{
    private int BitNum;

    private RectTransform TextRect01, TextRect02;
    public Text Text01, Text02;

    public Material UnderMate;

    private bool FadeWaiting;

    public float FadeWaitTime, WaitTime;

    // Start is called before the first frame update
    void Start()
    {
        BitNum = SaveDataManager.DataManage.Data.Bit;

        TextRect01 = Text01.GetComponent<RectTransform>();
        TextRect02 = Text02.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (FadeWaiting)
        {
            WaitTime += Time.deltaTime;

            if (WaitTime>=FadeWaitTime)
            {
                WaitTime = 0;

                Text01.DOFade(0f,3f);
                Text02.DOFade(0f,3f);

                UnderMate.DOFloat(0f,"_Alpha",3f);

                FadeWaiting = false;
            }
        }

        //もしBitを獲得したら
        if (SaveDataManager.DataManage.Data.Bit>BitNum)
        {
            BitKousin();

            BitNum = SaveDataManager.DataManage.Data.Bit;
        }
    }

    private void BitKousin()
    {
        TextRect01.localScale = new Vector3(0.8f,0.8f,1f);
        TextRect02.localScale = new Vector3(0.8f,0.8f,1f);

        TextRect01.DOScale(new Vector3(1f,1f,1f),0.2f);
        TextRect02.DOScale(new Vector3(1f,1f,1f),0.2f);

        Text01.color = new Color(0,0,1,1);
        Text02.color = new Color(0,0.9f,1,1);

        Text01.text = BitNum.ToString();
        Text02.text = BitNum.ToString();

        UnderMate.SetFloat("_Alpha",1f);

        FadeWaiting = true;
    }
}
