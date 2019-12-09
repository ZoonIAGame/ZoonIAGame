using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRenderer;

    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public MeshCollider meshCollider;

    public Renderer watertextureRenderer;

    public MeshFilter watermeshFilter;
    public MeshRenderer watermeshRenderer;
    public MeshCollider waterMeshCollider;

    public void DrawTexture(Texture2D texture)
    {
        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    public void DrawWaterTexture(Texture2D texture)
    {
        watertextureRenderer.sharedMaterial.mainTexture = texture;
        watertextureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    public void DrawMesh(MeshData meshData, Texture2D texture)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;
        if (meshRenderer.gameObject.GetComponent<MeshCollider>() == null)
        {
            meshCollider = meshRenderer.gameObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = meshData.CreateMesh();
        }
    }

    public void waterDrawMesh(MeshData meshData, Texture2D texture)
    {
        watermeshFilter.sharedMesh = meshData.CreateMesh();
        watermeshRenderer.sharedMaterial.mainTexture = texture;
        if (watermeshRenderer.gameObject.GetComponent<MeshCollider>() == null)
        {
            waterMeshCollider = watermeshRenderer.gameObject.AddComponent<MeshCollider>();
            waterMeshCollider.sharedMesh = meshData.CreateMesh();
        }
    }
}
