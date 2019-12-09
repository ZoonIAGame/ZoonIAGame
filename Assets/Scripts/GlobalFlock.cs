using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalFlock : MonoBehaviour
{
    public static int groupSize = 10;

    public static Vector3 goalPos = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Random.Range(0, 10000) < 50)
        {
            goalPos = new Vector3(Random.Range(-groupSize, groupSize), Random.Range(-groupSize, groupSize), Random.Range(-groupSize, groupSize));
           
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(goalPos, 1.0f);
    }
}
