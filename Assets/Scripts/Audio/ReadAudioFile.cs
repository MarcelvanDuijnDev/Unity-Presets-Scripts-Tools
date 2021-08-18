using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadAudioFile : MonoBehaviour
{
    [Header("Settings")]
    private AudioSource _AudioScource = null;
    private bool _PlayOnStart = true;

    [Header("Info")]
    public float[] Samples = new float[512];
    public float[] FreqBand = new float[8];
    public float[] BandBuffer = new float[8];
    public float[] BufferDecrease = new float[8];

    void Start()
    {
        if (_AudioScource == null)
            _AudioScource = GetComponent<AudioSource>();

        if (_PlayOnStart)
            _AudioScource.Play();
    }

    void Update()
    {
        GetSpectrumAudioSource();
        MakeFrequencyBands();
        UpdateBandBuffer();
    }

    void GetSpectrumAudioSource()
    {
        _AudioScource.GetSpectrumData(Samples, 0, FFTWindow.Blackman);
    }

    void UpdateBandBuffer()
    {
        for (int i = 0; i < 8; i++)
        {
            if (FreqBand[i] > BandBuffer[i])
            {
                BandBuffer[i] = FreqBand[i];
                BufferDecrease[i] = 0.005f;
            }
            if (FreqBand[i] < BandBuffer[i])
            {
                BandBuffer[i] -= BufferDecrease[i];
                BufferDecrease[i] *= 1.2f;
            }
        }
    }

    void MakeFrequencyBands()
    {
        float average = 0;
        int count = 0;

        for (int i = 0; i < 8; i++)
        {
            int sampleCount = (int)Mathf.Pow(2, i) * 2;

            if (i == 7)
                sampleCount += 2;

            for (int j = 0; j < sampleCount; j++)
            {
                average += Samples[count] * (count + 1);
                count++;
            }

            average /= count;
            FreqBand[i] = average * 10;
        }
    }
}