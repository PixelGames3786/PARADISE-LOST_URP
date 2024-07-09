using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TitleGamenChange : MonoBehaviour
{
    [SerializeField]
    private GameObject TitleBackGround,TitleLogo,Button;

    //黒い背景を削除しタイトルの背景にする&タイトルロゴ発光
    public void ChangeTitle()
    {
        Transform Canvas = GameObject.Find("TitleCanvas").transform;

        Canvas.GetChild(2).gameObject.SetActive(true);

        {
            Destroy(Canvas.GetChild(0).gameObject);

            Transform BackGround = Instantiate(TitleBackGround, Canvas).transform;

            BackGround.SetAsFirstSibling();
        }

        //タイトルロゴ
        {
            Instantiate(TitleLogo,Canvas);

            Destroy(gameObject);
        }

        //BGMを流す

        BGMController.Controller.BGMPlay(0);


    }
}
