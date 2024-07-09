using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManeger : MonoBehaviour
{
    public static TimeManeger TimeManage
    {
        get { return _timemanage ?? (_timemanage = FindObjectOfType<TimeManeger>()); }
    }

    static TimeManeger _timemanage;

    public bool TimeAdding;

    public int Minute, Hour;
    public float Seconds;


    // Start is called before the first frame update
    void Awake()
    {
        // 自身がインスタンスでなければ自滅
        if (this != TimeManage)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (!TimeAdding)
        {
            return;
        }

        Seconds += Time.deltaTime;

        if (Seconds>=60f)
        {
            Minute++;

            if (Minute>=60)
            {
                Hour++;

                Minute = 0;
            }

            Seconds -= 60;
        }
    }

    void OnDestroy()
    {

        // ※破棄時に、登録した実体の解除を行なっている

        // 自身がインスタンスなら登録を解除
        if (this == TimeManage) _timemanage = null;

    }
}
