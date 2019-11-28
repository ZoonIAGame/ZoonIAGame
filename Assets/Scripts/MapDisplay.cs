using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRenderer;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public Renderer watertextureRenderer;
    public MeshFilter watermeshFilter;
    public MeshRenderer watermeshRenderer;

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
    }

    public void waterDrawMesh(MeshData meshData, Texture2D texture)
    {
        watermeshFilter.sharedMesh = meshData.CreateMesh();
        watermeshRenderer.sharedMaterial.mainTexture = texture;
    }
}
