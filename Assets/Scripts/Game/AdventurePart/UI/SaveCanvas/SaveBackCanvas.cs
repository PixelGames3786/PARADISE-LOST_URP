using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveBackCanvas : MonoBehaviour
{
    private bool End;

    private Transform BackParent;

    public GameObject SaveBackPrefab;

    public AdventureController ac;

    private AudioGeneral Audio;

    private int[][] Position = {
        new int[]{1,0,0,0 },
        new int[]{0,0,0,0 },
        new int[]{0,0,0,0 },
        new int[]{0,0,0,0 },
        new int[]{0,0,0,0 },
        new int[]{0,0,0,0 },
        new int[]{0,0,0,0 },
        new int[]{0,0,0,0 },
    };

    private GameObject[,] BackMatome=new GameObject[8,4];

    // Start is called before the first frame update
    void Start()
    {
        Audio = GetComponent<AudioGeneral>();

        GetComponent<Canvas>().worldCamera = Camera.main.transform.GetChild(0).GetComponent<Camera>();

        BackParent = transform.GetChild(0);


        StartCoroutine("MakeBack");
    }

    IEnumerator MakeBack()
    {
        for (int i=0;i<Position.Length;i++)
        {
            for (int u=0;u<Position[i].Length;u++)
            {

                Vector2 Posi = new Vector2(-960+(480*u),540-(135*i));

                if (Position[i][u] ==2 ||Position[i][u]==3)
                {
                    Position[i][u]--;
                }

                //一つ作成
                if (Position[i][u]==1)
                {
                    Audio.PlayClips(0);

                    GameObject Back =Instantiate(SaveBackPrefab,BackParent);
                    Back.transform.localPosition = Posi;

                    BackMatome[i,u] = Back;

                    if (i<=6&& Position[i + 1][u] == 0)
                    {
                        Position[i + 1][u] = 3;
                    }

                    if (u<=2&&Position[i][u+1]==0)
                    {
                        Position[i][u + 1] = 3;
                    }

                    Position[i][u] = 4;
                }
            }
        }

        if (Position[7][3]==4)
        {
            Position[7][3] = 7;

            End = true;
        }

        if (!End)
        {
            yield return new WaitForSeconds(0.1f);

            StartCoroutine("MakeBack");
        }
        else
        {
            End = false;

            yield return new WaitForSeconds(3f);

            //Audio.PlayClips(1);

            StartCoroutine("DestroyBack");
        }

    }

    IEnumerator DestroyBack()
    {
        for (int i = 7; i >=0; i--)
        {
            for (int u = 3; u >=0; u--)
            {

                if (Position[i][u] == 5 || Position[i][u] == 6)
                {
                    Position[i][u]++;
                }

                if (Position[i][u] == 7)
                {
                    Audio.PlayClips(1);

                    BackMatome[i,u].GetComponent<SaveBackObject>().Out();

                    if (i >0 && Position[i -1][u] == 4)
                    {
                        Position[i -1][u] = 5;
                    }

                    if (u >0 && Position[i][u -1] == 4)
                    {
                        Position[i][u -1] = 5;
                    }

                    Position[i][u] = 0;
                }
            }
        }

        if (Position[0][0]==0)
        {
            End = true;
        }

        if (End)
        {
            yield return new WaitForSeconds(0.2f);

            ac.SaveEnd();

            Destroy(gameObject);

        }
        else
        {
            yield return new WaitForSeconds(0.1f);

            StartCoroutine("DestroyBack");
        }

    }
}
