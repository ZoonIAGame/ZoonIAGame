using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Variables de Generacion Inicial")]
    [Space(5)]
    [Min(1)]
    public int numActoresIniciales = 10;
    [Header("Variables Estandar")]
    [Min(0.001f)]
    [Tooltip("Esto edita el tamaño")]
    public float tamanoEstandar = 1f;
    [Min(0.001f)]
    public float velocidadEstandar = 5f;
    [Min(0.001f)]
    public float rangoSensEstandar = 5f;
    [Min(1)]
    public int tGestacionEstandar = 5;
    [Min(0)]
    public int nHijosEstandar = 3;
    [Min(0.001f)]
    public float fuerzaBaseEstandar = 1f;
    [Min(0.001f)]
    public float atractivoBaseEstandar = 1f;
    [Range(0.001f, 1f)]
    public float agroMismoSexEstandar = 0.25f;
    [Range(0.001f, 1f)]
    public float agroDistSexEstandar = 0.25f;
    [Range(0.001f, 1f)]
    public float agroOtrosEstandar = 0.25f;
    [Range(0.001f, 1f)]
    public float miedoEstandar = 0.25f;
    [Range(0.001f, 1f)]
    public float sociabBaseEstandar = 0.25f;
    [Range(0.001f, 1f)]
    public float vulneBaseEstandar = 0.02f;
    [Min(0)]
    public int duracionSueñoEstandar = 10;
    [Min(0.001f)]
    public int vEnvejecimientoEstandar = 1;
    [Range(0.001f, 1f)]
    public float pelajeEstandar = 0f;
    [Header("Variables de Mutacion")]
    [Space(5)]
    [Min(0.001f)]
    public float maxMutacion1 = 0.5f;
    [Min(1)]
    public int maxMutacionInf = 1;
    [Min(1f)]
    public float maxVariabilidad = 15;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
