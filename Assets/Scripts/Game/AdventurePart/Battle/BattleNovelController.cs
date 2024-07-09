using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleNovelController : MonoBehaviour
{
    public Sprite[] Icons;

    private Image IconImage;

    private Text Text;

    private string HyoujiText;
    private string[] LineContent;

    private int NowLine,NowNum;
    private bool TextWaiting,LineWaiting,EndWaiting;

    public float TextTime;
    private float LineWaitTarget,LineKeikaTime,KeikaTime;

    public bool SinkouEnd;

    //[System.NonSerialized]
    public List<string> Lines = new List<string>();

    private string Line="";

    // Start is called before the first frame update
    void Start()
    {
        transform.GetChild(0).GetComponent<Image>().DOFade(1f,0.5f);

        IconImage = transform.GetChild(1).GetComponent<Image>();

        Text = transform.GetChild(2).GetComponent<Text>();

        LineSinkou();
    }

    // Update is called once per frame
    void Update()
    {

        if (LineWaiting)
        {
            LineKeikaTime += Time.deltaTime;

            if (LineKeikaTime >=LineWaitTarget)
            {
                LineWaiting = false;

                LineSinkou();
            }
        }

        if (TextWaiting)
        {
            KeikaTime += Time.deltaTime;

            if (KeikaTime>=TextTime)
            {
                TextSinkou();
            }
        }
    }

    public void TextSinkou()
    {
        if (NowNum>=HyoujiText.Length)
        {
            NowNum = 0;
            KeikaTime = 0;
            LineKeikaTime = 0;

            LineWaiting = true;
            TextWaiting = false;

            return;
        }

        KeikaTime = 0;

        Text.text += HyoujiText[NowNum];

        NowNum++;
    }

    public void LineSinkou()
    {

        Text.text = "";

        Line = Lines[NowLine];

        if (Line.Contains("#"))
        {
            IventRead();

            return;
        }
        else
        {
            Line = Line.Replace("{","");
            Line = Line.Replace("}","");
        }

        LineContent = Line.Split(',');

        HyoujiText = LineContent[0];

        //表示するSprite探し
        Sprite HyoujiSprite=null;

        for (int i=0; i<Icons.Length;i++)
        {
            string name = Icons[i].name;

            name = name.Replace("BNIcon_","");

            if (name==LineContent[1])
            {
                HyoujiSprite = Icons[i];

                IconImage.sprite = HyoujiSprite;
                IconImage.color = new Color(1, 1, 1, 0);
                IconImage.DOFade(0.2f, 0.5f);

                break;
            }
        }

        LineWaitTarget = float.Parse(LineContent[2]);

        TextWaiting = true;

        NowLine++;
    }

    public void IventRead()
    {

        if (Line.Contains("#BattleNovelEnd"))
        {
            if (NowLine+1<Lines.Count&&Lines[NowLine+1]=="#BattleNovelResume")
            {
                NowLine += 2;

                LineSinkou();

                return;
            }

            End();

            return;
        }
        else if (Line.Contains("#BattleNovelResume"))
        {
            Resume();
        }

        NowLine++;

        LineSinkou();
    }

    public void End()
    {
        transform.GetChild(0).GetComponent<Image>().DOFade(0f, 0.5f);

        IconImage.DOFade(0f,0.5f);

        Text.DOFade(0f, 0.5f);

        SinkouEnd = true;

        NowLine++;
    }

    public void Resume()
    {
        print("Resume");

        transform.GetChild(0).GetComponent<Image>().DOKill();
        transform.GetChild(0).GetComponent<Image>().DOFade(1f, 0.5f);

        IconImage.DOKill();
        IconImage.DOFade(1f, 0.5f);

        Text.DOKill();
        Text.DOFade(1f, 0.5f);

        NowNum = 0;
        LineKeikaTime = 0;
        KeikaTime = 0;

        SinkouEnd = false;
    }

    public void ForcedEnd(int Type)
    {
        LineWaiting = false;
        TextWaiting = false;

        transform.GetChild(0).GetComponent<Image>().DOFade(0f, 0.5f);

        IconImage.DOFade(0f, 0.5f);

        Text.DOFade(0f, 0.5f).OnComplete(()=> 
        {
            Lines.Clear();

            Destroy(gameObject);
        });
    }
}
