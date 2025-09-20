using UnityEngine;

public static class TextureGenerator
{
    /// <summary>
    /// Creates a texture from a height map
    /// </summary>
    /// <param name="heightMap">2D array of height values between 0 and 1</param>
    /// <returns>Texture2D representing the height map</returns>
    public static Texture2D TextureFromHeightMap(float[,] heightMap)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        Color[] colorMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
            }
        }

        return TextureFromColorMap(colorMap, width, height);
    }

    /// <summary>
    /// Creates a texture from a color map
    /// </summary>
    /// <param name="colorMap">Array of colors</param>
    /// <param name="width">Width of the texture</param>
    /// <param name="height">Height of the texture</param>
    /// <returns>Texture2D with the specified colors</returns>
    public static Texture2D TextureFromColorMap(Color[] colorMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colorMap);
        texture.Apply();
        return texture;
    }
}