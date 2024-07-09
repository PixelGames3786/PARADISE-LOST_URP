using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldenAppleTracking : MonoBehaviour
{
    public Transform Target;

    private Rigidbody2D RB;

    public float Distance,Speed;

    //true 左 false 右
    private bool Direction;

    // Start is called before the first frame update
    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //右行き
        if (Target.position.x > transform.position.x)
        {
            if (Target.position.x - transform.position.x > Distance)
            {
                if (Direction)
                {
                    RB.velocity = new Vector2(0, 0);

                    Direction = false;
                }

                RB.velocity = new Vector2(Speed,0);
            }
            else
            {
                RB.velocity = new Vector2(0, 0);

            }
        }
        //左行き
        else
        {
            if (transform.position.x-Target.position.x > Distance)
            {
                if (!Direction)
                {
                    RB.velocity = new Vector2(0, 0);

                    Direction = true;
                }

                RB.velocity = new Vector2(-Speed, 0);
            }
            else
            {
                RB.velocity = new Vector2(0, 0);

            }
        }
    }
}
