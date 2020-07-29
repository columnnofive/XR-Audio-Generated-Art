using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Texture2DExtensions
{
    /// <summary>
    /// Returns array of pixels colors in format, Color[height][width].
    /// </summary>
    public static Color[][] GetFormattedPixels(this Texture2D tex)
    {
        Color[][] pixels = new Color[tex.height][];

        Color[] unformattedPixels = tex.GetPixels();

        int pixelIndex = 0;
        for (int i = 0; i < tex.height; i++)
        {
            pixels[i] = new Color[tex.width];

            for (int j = 0; j < tex.width; j++, pixelIndex++)
            {
                pixels[i][j] = unformattedPixels[pixelIndex];
            }
        }

        return pixels;
    }
}
