using UnityEngine;

public class CoastlineTerrainGenerator : MonoBehaviour
{
    [Header("Terrain Settings")]
    public Terrain terrain;
    public int terrainResolution = 513; // Higher resolution for smoother coastline
    public float terrainWidth = 500f;
    public float terrainLength = 500f;
    public float terrainHeight = 30f;
    
    [Header("Coastline Settings")]
    [Range(0, 1)]
    public float waterLevel = 0.3f; // Where the water meets land
    public float coastlineSmoothing = 5f;
    public float noiseScale = 0.02f;
    public float beachWidth = 20f;
    
    [Header("Terrain Layers")]
    public TerrainLayer sandLayer;
    public TerrainLayer grassLayer;
    public TerrainLayer rockLayer;
    
    void Start()
    {
        if (terrain == null)
            terrain = GetComponent<Terrain>();
            
        // Ensure we have a TerrainData
        if (terrain != null && terrain.terrainData == null)
        {
            terrain.terrainData = new TerrainData();
            terrain.terrainData.name = "CoastlineTerrainData";
        }
            
        if (terrain != null)
            GenerateCoastlineTerrain();
    }
    
    public void GenerateCoastlineTerrain()
    {
        if (terrain == null)
        {
            Debug.LogError("Terrain is null! Cannot generate coastline.");
            return;
        }
        
        // Get or create terrain data
        TerrainData terrainData = terrain.terrainData;
        if (terrainData == null)
        {
            terrainData = new TerrainData();
            terrain.terrainData = terrainData;
        }
        
        // Set terrain data dimensions
        terrainData.heightmapResolution = terrainResolution;
        terrainData.size = new Vector3(terrainWidth, terrainHeight, terrainLength);
        
        // Generate heightmap
        float[,] heights = new float[terrainResolution, terrainResolution];
        
        for (int x = 0; x < terrainResolution; x++)
        {
            for (int z = 0; z < terrainResolution; z++)
            {
                float xNorm = (float)x / terrainResolution;
                float zNorm = (float)z / terrainResolution;
                
                // Create base coastline shape
                float coastlineShape = Mathf.Sin(xNorm * Mathf.PI * 2f) * 0.1f + 
                                       Mathf.Sin(zNorm * Mathf.PI * 3f) * 0.05f;
                
                // Add Perlin noise for natural variation
                float noise1 = Mathf.PerlinNoise(xNorm * noiseScale * 100f, zNorm * noiseScale * 100f);
                float noise2 = Mathf.PerlinNoise(xNorm * noiseScale * 50f + 100f, zNorm * noiseScale * 50f + 100f) * 0.5f;
                float noise3 = Mathf.PerlinNoise(xNorm * noiseScale * 25f + 200f, zNorm * noiseScale * 25f + 200f) * 0.25f;
                
                float combinedNoise = (noise1 + noise2 + noise3) / 1.75f;
                
                // Create gradient from water to land
                float gradient = xNorm - 0.3f; // Coastline roughly at x=0.3
                gradient += coastlineShape;
                
                // Smooth transition at coastline
                float height = Mathf.Lerp(waterLevel * 0.5f, combinedNoise, 
                    Mathf.SmoothStep(0, 1, gradient * coastlineSmoothing));
                
                // Add beach elevation
                if (gradient > 0 && gradient < beachWidth / terrainWidth)
                {
                    height = Mathf.Lerp(waterLevel, height, gradient * terrainWidth / beachWidth);
                }
                
                heights[x, z] = height;
            }
        }
        
        // Apply heightmap to terrain
        terrainData.SetHeights(0, 0, heights);
        
        // Create and apply terrain layers if they don't exist
        if (sandLayer == null)
        {
            sandLayer = new TerrainLayer();
            sandLayer.diffuseTexture = CreateSolidColorTexture(new Color(0.9f, 0.8f, 0.6f)); // Sandy color
            sandLayer.tileSize = new Vector2(15, 15);
        }
        
        if (grassLayer == null)
        {
            grassLayer = new TerrainLayer();
            grassLayer.diffuseTexture = CreateSolidColorTexture(new Color(0.3f, 0.6f, 0.2f)); // Grass color
            grassLayer.tileSize = new Vector2(15, 15);
        }
        
        if (rockLayer == null)
        {
            rockLayer = new TerrainLayer();
            rockLayer.diffuseTexture = CreateSolidColorTexture(new Color(0.5f, 0.5f, 0.5f)); // Rock color
            rockLayer.tileSize = new Vector2(15, 15);
        }
        
        // Apply layers to terrain
        terrainData.terrainLayers = new TerrainLayer[] { sandLayer, grassLayer, rockLayer };
        
        // Paint the terrain based on height
        PaintTerrain(terrainData);
        
        Debug.Log("Coastline terrain generated successfully!");
    }
    
    void PaintTerrain(TerrainData terrainData)
    {
        float[,,] alphamaps = new float[terrainData.alphamapResolution, terrainData.alphamapResolution, 3];
        
        for (int x = 0; x < terrainData.alphamapResolution; x++)
        {
            for (int z = 0; z < terrainData.alphamapResolution; z++)
            {
                float xNorm = (float)x / terrainData.alphamapResolution;
                float zNorm = (float)z / terrainData.alphamapResolution;
                float height = terrainData.GetHeight(
                    Mathf.RoundToInt(xNorm * terrainData.heightmapResolution),
                    Mathf.RoundToInt(zNorm * terrainData.heightmapResolution)
                ) / terrainData.size.y;
                
                // Determine texture weights based on height
                float sandWeight = 0f;
                float grassWeight = 0f;
                float rockWeight = 0f;
                
                if (height < waterLevel + 0.05f)
                {
                    sandWeight = 1f; // Beach/sand near water
                }
                else if (height < waterLevel + 0.15f)
                {
                    // Transition from sand to grass
                    float t = (height - (waterLevel + 0.05f)) / 0.1f;
                    sandWeight = 1f - t;
                    grassWeight = t;
                }
                else if (height < 0.7f)
                {
                    grassWeight = 1f; // Grass in middle elevations
                }
                else
                {
                    // Transition to rock at high elevations
                    float t = (height - 0.7f) / 0.3f;
                    grassWeight = 1f - t;
                    rockWeight = t;
                }
                
                alphamaps[x, z, 0] = sandWeight;
                alphamaps[x, z, 1] = grassWeight;
                alphamaps[x, z, 2] = rockWeight;
            }
        }
        
        terrainData.SetAlphamaps(0, 0, alphamaps);
    }
    
    Texture2D CreateSolidColorTexture(Color color)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        return texture;
    }
    
    [ContextMenu("Regenerate Terrain")]
    public void RegenerateTerrain()
    {
        if (terrain != null)
            GenerateCoastlineTerrain();
        else
            Debug.LogError("Terrain reference is missing!");
    }
}