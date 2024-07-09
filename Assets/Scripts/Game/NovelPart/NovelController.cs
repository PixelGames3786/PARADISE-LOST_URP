using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NovelController : MonoBehaviour
{
    public SaveDataManager SaveManege;

    public TextController TC;

    [System.NonSerialized]
    public AudioGeneral Audio;

    public Text Text,SpeakerText;
    public TextAsset Scenario;

    public Image BGBefore, BGAfter;

    // Start is called before the first frame update
    void Start()
    {
        Audio = GetComponent<AudioGeneral>();

        TC = new TextController(this);

        SaveManege = FindObjectOfType<SaveDataManager>().GetComponent<SaveDataManager>();
    }

    void Update()
    {
        TC.TextSinkou();
        TC.WaitButton();
    }

    //最初から始める時
    public void SetFirstScene()
    {
        TC.CanText = true;
        TC.SetScene(SaveManege.Data.NovelScene.ToString());
    }

    public void Save()
    {
        TC.SaveNovelData();
    }
}
