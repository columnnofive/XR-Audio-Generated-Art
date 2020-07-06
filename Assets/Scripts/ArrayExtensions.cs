using System;
using System.Collections;
using System.Collections.Generic;

public static class ArrayExtensions
{
    public static bool contains<T>(this T[] array, T target)
    {
        return Array.Find(array, val => val.Equals(target)).Equals(target);
    }

    public static void setValues<T>(this T[] array, T value)
    {
        for (int i = 0; i < array.Length; i++)
            array[i] = value;
    }
}
