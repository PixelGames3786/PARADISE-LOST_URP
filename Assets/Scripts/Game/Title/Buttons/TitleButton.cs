using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;
using System.IO;

public class TitleButton : InputComponent
{
    public GameObject SlotPrefab;
    private GameObject SlotCanvas;

    public OptionButton OptionButton;
    
    private AudioGeneral Audio;

    public Transform OptionCanvasParent;
    private Transform StartParent, ContinueParent,OptionParent;

    private TitleLineTween[] Lines=new TitleLineTween[3];
    public float[] LineWidth = new float[3];

    public Volume volume;

    private int NowLine;
    private float Vertical,CAValue,GrainValue;

    private bool VolumeChange,VerticalStop;

    public float StopTime;

    void Awake()
    {
        ContinueParent = transform.GetChild(0);
        StartParent = transform.GetChild(1);
        OptionParent = transform.GetChild(2);

        Lines[0] = ContinueParent.GetChild(1).GetComponent<TitleLineTween>();
        Lines[1] = StartParent.GetChild(1).GetComponent<TitleLineTween>();
        Lines[2] = OptionParent.GetChild(1).GetComponent<TitleLineTween>();

    }

    // Start is called before the first frame update
    void Start()
    {
        Audio = GetComponent<AudioGeneral>();

        //セーブデータがなかったらコンティニューボタンを消してスタートの位置を移動
        if (!File.Exists(Application.persistentDataPath + "/SaveData/SaveData"))
        {
            Destroy(ContinueParent.gameObject);

            NowLine = 1;
        }
        else
        {
            NowLine = 0;
        }

        //Start下ラインをInさせる
        Lines[NowLine].LineTweenIn(LineWidth[NowLine],20,0.5f);

    }

    // Update is called once per frame
    void Update()
    {

        //上
        if (Vertical>0&&!VerticalStop)
        {
            StartCoroutine("VerticalStopChange");

            Vertical = 0f;

            NowLine--;

            if (NowLine<3-transform.childCount)
            {
                NowLine = 3-transform.childCount;
            }
            else
            {
                Lines[NowLine].LineTweenIn(LineWidth[NowLine], 20, 0.25f);
                Lines[NowLine + 1].LineTweenOut(20,0.25f);

                Audio.PlayClips(1);
            }
        }

        //下
        if (Vertical<0&&!VerticalStop)
        {
            StartCoroutine("VerticalStopChange");

            Vertical = 0f;

            NowLine++;

            if (NowLine > 2)
            {
                NowLine = 2;
            }
            else
            {
                Lines[NowLine].LineTweenIn(LineWidth[NowLine], 20, 0.25f);
                Lines[NowLine + -1].LineTweenOut(20,0.25f);

                Audio.PlayClips(1);

            }
        }

        //決定
        if (GetKeyDown(Set:KeybordSets.Decision)||GetKeyDown(Button:XboxControllerButtons.B))
        {
            Audio.PlayClips(0);


            //今選択している場所によって処理変更
            switch (NowLine)
            {
                //Continue
                case 0:

                    {
                        BGMController.Controller.BGMFadeOut();

                        SceneController.Controller.BackColor = new Color32(69, 91, 255, 255);
                        SceneController.Controller.ContinueFlag = true;

                        //NovelかAdventureかで変える
                        switch (SaveDataManager.DataManage.Data.Scene)
                        {
                            case SaveData.SceneType.Novel:

                                {
                                    SceneController.Controller.Transition("Novel",SceneFader.FadeType.Loading);
                                }

                                break;

                            case SaveData.SceneType.Adventure:

                                {
                                    string name = SaveDataManager.DataManage.Data.AdventureScene;
                                    
                                    SceneController.Controller.MapTrans = true;

                                    if (SaveDataManager.DataManage.Data.Chara==SaveData.CharaType.Normal)
                                    {
                                        SceneController.Controller.MapTransition(name, SceneController.CharaStatus.Normal,SceneFader.FadeType.Loading,-1);

                                    }
                                    else
                                    {
                                        SceneController.Controller.MapTransition(name, SceneController.CharaStatus.Battle, SceneFader.FadeType.Loading, -1);

                                    }

                                }

                                break;
                        }

                        enabled = false;
                    }

                    break;

                //Start
                case 1:

                    {

                        if (!SlotCanvas)
                        {
                            SlotCanvas=Instantiate(SlotPrefab);

                            SlotCanvas.GetComponent<SaveSlotSelect>().titleButton = this;

                        }
                        else
                        {
                            SlotCanvas.GetComponent<SaveSlotSelect>().enabled = true;

                            StartCoroutine(SlotCanvas.GetComponent<SaveSlotSelect>().SlotsIn());
                        }


                        enabled = false;

                        /*

                        SaveDataManager.DataManage.SaveDataInitialize();

                        SceneController.Controller.BackColor = new Color32(69,91,255,255);
                        SceneController.Controller.Transition("Novel",SceneFader.FadeType.Loading);

                        SaveDataManager.DataManage.Save();

                        enabled = false;

    */
                    }

                    break;

                //Option
                case 2:

                    {
                        //OptionCanvasがtrueになってたら重複を防ぐため処理をやめる
                        if (OptionCanvasParent.GetChild(0).gameObject.activeInHierarchy)
                        {
                            return;
                        }

                        //OptionCanvasを入れる
                        OptionCanvasParent.GetChild(0).gameObject.SetActive(true);

                        Tween OptionIn01 = OptionCanvasParent.DOScale(new Vector3(1.1f,0.1f,1f),0.3f);
                        Tween OptionIn02 = OptionCanvasParent.DOScale(new Vector3(1f, 1f,1f), 0.5f);

                        Sequence Sequence = DOTween.Sequence();

                        Sequence.Append(OptionIn01).Append(OptionIn02).OnComplete(() =>
                        {
                            VolumeChange = false;

                            GrainValue = 0f;
                            CAValue = 0f;

                            OptionButton.enabled = true;

                            //TitleCanvasをオフにする
                            transform.parent.gameObject.SetActive(false);
                        });

                        VolumeChange = true;

                        this.enabled = false;
                    }

                    break;
            }
        }

        //HDRPVolumeの変更
        if (VolumeChange)
        {
            GrainValue += Time.deltaTime;
            CAValue += Time.deltaTime;

            GrainValue=Mathf.Clamp(GrainValue,0f,0.8f);
            CAValue=Mathf.Clamp(CAValue, 0f, 0.3f);


            if (volume.profile.TryGet(out ChromaticAberration Chroma))
            {
                ChromaticAberration CA = Chroma;

                CA.intensity.SetValue(new ClampedFloatParameter(CAValue, 0f, 1f, true));
            }

            if (volume.profile.TryGet(out FilmGrain Grain))
            {
                FilmGrain Gra = Grain;

                Grain.intensity.SetValue(new ClampedFloatParameter(GrainValue, 0f, 1f, true));
            }
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

    public override bool GetKeyDown(KeyCode Key=KeyCode.None, XboxControllerButtons Button=XboxControllerButtons.None, KeybordSets Set=KeybordSets.None)
    {
        return base.GetKeyDown(Key,Button,Set);
    }

    IEnumerator VerticalStopChange()
    {
        VerticalStop = true;

        yield return new WaitForSeconds(StopTime);

        VerticalStop = false;
    }


}
