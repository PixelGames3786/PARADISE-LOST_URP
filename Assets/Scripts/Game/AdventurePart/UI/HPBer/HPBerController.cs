using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HPBerController : MonoBehaviour
{
    [System.NonSerialized]
    public BattleController BC;

    public AudioGeneral Audio;

    public GameObject DashGaugeParent;
    public DashGaugeController DashC;

    [System.NonSerialized]
    public GameObject Back, Flont;

    public GameObject DamageEffePre,HealEffePre;

    public int MapChange;

    public bool FirstCharge;

    // Start is called before the first frame update
    void Start()
    {
        if (SaveDataManager.DataManage.Data.CanDash)
        {
            MakeDashGauge();
        }
    }

    public void Shake(float ShakeHP,float MaxHP)
    {
        float ShakeNum = ShakeHP / MaxHP * 10f;

        transform.DOShakePosition(0.5f,ShakeNum,20);
    }

    public void HPChange(float NowHP,float MaxHP,bool Damage,bool Heal)
    {
        //比率計算
        float Ratio = NowHP / MaxHP;

        Flont.GetComponent<RectTransform>().DOSizeDelta(new Vector2(Ratio*700,40),0.5f);

        //ダメージを受けた時ダメージエフェクトを出す
        if (Damage)
        {
            RectTransform Effect = Instantiate(DamageEffePre, transform).GetComponent<RectTransform>();

            Effect.localPosition = new Vector3(-940+ Flont.GetComponent<RectTransform>().sizeDelta.x,490);

            Effect.DOLocalMoveX(-940+Ratio*700,0.5f);
        }

        //回復エフェクトを出す
        if (Heal)
        {
            Instantiate(HealEffePre,transform);
        }
    }

    public void HPBerIn()
    {
        Audio.PlayClips(0);

        Back = transform.GetChild(0).gameObject;
        Flont = transform.GetChild(1).gameObject;

        Flont.GetComponent<Image>().material.SetFloat("_Alpha", 1f);

        if (MapChange==1)
        {

            Tween BackFade= Back.GetComponent<Image>().DOFade(1f, 0.5f);

            //比率計算
            string CharaName = GameObject.Find("Charactor").GetComponent<BattleCharaController>().Chara.ToString();
            CharactorData Data = SaveDataManager.DataManage.Data.CharactorDatas[CharaName];

            float MaxHP = Data.CharaHPMax;
            float HP = Data.CharaHP;

            float Target = (HP / MaxHP)*700;

            Tween FlontIn = Flont.GetComponent<RectTransform>().DOSizeDelta(new Vector2(Target, 40), 0.5f);

            Sequence InSequence = DOTween.Sequence();

            InSequence.Append(BackFade).Append(FlontIn);

            InSequence.Play();

        }
        else
        {

            Tween BackFade = Back.GetComponent<Image>().DOFade(1f, 0.5f);

            Tween FlontIn = Flont.GetComponent<RectTransform>().DOSizeDelta(new Vector2(700, 40), 0.5f);

            Sequence InSequence = DOTween.Sequence();

            InSequence.Append(BackFade).Append(FlontIn).OnComplete(() =>
            {
                BC.BattleStart();
            });

            InSequence.Play();
        }

    }

    public void HPBerFadeOut()
    {
        Back.GetComponent<Image>().DOFade(0f,1f);
        Flont.GetComponent<Image>().material.DOFloat(0f,"_Alpha",1f);
    }

    public void HPBerFadeIn()
    {
        Back.GetComponent<Image>().DOFade(1f, 1.5f);
        Flont.GetComponent<Image>().material.DOFloat(1f, "_Alpha", 1.5f);
    }


    public void MakeDashGauge()
    {

        DashC=Instantiate(DashGaugeParent, transform).GetComponent<DashGaugeController>();

        DashC.HPBerC = this;

        CharactorData Data = SaveDataManager.DataManage.Data.CharactorDatas[BC.BCC.Chara.ToString()];

        
    }
}
