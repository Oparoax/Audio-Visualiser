using UnityEngine;
using UnityEngine.Events;

public class AudioManipulator : MonoBehaviour
{
    //Bool for which data stream to use
    public bool useBuffer;

    [Header("Required Components")]
    //The shader we want manipulate
    public Material rippleMat;
    //The audio source we are reading
    public AudioSource audioSource;
    [Space]
    [Header("Amplitude Values")]
    //Bool for whether amplitude is affected
    public bool changeAmplitude;
    //The reading from the audio stream
    public float currentAmplitude;
    //The multiplier we apply onto the data stream onto the object
    public float ampScaleMultiplier;
    //The initial value for amplitude on startup
    public float startAmp;
    [Space]
    [Header("Frequency Values")]
    //Bool for whether frequency is affected
    public bool changeFreqency;
    //The reading from the audio stream
    public float currentFrequency;
    //The multiplier we apply onto the data stream onto the object
    public float freqScaleMultiplier;
    //The initial value for frequency on startup
    public float startFreq;

    [Header("Frequency Band")]
    [Tooltip("Please select a band 1-8 to represent")]
    //Band selector
    [Range(0, 7)] public int Band;
    //Chosen streams amplitude
    private float chosenStream;
    //Array placeholder to access the chosen data stream
    private float[] chosenArray;

    [Header("Colour Change settings")]
    //Bool for whether colour is changed
    public bool changeColour;
    //Float value that has to be exceeded to change its colour
    public float colourChangeThreshold;
    //Array of colours to change into
    public Color[] vibrantColours;
    //Bool to determine if the color has changed
    private bool hasChangedColour = false;
    //New colour to apply to the shader
    public static Color newColour = new Color(0,0,0,255);

    public static UnityEvent OnColourChange;

    /// <summary>
    /// Sets the Amplitude & Frequency to their starting values
    /// </summary>
    void Start()
    {
        //Sets starting frequency of shader
        rippleMat.SetFloat("_frequency", startFreq);
        //Sets starting amplitude of shader
        rippleMat.SetFloat("_amplitude", startAmp);
    }


    /// <summary>
    /// Applies transformation data/manipulation of the shader
    /// </summary>
    void Update()
    {
        //Checks to see if we use the buffered values or unbuffered values array
        if (useBuffer)
        {
            //Normal un buffered audio stream
            chosenStream = AudioReader.Amplitude;
            chosenArray = AudioReader.audioBand;
        }
        if (!useBuffer)
        {
            //Buffered audio stream
            chosenStream = AudioReader.AmplitudeBuffer;
            chosenArray = AudioReader.audioBandBuffer;
        }

        //Checks if frequency manipulation is required
        if (changeFreqency)
        {
            //Sets frequency of shader
            rippleMat.SetFloat("_frequency", chosenArray[Band]* freqScaleMultiplier);
        }
        //Checks if amplitude manipulation is required
        if (changeAmplitude)
        {
            //Sets amplitude of shader
            currentAmplitude = chosenStream * ampScaleMultiplier;
            rippleMat.SetFloat("_amplitude", startAmp * currentAmplitude);
        }

        //Checks frequency against colour change threshold and if it has laready changed this frame
        if (changeColour)
        {
            //Changes colour if the amplitude reaches a set threshold and if the method hasn't already been called that frame
            if (currentAmplitude >= colourChangeThreshold && !hasChangedColour) ChangeColor();
            else hasChangedColour = false;
        }
        

    }

    /// <summary>
    /// Changes shader colour when amplitude reaches a threshold
    /// </summary>
    private void ChangeColor()
    {
        //Generates random colour from array
        newColour = vibrantColours[Random.Range(0, vibrantColours.Length -1)];
        //Sets emission colour propertie on shader to new colour
        rippleMat.SetColor("_emission", newColour);
        //Sets boolean to true so it isn't repeated
        hasChangedColour = true;

        OnColourChange?.Invoke();
    }
}
