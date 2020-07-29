using UnityEngine;

public static class GraphicScoreInterpreter
{
    private readonly static float whiteGrayscale = 1f;

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

                if (pixelGrayscale == whiteGrayscale)
                    whitePixels++;
            }

            displacementValues[i] = 1f - ((float)whitePixels / tex.height);
        }

        return displacementValues;
    }
}
