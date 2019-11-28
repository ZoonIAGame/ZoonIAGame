using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PruebaVelocidadSim : MonoBehaviour
{
    public enum loopType
    {
        update, fixedUpdate
    }

    public loopType tipoDeLoop;
    [Range(1,100)]
    public float timeSpeed = 1;
    public Vector3 velocidad = new Vector3(0, 0, 5);
    public Vector3 anguloRot = new Vector3(0, 20, 0);

    private void Start()
    {
        if (tipoDeLoop == loopType.fixedUpdate)
        {
            Time.timeScale = timeSpeed;
        }
    }

    void Update()
    {
        if(tipoDeLoop == loopType.update)
        {
            Time.timeScale = timeSpeed;
            moverse();
        }
    }
    void FixedUpdate()
    {
        if (tipoDeLoop == loopType.fixedUpdate)
        {
            Time.timeScale = timeSpeed;
            moverse();
        }
    }

    public void moverse()
    {
        this.transform.Rotate(anguloRot);
        this.transform.Translate(velocidad);
    }
}
