using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRender;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    void Awake()
    {
        // Set up components if they don't exist
        if (textureRender == null)
        {
            // Create a plane for texture display
            GameObject textureObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            textureObject.name = "Texture Display";
            textureObject.transform.SetParent(transform);
            textureObject.transform.localPosition = Vector3.zero;
            textureObject.transform.localRotation = Quaternion.Euler(90, 0, 0);
            textureRender = textureObject.GetComponent<Renderer>();
        }

        if (meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }

        if (meshRenderer == null)
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
            // Set a default material
            meshRenderer.material = new Material(Shader.Find("Standard"));
        }
    }

    public void DrawTexture(Texture2D texture)
    {
        if (textureRender != null)
        {
            textureRender.sharedMaterial.mainTexture = texture;
            textureRender.transform.localScale = new Vector3(texture.width / 10f, 1, texture.height / 10f);
            
            // Hide mesh when showing texture
            if (meshRenderer != null)
                meshRenderer.enabled = false;
            
            textureRender.enabled = true;
        }
    }

    public void DrawMesh(MeshData meshData)
    {
        if (meshFilter != null && meshRenderer != null)
        {
            meshFilter.sharedMesh = meshData.CreateMesh();
            
            // Hide texture when showing mesh
            if (textureRender != null)
                textureRender.enabled = false;
            
            meshRenderer.enabled = true;
        }
    }
}