using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class NovelCharactor : MonoBehaviour
{ 
    private AdventureTextController TextCon;

    private Image CharaImage;

    private Dictionary<string, Sprite> CharaSprites = new Dictionary<string, Sprite>();

    public string SpriteAddress;

    public string CharactorName;

    public string Position;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void MakeNew(string Address,string Name,AdventureTextController atc)
    {
        TextCon = atc;

        SpriteAddress = Address;

        CharaImage = GetComponent<Image>();

        LoadImage();
        SetImage(Name);
    }

    private void LoadImage()
    {
        var Sprites = Resources.LoadAll<Sprite>(SpriteAddress).ToList();

        foreach (Sprite Sprite in Sprites)
        {

            CharaSprites.Add(Sprite.name,Sprite);
        }
    }

    public void SetImage(string Name)
    {
        if (!CharaImage)
        {
            CharaImage = GetComponent<Image>();
        }

        CharaImage.sprite = CharaSprites[Name];

        //画像を変えたらフェードインするよ
        FadeIn();
    }

    private void FadeIn()
    {
        CharaImage.color = new Color(1f,1f,1f,0f);

        CharaImage.DOFade(1f,0.2f);
    }

    public void FadeOut()
    {
        CharaImage.DOFade(0f,0.2f).OnComplete(()=> 
        {
            TextCon.Charactor.RemoveAll((chara)=>chara.CharactorName==CharactorName);
            Destroy(gameObject);
        });
    }

    //喋っていないときに暗くする
    public void FadeDim()
    {
        CharaImage.DOColor(new Color(0.7f,0.7f,0.7f),0.3f);
    }

    //喋っているときに明るくする
    public void FadeBright()
    {
        CharaImage.DOColor(new Color(1f, 1f, 1f), 0.3f);

    }

    //フェード
    public void Fade(float EndValue)
    {
        CharaImage.DOFade(EndValue,0.3f);
    }

    //スケールを変える
    public void ScaleChange(float EndScaleX,float Duration)
    {
        CharaImage.DOFade(0f,Duration/2).OnComplete(()=> 
        {
            transform.localScale = new Vector2(EndScaleX, transform.localScale.y);

            CharaImage.DOFade(1f, Duration / 2);
        });
    }
}
