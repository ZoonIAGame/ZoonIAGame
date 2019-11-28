using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBushGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    [Tooltip("Prefab de los arboles")]
    public GameObject treePrefab;
    [Tooltip("Prefab de los arbustos")]
    public GameObject bushPrefab;
    [Header("Ajustes")]
    [Space(5)]
    [Range(0, 1)]
    [Tooltip("Probabilidad de que aparezca un arbol en un vertice para decorar")]
    public float probabilityOfTrees;
    [Range(0, 1)]
    [Tooltip("Probabilidad de que aparezca un arbusto en un vertice si no hay arbol")]
    public float probabilityOfBushes;
    [Header("MeshPadre")]
    [Space(5)]
    [Tooltip("Mesh padre donde se van a generar los arboles y arbustos")]
    public GameObject meshPadre;

    private enum plantationType
    {
        none, tree, bush
    }

    private struct SitioPlantacion
    {
        Vector3 pos;
        plantationType plantacion;
        
    }

    private List<SitioPlantacion> sitiosPlantacion = new List<SitioPlantacion>();

    public void generarPlantaciones(MeshData meshData)
    {

    }

    public void generarArbolesYArbustos()
    {
        foreach (SitioPlantacion plantsite in sitiosPlantacion)
        {

        }
    }

    public void generarArbustos()
    {

    }
}
