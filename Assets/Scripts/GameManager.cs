using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Variables de Generacion Inicial")]
    [Space(5)]
    [Min(1)]
    public int numActoresIniciales = 10;
    public float edadActoresIniciales = 10;
    public float radioSpawn = 10;
    public float alturaSpawn = 5;
    public enum BocaType
    {
        Herviboro, Omnivoro, Carnivoro
    }
    public enum ColorType
    {
        Cyan, Magenta, Amarillo, Rojo, Verde, Azul, Blanco
    }

    [Header("Variables Estandar")]
    public BocaType bocaTipoEstandar = BocaType.Herviboro;
    public ColorType colorEstandar = ColorType.Rojo;
    [HideInInspector]
    public Color colorIni;
    [Min(0.001f)]
    [Tooltip("Esto edita el tamaño")]
    public float tamanoEstandar = 1f;
    [Min(0.001f)]
    public float velocidadEstandar = 5f;
    [Min(0.001f)]
    public float rangoSensEstandar = 5f;
    [Range(0, 1)]
    public float proporcionHembras = 0.5f;
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

    public enum LoopType
    {
        update, fixedUpdate
    }

    [Header("Variables de control de Simulacion")]
    [Space(5)]
    public LoopType tipoLoop = LoopType.update;
    [Range(1, 100)]
    public float timeSpeed = 100;

    [Header("Variables de Mutacion")]
    [Space(5)]
    [Min(0.001f)]
    public float maxMutacion1 = 0.5f;
    [Min(1)]
    public int maxMutacionInf = 1;
    [Min(1f)]
    public float maxVariabilidad = 15;

    [Header("Prefabs")]
    [Space(5)]
    public GameObject herviboro;
    public GameObject omnivoro;
    public GameObject carnivoro;


    // Start is called before the first frame update
    void Start()
    {
        colorIni = colorEstandar == ColorType.Cyan ? Color.cyan : (
            colorEstandar == ColorType.Magenta ? Color.magenta : (
            colorEstandar == ColorType.Amarillo ? Color.yellow : (
            colorEstandar == ColorType.Azul ? Color.blue : (
            colorEstandar == ColorType.Rojo ? Color.red : (
            colorEstandar == ColorType.Verde ? Color.green : Color.white)))));

        for (int i = 0; i<numActoresIniciales; i++)
        {
            GameObject actor = Instantiate(bocaTipoEstandar == BocaType.Herviboro ? herviboro : (
                bocaTipoEstandar == BocaType.Carnivoro ? carnivoro : omnivoro),
                new Vector3 (this.transform.position.x + Random.Range(-radioSpawn, radioSpawn), alturaSpawn, this.transform.position.z + Random.Range(-radioSpawn, radioSpawn)),
                Quaternion.identity);
            actor.transform.SetParent(GameObject.FindGameObjectWithTag("BichitosGroup").transform);
            actor.GetComponent<Actor>().aparecer();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = timeSpeed;
    }

    void FixedUpdate()
    {
        
    }
}
