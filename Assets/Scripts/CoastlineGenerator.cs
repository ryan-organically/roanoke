using UnityEngine;

public static class CoastlineGenerator
{
    /// <summary>
    /// Generates a North Carolina-style coastline with barrier islands and sounds
    /// </summary>
    /// <param name="width">Map width</param>
    /// <param name="height">Map height</param>
    /// <param name="seed">Random seed</param>
    /// <returns>Height map with coastline features</returns>
    public static float[,] GenerateCoastlineMap(int width, int height, int seed)
    {
        float[,] heightMap = new float[width, height];
        System.Random prng = new System.Random(seed);
        
        // Create base coastline running north-south along the eastern edge
        float coastlinePosition = width * 0.75f; // Coastline at 75% across the map (eastern side)
        float coastlineVariation = width * 0.15f; // How much the coastline can vary
        
        for (int y = 0; y < height; y++)
        {
            // Create meandering coastline with Perlin noise
            float noiseValue = Mathf.PerlinNoise(y * 0.01f, seed * 0.001f);
            float currentCoastline = coastlinePosition + (noiseValue - 0.5f) * coastlineVariation;
            
            // Add barrier islands - small islands off the coast
            float barrierIslandNoise = Mathf.PerlinNoise(y * 0.005f, (seed + 100) * 0.001f);
            float barrierDistance = width * 0.1f; // Distance from main coast
            float barrierPosition = currentCoastline + barrierDistance + (barrierIslandNoise - 0.5f) * 20f;
            
            for (int x = 0; x < width; x++)
            {
                float distanceFromCoast = x - currentCoastline;
                float distanceFromBarrier = x - barrierPosition;
                
                // Mainland (west of coastline)
                if (x < currentCoastline)
                {
                    // Gradual elevation increase inland
                    float mainlandHeight = (currentCoastline - x) / coastlinePosition;
                    mainlandHeight = Mathf.Pow(mainlandHeight, 0.5f); // Gentler curve
                    
                    // Add some rolling hills inland
                    float hillNoise = Mathf.PerlinNoise(x * 0.02f, y * 0.02f) * 0.3f;
                    heightMap[x, y] = mainlandHeight * 0.7f + hillNoise;
                }
                // Barrier islands
                else if (Mathf.Abs(distanceFromBarrier) < 15f && barrierIslandNoise > 0.3f)
                {
                    // Create barrier island profile
                    float islandHeight = (15f - Mathf.Abs(distanceFromBarrier)) / 15f;
                    islandHeight = Mathf.Pow(islandHeight, 2f); // Sharp dropoff
                    heightMap[x, y] = islandHeight * 0.4f; // Lower than mainland
                }
                // Sounds (water between mainland and barriers)
                else if (x > currentCoastline && x < barrierPosition)
                {
                    // Shallow sound water with some variation
                    float soundDepth = Mathf.PerlinNoise(x * 0.05f, y * 0.05f) * 0.1f;
                    heightMap[x, y] = -0.1f + soundDepth; // Slightly below sea level
                }
                // Open ocean (east of barriers)
                else
                {
                    // Deep ocean with gentle waves
                    float oceanDepth = Mathf.PerlinNoise(x * 0.03f, y * 0.03f) * 0.2f;
                    heightMap[x, y] = -0.3f + oceanDepth; // Ocean depth
                }
            }
        }
        
        return heightMap;
    }
}