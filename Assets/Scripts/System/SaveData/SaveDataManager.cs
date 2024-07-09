using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System.IO;

public class SaveDataManager : MonoBehaviour
{

    public static SaveDataManager DataManage
    {
        get { return _datamanage ?? (_datamanage = FindObjectOfType<SaveDataManager>()); }
    }

    static SaveDataManager _datamanage;

    public SaveData Data;
    
    public OptionSaveData OptionData;

    public AudioMixer Mixer;

    public float TextSpeed;

    public int NowSlot=1;

    public List<string> HyoujiFlag;

    void Awake()
    {
        // 自身がインスタンスでなければ自滅
        if (this != DataManage)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

    }

    // Start is called before the first frame update
    void Start()
    {
        //AllLoad();

        //オプションセーブデータがあったらロードする
        if (File.Exists(Application.persistentDataPath + "/SaveData/OptionSaveData"))
        {
            OptionData = OptionLoad();
        }
        else
        {
            OptionDataInitialize();
        }
    }

    //セーブデータロード
    public void AllLoad()
    {
        //セーブデータがあったらロードする
        if (File.Exists(Application.persistentDataPath + "/SaveData/SaveData"))
        {
            Data = Load();
        }
        else
        {
            SaveDataInitialize();
        }

        //オプションセーブデータがあったらロードする
        if (File.Exists(Application.persistentDataPath + "/SaveData/OptionSaveData"))
        {
            OptionData = OptionLoad();
        }
        else
        {
            OptionDataInitialize();
        }
    }

    //セーブデータ初期化
    public void SaveDataInitialize()
    {
        Data = new SaveData();

        Data.Scene = SaveData.SceneType.Novel;

        Data.NovelScene = 1;
        Data.NovelLine = 3;

        Data.DashGaugeMax = 3;

        Data.GameFlags = new Dictionary<string, bool>();
        Data.CharactorDatas = new Dictionary<string, CharactorData>();

        //アップルのだけ設定しておく
        CharactorData AppleData = new CharactorData();

        AppleData.CharaHPMax = 50;
        AppleData.CharaHP = 50;

        AppleData.DashCount = 0;

        Data.CharactorDatas.Add("Apple",AppleData);
    }

    //オプションデータ初期化
    private void OptionDataInitialize()
    {
        OptionData = new OptionSaveData();

        OptionData.Volume = new int[3];

        //音量設定
        OptionData.Volume[0] = 3;
        OptionData.Volume[1] = 3;
        OptionData.Volume[2] = 3;

        OptionData.TextSpeed = 2.5f;

        Mixer.SetFloat("MasterVolume", -16);
        Mixer.SetFloat("SEVolume", 0);
        Mixer.SetFloat("BGMVolume", 0);
    }


    public void Save()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/SaveData/0" + NowSlot))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/SaveData/0" + NowSlot);
        }

        string NormalPath=Application.persistentDataPath + "/SaveData/0" + NowSlot;

        string Path = NormalPath+ "/SaveData";
        string FlagPath = NormalPath + "/FlagData";
        string CharaPath = NormalPath + "/CharaData";

        //セーブデータに経過時間を記録する
        Data.GameHour = TimeManeger.TimeManage.Hour;
        Data.GameMinute = TimeManeger.TimeManage.Minute;

        //true 追記 false 上書き
        bool isAppend = false;

        string DataJson = JsonUtility.ToJson(Data);
        string FlagJson = JsonUtility.ToJson(new Serialization<string, bool>(Data.GameFlags));
        string CharaJson = JsonUtility.ToJson(new Serialization<string, CharactorData>(Data.CharactorDatas));

        //書き込み
        using (var fs = new StreamWriter(Path, isAppend, System.Text.Encoding.GetEncoding("UTF-8")))
        {
            fs.Write(DataJson);
        }

        using (var fs = new StreamWriter(FlagPath, isAppend, System.Text.Encoding.GetEncoding("UTF-8")))
        {
            fs.Write(FlagJson);
        }

        using (var fs = new StreamWriter(CharaPath, isAppend, System.Text.Encoding.GetEncoding("UTF-8")))
        {
            fs.Write(CharaJson);
        }
    }

    public SaveData Load()
    {
        SaveData data = new SaveData();

        string NormalPath = Application.persistentDataPath + "/SaveData/0" + NowSlot;

        string Path = NormalPath + "/SaveData";
        string FlagPath= NormalPath + "/FlagData";
        string CharaPath= NormalPath + "/CharaData";

        using (var fs = new StreamReader(Path, System.Text.Encoding.GetEncoding("UTF-8")))
        {
            string LoadResult = fs.ReadToEnd();

            data = JsonUtility.FromJson<SaveData>(LoadResult);
        }

        using (var fs = new StreamReader(FlagPath, System.Text.Encoding.GetEncoding("UTF-8")))
        {
            string LoadResult = fs.ReadToEnd();

            data.GameFlags = JsonUtility.FromJson<Serialization<string,bool>>(LoadResult).ToDictionary();
        }

        using (var fs = new StreamReader(CharaPath, System.Text.Encoding.GetEncoding("UTF-8")))
        {
            string LoadResult = fs.ReadToEnd();

            data.CharactorDatas = JsonUtility.FromJson<Serialization<string, CharactorData>>(LoadResult).ToDictionary();
        }

        int count = 0;

        foreach (string Name in data.GameFlags.Keys)
        {
            HyoujiFlag.Add(Name);
        }

        foreach (bool Value in data.GameFlags.Values)
        {
            HyoujiFlag[count] += ":" + Value.ToString();

            count++;
        }

        return data;

    }


    public void OptionSave()
    {
        string Path = Application.persistentDataPath + "/SaveData/OptionSaveData";

        //true 追記 false 上書き
        bool isAppend = false;

        string JsonString = JsonUtility.ToJson(OptionData);

        //書き込み
        using (var fs = new StreamWriter(Path, isAppend, System.Text.Encoding.GetEncoding("UTF-8")))
        {
            fs.Write(JsonString);
        }
    }

    public OptionSaveData OptionLoad()
    {
        OptionSaveData Result = new OptionSaveData();

        string Path = Application.persistentDataPath + "/SaveData/OptionSaveData";

        using (var fs = new StreamReader(Path, System.Text.Encoding.GetEncoding("UTF-8")))
        {
            string LoadResult = fs.ReadToEnd();

            Result = JsonUtility.FromJson<OptionSaveData>(LoadResult);
        }

        Mixer.SetFloat("MasterVolume", -22 + (Result.Volume[0] * 2));
        Mixer.SetFloat("SEVolume", -3 + (Result.Volume[1] * 1));
        Mixer.SetFloat("BGMVolume", -3 + (Result.Volume[2] * 1));

        SpeedSet();

        return Result;
    }


    public void SpeedSet()
    {
        TextSpeed = 0.055f + (OptionData.TextSpeed * 0.015f);
    }


    //フラグ関連
    public void FlagSet(string FlagName,bool FlagContent)
    {
        //既にその名前のフラグがあったら上書きする
        if (Data.GameFlags.ContainsKey(FlagName))
        {
            Data.GameFlags[FlagName] = FlagContent;
        }
        //その名前のフラグがなかったら追加する
        else
        {
            Data.GameFlags.Add(FlagName,FlagContent);
        }
    }

    public void FlagRemove(string FlagName)
    {
        Data.GameFlags.Remove(FlagName);
    }

    void OnDestroy()
    {

        // ※破棄時に、登録した実体の解除を行なっている

        // 自身がインスタンスなら登録を解除
        if (this == DataManage) _datamanage = null;

    }
}
