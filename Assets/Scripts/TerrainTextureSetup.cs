using UnityEngine;
using UnityEditor;

public class TerrainTextureSetup : MonoBehaviour
{
    [ContextMenu("Setup Terrain Textures")]
    public void SetupTerrainTextures()
    {
        Terrain terrain = GetComponent<Terrain>();
        if (terrain == null || terrain.terrainData == null)
        {
            Debug.LogError("No terrain or terrain data found!");
            return;
        }
        
        // Create default textures if they don't exist
        Texture2D sandTexture = CreateTexture(new Color(0.9f, 0.8f, 0.6f), "Sand");
        Texture2D grassTexture = CreateTexture(new Color(0.3f, 0.6f, 0.2f), "Grass");
        Texture2D rockTexture = CreateTexture(new Color(0.5f, 0.5f, 0.5f), "Rock");
        
        // Create terrain layers
        TerrainLayer[] terrainLayers = new TerrainLayer[3];
        
        terrainLayers[0] = new TerrainLayer();
        terrainLayers[0].diffuseTexture = sandTexture;
        terrainLayers[0].tileSize = new Vector2(15, 15);
        terrainLayers[0].name = "SandLayer";
        
        terrainLayers[1] = new TerrainLayer();
        terrainLayers[1].diffuseTexture = grassTexture;
        terrainLayers[1].tileSize = new Vector2(15, 15);
        terrainLayers[1].name = "GrassLayer";
        
        terrainLayers[2] = new TerrainLayer();
        terrainLayers[2].diffuseTexture = rockTexture;
        terrainLayers[2].tileSize = new Vector2(15, 15);
        terrainLayers[2].name = "RockLayer";
        
        // Apply to terrain
        terrain.terrainData.terrainLayers = terrainLayers;
        
        // Paint the terrain
        PaintTerrain(terrain.terrainData);
        
        Debug.Log("Terrain textures setup complete!");
    }
    
    Texture2D CreateTexture(Color color, string name)
    {
        Texture2D texture = new Texture2D(4, 4);
        Color[] pixels = new Color[16];
        
        // Add slight variation for more realistic look
        for (int i = 0; i < 16; i++)
        {
            float variation = Random.Range(-0.05f, 0.05f);
            pixels[i] = new Color(
                Mathf.Clamp01(color.r + variation),
                Mathf.Clamp01(color.g + variation),
                Mathf.Clamp01(color.b + variation),
                1f
            );
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        texture.name = name + "Texture";
        return texture;
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
                
                // Sample height at this position
                int heightX = Mathf.RoundToInt(xNorm * (terrainData.heightmapResolution - 1));
                int heightZ = Mathf.RoundToInt(zNorm * (terrainData.heightmapResolution - 1));
                float height = terrainData.GetHeight(heightX, heightZ) / terrainData.size.y;
                
                // Determine texture weights
                if (height < 0.35f)
                {
                    // Sand
                    alphamaps[x, z, 0] = 1f;
                    alphamaps[x, z, 1] = 0f;
                    alphamaps[x, z, 2] = 0f;
                }
                else if (height < 0.4f)
                {
                    // Blend sand to grass
                    float blend = (height - 0.35f) / 0.05f;
                    alphamaps[x, z, 0] = 1f - blend;
                    alphamaps[x, z, 1] = blend;
                    alphamaps[x, z, 2] = 0f;
                }
                else if (height < 0.7f)
                {
                    // Grass
                    alphamaps[x, z, 0] = 0f;
                    alphamaps[x, z, 1] = 1f;
                    alphamaps[x, z, 2] = 0f;
                }
                else if (height < 0.8f)
                {
                    // Blend grass to rock
                    float blend = (height - 0.7f) / 0.1f;
                    alphamaps[x, z, 0] = 0f;
                    alphamaps[x, z, 1] = 1f - blend;
                    alphamaps[x, z, 2] = blend;
                }
                else
                {
                    // Rock
                    alphamaps[x, z, 0] = 0f;
                    alphamaps[x, z, 1] = 0f;
                    alphamaps[x, z, 2] = 1f;
                }
            }
        }
        
        terrainData.SetAlphamaps(0, 0, alphamaps);
    }
    
    void Start()
    {
        // Auto-setup on start
        SetupTerrainTextures();
    }
}