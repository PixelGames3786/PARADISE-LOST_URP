using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSelfDestory : MonoBehaviour
{
    private enum DestroyType
    {
        Destroy,
        Off
    }

    [SerializeField]
    private DestroyType Type;

    private ParticleSystem particle;

    // Start is called before the first frame update
    void Start()
    {
        particle = this.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (particle.isStopped) //パーティクルが終了したか判別
        {
            switch (Type)
            {
                case DestroyType.Destroy:

                    Destroy(gameObject);//パーティクル用ゲームオブジェクトを削除

                    break;

                case DestroyType.Off:

                    gameObject.SetActive(false);

                    break;
            }

        }
    }
}
