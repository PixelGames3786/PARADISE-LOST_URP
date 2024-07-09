using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.IO;

public class SaveSlotSelect : InputComponent
{
    public TitleButton titleButton;

    private AudioGeneral Audio;

    private Transform[] Slots = new Transform[4];
    private Material[] SlotMaterials = new Material[4];
    private Text[] PlaceTexts = new Text[4];
    private Text[] TimeTexts = new Text[4];

    private RectTransform SelectWindow;

    public Shader EmissionShader;
    public Shader NoiseShader;

    public int NowSelect;

    public float StopTime;
    private float Horizontal;

    private bool CanSelect,HorizontalStop;

    // Start is called before the first frame update
    void Start()
    {
        Audio = GetComponent<AudioGeneral>();

        GetComponent<Canvas>().worldCamera = Camera.main.transform.GetChild(0).GetComponent<Camera>();

        SelectWindow = transform.GetChild(2).GetComponent<RectTransform>();

        int i = 0;

        foreach (Transform Slot in transform.GetChild(1))
        {
            Slots[i] = Slot;
            PlaceTexts[i] = Slot.GetChild(0).GetComponent<Text>();
            TimeTexts[i] = Slot.GetChild(1).GetComponent<Text>();

            if (i>0)
            {
                Slot.GetComponent<Image>().color = new Color(1f,1f,1f,0.5f);
            }

            DataCheck(i);
            
            i++;
        }

        StartCoroutine("SlotsIn");
    }

    // Update is called once per frame
    void Update()
    {
        if (!CanSelect)
        {
            return;
        }

        //右
        if (Horizontal>0&&NowSelect!=3&&!HorizontalStop)
        {
            Audio.PlayClips(3);

            StartCoroutine("HorizontalStopChange");

            Horizontal = 0;

            NowSelect++;

            Slots[NowSelect].GetComponent<Image>().DOFade(1f,0.3f);
            Slots[NowSelect-1].GetComponent<Image>().DOFade(0.5f,0.3f);
        }

        //左
        if (Horizontal<0&&NowSelect!=0 && !HorizontalStop)
        {
            Audio.PlayClips(3);

            StartCoroutine("HorizontalStopChange");

            Horizontal = 0;

            NowSelect--;

            Slots[NowSelect].GetComponent<Image>().DOFade(1f, 0.3f);
            Slots[NowSelect +1].GetComponent<Image>().DOFade(0.5f, 0.3f);
        }

        //決定
        if (GetKeyDown(Set: KeybordSets.Decision) || GetKeyDown(Button: XboxControllerButtons.B))
        {
            Audio.PlayClips(2);

            //データがあるとしたら
            if (PlaceTexts[NowSelect].text!="No Data")
            {
                //初めからかロードかを選ばせる
                SelectWindowIn();

                //StartCoroutine("LoadGame");
            }
            else
            {
                BGMController.Controller.BGMFadeOut();

                NewGame();
            }

            enabled = false;

        }

        //キャンセル
        if (GetKeyDown(Set: KeybordSets.Cancel) || GetKeyDown(Button: XboxControllerButtons.A))
        {
            StartCoroutine("SlotsOut");
        }

        //コントローラーの入力取得
        if (InputManager.IsController)
        {
            Horizontal = (Input.GetAxis("Dpad Horizontal") * -1) + Input.GetAxis("Leftstick Horizontal");
        }
        else
        {
            Horizontal = Input.GetAxis("Horizontal");
        }
    }

    private void DataCheck(int num)
    {

        if (!Directory.Exists(Application.persistentDataPath + "/SaveData/0" + (num+1)))
        {
            PlaceTexts[num].text = "No Data";
        }
        else
        {
            SaveData data = new SaveData();

            string NormalPath = Application.persistentDataPath + "/SaveData/0" + (num+1);

            string Path = NormalPath + "/SaveData";
            string FlagPath = NormalPath + "/FlagData";
            string CharaPath = NormalPath + "/CharaData";

            using (var fs = new StreamReader(Path, System.Text.Encoding.GetEncoding("UTF-8")))
            {
                string LoadResult = fs.ReadToEnd();

                data = JsonUtility.FromJson<SaveData>(LoadResult);
            }

            PlaceTexts[num].text = data.SavePlace;
            TimeTexts[num].text = data.GameHour.ToString("00") + ":" + data.GameMinute.ToString("00");
        }
    }

    private void SelectWindowIn()
    {
        SelectWindow.localPosition = new Vector2(Slots[NowSelect].localPosition.x+100,0);

        SelectWindow.DOScaleX(1f,0.3f).OnComplete(()=> 
        {
            SelectWindow.GetComponent<SelectWindow>().enabled = true;

            SelectWindow.GetComponent<SelectWindow>().SelectParent=this;

            SelectWindow.GetComponent<SelectWindow>().ReverseIn();

            enabled = false;
        });


    }

