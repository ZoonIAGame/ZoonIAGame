using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float yvalue = 0;

        if (Input.GetKey(KeyCode.Q))
        {
            yvalue = -1;
        }else if (Input.GetKey(KeyCode.E))
        {
            yvalue = 1;
        }

        this.transform.position = new Vector3(this.transform.position.x + Input.GetAxis("Horizontal"), this.transform.position.y + yvalue, this.transform.position.z + Input.GetAxis("Vertical"));
    }
}
