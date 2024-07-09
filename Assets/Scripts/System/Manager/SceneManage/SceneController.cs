using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public enum CharaStatus
    {
        Normal,
        Battle
    }

    public CharaStatus Status;

    public AsyncOperation SceneLoad,SceneUnLoad;

    public Color32 BackColor;

    public bool ContinueFlag,MapTrans;

    public Vector2 Position;

    public int PositionNum,SpecialNum,IventNum;

    public List<string> OtherElements;

    private string SceneName;

    public static SceneController Controller
    {
        get { return _controller ?? (_controller = FindObjectOfType<SceneController>()); }
    }

    static SceneController _controller=null;

    public static SceneController Create()
    {
        GameObject sceneControllerGameObject = new GameObject("SceneController");
        _controller = sceneControllerGameObject.AddComponent<SceneController>();

        return _controller;
    }

    void Awake()
    {
        // 自身がインスタンスでなければ自滅
        if (this != Controller)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void Transition(string scenename,SceneFader.FadeType FadeType)
    {
        SceneFader.BackColor = BackColor;

        SceneFader.FadeSceneOut(FadeType);
        
        SceneName = scenename;
    }

    public void MapTransition(string scenename,CharaStatus status,SceneFader.FadeType FadeType,int Pnum,int Special=0,bool maptrans=true,int SpecialIvent=0,List<string> other=null)
    {
        SceneFader.BackColor = BackColor;

        SceneFader.FadeSceneOut(FadeType);

        Status = status;
        SceneName = scenename;
        PositionNum = Pnum;
        SpecialNum = Special;
        IventNum = SpecialIvent;

        OtherElements = other;

        MapTrans = maptrans;
    }

    public void StartSceneLoad()
    {
        SceneLoad = SceneManager.LoadSceneAsync(SceneName);

        SceneLoad.allowSceneActivation = false;
    }

    private void TransitionToScene(string SceneName,SceneFader.FadeType FadeType)
    {
        SceneLoad = SceneManager.LoadSceneAsync(SceneName);

        SceneLoad.allowSceneActivation = false;

        SceneFader.BackColor = BackColor;

        SceneFader.FadeSceneOut(FadeType);
    }

    void OnDestroy()
    {

        // ※破棄時に、登録した実体の解除を行なっている

        // 自身がインスタンスなら登録を解除
        if (this == Controller) _controller = null;

    }
}
