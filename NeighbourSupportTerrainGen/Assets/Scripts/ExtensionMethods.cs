using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    /// <summary>
    /// Returns a randomly chosen element from a list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static T PickRandom<T>(this List<T> list)
    {
        if (list.Count == 0) return default(T);
        int randomIndex = UnityEngine.Random.Range(0, list.Count);
        return list[randomIndex];
    }

    /// <summary>
    /// Returns a randomly chosen element from an array.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <returns></returns>
    public static T PickRandom<T>(this T[] array)
    {
        if (array.Length == 0) return default(T);
        int randomIndex = UnityEngine.Random.Range(0, array.Length);
        return array[randomIndex];
    }

    /// <summary>
    /// The distance from the origin on a hex grid of these coordinates.
    /// </summary>
    /// <param name="coords"></param>
    /// <returns></returns>
    public static int HexDistFromOrigin(this Vector2Int coords)
    {
        return Mathf.Max(Mathf.Abs(coords.x), Mathf.Abs(coords.y), Mathf.Abs(coords.x + coords.y));
    }
}
