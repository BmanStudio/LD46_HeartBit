using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveAnimator : MonoBehaviour
{
    private static string PEAK_AMPLITUDE_PARAM = "_PeakAmplitude";
    private static string WAVE_COLOR_PARAM = "_LineColor";
    private static string WAVE_LENGTH_PARAM = "_WaveLength";

    [SerializeField]
    private Color m_waveColor;

    [SerializeField]
    private float m_waveAmplitude;

    [SerializeField]
    private float m_waveOctaves;

    private Renderer m_waveRenderer;

    private MaterialPropertyBlock m_wavePropertyBlock;

    private float m_previousAmpltiude;
    private float m_nextAmplitude;
    private float m_animationDuration;
    private float m_animationStartTime;

    // Start is called before the first frame update
    void Start()
    {
        m_wavePropertyBlock = new MaterialPropertyBlock();
        m_waveRenderer = GetComponent<MeshRenderer>();

        m_waveRenderer.GetPropertyBlock(m_wavePropertyBlock, 0);

        m_wavePropertyBlock.SetColor(WAVE_COLOR_PARAM, m_waveColor);
        m_wavePropertyBlock.SetFloat(WAVE_LENGTH_PARAM, m_waveOctaves);
        m_waveRenderer.SetPropertyBlock(m_wavePropertyBlock);

        m_animationStartTime = Time.time;
        m_animationDuration = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= m_animationStartTime + m_animationDuration)
        {
            m_animationDuration = Random.Range(0.5f, 2.0f);
            m_animationStartTime = Time.time;

            m_previousAmpltiude = m_wavePropertyBlock.GetFloat(PEAK_AMPLITUDE_PARAM);
            m_nextAmplitude = Random.Range(-10.0f, 10.0f);
        }
        else
        {
            float m_timePassed = Time.time - m_animationStartTime;
            float m_animationProgress = m_timePassed / m_animationDuration;

            float currentAmplitude = Mathf.Lerp(m_previousAmpltiude, m_nextAmplitude, m_animationProgress);
            m_wavePropertyBlock.SetFloat(PEAK_AMPLITUDE_PARAM, currentAmplitude);
            m_waveRenderer.SetPropertyBlock(m_wavePropertyBlock);
        }
    }
}
