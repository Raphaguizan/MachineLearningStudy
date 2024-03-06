using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static void Print<T>(this List<T> myLIst, string listName = "myList")
    {
        Debug.Log(listName+":----------------------\n");
        foreach (var item in myLIst)
        {
            Debug.Log(item.ToString()+"\n");
        }
    }

    public static void Print<T>(this T[] myLIst, string listName = "myList")
    {
        Debug.Log(listName + ":----------------------\n");
        foreach (var item in myLIst)
        {
            Debug.Log(item.ToString() + "\n");
        }
    }

    public static float Map(this float value, float newfrom, float newto, float origfrom, float origto)
    {
        if (value <= origfrom)
            return newfrom;
        else if (value >= origto)
            return newto;
        return (newto - newfrom) * ((value - origfrom) / (origto - origfrom)) + newfrom;
    }

    public static float Round(this float x)
    {
        return (float)System.Math.Round(x, System.MidpointRounding.AwayFromZero) / 2.0f;
    }

    public static string ToTime(this float t, string format = "dd':'hh':'mm':'ss")
    {
        TimeSpan time = TimeSpan.FromSeconds(t);
        return time.ToString(format);
    }
}
