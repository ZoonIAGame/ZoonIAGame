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
    public float contadorEmbarazo;
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
    public float contadorSueño = 0;
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
    public bool durmiendo = false;

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
    public int prioridadObjetivoActual; // 1-Random/Flocking, 2-Reproduccion, 3-Sueño, 4-Energia
    public float tiempoPerseguido;

    private GameManager manager;
    private float udtEnFrames;
    private SphereCollider TriggerSens;
    private List<Collider> actorsCollidingWith = new List<Collider>();
    private List<Collider> bushesCollidingWith = new List<Collider>();
    private bool turning = false;
    private float neighbourDistance = 3.0f;
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
            if(energia <= 0)
            {
                vida--;
            }
            if(vida <= 0)
            {
                //Debug.Log("Inanición... Así es la vida")
                Destroy(this.gameObject);
            }
            if (embarazada)
            {
                contadorEmbarazo += Time.deltaTime;
                if (contadorEmbarazo > tGestacion)
                {
                    for(int i=0; i<nHijos; i++)
                    {
                        GameObject actor = Instantiate(manager.herviboro, new Vector3(this.transform.position.x, this.transform.position.y + ((i + 1) * 10), this.transform.position.z), Quaternion.identity);
                        actor.transform.SetParent(GameObject.FindGameObjectWithTag("BichitosGroup").transform);
                        actor.GetComponent<Actor>().nacer(this);
                        contadorEmbarazo = 0;
                    }
                    contadorEmbarazo = 0;
                    embarazada = false;
                }
            }
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

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Actor")
        {
            if (
                soyHembra != other.gameObject.GetComponent<Actor>().soyHembra &&
                other.gameObject.GetComponent<Actor>().buscoPareja && buscoPareja && soyHembra
                
                )
            {
                Color colorother = other.gameObject.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material.color;
                Color myColor = this.gameObject.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material.color;
                if (
                    (myColor == Color.red && (colorother == Color.red || colorother == Color.magenta || colorother == Color.yellow)) ||
                    (myColor == Color.magenta && (colorother == Color.magenta || colorother == Color.blue || colorother == Color.red)) ||
                    (myColor == Color.blue && (colorother == Color.blue || colorother == Color.cyan || colorother == Color.magenta)) ||
                    (myColor == Color.cyan && (colorother == Color.cyan || colorother == Color.green || colorother == Color.blue)) ||
                    (myColor == Color.green && (colorother == Color.green || colorother == Color.yellow || colorother == Color.cyan)) ||
                    (myColor == Color.yellow && (colorother == Color.yellow || colorother == Color.red || colorother == Color.green))
                    )
                {
                    embarazada = true;
                    reproduccion = 100;
                    buscoPareja = false;
                    other.gameObject.GetComponent<Actor>().reproduccion = 100;
                    other.gameObject.GetComponent<Actor>().buscoPareja = false;
                }
            }
        }else if (other.gameObject.tag == "TreeBush")
        {
            if (other.gameObject.GetComponent<TreeBush>().mySitio.plantacion == SitioPlantacion.plantationType.bush)
            {
                energia = 100;
                other.gameObject.GetComponent<TreeBush>().mySitio.plantacion = SitioPlantacion.plantationType.none;
                Destroy(other.gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Actor")
        {
            if (!actorsCollidingWith.Contains(other))
            {
                actorsCollidingWith.Add(other);
            }
        }else if (other.gameObject.tag == "TreeBush")
        {
            if (!bushesCollidingWith.Contains(other))
            {
                bushesCollidingWith.Add(other);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Actor")
        {
            if (actorsCollidingWith.Contains(other))
            {
                actorsCollidingWith.Remove(other);
            }
        }else if (other.gameObject.tag == "TreeBush")
        {
            if (bushesCollidingWith.Contains(other))
            {
                bushesCollidingWith.Remove(other);
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
        if (!durmiendo)
        {
            tiempoPerseguido += Time.deltaTime;
            Vector3 direccion = new Vector3(objetivo.x - this.transform.position.x, 0, objetivo.z - this.transform.position.z);
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direccion), (velocidadEdad / 2) * Time.deltaTime);
            this.transform.Translate(Vector3.forward * velocidadEdad * Time.deltaTime, Space.Self);
        }else
        {
            contadorSueño += Time.deltaTime;
            if (contadorSueño >= duracionSueño)
            {
                sueño = 100;
                durmiendo = false;
            }
        }
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

        sociable = sociabFinal > Mathf.Max(agroMismoSexo, agroDistSexo);
        #endregion

        #region Update de mi muerte, asi es la vida
        if (edad >= 180)
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
        if (energia < 75)
        {
            float distanceMin = rangoSensEdad;
            foreach (Collider col in bushesCollidingWith)
            {
                if (col != null)
                {
                    if (Vector3.Distance(this.transform.position, col.gameObject.transform.position) < distanceMin &&
                        col.gameObject.GetComponent<TreeBush>().mySitio.plantacion == SitioPlantacion.plantationType.bush)
                    {
                        if (prioridadObjetivoActual <= 4)
                        {
                            objetivo = col.gameObject.transform.position;
                            prioridadObjetivoActual = 4;
                            distanceMin = Vector3.Distance(this.transform.position, col.gameObject.transform.position);
                        }
                    }
                }
            }
            bushesCollidingWith.Remove(null);
        }
        if (sueño < energia && sueño < 75)
        {
            if (prioridadObjetivoActual < 3)
            {
                prioridadObjetivoActual = 3;
                durmiendo = true;
            }
        }
        if (reproduccion < energia && reproduccion < sueño && reproduccion < 75) {
            float distanceMin = rangoSensEdad;
            buscoPareja = true;
            foreach (Collider col in actorsCollidingWith)
            {
                if (col != null)
                {
                    Color colorother = col.gameObject.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material.color;
                    Color myColor = this.gameObject.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material.color;
                    if (
                        Vector3.Distance(this.transform.position, col.gameObject.transform.position) < distanceMin &&
                        soyHembra != col.gameObject.GetComponent<Actor>().soyHembra &&
                        col.gameObject.GetComponent<Actor>().buscoPareja &&
                        ((myColor == Color.red && (colorother == Color.red || colorother == Color.magenta || colorother == Color.yellow)) ||
                        (myColor == Color.magenta && (colorother == Color.magenta || colorother == Color.blue || colorother == Color.red)) ||
                        (myColor == Color.blue && (colorother == Color.blue || colorother == Color.cyan || colorother == Color.magenta)) ||
                        (myColor == Color.cyan && (colorother == Color.cyan || colorother == Color.green || colorother == Color.blue)) ||
                        (myColor == Color.green && (colorother == Color.green || colorother == Color.yellow || colorother == Color.cyan)) ||
                        (myColor == Color.yellow && (colorother == Color.yellow || colorother == Color.red || colorother == Color.green)))
                        )
                    {
                        if (prioridadObjetivoActual <= 2)
                        {
                            objetivo = col.gameObject.transform.position;
                            prioridadObjetivoActual = 2;
                            distanceMin = Vector3.Distance(this.transform.position, col.gameObject.transform.position);
                        }
                    }
                }
            }
            actorsCollidingWith.Remove(null);
        }
        if (sociable)
        {
            if (prioridadObjetivoActual <= 1)
            {
                applyFlock();
                prioridadObjetivoActual = 1;
            }
        }
        if ((Mathf.Abs(this.transform.position.x - objetivo.x) <= 1f && Mathf.Abs(this.transform.position.z - objetivo.z) <= 1f) || tiempoPerseguido >= perseverancia)
        {
            prioridadObjetivoActual = -1;
            if (prioridadObjetivoActual < 0)
            {
                objetivo = new Vector3(
                    this.transform.position.x + Random.Range(-rangoSensEdad, rangoSensEdad),
                    this.transform.position.y,
                    this.transform.position.z + Random.Range(-rangoSensEdad, rangoSensEdad)
                    );
                prioridadObjetivoActual = 0;
                tiempoPerseguido = 0;
            }
        }
    }

    public void applyFlock()
    {
        if (Vector3.Distance(transform.position, Vector3.zero) >= GlobalFlock.groupSize)
        {
            turning = true;
        }
        else
        {
            turning = false;
        }

        if (turning) //Se mueve hasta la manada
        {
            objetivo = GlobalFlock.goalPos;
        }
        else
        {
            if (Random.Range(0, 5) < 1)
                applyFlockingRules();
        }
    }

    //Tres reglas de flocking
    public void applyFlockingRules()
    {
        Vector3 vcentre = Vector3.zero;
        Vector3 vavoid = Vector3.zero;

        Vector3 goalPos = GlobalFlock.goalPos;

        float gspeed = 5.0f;

        float dist;
        int groupSize = 0;
        foreach (Collider col in actorsCollidingWith)
        {
            if (col != null)
            {
                GameObject go = col.gameObject;
                if (go != this.gameObject)
                {
                    dist = Vector3.Distance(go.transform.position, this.transform.position);
                    if (dist <= neighbourDistance)
                    {
                        vcentre += go.transform.position;
                        groupSize++;

                        if (dist < 0.5f)
                        {
                            //AVOID CHOCARSE
                            vavoid = vavoid + (this.transform.position - go.transform.position);
                        }

                        Actor anotherActor = go.GetComponent<Actor>();
                        gspeed = gspeed + anotherActor.velocidadEdad;

                    }
                }
            }
        }
        actorsCollidingWith.Remove(null);

        if (groupSize > 0)
        {
            vcentre = vcentre / groupSize + (goalPos - this.transform.position);
            velocidadEdad = gspeed / groupSize;

            Vector3 dir = (vcentre + vavoid) - transform.position;
            if (dir != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z)), velocidadEdad / 2 * Time.deltaTime);
            }
        }
    }
    #endregion

    #region metodos interaccion
    public void embarazarse()
    {
        embarazada = true;
    }

    public void nacer(Actor madre)
    {
        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        //variables actor
        tamano = Mathf.Max(madre.tamano + Random.Range(-manager.maxMutacionInf, manager.maxMutacionInf),0.001f);
        velocidad = Mathf.Max(madre.velocidad + Random.Range(-manager.maxMutacionInf, manager.maxMutacionInf), 0.001f);
        rangoSens = Mathf.Max(madre.rangoSens + Random.Range(-manager.maxMutacionInf, manager.maxMutacionInf), 0.001f);
        perseverancia = Mathf.Max(madre.perseverancia + Random.Range(-manager.maxMutacionInf, manager.maxMutacionInf), 0.001f);
        soyHembra = Random.Range(0f, 1f) < 0.5f ? true : false;
        embarazada = false;
        tGestacion = Mathf.Max(madre.tGestacion + (int)Random.Range(-manager.maxMutacionInf, manager.maxMutacionInf), 1);
        nHijos = Mathf.Max(madre.nHijos + (int)Random.Range(-manager.maxMutacionInf, manager.maxMutacionInf), 1);
        fuerzaBase = Mathf.Max(madre.fuerzaBase + Random.Range(-manager.maxMutacionInf, manager.maxMutacionInf), 0.001f);
        atractivoBase = Mathf.Max(madre.atractivoBase + Random.Range(-manager.maxMutacionInf, manager.maxMutacionInf), 0.001f);

        agroMismoSexBase = Mathf.Clamp01(madre.agroMismoSexBase + Random.Range(-manager.maxMutacion1, manager.maxMutacion1));
        agroDistSexBase = Mathf.Clamp01(madre.agroDistSexBase + Random.Range(-manager.maxMutacion1, manager.maxMutacion1));
        agroOtrosBase = Mathf.Clamp01(madre.agroOtros + Random.Range(-manager.maxMutacion1, manager.maxMutacion1));
        miedo = Mathf.Clamp01(madre.miedo + Random.Range(-manager.maxMutacion1, manager.maxMutacion1));
        sociabBase = Mathf.Clamp01(madre.sociabBase + Random.Range(-manager.maxMutacion1, manager.maxMutacion1));
        sociable = false;
        enManada = false;
        vulneBase = Mathf.Clamp01(madre.vulneBase + Random.Range(-manager.maxMutacion1, manager.maxMutacion1));

        duracionSueño = Mathf.Max(madre.duracionSueño + (int)Random.Range(-manager.maxMutacionInf, manager.maxMutacionInf),1);
        vEnvejecimiento = Mathf.Max(madre.vEnvejecimiento + Random.Range(-manager.maxMutacionInf, manager.maxMutacionInf),0.001f);

        pelaje = Mathf.Clamp01(madre.pelaje + Random.Range(-manager.maxMutacion1, manager.maxMutacion1));

        edad = madre.tGestacion;

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

        temperatura = 0.5f;
        humedad = 0.5f;

        fuerzaFinal = fuerzaBaseEdad + tamanoEdad;
        atractivoFinal = atractivoBaseEdad + fuerzaFinal + pelajeEdad;
        agroMismoSexo = agroMismoSexBase * (1 - Mathf.Min(energia / 100, reproduccion / 100));
        agroDistSexo = Mathf.Max(agroDistSexBase * ((1 - energia / 100) - (1 - reproduccion / 100)), 0);
        agroOtros = Mathf.Max(agroOtrosBase - miedo);
        sociabFinal = Mathf.Max(sociabBase + miedo / 10, 0);
        variacionTempIdeal = Mathf.Abs(temperatura - (1 - pelajeEdad));
        vulneFinal = Mathf.Min((vulneBaseEdad + (variacionTempIdeal * 0.1f)) / 10, 1);

        //color actor
        #region Colors ifs
        Color micolor = Color.white;
        if (Random.Range(0f, 1f) < manager.maxMutacion1)
        {
            Color madreColor = madre.gameObject.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material.color;
            if (madreColor == Color.red)
            {
                if (Random.Range(0f, 1f) < 0.5f)
                {
                    micolor = Color.magenta;
                }
                else
                {
                    micolor = Color.yellow;
                }
            }
            else if (madreColor == Color.magenta)
            {
                if (Random.Range(0f, 1f) < 0.5f)
                {
                    micolor = Color.blue;
                }
                else
                {
                    micolor = Color.red;
                }
            }
            else if (madreColor == Color.magenta)
            {
                if (Random.Range(0f, 1f) < 0.5f)
                {
                    micolor = Color.blue;
                }
                else
                {
                    micolor = Color.red;
                }
            }
            else if (madreColor == Color.blue)
            {
                if (Random.Range(0f, 1f) < 0.5f)
                {
                    micolor = Color.cyan;
                }
                else
                {
                    micolor = Color.magenta;
                }
            }
            else if (madreColor == Color.cyan)
            {
                if (Random.Range(0f, 1f) < 0.5f)
                {
                    micolor = Color.green;
                }
                else
                {
                    micolor = Color.blue;
                }
            }
            else if (madreColor == Color.green)
            {
                if (Random.Range(0f, 1f) < 0.5f)
                {
                    micolor = Color.yellow;
                }
                else
                {
                    micolor = Color.cyan;
                }
            }
            else if (madreColor == Color.yellow)
            {
                if (Random.Range(0f, 1f) < 0.5f)
                {
                    micolor = Color.red;
                }
                else
                {
                    micolor = Color.green;
                }
            }
        }
        else
        {
            micolor = madre.gameObject.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material.color;
        }
        #endregion
        this.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material.SetColor("_Color", micolor);
        this.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Renderer>().material.SetColor("_Color", micolor);

        //Trigger Rango Sens
        TriggerSens = this.gameObject.AddComponent<SphereCollider>();
        TriggerSens.isTrigger = true;
        TriggerSens.radius = rangoSensEdad;
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
