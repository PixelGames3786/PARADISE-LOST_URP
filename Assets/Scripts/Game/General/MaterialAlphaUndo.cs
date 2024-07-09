using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialAlphaUndo : MonoBehaviour
{
    public Material[] Materials;

    // Start is called before the first frame update
    void Start()
    {
        Undo();
    }

    public void Undo()
    {
        for (int i = 0; i < Materials.Length; i++)
        {
            Materials[i].SetFloat("_Alpha", 1f);
        }
    }
}
