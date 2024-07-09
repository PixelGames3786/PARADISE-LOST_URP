using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DashGaugeEffect : MonoBehaviour
{
    [System.NonSerialized]
    public RectTransform rect;

    [System.NonSerialized]
    public Material material;

    public Texture2D MotoSprite;

    public Shader MotoShader;

    [Space(10)]

    public float Intensity;

    public Vector3 TagSize;

    public float TagTime;

    public void LoopStart(float Alpha,Vector3 Size)
    {

        rect = GetComponent<RectTransform>();
        rect.localScale = Size;



        material = new Material(MotoShader);

        material.SetTexture("_MainTex", MotoSprite);

        float Factor = Mathf.Pow(2, Intensity);
        material.SetColor("_Color", new Color(1f * Factor, 1f * Factor, 1f * Factor));

        material.SetFloat("_Alpha", Alpha);

        GetComponent<Image>().material = material;



        Sequence sequence = DOTween.Sequence();

        Tween FadeIn = material.DOFloat(1f, "_Alpha", TagTime);
        Tween Tolarge = rect.DOScale(TagSize, TagTime);

        sequence.Append(FadeIn).Join(Tolarge).OnComplete(() =>
        {
            Loop();
        });
    }

    private void Loop()
    {
        material.SetFloat("_Alpha",0f);
        rect.localScale = new Vector3(1f,1f,1f);

        Sequence sequence = DOTween.Sequence();

        Tween FadeIn = material.DOFloat(1f, "_Alpha", TagTime);
        Tween Tolarge = rect.DOScale(TagSize,TagTime);

        sequence.Append(FadeIn).Join(Tolarge).OnComplete(() => 
        {
            Loop();
        });
    }

    
}
