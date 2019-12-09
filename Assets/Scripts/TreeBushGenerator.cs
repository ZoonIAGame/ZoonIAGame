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
    [Tooltip("Altura mínima para un platationSite")]
    public float alturaMin = 0.4f;
    [Tooltip("Altura máxima para un platationSite")]
    public float alturaMax = 2.73f;
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

    public List<SitioPlantacion> sitiosPlantacion = new List<SitioPlantacion>();

    public GameObject arbolitosGroup;

    public void Awake()
    {
        arbolitosGroup = GameObject.FindGameObjectWithTag("ArbolitosGroup");
    }

    public void generarPlantaciones(MeshData meshData)
    {
        arbolitosGroup = GameObject.FindGameObjectWithTag("ArbolitosGroup");
        foreach (SitioPlantacion plantsite in sitiosPlantacion)
        {
            Destroy(plantsite.model);
        }
        sitiosPlantacion.Clear();
        foreach (Vector3 v in meshData.vertices)
        {
            if (v.y > alturaMin && v.y < alturaMax)
            {
                sitiosPlantacion.Add(new SitioPlantacion(new Vector3(v.x* 10, v.y* 10, v.z*10), SitioPlantacion.plantationType.none));
            }
        }
    }

    public void destruirPlantaciones()
    {
        foreach (SitioPlantacion plantsite in sitiosPlantacion)
        {
            Destroy(plantsite.model);
        }
        sitiosPlantacion.Clear();
    }

    public void generarArbolesYArbustos()
    {
        foreach (SitioPlantacion plantsite in sitiosPlantacion)
        {
            float random = Random.Range(0f,1f);
            if(random <= probabilityOfTrees)
            {
                plantsite.plantacion = SitioPlantacion.plantationType.tree;
                plantsite.model = Instantiate(treePrefab, plantsite.pos, Quaternion.identity);
                plantsite.model.transform.SetParent(arbolitosGroup.transform);
            }
            else if (random <= probabilityOfBushes)
            {
                plantsite.plantacion = SitioPlantacion.plantationType.tree;
                plantsite.model = Instantiate(bushPrefab, plantsite.pos, Quaternion.identity);
                plantsite.model.transform.SetParent(arbolitosGroup.transform);
            }
        }
    }

    public void generarArbustos()
    {
        foreach (SitioPlantacion plantsite in sitiosPlantacion)
        {
            float random = Random.Range(0f, 1f);
            if (random <= probabilityOfBushes)
            {
                plantsite.plantacion = SitioPlantacion.plantationType.tree;
                plantsite.model = Instantiate(bushPrefab, plantsite.pos, Quaternion.identity);
                plantsite.model.transform.SetParent(arbolitosGroup.transform);
            }
        }
    }
}
