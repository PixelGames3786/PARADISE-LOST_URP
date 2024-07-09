using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class GeneralBullet : MonoBehaviour
{
    public float BulletLifeTime, RiseTime;

    private float Keika;

    // Start is called before the first frame update
    void Start()
    {
        transform.DOScaleX(1f,RiseTime);
    }

    // Update is called once per frame
    void Update()
    {
        Keika += Time.deltaTime;

        if (Keika>=BulletLifeTime)
        {
            transform.DOScaleX(0f, RiseTime).OnComplete(()=>Destroy(gameObject));

        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PseudoTag>())
        {
            PseudoTag Tag = collision.GetComponent<PseudoTag>();

            //透明タグがあったらすり抜ける
            if (Tag.PsendoTags.Any(value=>value==PseudoTag.Pseudo.Invisible))
            {
                return;
            }
        }

        if (collision.tag=="Charactor")
        {
            Destroy(gameObject);
        }
    }
}
