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
        for (int y = 0; y < tex.height; y++)
        {
            pixels[y] = new Color[tex.width];

            for (int x = 0; x < tex.width; x++, pixelIndex++)
            {
                pixels[y][x] = unformattedPixels[pixelIndex];
            }
        }

        return pixels;
    }
}
