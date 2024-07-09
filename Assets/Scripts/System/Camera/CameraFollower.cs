using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public Vector2 MaxPosition,MiniPosition;

    public Transform CharaTrans;

    public float AttenRate = 3.0f,OverDistance=20f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        CameraFollow();
    }

    private void CameraFollow()
    {
        if (!CharaTrans)
        {
            if (GameObject.Find("Charactor"))
            {
                CharaTrans = GameObject.Find("Charactor").transform;
            }
            else
            {
                return;
            }
        }

        var pos = new Vector3(CharaTrans.position.x, 0, -10); // 本来到達しているべきカメラ位置

        if (CharaTrans.position.x-transform.position.x>=OverDistance)
        {
            transform.position = pos;

            return;
        }

        var TargetPosition = Vector3.Lerp(transform.position, pos, Time.deltaTime * AttenRate); // Lerp減衰

        TargetPosition.x = Mathf.Clamp(TargetPosition.x,MiniPosition.x,MaxPosition.x);

        transform.position = TargetPosition;

        //transform.position = new Vector3(XPosition,transform.position.y,transform.position.z);
    }
}
