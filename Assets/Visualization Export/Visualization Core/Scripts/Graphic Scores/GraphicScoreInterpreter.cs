using UnityEngine;

public static class GraphicScoreInterpreter
{
    /// <summary>
    /// Value at which a grayscale color will be considered white.
    /// </summary>
    private readonly static float whiteGrayscaleThreshold = .9f;

    public static float[] getDisplacementValues(Texture2D tex)
    {
        float[] displacementValues = new float[tex.width];

        Color[][] pixels = tex.GetFormattedPixels();
        
        for (int i = 0; i < tex.width; i++)
        {
            int whitePixels = 0;
            
            for (int j = 0; j < tex.height; j++)
            {
                float pixelGrayscale = pixels[j][i].grayscale;

                //Buffer white values in case of discoloration
                if (pixelGrayscale >= whiteGrayscaleThreshold)
                    whitePixels++;
            }

            displacementValues[i] = 1f - ((float)whitePixels / tex.height);
        }

        return displacementValues;
    }
}
