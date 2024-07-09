using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashEffectFollow : MonoBehaviour
{
    public Transform Target,Trans;

    // Start is called before the first frame update
    void Start()
    {
        Trans = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Target)
        {
            Trans.position = new Vector3(Target.position.x, Trans.position.y);

        }

    }
}
