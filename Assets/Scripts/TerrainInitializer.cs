using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class TerrainInitializer : MonoBehaviour
{
    void OnEnable()
    {
        var terrain = GetComponent<Terrain>();
        var generator = GetComponent<CoastlineTerrainGenerator>();
        
        if (terrain != null && terrain.terrainData == null)
        {
            // Create terrain data
            TerrainData terrainData = new TerrainData();
            terrainData.heightmapResolution = 513;
            terrainData.size = new Vector3(500, 30, 500);
            terrain.terrainData = terrainData;
            
            // Also set it on the collider
            var collider = GetComponent<TerrainCollider>();
            if (collider != null)
                collider.terrainData = terrainData;
            
            Debug.Log("Created TerrainData for Coastline Terrain");
            
            // Trigger generation
            if (generator != null)
            {
                generator.GenerateCoastlineTerrain();
            }
        }
    }
}