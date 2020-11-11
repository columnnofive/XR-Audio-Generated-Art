using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioDataSourceController : MonoBehaviour
{
    [SerializeField, HideInInspector]
    private AudioSource audioSource;

    public enum AudioDataSource { Clip, Microphone }

    [SerializeField, Tooltip("Determines where the audio data is coming from.")]
    private AudioDataSource audioDataSource = AudioDataSource.Clip;

    #region Microphone Audio Settings

    public bool useDefaultMicrophone = false;

    [System.Serializable]
    public class MicrophoneDevice
    {
        public string name = "";
        public int selectedIndex = 0;
    }
    
    public MicrophoneDevice micDevice = new MicrophoneDevice();

    /// <summary>
    /// Length in seconds of the microphone audio clip.
    /// </summary>
    private int microphoneAudioClipLength = 10;

    #endregion Microphone Audio Settings

    private void OnValidate()
    {
        if (!audioSource)
        {
            audioSource = GetComponent<AudioSource>();
        }
        
        if (audioSource.playOnAwake)
            audioSource.playOnAwake = false;
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        initializeDataSource();
    }
    
    private void initializeDataSource()
    {
        if (audioDataSource == AudioDataSource.Microphone && Microphone.devices.Length > 0)
        {
            string micDeviceName = "";

            if (useDefaultMicrophone)
            {
                if (Microphone.devices.Length > 0)
                    micDeviceName = Microphone.devices[0].ToString();
                else
                    Debug.LogError("Microphone not found.");
            }
            else
                micDeviceName = micDevice.name;

            if (micDeviceName == "")
            {
                Debug.LogError("Microphone name empty, could not start microphone.");
                return;
            }

            audioSource.clip = Microphone.Start(micDeviceName, true, 10, AudioSettings.outputSampleRate);
        }

        audioSource.Play();
    }
}
