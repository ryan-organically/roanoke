using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class SimpleCoastlineTerrain : MonoBehaviour
{
    private Terrain terrain;
    private TerrainData terrainData;
    
    [Header("Terrain Settings")]
    public int terrainSize = 500;
    public int heightmapResolution = 513;
    public float terrainHeight = 30f;
    
    [Header("Coastline Settings")]
    public float coastlinePosition = 0.3f; // Where the coast is (0-1 across terrain)
    public float beachWidth = 0.1f;
    public float noiseScale = 0.03f;
    
    void OnEnable()
    {
        CreateOrUpdateTerrain();
    }
    
    [ContextMenu("Generate Coastline")]
    public void CreateOrUpdateTerrain()
    {
        // Find or create terrain component
        terrain = GetComponent<Terrain>();
        if (terrain == null)
        {
            terrain = gameObject.AddComponent<Terrain>();
        }
        
        // Find or create terrain collider
        TerrainCollider collider = GetComponent<TerrainCollider>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<TerrainCollider>();
        }
        
        // Create terrain data asset
        if (terrain.terrainData == null)
        {
            terrainData = new TerrainData();
            terrain.terrainData = terrainData;
            collider.terrainData = terrainData;
            
            #if UNITY_EDITOR
            // Save the terrain data as an asset
            string path = "Assets/TerrainData_" + gameObject.name + ".asset";
            AssetDatabase.CreateAsset(terrainData, path);
            AssetDatabase.SaveAssets();
            #endif
        }
        else
        {
            terrainData = terrain.terrainData;
        }
        
        // Configure terrain data
        terrainData.heightmapResolution = heightmapResolution;
        terrainData.size = new Vector3(terrainSize, terrainHeight, terrainSize);
        
        // Generate heightmap
        GenerateCoastlineHeightmap();
        
        // Apply basic textures
        ApplyTextures();
        
        Debug.Log("Coastline terrain generated!");
    }
    
    void GenerateCoastlineHeightmap()
    {
        int width = terrainData.heightmapResolution;
        int height = terrainData.heightmapResolution;
        float[,] heights = new float[width, height];
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xCoord = (float)x / width;
                float yCoord = (float)y / height;
                
                // Create base gradient for coastline
                float distanceFromCoast = xCoord - coastlinePosition;
                
                // Add Perlin noise for natural variation
                float noise = Mathf.PerlinNoise(xCoord * noiseScale * 100f, yCoord * noiseScale * 100f) * 0.3f;
                noise += Mathf.PerlinNoise(xCoord * noiseScale * 50f + 100f, yCoord * noiseScale * 50f) * 0.2f;
                noise += Mathf.PerlinNoise(xCoord * noiseScale * 25f + 200f, yCoord * noiseScale * 25f) * 0.1f;
                
                // Combine gradient and noise
                float heightValue = 0f;
                
                if (distanceFromCoast < -beachWidth)
                {
                    // Water area - keep it low
                    heightValue = 0.1f + noise * 0.1f;
                }
                else if (distanceFromCoast < 0)
                {
                    // Beach area - gentle slope
                    float t = (distanceFromCoast + beachWidth) / beachWidth;
                    heightValue = Mathf.Lerp(0.1f, 0.3f, t) + noise * 0.2f;
                }
                else
                {
                    // Land area - higher elevation with more variation
                    heightValue = 0.3f + distanceFromCoast * 0.5f + noise * 0.4f;
                }
                
                // Clamp height value
                heights[x, y] = Mathf.Clamp01(heightValue);
            }
        }
        
        // Apply heights to terrain
        terrainData.SetHeights(0, 0, heights);
    }
    
    void ApplyTextures()
    {
        // Create simple colored textures if layers don't exist
        if (terrainData.terrainLayers == null || terrainData.terrainLayers.Length == 0)
        {
            TerrainLayer[] layers = new TerrainLayer[3];
            
            // Sand layer
            layers[0] = new TerrainLayer();
            layers[0].diffuseTexture = CreateColorTexture(new Color(0.9f, 0.8f, 0.6f));
            layers[0].tileSize = new Vector2(15, 15);
            
            // Grass layer
            layers[1] = new TerrainLayer();
            layers[1].diffuseTexture = CreateColorTexture(new Color(0.4f, 0.6f, 0.3f));
            layers[1].tileSize = new Vector2(15, 15);
            
            // Rock layer
            layers[2] = new TerrainLayer();
            layers[2].diffuseTexture = CreateColorTexture(new Color(0.6f, 0.6f, 0.6f));
            layers[2].tileSize = new Vector2(15, 15);
            
            terrainData.terrainLayers = layers;
            
            // Paint the terrain
            PaintTerrain();
        }
    }
    
    void PaintTerrain()
    {
        int alphamapWidth = terrainData.alphamapWidth;
        int alphamapHeight = terrainData.alphamapHeight;
        float[,,] alphamaps = new float[alphamapWidth, alphamapHeight, 3];
        
        for (int x = 0; x < alphamapWidth; x++)
        {
            for (int y = 0; y < alphamapHeight; y++)
            {
                float xNorm = (float)x / alphamapWidth;
                float yNorm = (float)y / alphamapHeight;
                
                // Sample height
                float height = terrainData.GetInterpolatedHeight(xNorm, yNorm) / terrainData.size.y;
                
                // Determine texture based on height
                if (height < 0.3f)
                {
                    // Sand
                    alphamaps[x, y, 0] = 1f;
                }
                else if (height < 0.5f)
                {
                    // Blend sand to grass
                    float t = (height - 0.3f) / 0.2f;
                    alphamaps[x, y, 0] = 1f - t;
                    alphamaps[x, y, 1] = t;
                }
                else if (height < 0.7f)
                {
                    // Grass
                    alphamaps[x, y, 1] = 1f;
                }
                else
                {
                    // Blend grass to rock
                    float t = (height - 0.7f) / 0.3f;
                    alphamaps[x, y, 1] = 1f - t;
                    alphamaps[x, y, 2] = t;
                }
            }
        }
        
        terrainData.SetAlphamaps(0, 0, alphamaps);
    }
    
    Texture2D CreateColorTexture(Color color)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        return texture;
    }
}