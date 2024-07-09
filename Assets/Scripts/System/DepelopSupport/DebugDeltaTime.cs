using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugDeltaTime : MonoBehaviour
{
    public bool Baisoku;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {

        if (Baisoku)
        {
            Time.timeScale = 2f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
}
