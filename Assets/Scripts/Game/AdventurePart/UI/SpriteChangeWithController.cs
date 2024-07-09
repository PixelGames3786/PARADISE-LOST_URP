using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteChangeWithController : MonoBehaviour
{
    public enum ImageType
    {
        sprite,
        image
    }

    public enum ControlType
    {
        Controller,
        Keyborad
    }

    public ImageType Type;
    public ControlType Control;

    public Sprite KeyboradSprite, ControllerSprite;

    [Space(10)]

    public float ConPositionX;
    public float ConPositionY,ConSizeX,ConSizeY;

    [Space(10)]

    public float KeyPositionX;
    public float KeyPositionY, KeySizeX, KeySizeY;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.IsController)
        {
            if (Control==ControlType.Keyborad)
            {
                ImageChange();
            }
        }
        else
        {
            if (Control==ControlType.Controller)
            {
                ImageChange();
            }
        }
    }

    private void ImageChange()
    {
        //キーボードからコントローラー移行
        if (Control == ControlType.Keyborad)
        {
            Control = ControlType.Controller;

            if (Type==ImageType.image)
            {
                GetComponent<Image>().sprite = ControllerSprite;

                transform.localPosition = new Vector2(ConPositionX,ConPositionY);

                GetComponent<RectTransform>().sizeDelta = new Vector2(ConSizeX,ConSizeY);
            }
            else
            {
                GetComponent<SpriteRenderer>().sprite = ControllerSprite;

                transform.localPosition = new Vector2(ConPositionX, ConPositionY);

                transform.localScale = new Vector2(ConSizeX, ConSizeY);
            }

        }
        //コントローラーからキーボード移行
        else
        {
            Control = ControlType.Keyborad;

            if (Type == ImageType.image)
            {
                GetComponent<Image>().sprite = KeyboradSprite;

                transform.localPosition = new Vector2(KeyPositionX, KeyPositionY);

                GetComponent<RectTransform>().sizeDelta = new Vector2(KeySizeX, KeySizeY);
            }
            else
            {
                GetComponent<SpriteRenderer>().sprite = ControllerSprite;

                transform.localPosition = new Vector2(KeyPositionX, KeyPositionY);

                transform.localScale = new Vector2(KeySizeX, KeySizeY);
            }
        }
    }
}
