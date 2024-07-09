using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BitDropParent : MonoBehaviour
{
    public Transform Target;

    private AudioSource Audio;

    public int BitNumber;
    public int EachBit;

    public float Speed;
    private float Keika;

    public GameObject BitPrefab;
    public GameObject[] Bits;

    public float AddForceX, AddForceY;

    public bool Tracking;

    // Start is called before the first frame update
    void Start()
    {
        Audio = GetComponent<AudioSource>();

        MakeBits();

        StartCoroutine("WaitMinute");
    }

    // Update is called once per frame
    void Update()
    {
        if (Tracking)
        {
            Keika += Time.deltaTime/2;

            for (int i = 0; i < BitNumber; i++)
            {
                if (Bits[i])
                {
                    Bits[i].transform.position = Vector2.MoveTowards(Bits[i].transform.position, Target.transform.position, Speed * Keika);
                }
            }

            if (transform.childCount==0)
            {
                Destroy(gameObject);
            }
        }


    }

    public void MakeBits()
    {
        Bits = new GameObject[BitNumber];

        for (int i = 0; i < BitNumber; i++)
        {
            Bits[i] = Instantiate(BitPrefab, transform);

            float ForceX = Random.Range(-AddForceX, AddForceX);
            float ForceY = Random.Range(0, AddForceY);

            Bits[i].GetComponent<Rigidbody2D>().AddForce(new Vector2(ForceX, ForceY), ForceMode2D.Impulse);

        }
    }

    public void GetBit(Transform BitTrans)
    {
        SaveDataManager.DataManage.Data.Bit += EachBit;

        Audio.Play();

        BitTrans.DOScale(new Vector3(0, 0, 0), 0.5f).OnComplete(() => 
        {
            Destroy(BitTrans.gameObject);
        });

    }

    IEnumerator WaitMinute()
    {
        yield return new WaitForSeconds(2f);

        Tracking = true;

        if (!Target)
        {
            Target = GameObject.Find("Charactor").transform;
        }
    }
}
