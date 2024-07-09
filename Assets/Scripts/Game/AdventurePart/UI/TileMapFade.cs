using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapFade : MonoBehaviour
{
    public Tilemap Tile;

    private float Target, FadeTime,Plus;

    private bool Fading,ActiveChange;

    // Start is called before the first frame update
    void Start()
    {
        Tile = GetComponent<Tilemap>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Fading)
        {
            Tile.color = new Color(1f,1f,1f,Tile.color.a+Plus);

            if (Tile.color.a==Target)
            {
                Fading = false;

                if (ActiveChange)
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }

    public void Fade(float tag,float time,bool active)
    {
        Fading = true;

        Target = tag-Tile.color.a;
        FadeTime = time;

        Plus = Target/(time * Time.deltaTime);


        ActiveChange = active;
    }
}
