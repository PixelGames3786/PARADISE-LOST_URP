using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SupportButton : MonoBehaviour
{
    public Sprite ButtonControllerSprite, EffectControllerSprite;
    public Sprite ButtonKeyboradSprite, EffectKeyboradSprite;

    public Material ButtonControllerMaterial, EffectControllerMaterial;
    public Material ButtonKeyboradMaterial, EffectKeyboradrMaterial;

    public SpriteRenderer ButtonRenderer,EffectRenderer;

    private ButtonEffectLoop EffectLoop;

    private Tween Tween;

    private bool FadeOuting,IsController;

    void Awake()
    {
        IsController = InputManager.ControllerCheck();

        ButtonRenderer = GetComponent<SpriteRenderer>();
        EffectRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();

        EffectLoop = GetComponentInChildren<ButtonEffectLoop>();

        IsController = false;

        if (!IsController)
        {
            ButtonRenderer.sprite = ButtonKeyboradSprite;
            ButtonRenderer.material = ButtonKeyboradMaterial;

            EffectRenderer.sprite = EffectKeyboradSprite;
            EffectRenderer.material = EffectKeyboradrMaterial;
        }

        gameObject.SetActive(false);
    }

    void LateUpdate()
    {
        if (FadeOuting&&ButtonRenderer.material.GetFloat("_Alpha")==0f)
        {
            FadeOuting = false;

            gameObject.SetActive(false);
        }

        SpriteMaterialChange();
    }

    public void FadeIn()
    {
        ButtonRenderer.material.SetFloat("_Alpha", 0f);

        if (FadeOuting)
        {
            FadeOuting = false;

            Tween.Kill();

            gameObject.SetActive(true);
        }

        SpriteMaterialChange();

        ButtonRenderer.material.DOFloat(1f,"_Alpha",0.5f);

        EffectLoop.StopCoroutine("AlphaChange");
        EffectLoop.StartCoroutine("AlphaChange");
    }

    public void FadeOut()
    {
        FadeOuting = true;

        Tween=ButtonRenderer.material.DOFloat(0f, "_Alpha", 0.5f).OnComplete(()=> 
        {
        });
    }

    public void SpriteMaterialChange()
    {
        if (InputManager.IsController != IsController)
        {
            IsController = InputManager.IsController;

            if (IsController)
            {
                ButtonRenderer.sprite = ButtonControllerSprite;
                ButtonRenderer.material = ButtonControllerMaterial;

                EffectRenderer.sprite = EffectControllerSprite;
                EffectRenderer.material = EffectControllerMaterial;

                EffectLoop.StopCoroutine("AlphaChange");
                EffectLoop.StartCoroutine("AlphaChange");

                ButtonRenderer.material.SetFloat("_Alpha", 1f);

            }
            else
            {
                ButtonRenderer.sprite = ButtonKeyboradSprite;
                ButtonRenderer.material = ButtonKeyboradMaterial;

                EffectRenderer.sprite = EffectKeyboradSprite;
                EffectRenderer.material = EffectKeyboradrMaterial;

                EffectLoop.StopCoroutine("AlphaChange");
                EffectLoop.StartCoroutine("AlphaChange");

                ButtonRenderer.material.SetFloat("_Alpha", 1f);

            }
        }
    }
}
