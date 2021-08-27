using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBehaviour : MonoBehaviour
{
    private const int SPEED = 10;

    [SerializeField] GameObject stop1;
    [SerializeField] GameObject stop2;
    [SerializeField] GameObject stop3;
    [SerializeField] GameObject stop4;
    [SerializeField] GameObject bus;
    Vector3 currentStop = new Vector3(0f, 0f, 0f);
    Vector3 currentPosition;
    private Transform stop;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        
        //if (currentStop == new Vector3(0f, 0f, 0f))
        //{
        //    currentStop = stop1.transform.position;
        //}
        //else if ((currentStop == stop1.transform.position) && ((currentPosition.z < currentStop.z) && (currentPosition.x < currentStop.x)))
        //{
        //    currentStop = stop2.transform.position;
        //}
        //else if ((currentStop == stop2.transform.position) && ((currentPosition.z > currentStop.z) && (currentPosition.x > currentStop.x)))
        //{
        //    currentStop = stop1.transform.position;
        //}

        currentPosition = bus.transform.position;

        float step = SPEED * Time.deltaTime;

        transform.position = Vector3.MoveTowards(transform.position, stop.position, step);

        if (Vector3.Distance(transform.position, stop.position) < 0.001f)
        {
            stop.position *= -1.0f;
        }

        //if (bus.transform.position.x < currentStop.x)
        //{
        //    transform.position = transform.position + new Vector3(1f, 0f, 0f) * Time.deltaTime * SPEED;
        //}
        //else if (bus.transform.position.x > currentStop.x)
        //{
        //    transform.position = transform.position + new Vector3(-1f, 0f, 0f) * Time.deltaTime * SPEED;
        //}

        //else if (bus.transform.position.z < currentStop.z)
        //{
        //    transform.position = transform.position + new Vector3(0, 0f, 1f) * Time.deltaTime * SPEED;
        //}
        //else if (bus.transform.position.z > currentStop.z)
        //{
        //    transform.position = transform.position + new Vector3(0, 0f, -1f) * Time.deltaTime * SPEED;
        //}


        //if (currentStop == currentPosition)
        //{
        //    currentStop = stop2.transform.position;
        //}


    }

}
