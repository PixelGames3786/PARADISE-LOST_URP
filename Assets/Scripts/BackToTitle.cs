using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackToTitle : InputComponent
{
    private bool Seni;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GetKeyDown(Key=KeyCode.Space,ControllerButton=XboxControllerButtons.RightBumper))
        {
            if (Seni)
            {
                return;
            }

            Seni = true;

            FindObjectOfType<AdventureController>().SceneChange("Title",SceneFader.FadeType.Black);
        }
    }

    //継承
    public override bool GetKeyDown(KeyCode Key = KeyCode.None, XboxControllerButtons Button = XboxControllerButtons.None, KeybordSets Set = KeybordSets.None)
    {
        return base.GetKeyDown(Key, Button, Set);
    }
}
