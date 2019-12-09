using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCloseDelete : MonoBehaviour
{
    void OnApplicationQuit()
    {
        Destroy(this.gameObject);
    }
}
