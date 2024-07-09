using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscCloseGame : MonoBehaviour
{
    static EscCloseGame _instance = null;

    static EscCloseGame instance
    {
        get { return _instance ?? (_instance = FindObjectOfType<EscCloseGame>()); }
    }

    // Start is called before the first frame update
    void Awake()
    {
        // 自身がインスタンスでなければ自滅
        if (this != instance)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    void OnDestroy()
    {

        // ※破棄時に、登録した実体の解除を行なっている

        // 自身がインスタンスなら登録を解除
        if (this == instance) _instance = null;

    }
}
