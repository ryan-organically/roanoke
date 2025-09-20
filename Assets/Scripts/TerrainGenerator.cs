using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [Header("Map Generation")]
    public int mapWidth = 241;
    public int mapHeight = 241;
    public float noiseScale = 50f;
    public int octaves = 4;
    [Range(0, 1)]
    public float persistance = 0.5f;
    public float lacunarity = 2f;
    public int seed = 42;
    public Vector2 offset;

    [Header("Mesh Generation")]
    public float meshHeightMultiplier = 20f;
    public AnimationCurve meshHeightCurve;
    public int levelOfDetail = 0;

    [Header("Display")]
    public bool autoUpdate = true;
    public DrawMode drawMode = DrawMode.NoiseMap;

    public enum DrawMode
    {
        NoiseMap,
        Mesh
    }

void Start()
    {
        // Initialize height curve if it has no keys
        if (meshHeightCurve == null || meshHeightCurve.keys.Length == 0)
        {
            meshHeightCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        }
        GenerateTerrain();
    }

public void GenerateTerrain()
    {
        // Generate coastline-specific height map
        float[,] noiseMap = CoastlineGenerator.GenerateCoastlineMap(mapWidth, mapHeight, seed);

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (display == null)
        {
            // Create a MapDisplay if one doesn't exist
            GameObject displayObject = new GameObject("Map Display");
            display = displayObject.AddComponent<MapDisplay>();
        }

        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail));
        }
    }

    void OnValidate()
    {
        if (mapWidth < 1)
        {
            mapWidth = 1;
        }
        if (mapHeight < 1)
        {
            mapHeight = 1;
        }
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves < 0)
        {
            octaves = 0;
        }

        if (levelOfDetail < 0)
        {
            levelOfDetail = 0;
        }
    }
}