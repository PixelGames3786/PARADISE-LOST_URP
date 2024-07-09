using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackGroundIn : MonoBehaviour
{
    private float Alpha;

    private Material Material;

    void Start()
    {
        Material = gameObject.GetComponent<Image>().material;

        Material.SetFloat("_Alpha",0);
    }

    // Update is called once per frame
    void Update()
    {
        Alpha += Time.deltaTime;

        Material.SetFloat("_Alpha",Alpha);

        if (Alpha>=1)
        {
            Destroy(this);
        }
        
    }
}