    public void GameDelete()
    {
        string NormalPath = Application.persistentDataPath + "/SaveData/0" + (NowSelect + 1);

        Directory.Delete(NormalPath,true);
    }

    public void NewGame()
    {
        TimeManeger.TimeManage.TimeAdding = true;

        SaveDataManager.DataManage.NowSlot = NowSelect + 1;


        SaveDataManager.DataManage.SaveDataInitialize();

        SceneController.Controller.BackColor = new Color32(69, 91, 255, 255);
        SceneController.Controller.Transition("Novel", SceneFader.FadeType.Loading);

        //SaveDataManager.DataManage.Save();
    }

    public IEnumerator LoadGame()
    {
        SaveDataManager.DataManage.NowSlot = NowSelect + 1;

        SaveDataManager.DataManage.Data = SaveDataManager.DataManage.Load();

        BGMController.Controller.BGMFadeOut();

        SceneController.Controller.BackColor = new Color32(69, 91, 255, 255);
        SceneController.Controller.ContinueFlag = true;

        yield return new WaitForSeconds(0.1f);

        TimeManeger.TimeManage.Hour = SaveDataManager.DataManage.Data.GameHour;
        TimeManeger.TimeManage.Minute = SaveDataManager.DataManage.Data.GameMinute;

        TimeManeger.TimeManage.TimeAdding = true;

        //NovelかAdventureかで変える
        switch (SaveDataManager.DataManage.Data.Scene)
        {
            case SaveData.SceneType.Novel:

                {
                    SceneController.Controller.Transition("Novel", SceneFader.FadeType.Loading);
                }

                break;

            case SaveData.SceneType.Adventure:

                {
                    string name = SaveDataManager.DataManage.Data.AdventureScene;

                    SceneController.Controller.MapTrans = true;

                    if (SaveDataManager.DataManage.Data.Chara == SaveData.CharaType.Normal)
                    {
                        SceneController.Controller.MapTransition(name, SceneController.CharaStatus.Normal, SceneFader.FadeType.Loading, -1);

                    }
                    else
                    {
                        SceneController.Controller.MapTransition(name, SceneController.CharaStatus.Battle, SceneFader.FadeType.Loading, -1);

                    }

                }

                break;
        }

        yield return null;
    }

    public IEnumerator SlotsIn()
    {
        transform.GetChild(0).GetComponent<Image>().DOFade(0.6f,0.5f);

        Audio.PlayClips(0);

        Tween SlotMove01 = Slots[0].DOLocalMoveY(0,0.5f);

        yield return new WaitForSeconds(0.2f);

        Audio.PlayClips(0);

        Tween SlotMove02 = Slots[1].DOLocalMoveY(0,0.5f);

        yield return new WaitForSeconds(0.2f);

        Audio.PlayClips(0);

        Tween SlotMove03 = Slots[2].DOLocalMoveY(0,0.5f);

        yield return new WaitForSeconds(0.2f);

        Audio.PlayClips(0);

        Tween SlotMove04 = Slots[3].DOLocalMoveY(0,0.5f).OnComplete(()=>CanSelect=true);

        yield return null;
    }

    private IEnumerator SlotsOut()
    {
        CanSelect = false;


        Audio.PlayClips(1);

        Tween SlotMove01 = Slots[0].DOLocalMoveY(-1080, 0.5f);

        yield return new WaitForSeconds(0.2f);

        Audio.PlayClips(1);

        Tween SlotMove02 = Slots[1].DOLocalMoveY(-1080, 0.5f);

        yield return new WaitForSeconds(0.2f);

        Audio.PlayClips(1);

        Tween SlotMove03 = Slots[2].DOLocalMoveY(-1080, 0.5f);

        yield return new WaitForSeconds(0.2f);

        Audio.PlayClips(1);

        Tween SlotMove04 = Slots[3].DOLocalMoveY(-1080, 0.5f);

        transform.GetChild(0).GetComponent<Image>().DOFade(0f, 0.5f).OnComplete(() =>
        {
            titleButton.enabled = true;

            enabled = false;
        });


        yield return null;
    }


    public override bool GetKeyDown(KeyCode Key = KeyCode.None, XboxControllerButtons Button = XboxControllerButtons.None, KeybordSets Set = KeybordSets.None)
    {
        return base.GetKeyDown(Key, Button, Set);
    }

    IEnumerator HorizontalStopChange()
    {
        HorizontalStop = true;

        yield return new WaitForSeconds(StopTime);

        HorizontalStop = false;
    }
}
