using System.IO;
using UnityEditor;
using UnityEngine;

public class AmbisonicAudioCreation : EditorWindow
{
    private AudioClip w;
    private AudioClip x;
    private AudioClip y;
    private AudioClip z;

    public enum AmbisonicFormat
    {
        AmbiX,
        FuMa
    }

    private AmbisonicFormat format = AmbisonicFormat.AmbiX;

    private string filePath = "";
    private string fileName = "";

    [MenuItem("Window/Ambisonic Audio Creation")]
    public static void ShowWindow()
    {
        GetWindow<AmbisonicAudioCreation>("Ambisonic Audio Creation");
    }

    private void OnGUI()
    {
        GUILayout.Label("Generates a B format ambisonic file in AmbiX format. The output file will have four channels, " +
            "ordered WYZX.");

        w = (AudioClip)EditorGUILayout.ObjectField("W", w, typeof(AudioClip), true);
        x = (AudioClip)EditorGUILayout.ObjectField("X", x, typeof(AudioClip), true);
        y = (AudioClip)EditorGUILayout.ObjectField("Y", y, typeof(AudioClip), true);
        z = (AudioClip)EditorGUILayout.ObjectField("Z", z, typeof(AudioClip), true);

        //format = (AmbisonicFormat)EditorGUILayout.EnumPopup(format);

        GUILayout.Label("Generated Audio File", EditorStyles.boldLabel);

        filePath = EditorGUILayout.TextField("File Path", filePath);
        fileName = EditorGUILayout.TextField("File Name", fileName);

        if (GUILayout.Button("Generate"))
            generateAmbisonicFile();
    }

    private void generateAmbisonicFile()
    {
        AudioClip[] channels = new AudioClip[4];

        if (format == AmbisonicFormat.AmbiX)
        {
            channels[0] = w;
            channels[1] = y;
            channels[2] = z;
            channels[3] = x;
        }
        else if (format == AmbisonicFormat.FuMa)
        {
            channels[0] = w;
            channels[1] = x;
            channels[2] = y;
            channels[3] = z;
        }

        AudioClip generatedClip = createAmbisonicAudioClip(channels);
        saveAudioClip(generatedClip);
    }

    private AudioClip createAmbisonicAudioClip(AudioClip[] channelClips)
    {
        int samples = channelClips[0].samples;
        int frequency = channelClips[0].frequency;
        int channels = channelClips.Length;

        float[][] channelData = getChannelData(channelClips, channels, samples);

        AudioClip clip = AudioClip.Create(fileName, samples * channels, channels, frequency, false);

        int dataLength = samples * channels;
        float[] clipData = new float[dataLength];

        int i = 0;
        while (i < clipData.Length)
        {
            for (int j = 0; j < samples; j++)
            {
                for (int k = 0; k < channels; k++, i++)
                {
                    //Get channel clip data at specific sample
                    clipData[i] = channelData[k][j];
                }
            }
        }

        //Set clip data
        clip.SetData(clipData, 0);

        return clip;
    }

    private float[][] getChannelData(AudioClip[] channelClips, int channels, int samples)
    {
        float[][] channelData = new float[channels][];

        for (int i = 0; i < channels; i++)
        {
            channelData[i] = new float[samples];
            channelClips[i].GetData(channelData[i], 0);
        }

        return channelData;
    }

    private void saveAudioClip(AudioClip clip)
    {
        string fullFilePath = Path.Combine(Application.dataPath, filePath);

        if (SavWav.Save(fileName, fullFilePath, clip))
        {
            Debug.Log("Saved audio");
            AssetDatabase.Refresh();
        }
        else
            Debug.Log("Failed to save audio");
    }
}
