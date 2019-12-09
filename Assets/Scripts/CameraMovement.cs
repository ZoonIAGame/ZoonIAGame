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
        float rotvalueY = 0;
        float rotvalueX = 0;

        if (Input.GetKey(KeyCode.Q) && !Input.GetKey(KeyCode.LeftShift))
        {
            yvalue = -1;
        }else if (Input.GetKey(KeyCode.E) && !Input.GetKey(KeyCode.LeftShift))
        {
            yvalue = 1;
        }

        if (Input.GetKey(KeyCode.Q) && Input.GetKey(KeyCode.LeftShift))
        {
            rotvalueY = -8;
        }
        else if (Input.GetKey(KeyCode.E) && Input.GetKey(KeyCode.LeftShift))
        {
            rotvalueY = 8;
        }

        if (Input.GetKey(KeyCode.R) && Input.GetKey(KeyCode.LeftShift))
        {
            rotvalueX = -4;
        }
        else if (Input.GetKey(KeyCode.F) && Input.GetKey(KeyCode.LeftShift))
        {
            rotvalueX = 4;
        }

        this.transform.Translate(new Vector3(Input.GetAxis("Horizontal"), yvalue, Input.GetAxis("Vertical")));
        this.transform.Rotate(new Vector3(rotvalueX, rotvalueY, 0));
    }
}
