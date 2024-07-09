using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DashGaugeController : MonoBehaviour
{
    public HPBerController HPBerC;

    public AudioGeneral Audio;

    [System.NonSerialized]
    public BattleCharaController CharaCon;

    public GameObject GaugePrefab;

    public DashGauge[] Gauges;

    // Start is called before the first frame update
    void Start()
    {
        Audio = GetComponent<AudioGeneral>();

        CharaCon = GameObject.Find("Charactor").GetComponent<BattleCharaController>();

        int GaugeCount = SaveDataManager.DataManage.Data.DashGaugeMax;

        Gauges = new DashGauge[GaugeCount];

        for (int i=0;i<GaugeCount;i++)
        {
            GameObject Gauge = Instantiate(GaugePrefab,transform);

            Gauge.GetComponent<RectTransform>().localPosition = new Vector2(-910+(30*i),425);

            Gauge.GetComponent<Image>().DOFade(1f,0.5f);

            Gauges[i] = Gauge.GetComponent<DashGauge>();

            Gauges[i].Parent = this;
        }

        if (HPBerC.FirstCharge)
        {
            //Gauges[0].GaugeCharge();
        }

        if (CharaCon.CanDashCount>=1)
        {
            int Charged = CharaCon.CanDashCount - 1;

            for (int i = 0; i <= Charged; i++)
            {
                Gauges[i].ChargedFlag = true;
            }

            if (Charged<Gauges.Length-1)
            {
                Gauges[Charged + 1].GaugeCharge();
            }

        }
        else
        {
            Gauges[0].GaugeCharge();
        }
    }

    public void SetCharge(int Charged)
    {

        for (int i=0;i<Charged;i++)
        {
            Gauges[i].ChargeSkip();
        }

        Gauges[Charged + 1].GaugeCharge();
    }

    // Update is called once per frame
    public void NextCharge()
    {
        Audio.PlayClips(0);

        if (CharaCon)
        {
        }

        if (SaveDataManager.DataManage.Data.DashGaugeMax>CharaCon.CanDashCount)
        {
            Gauges[CharaCon.CanDashCount].GaugeCharge();
        }

    }

    public void DecreaseGauge()
    {
        int NowNum = CharaCon.CanDashCount-1;

        if (NowNum>0)
        {
            if (Gauges[NowNum-1].ChargeImage.localPosition.x==0f)
            {
                Gauges[NowNum].Decrease();

                if (NowNum<2)
                {
                    Gauges[NowNum + 1].Decrease();
                    Gauges[NowNum].GaugeCharge();
                }
                else
                {
                    Gauges[NowNum].GaugeCharge();

                }

            }

        }
        else
        {
            Gauges[NowNum].Decrease();
            Gauges[NowNum].GaugeCharge();

            Gauges[NowNum + 1].Decrease();

        }
    }

    public void DashGaugeFadeOut()
    {
        for (int i=0;i<Gauges.Length;i++)
        {
            Gauges[i].FadeOut();
        }
    }

    public void DashGaugePause(bool UnDoorDo)
    {
        for (int i = 0; i < Gauges.Length; i++)
        {
            Gauges[i].PauseUnPause(UnDoorDo);
        }
    }

    public void DashGaugeFadeIn()
    {
        for (int i=0;i<Gauges.Length;i++)
        {
            Gauges[i].FadeIn();
        }
    }

    /*

    public void SetEffect()
    {
        int NowGauge = CharaCon.CanDashCount;

        print(NowGauge);

        if (NowGauge==0)
        {
            Gauges[NowGauge].Effect.LoopStart(0f,new Vector3(1f,1f,1f));
        }
        else
        {
            float alpha = Gauges[NowGauge - 1].Effect.material.GetFloat("_Alpha");
            Vector3 size = Gauges[NowGauge - 1].Effect.rect.localScale;

            Gauges[NowGauge].Effect.LoopStart(alpha,size);

        }
    }

    */
}
