using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.DOShakePosition(5.0f,0.1f,10000);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
