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
            //micDevice = Microphone.devices[0].ToString();
            audioSource.clip = Microphone.Start(micDevice.name, true, 10, AudioSettings.outputSampleRate);
        }

        audioSource.Play();
    }
}
