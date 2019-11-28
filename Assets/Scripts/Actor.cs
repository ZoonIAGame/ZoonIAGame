using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    #region variables
    private float tamano;
    private float velocidad;
    private float rangoSens;
    private bool soyHembra;
    private bool embarazada;
    private int tGestacion;
    private int nHijos;
    private float fuerzaBase;
    private float atractivoBase;
    private float agroMismoSexBase;
    private float agroDistSexBase;
    private float agroOtrosBase;
    private float miedo;
    private float sociabBase;
    private bool sociable;
    private bool enManada;
    private float vulneBase;
    private int duracionSueño;
    private int vEnvejecimiento;
    private float pelaje;
    private float edad;

    private float vida;
    private float energia;
    private float agua;
    private float reproduccion;
    private float sueño;

    private float fuerzaFinal;
    private float atractivoFinal;
    private float agroMismoSexo;
    private float agroDistSexo;
    private float agroOtros;
    private float sociabFinal;
    private float variacionTempIdeal;
    private float vulneFinal;

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
    private void LateUpdate()
    {
        
    }

    #region Maquina de estados
    private void tomaDecisiones()
    {

    }
    #endregion

    #region metodos interaccion
    private void nacer()
    {

    }
    #endregion 
}
