using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TargetController : MonoBehaviour
{
    private AdventureController AC;

    private Text TargetText;

    private RectTransform ThisRect,WindowRect,EffectRect;

    public string NowTarget;

    // Start is called before the first frame update
    void Start()
    {
        AC = FindObjectOfType<AdventureController>();

        TargetText = GetComponentInChildren<Text>();

        ThisRect = GetComponent<RectTransform>();
        WindowRect = transform.Find("TargetWindow").GetComponent<RectTransform>();
        EffectRect = transform.Find("TargetEffectParent").GetComponent<RectTransform>();

        TargetSet();
    }

    public void TargetSet()
    {
        NowTarget = AC.NowScene.Targets[SaveDataManager.DataManage.Data.TargetNumber];

        TargetText.text = NowTarget;

        WindowRect.sizeDelta = new Vector2((NowTarget.Length*50)+200,115);
        EffectRect.localPosition = new Vector2(870-(NowTarget.Length*50),480);

        ThisRect.localPosition = new Vector2(WindowRect.sizeDelta.x,0);

        ThisRect.DOLocalMoveX(0f,1f);
    }

    public void TargetHide()
    {
        ThisRect.DOLocalMoveX(WindowRect.sizeDelta.x,1f);
    }

    public void TargetRise()
    {
        ThisRect.DOLocalMoveX(0,1f);
    }
}
