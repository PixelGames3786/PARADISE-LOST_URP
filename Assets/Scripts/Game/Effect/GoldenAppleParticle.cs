using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldenAppleParticle : MonoBehaviour
{
    private float Count;
    public float PopSpeed,LifeTime;

    public float[] HoriVelocity;
    public float[] VertiVelocity;
    public float[] Sizes;

    public GameObject ParticlePrefab;
    public Transform ParticleParent,GoldenApple;

    public List<GameObject> ParticleChildren;
    public List<float> ChildrenLifeTime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Count += Time.deltaTime;

        if (Count>=PopSpeed)
        {
            Count = 0;

            ParticlePop();
        }

        if (ParticleChildren.Count>0)
        {
            for (int i=0;i<ParticleChildren.Count;i++)
            {
                ChildrenLifeTime[i] += Time.deltaTime;

                if (ChildrenLifeTime[i]>=LifeTime)
                {
                    ParticleReuse(i);

                    /*
                    Destroy(ParticleChildren[i]);

                    ParticleChildren.RemoveAt(i);
                    ChildrenLifeTime.RemoveAt(i);
                    */
                }
            }
        }
    }

    private void ParticlePop()
    {
        Rigidbody2D RB;

        float Size = Random.Range(Sizes[0],Sizes[1]);

        for (int i=0;i<ParticleChildren.Count;i++)
        {
            if (!ParticleChildren[i].activeSelf)
            {
                ParticleChildren[i].SetActive(true);

                ParticleChildren[i].transform.position = transform.position;
                ParticleChildren[i].transform.localScale = new Vector2(Size, Size);

                RB = ParticleChildren[i].GetComponent<Rigidbody2D>();

                RB.velocity = new Vector2(Random.Range(HoriVelocity[0], HoriVelocity[1]), Random.Range(VertiVelocity[0], VertiVelocity[1]));

                return;
            }
        }

        RB = Instantiate(ParticlePrefab, ParticleParent).GetComponent<Rigidbody2D>();

        ParticleChildren.Add(RB.gameObject);
        ChildrenLifeTime.Add(0);

        RB.gameObject.transform.position = transform.position;
        RB.gameObject.transform.localScale = new Vector2(Size,Size);

        RB.velocity = new Vector2(Random.Range(HoriVelocity[0],HoriVelocity[1]),Random.Range(VertiVelocity[0],VertiVelocity[1]));
    }

    private void ParticleReuse(int Num)
    {
        ChildrenLifeTime[Num] = 0;
        ParticleChildren[Num].transform.localPosition = new Vector3(0,0,0);

        ParticleChildren[Num].GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);

        ParticleChildren[Num].SetActive(false);
    }
}
