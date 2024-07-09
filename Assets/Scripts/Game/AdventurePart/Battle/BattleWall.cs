using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleWall : MonoBehaviour
{
    private Animator Animator;

    public Shader DefaultShader;
    public Texture2D Sprite;

    private Material Mate;

    public float Intensity;

    // Start is called before the first frame update
    void Start()
    {
        Animator = GetComponent<Animator>();

        Mate = new Material(DefaultShader);

        Mate.SetTexture("_MainTex", Sprite);
        Mate.SetFloat("_Alpha", 0f);

        float Factor = Mathf.Pow(2, Intensity);
        Mate.SetColor("_Color", new Color(1f * Factor, 1f * Factor, 1f * Factor));

        GetComponent<SpriteRenderer>().material = Mate;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.tag=="Charactor")
        {
            Animator.SetTrigger("InTrigger");
        }
        
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Charactor")
        {
            Animator.SetTrigger("OutTrigger");
        }
    }
}
