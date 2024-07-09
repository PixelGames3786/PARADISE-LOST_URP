using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleLogoIn : MonoBehaviour
{
    private float Alpha;

    private Material Material;

    void Start()
    {
        Material = gameObject.GetComponent<Image>().material;

        Material.SetFloat("Alpha", 0);
    }

    // Update is called once per frame
    void Update()
    {
        Alpha += Time.deltaTime*2;

        Material.SetFloat("Alpha", Alpha);

        if (Alpha >= 5)
        {
            Destroy(this);
        }

    }
}
