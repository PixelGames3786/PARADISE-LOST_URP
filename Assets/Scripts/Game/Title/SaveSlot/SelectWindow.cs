using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SelectWindow : InputComponent
{
    private AudioGeneral Audio;

    public SaveSlotSelect SelectParent;

    private RectTransform SelectImage;

    public int NowSelect;

    private bool Selecting,VerticalStop;

    private float Vertical;

    public float StopTime;

    // Start is called before the first frame update
    void Start()
    {
        Audio = GetComponent<AudioGeneral>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!Selecting)
        {
            return;
        }

        //上
        if (Vertical > 0&&NowSelect!=0&&!VerticalStop)
        {
            Audio.PlayClips(3);

            StartCoroutine("VerticalStopChange");

            Vertical = 0;

            NowSelect--;

            SelectImage.localPosition = new Vector2(-100,30);
            SelectImage.sizeDelta = new Vector2(0,50);

            SelectImage.DOSizeDelta(new Vector2(200, 50), 0.2f);
        }

        //下
        if (Vertical < 0 && NowSelect != 1 && !VerticalStop)
        {
            Audio.PlayClips(3);

            StartCoroutine("VerticalStopChange");

            Vertical = 0;

            NowSelect++;

            SelectImage.localPosition = new Vector2(-100, -30);
            SelectImage.sizeDelta = new Vector2(0, 50);

            SelectImage.DOSizeDelta(new Vector2(200, 50), 0.2f);
        }

        //決定
        if (GetKeyDown(Set: KeybordSets.Decision) || GetKeyDown(Button: XboxControllerButtons.B))
        {
            Audio.PlayClips(2);

            BGMController.Controller.BGMFadeOut();

            switch (NowSelect)
            {
                //ロードゲーム
                case 0:

                    {
                        enabled = false;

                        StartCoroutine(SelectParent.LoadGame());
                    }

                    break;

                //νゲーム
                case 1:

                    {
                        enabled = false;

                        SelectParent.GameDelete();
                        SelectParent.NewGame();
                    }

                    break;
            }
        }

        //キャンセル
        if (GetKeyDown(Set: KeybordSets.Cancel) || GetKeyDown(Button: XboxControllerButtons.A))
        {
            Audio.PlayClips(4);

            Selecting = false;

            transform.DOScaleX(0f, 0.3f).OnComplete(() =>
             {
                 NowSelect = 0;

                 SelectImage.localPosition = new Vector2(-100, 30);
                 SelectImage.sizeDelta = new Vector2(0, 50);

                 SelectParent.enabled = true;

                 enabled = false;
             });
        }

        //コントローラーの入力取得
        if (InputManager.IsController)
        {
            Vertical = (Input.GetAxis("Dpad Vertical") * -1) + Input.GetAxis("Leftstick Vertical");
        }
        else
        {
            Vertical = Input.GetAxis("Vertical");
        }
    }

    public void ReverseIn()
    {
        if (!SelectImage)
        {
            SelectImage = transform.GetChild(0).GetComponent<RectTransform>();
        }

        SelectImage.DOSizeDelta(new Vector2(200,50),0.2f).OnComplete(()=> 
        {
            Selecting = true;
        });
    }

    IEnumerator VerticalStopChange()
    {
        VerticalStop = true;

        yield return new WaitForSeconds(StopTime);

        VerticalStop = false;
    }

    public override bool GetKeyDown(KeyCode Key = KeyCode.None, XboxControllerButtons Button = XboxControllerButtons.None, KeybordSets Set = KeybordSets.None)
    {
        return base.GetKeyDown(Key, Button, Set);
    }
}
