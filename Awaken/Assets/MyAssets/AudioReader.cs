using Assets.MyAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent (typeof (AudioSource))]
public class AudioReader : MonoBehaviour
{
    //Audio source to retrieve audio being played
    public AudioSource audioSource;

    //Float array to store all the 512 samples
    float[] samples = new float[512];
    //Float array to store all the 8 sample bands
    float[] freqBand = new float[8];
    //Float array to compare to, to buffer the value when the amplitude decreases
    float[] bandBuffer = new float[8];
    //Band buffer decrease values
    float[] bufferDecrease = new float[8];
    //Highest frequency in each band 
    float[] freqBandHighest = new float[8];
    //Float array to store more usable values for unbuffered frequency
    public static float[] audioBand = new float[8];
    //Float array to store more usable values for buffered frequency
    public static float[] audioBandBuffer = new float[8];

    //Floats to store the average amplitude of unbuffered and buffered audio
    public static float Amplitude, AmplitudeBuffer;
    //Highest Amplitude reached
    float AmplitudeHighest;
    //Boolean for update mesh
    public bool isReady;

    private void Start()
    {
        //Retrieves audio source component
        audioSource = GetComponent<AudioSource>();
        //Intialises array
        audioBands = new AudioBand[AudioFloor.requiredAudioHistory];
    }

    private void Update()
    {
        GetSpectrumAudioSource();
        MakeFrequencyBands();
        BandBuffer();
        CreateAudioBands();
        SetAudioHistory();
        GetAmplitude();

        //Tells mesh its ready to send values
        isReady = true;
    }

    /// <summary>
    /// Generates average amplitude of audio samples
    /// </summary>
    void GetAmplitude()
    {
        //Temp variables
        float currentAmplitude = 0;
        float currentAmplitudeBuffer = 0;
        //For loop through all samples
        for (int i = 0; i < 8; i++)
        {
            //Add all the samples together
            currentAmplitude += audioBand[i];
            currentAmplitudeBuffer += audioBandBuffer[i];
        }
        //Find the highest amplitude for average calculation
        if (currentAmplitude > AmplitudeHighest)
        {
            AmplitudeHighest = currentAmplitude;
        }
        //Perform the mean of each audio stream samples
        Amplitude = currentAmplitude / AmplitudeHighest;
        AmplitudeBuffer = currentAmplitudeBuffer / AmplitudeHighest;
    }

    /// <summary>
    /// Sets values of each band to be in usable range i.e between 0 & 1
    /// </summary>
    void CreateAudioBands()
    //Sets values of each band to be in usable range i.e between 0 & 1
    {
        //Loops through each band
        for (int i = 0; i < 8; i++)
        {
            //Checks each frequency to see if its the highest in its band
            if (freqBand[i] > freqBandHighest[i]) freqBandHighest[i] = freqBand[i];
            //Applies a division on the size of the frequency to make the value be in a usable range 
            audioBand[i] = freqBand[i] == 0 ? 0 : (freqBand[i] / freqBandHighest[i]);
            audioBandBuffer[i] = freqBand[i] == 0 ? 0 : (bandBuffer[i] / freqBandHighest[i]);
        }
    }

    /* This code was derived from a tutorial online, I have modified it to fit my needs
     * This is Part 5 of the tutorial which helped me create the buffer: https://www.youtube.com/watch?v=lEUuC3LQnzs
    */ 

    /// <summary>
    /// Breaks audio down into samples we can use
    /// </summary>
    void GetSpectrumAudioSource()
    {
        //Divides audio into 512 samples
        audioSource.GetSpectrumData(samples, 0, FFTWindow.Blackman);
    }

    /// <summary>
    /// Band buffer bands output more smooth and instead of snapping to the next value
    /// </summary>
    void BandBuffer()
    {
        //Loop through the sample bands
        for (int g = 0; g < 8; g++)
        {
            //if frequency increases apply decrease
            if (freqBand[g] > bandBuffer[g]) 
            {
                bandBuffer[g] = freqBand[g];
                bufferDecrease[g] = 0.005f;
            }
            //if frequency reduces apply decrease
            if (freqBand[g] < bandBuffer[g])
            {
                bandBuffer[g] -= bufferDecrease[g];
                bufferDecrease[g] *=  1.2f;
            }
        }
    }

    /// <summary>
    /// Takes the 512 samples and avgs them down into the typical 8 frequency bands
    /// </summary>
    void MakeFrequencyBands()
    {
        //Divides samples into the 8 bands

        int count = 0;

        for (int i = 0; i < 8; i++)
        {
            //Average amplitude per band
            float average = 0;
            //Sample bands are not even size and get exponentially bigger
            int sampleCount = (int)Mathf.Pow(2, i) * 2;
            
            if (i == 7)
            {
                //Completes full frequency range
                sampleCount += 2;
            }
           
            //Loop to generate avg of all frequencies in band
            for (int j = 0; j < sampleCount; j++)
            {
                average += samples[count] * (count + 1);
                    count++;
            }

            //Sample avg divided number of frequencies in the band
            average /= count;

            //Sample band avg increased as it will produce really small numbers
            freqBand[i] = average*10;
        }
    }

    /// <summary>
    /// Supplys an array with a set amount of previous frequencies across all 8 bands
    /// </summary>
    public void SetAudioHistory()
    {
        //Defines size of array
        audioBands = new AudioBand[AudioFloor.requiredAudioHistory];
        //First array in array
        for (int audioSet = 0; audioSet < AudioFloor.requiredAudioHistory; audioSet++)
        {
            //First item in array
            for (int band = 0; band < audioBand.Length; band++)
            {
                //Creates new audio band at audioSet in audiobands 
                audioBands[audioSet] = new AudioBand(audioBand);
            }
        }
    }

    //Intialises float array
    public AudioBand[] audioBands;

    /// <summary>
    /// Represents a singular audioband
    /// </summary>
    public class AudioBand
    {
        //Stored points within audio band
        public float[] frequencies;

        //Initialiser function for class
        public AudioBand(float[] frequencies)
        {
            this.frequencies = frequencies;
        }
    }
}
