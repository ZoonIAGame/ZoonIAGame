using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SitioPlantacion
{
    public enum plantationType
    {
        none, tree, bush
    }
    
    public Vector3 pos;
    public plantationType plantacion;
    public GameObject model;

    public SitioPlantacion(Vector3 p, plantationType tipo)
    {
        pos = p;
        plantacion = tipo;
    }
}
