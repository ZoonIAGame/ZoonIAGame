using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    #region variables
    [System.Serializable]
    public struct Textura
    {
        public string nombre;
        public Texture tex;
    }

    public Textura[] texturas;

    [Header("Variables Actor")]
    [Space(5)]
    public float tamano;
    public float velocidad;
    public float rangoSens;
    public bool soyHembra;
    public bool embarazada;
    public int tGestacion;
    public int nHijos;
    public float fuerzaBase;
    public float atractivoBase;
    public float agroMismoSexBase;
    public float agroDistSexBase;
    public float agroOtrosBase;
    public float miedo;
    public float sociabBase;
    public bool sociable;
    public bool enManada;
    public float vulneBase;
    public int duracionSueño;
    public int vEnvejecimiento;
    public float pelaje;
    public float edad;

    [Header("Variables segun edad")]
    [Space(5)]
    //variables que dependen de la edad
    public float tamanoEdad;
    public float velocidadEdad;
    public float rangoSensEdad;
    public float fuerzaBaseEdad;
    public float atractivoBaseEdad;
    public float vulneBaseEdad;
    public float pelajeEdad;

    [Header("Variables de necesidad")]
    [Space(5)]
    public float vida;
    public float energia;
    public float agua;
    public float reproduccion;
    public float sueño;

    [Header("Variables ambientales")]
    [Space(5)]
    public float temperatura;
    public float humedad;

    [Header("Variables finales")]
    [Space(5)]
    public float fuerzaFinal;
    public float atractivoFinal;
    public float agroMismoSexo;
    public float agroDistSexo;
    public float agroOtros;
    public float sociabFinal;
    public float variacionTempIdeal;
    public float vulneFinal;

    [Header("Objetivo")]
    [Space(5)]
    public Vector3 objetivo;

    private GameManager manager;
    #endregion

    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        if (manager.tipoLoop == GameManager.LoopType.update)
        {
            tomaDecisiones();
        }
    }

    void FixedUpdate()
    {
        if (manager.tipoLoop == GameManager.LoopType.fixedUpdate)
        {
            tomaDecisiones();
        }
    }

    //animaciones
    void LateUpdate()
    {
        
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(this.transform.position, rangoSensEdad);
    }

#region Maquina de estados
    private void tomaDecisiones()
    {
        this.transform.Rotate(0, 2, 0);
        this.transform.Translate(0, 0, velocidadEdad);
    }
#endregion

#region metodos interaccion
    public void nacer()
    {
        /*tamano;
        velocidad;
        rangoSens;
        soyHembra;
        embarazada;
        tGestacion;
        nHijos;
        fuerzaBase;
        atractivoBase;
        agroMismoSexBase;
        agroDistSexBase;
        agroOtrosBase;
        miedo;
        sociabBase;
        sociable;
        enManada;
        vulneBase;
        duracionSueño;
        vEnvejecimiento;
        pelaje;
        edad;

        vida;
        energia;
        agua;
        reproduccion;
        sueño;

        fuerzaFinal;
        atractivoFinal;
        agroMismoSexo;
        agroDistSexo;
        agroOtros;
        sociabFinal;
        variacionTempIdeal;
        vulneFinal;*/
    }

    public void aparecer()
    {
        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        //variables actor
        tamano = manager.tamanoEstandar;
        velocidad = manager.velocidadEstandar;
        rangoSens = manager.rangoSensEstandar;
        soyHembra = Random.Range(0f, 1f) < manager.proporcionHembras ? true : false;
        embarazada = false;
        tGestacion = manager.tGestacionEstandar;
        nHijos = manager.nHijosEstandar;
        fuerzaBase = manager.fuerzaBaseEstandar;
        atractivoBase = manager.atractivoBaseEstandar;
        agroMismoSexBase = manager.agroMismoSexEstandar;
        agroDistSexBase = manager.agroDistSexEstandar;
        agroOtrosBase = manager.agroOtrosEstandar;
        miedo = manager.miedoEstandar;
        sociabBase = manager.sociabBaseEstandar;
        sociable = false;
        enManada = false;
        vulneBase = manager.vulneBaseEstandar;
        duracionSueño = manager.duracionSueñoEstandar;
        vEnvejecimiento = manager.vEnvejecimientoEstandar;
        pelaje = manager.pelajeEstandar;
        edad = manager.edadActoresIniciales;

        tamanoEdad = tamano * Mathf.Sin(Mathf.Deg2Rad * edad);
        velocidadEdad = velocidad * Mathf.Sin(Mathf.Deg2Rad * edad);
        rangoSensEdad = rangoSens * Mathf.Sin(Mathf.Deg2Rad * edad);
        fuerzaBaseEdad = fuerzaBase * Mathf.Sin(Mathf.Deg2Rad * edad);
        atractivoBaseEdad = atractivoBase * Mathf.Sin(Mathf.Deg2Rad * edad);
        vulneBaseEdad = Mathf.Max(Mathf.Abs(vulneBase * Mathf.Cos(Mathf.Deg2Rad * edad)), vulneBase * 0.1f);
        pelajeEdad = pelaje * Mathf.Sin(Mathf.Deg2Rad * edad);

        vida = 100;
        energia = 100;
        agua = 100 + tamanoEdad * 10;
        reproduccion = 100;
        sueño = 100;

        temperatura = 0;
        humedad = 0;

        fuerzaFinal = fuerzaBaseEdad + tamanoEdad;
        atractivoFinal = atractivoBaseEdad + fuerzaFinal + pelajeEdad;
        agroMismoSexo = agroMismoSexBase * (1 - Mathf.Min(energia/100, reproduccion/100));
        agroDistSexo = Mathf.Max(agroDistSexBase * ((1 - energia/100) - (1 - reproduccion/100)), 0);
        agroOtros = Mathf.Max(agroOtrosBase - miedo);
        sociabFinal = Mathf.Max(sociabBase + miedo/10, 0);
        variacionTempIdeal = Mathf.Abs(temperatura - (1 - pelajeEdad));
        vulneFinal = Mathf.Min(vulneBaseEdad + (variacionTempIdeal * 0.1f), 1);

        //color actor
        this.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_Color", manager.colorIni);
        this.transform.GetChild(1).GetChild(0).GetComponent<Renderer>().material.SetColor("_Color", manager.colorIni);
    }
#endregion
}
