using System.Collections.Generic;
using UnityEngine;

public class MediaLibrary : MonoBehaviour
{
    private static readonly Dictionary<string, Sprite> spriteCollection = new();
    
    public static Sprite GetSpriteIfExists(string url)
    {
        if (url == null)
            return null;

        if (spriteCollection.TryGetValue(url, out Sprite sprite))
            return sprite;

        return null;
    }

    public static void AddSprite(string url, Sprite sprite)
    {
        if (sprite != null)
        {
            if (!spriteCollection.ContainsKey(url))
                spriteCollection.Add(url, sprite);
        }
    }

}
