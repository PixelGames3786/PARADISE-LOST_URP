using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubCameraInterlocking : MonoBehaviour
{
    private Camera Sub, Main;

    // Start is called before the first frame update
    void Start()
    {
        Main = transform.parent.GetComponent<Camera>();

        Sub = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Main.orthographicSize!=Sub.orthographicSize)
        {
            Sub.orthographicSize = Main.orthographicSize;
        }
    }
}
