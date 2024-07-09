using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BitDropTracking : MonoBehaviour
{
    private ParticleSystem ps;
    private ParticleSystem.Particle[] m_Particles;

    public Transform Target;

    public float threshold = 100f;
    public float intensity = 1f;

    public bool Tracking;

    public float WaitTime;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();

        Target = GameObject.Find("Charactor").transform;

        StartCoroutine("WaitMinute");
    }

    // Update is called once per frame
    void Update()
    {

        if (Tracking)
        {
            m_Particles = new ParticleSystem.Particle[ps.main.maxParticles];
            int ParticleNumber = ps.GetParticles(m_Particles);

            for (int i = 0; i < ParticleNumber; i++)
            {
                var velocity = ps.transform.TransformPoint(m_Particles[i].velocity);
                var position = ps.transform.TransformPoint(m_Particles[i].position);

                var period = m_Particles[i].remainingLifetime*0.5f;

                //ターゲットと自分自身の差
                var diff = Target.TransformPoint(Target.position)/3 - position;
                Vector3 accel = (diff - velocity * period) * 1f / (period * period);

                //加速度が一定以上だと追尾を弱くする
                if (accel.magnitude > threshold)
                {
                    accel = accel.normalized * threshold;
                }

                // 速度の計算
                velocity += accel * Time.deltaTime * intensity;
                m_Particles[i].velocity = ps.transform.InverseTransformPoint(velocity);
            }

            ps.SetParticles(m_Particles, ParticleNumber);
        }
    }

    IEnumerator WaitMinute()
    {
        yield return new WaitForSeconds(WaitTime);

        Tracking = true;
    }
}
