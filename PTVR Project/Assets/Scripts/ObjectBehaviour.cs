using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBehaviour : MonoBehaviour
{
    private const int SPEED = 10;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 currentStop = new Vector3(100.0f, 100.0f, 0.0f);
        GameObject stop = GameObject.Find("Point A");

        GameObject bus = GameObject.Find("MovingCube");
        Vector3 currentPosition = bus.transform.position;


        if (bus.transform.position.x < currentStop.x)
        {
            transform.position = transform.position + new Vector3(currentPosition.x * SPEED * Time.deltaTime, 0, currentPosition.z);
        }
        else if (bus.transform.position.x > currentStop.x)
        {
            transform.position = transform.position + new Vector3(currentPosition.x * SPEED * Time.deltaTime, 0, currentPosition.z);
        }
        
        if (bus.transform.position.z < currentStop.z)
        {
            transform.position = transform.position + new Vector3(currentPosition.x, 0, currentPosition.z * SPEED * Time.deltaTime);
        }
        else if (bus.transform.position.z > currentStop.z)
        {
            transform.position = transform.position + new Vector3(currentPosition.x, 0, currentPosition.z * SPEED * Time.deltaTime);
        }

    }

    private void currentDestination()
    {

    }

}
