using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FilterController : MonoBehaviour
{
    public static FilterController Controller
    {
        get { return _controller ?? (_controller = FindObjectOfType<FilterController>()); }
    }

    static FilterController _controller = null;

    public Material Original;

    public List<Material> Materials;

    private RawImage Image;

    private string FilterType;

    // Start is called before the first frame update
    void Start()
    {
        Image = GetComponent<RawImage>();
    }

    public void ChangeFilter(string type)
    {

        FilterType = type;

        FilterReset();

        switch (FilterType)
        {
            case "Normal":

                Image.material = Original;

                break;

            case "Noise":

                Image.material = Materials[0];

                break;

            case "GameOver":

                Image.material = Materials[1];

                Sequence sequence = DOTween.Sequence();

                Tween GameOverTween1 = Materials[1].DOFloat(1f,"_Alpha",1f);
                Tween GameOverTween2 = Materials[1].DOFloat(-0.1f,"_StepIn",1f);

                sequence.Append(GameOverTween1).AppendInterval(1f).Append(GameOverTween2);

                sequence.Play();

                break;
        }
    }

    public void FilterReset()
    {
        switch (FilterType)
        {
            case "GameOver":

                Materials[1].SetFloat("_Alpha",0);
                Materials[1].SetFloat("_StepIn",1);

                break;
        }
    }
}
