using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DeltaGNWing : MonoBehaviour
{
    public DeltaGNController GNC;

    private GameObject[] Bits = new GameObject[3];

    public GameObject Sword;
    public DeltaGNSword GNS;

    private bool BitPurging;

    // Start is called before the first frame update
    void Start()
    {
        GNC = transform.parent.GetComponent<DeltaGNController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (BitPurging&&GNC.BitParent.childCount==0)
        {
            GNC.StateChange(DeltaGNController.DeltaGNState.Sword);

            BitPurging = false;
            GNC.PurgedFlag = false;
        }
    }

    public void BitPurge()
    {
        GameObject Prefab;

        Vector3 TsPosition,TsRotation;

        for (int i=0;i<3;i++)
        {
            Prefab = transform.GetChild(i).gameObject;
            TsPosition = new Vector3(Prefab.transform.position.x,Prefab.transform.position.y);
            TsRotation = new Vector3(transform.localRotation.x,transform.localRotation.y);

            Bits[i] = Instantiate(Prefab, GNC.BitParent);

            Destroy(transform.GetChild(i).gameObject);

            Bits[i].transform.position = TsPosition;
            Bits[i].transform.rotation = Quaternion.Euler(TsRotation);

            Bits[i].GetComponent<TrailRenderer>().enabled = true;
            Bits[i].GetComponent<DeltaGNBit>().enabled = true;
            Bits[i].GetComponent<PolygonCollider2D>().enabled = true;
            Bits[i].GetComponent<AudioSource>().enabled = true;
            Bits[i].GetComponent<SpriteRenderer>().color = new Color(1,1,1,1);

            Bits[i].GetComponent<SpriteRenderer>().sortingLayerName = "FlontEnemy";

            Bits[i].GetComponent<DeltaGNBit>().GNW = this;
        }

        BitPurging = true;

    }

    public void SwordPurge()
    {
        GameObject Prefab;

        Vector3 TsPosition, TsRotation;

        Prefab = transform.GetChild(0).gameObject;
        TsPosition = new Vector3(Prefab.transform.position.x, Prefab.transform.position.y);
        TsRotation = new Vector3(0,0,Prefab.transform.rotation.z);

        Sword = Instantiate(Prefab,GNC.BitParent);

        Destroy(transform.GetChild(0).gameObject);

        Sword.transform.position = TsPosition;
        Sword.transform.rotation = Quaternion.Euler(TsRotation);

        Sword.GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f,1f);

        GNS = Sword.GetComponent<DeltaGNSword>();

        GNS.GNW = this;
        GNS.enabled = true;

        GNS.OriginalPosi = new Vector2(Prefab.transform.localPosition.x,Prefab.transform.localPosition.y);
        GNS.OriginalQuater = TsRotation;

        Sword.GetComponent<PolygonCollider2D>().enabled = true;
        Sword.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

    }

    public void SwordReturn()
    {
        GameObject Prefab;

        Prefab = Sword;
        Prefab.GetComponent<DeltaGNSword>().enabled = false;
        Sword.GetComponent<EnemyAttackCollider>().enabled = false;

        //新しいSword作成
        Sword = Instantiate(Prefab,transform);
        Sword.transform.localScale = new Vector3(1f,1f,1f);

        Sword.name = "Sword01";
        
        Sword.transform.localPosition = GNS.OriginalPosi;
        Sword.transform.localRotation = Quaternion.Euler(transform.InverseTransformDirection(GNS.OriginalQuater));

        Destroy(Prefab);

        GNS = Sword.GetComponent<DeltaGNSword>();

        GNS.enabled = false;

        Sword.GetComponent<PolygonCollider2D>().enabled = false;
        Sword.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

        Sequence sequence =DOTween.Sequence();

        Tween Fade = Sword.GetComponent<SpriteRenderer>().DOFade(1f, 0.5f);

        if (!GNC.PurgedFlag)
        {
            sequence.Append(Fade).OnComplete(() => GNC.StateChange(DeltaGNController.DeltaGNState.Avoid));

            GNC.PurgedFlag = true;
        }
        else
        {
            sequence.Append(Fade);
        }


        sequence.Play();

    }
}
