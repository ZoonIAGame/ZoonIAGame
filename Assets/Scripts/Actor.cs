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
    public bool corriendo;
    public float rangoSens;
    public float perseverancia;
    public bool soyHembra;
    public bool buscoPareja;
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
    public float vEnvejecimiento;
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
    public int prioridadObjetivoActual; // 1-Random, 2-AcercarmeAManada, 3-Sueño, 4-Reproduccion, 5-Agua, 6-Comer, 7-Huir/Atacar
    public float tiempoPerseguido;

    private GameManager manager;
    private float udtEnFrames;
    private SphereCollider TriggerSens;
    private List<Collider> collidingWith = new List<Collider>();
    #endregion

    #region Metodos Unity
    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        objetivo = this.transform.position;
    }

    void Update()
    {
        if (manager.tipoLoop == GameManager.LoopType.update)
        {
            udtEnFrames = calcularUdtEnFrames();
            updateActorValues();
            tomaDecisiones();
            moverse();
        }
        edad += (manager.udtPorSegundo*Time.deltaTime)*vEnvejecimiento;
    }

    void FixedUpdate()
    {
        if (manager.tipoLoop == GameManager.LoopType.fixedUpdate)
        {
            udtEnFrames = 1;
            updateActorValues();
            tomaDecisiones();
            moverse();
        }
    }

    //animaciones
    void LateUpdate()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Actor")
        {
            if (!collidingWith.Contains(other))
            {
                collidingWith.Add(other);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Actor")
        {
            if (collidingWith.Contains(other))
            {
                collidingWith.Remove(other);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(this.transform.position, rangoSensEdad);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(objetivo, 1f);
    }
    #endregion

    #region Actualización de mi estado y Miscelanea
    void die()
    {
        this.transform.position = new Vector3(0, -10000, 0);
        Destroy(this.gameObject);
    }

    float calcularUdtEnFrames()
    {
        float fps = 1.0f / Time.deltaTime;

        return (manager.udtPorSegundo / fps);
    }

    void moverse()
    {
        tiempoPerseguido += Time.deltaTime;
        Vector3 direccion = new Vector3(objetivo.x - this.transform.position.x, 0, objetivo.z - this.transform.position.z);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direccion), (velocidadEdad/2)*Time.deltaTime);
        this.transform.Translate(Vector3.forward*velocidadEdad*Time.deltaTime, Space.Self);
    }

    void updateActorValues()
    {
        #region Update de Variables
        tamanoEdad = tamano * Mathf.Sin(Mathf.Deg2Rad * Mathf.Min(edad,90));
        velocidadEdad = velocidad * Mathf.Sin(Mathf.Deg2Rad * edad);

        rangoSensEdad = rangoSens * Mathf.Sin(Mathf.Deg2Rad * edad);
        TriggerSens.radius = rangoSensEdad;

        fuerzaBaseEdad = fuerzaBase * Mathf.Sin(Mathf.Deg2Rad * edad);
        atractivoBaseEdad = atractivoBase * Mathf.Sin(Mathf.Deg2Rad * edad);
        vulneBaseEdad = Mathf.Max(Mathf.Abs(vulneBase * Mathf.Cos(Mathf.Deg2Rad * edad)), vulneBase * 0.1f);
        pelajeEdad = pelaje * Mathf.Sin(Mathf.Deg2Rad * edad);

        this.transform.GetChild(0).localScale = new Vector3(tamanoEdad, tamanoEdad, tamanoEdad);
        this.transform.GetChild(0).GetChild(1).localScale = new Vector3(pelajeEdad * 100, pelajeEdad * 100, pelajeEdad * 100);

        temperatura = 1f; //Sacado del escenario
        humedad = 0.5f; //Sacado del escenario

        float consumoEnergia = (((tamanoEdad * 5) + (velocidadEdad * 2) + (rangoSensEdad / 10) + (fuerzaBaseEdad * 2) + (atractivoBaseEdad) + (pelajeEdad) - (vulneBaseEdad * 100)) * manager.multiplicadorRestaEnergia); //Formula del excel
        if (corriendo)
        {
            energia -= 2 * consumoEnergia * udtEnFrames;
        }
        else {
            energia -= consumoEnergia * udtEnFrames;
        }

        float consumoAgua = (1-(humedad*0.5f))*(manager.multiplicadorRestaAgua)*Mathf.Sin(Mathf.Deg2Rad * edad);//Formula del excel
        if (corriendo)
        {
            agua -= 2*(consumoAgua) * udtEnFrames;
        }
        else
        {
            agua -= (consumoAgua) * udtEnFrames;
        }

        reproduccion -= ((1* Mathf.Sin(Mathf.Deg2Rad * edad))*manager.multiplicadorRestaSexo)*udtEnFrames; //Formula del excel
        sueño -= 0.5f * udtEnFrames; //Formula del excel

        fuerzaFinal = fuerzaBaseEdad + tamanoEdad;
        atractivoFinal = atractivoBaseEdad + fuerzaFinal + pelajeEdad;
        agroMismoSexo = agroMismoSexBase * (1 - Mathf.Min(energia / 100, reproduccion / 100));
        agroDistSexo = Mathf.Max(agroDistSexBase * ((1 - energia / 100) - (1 - reproduccion / 100)), 0);
        agroOtros = Mathf.Max(agroOtrosBase - miedo);
        sociabFinal = Mathf.Max(sociabBase + miedo / 10, 0);
        variacionTempIdeal = Mathf.Abs(temperatura - (1 - pelajeEdad));
        vulneFinal = Mathf.Min((vulneBaseEdad + (variacionTempIdeal * 0.1f))/10, 1);
        #endregion

        #region Update de mi muerte, asi es la vida
        if(edad >= 180)
        {
            //Debug.Log("Muerte natural... así es la vida");
            Destroy(this.gameObject);
        }else if (Random.Range(0f,1f) <= (vulneFinal*udtEnFrames))
        {
            //Debug.Log("Enfermedad... así es la vida");
            Destroy(this.gameObject);
        }
        #endregion
    }
    #endregion

    #region Maquina de estados
    private void tomaDecisiones()
    {
        if (false)
        {

        }
        if (reproduccion <= 75) {
            float distanceMin = rangoSensEdad;
            buscoPareja = true;
            foreach (Collider col in collidingWith)
            {
                if (col != null)
                {
                    if (
                        Vector3.Distance(this.transform.position, col.gameObject.transform.position) < distanceMin &&
                        soyHembra != col.gameObject.GetComponent<Actor>().soyHembra &&
                        col.gameObject.GetComponent<Actor>().buscoPareja
                        )
                    {
                        objetivo = col.gameObject.transform.position;
                        prioridadObjetivoActual = 4;
                        distanceMin = Vector3.Distance(this.transform.position, col.gameObject.transform.position);
                    }
                }
            }
        }
        if ((Mathf.Abs(this.transform.position.x - objetivo.x) <= 1f && Mathf.Abs(this.transform.position.z - objetivo.z) <= 1f) || tiempoPerseguido >= perseverancia)
        {
            prioridadObjetivoActual = 0;
            if (prioridadObjetivoActual < 1)
            {
                objetivo = new Vector3(
                    this.transform.position.x + Random.Range(-rangoSensEdad, rangoSensEdad),
                    this.transform.position.y,
                    this.transform.position.z + Random.Range(-rangoSensEdad, rangoSensEdad)
                    );
                prioridadObjetivoActual = 1;
                tiempoPerseguido = 0;
            }
        }
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
        perseverancia = manager.perseveranciaEstandar;
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
        vulneFinal = Mathf.Min((vulneBaseEdad + (variacionTempIdeal * 0.1f))/10, 1);

        //color actor
        this.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material.SetColor("_Color", manager.colorIni);
        this.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Renderer>().material.SetColor("_Color", manager.colorIni);

        //Trigger Rango Sens
        TriggerSens = this.gameObject.AddComponent<SphereCollider>();
        TriggerSens.isTrigger = true;
        TriggerSens.radius = rangoSensEdad;
    }
#endregion
}
