using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ScreenShake : MonoBehaviour
{
    private CinemachineVirtualCamera m_VCam;
    [SerializeField] private float m_ShakeDuration = 0.5f;
    [SerializeField] private float m_ShakeAmount = 0.2f;

    private float m_ShakeTimer;
    private CinemachineBasicMultiChannelPerlin m_ChannelPerlin;

    private void Awake()
    {
        m_VCam = GetComponent<CinemachineVirtualCamera>();
    }

    // Start is called before the first frame update
    void Start()
    {
        StopShakeCamera();
    }

    void Update()
    {
        if (m_ShakeTimer > 0)
        {
            m_ShakeTimer -= Time.deltaTime;

            if (m_ShakeTimer <= 0)
            {
                StopShakeCamera();
            }
        }
    }

    public void ShakeCamera()
    {
        m_ChannelPerlin = m_VCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        m_ChannelPerlin.m_AmplitudeGain = m_ShakeAmount;

        m_ShakeTimer = m_ShakeDuration;
    }

    public void StopShakeCamera()
    {
        m_ChannelPerlin = m_VCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        m_ChannelPerlin.m_AmplitudeGain = 0f;

        m_ShakeTimer = 0f;
    }
}
