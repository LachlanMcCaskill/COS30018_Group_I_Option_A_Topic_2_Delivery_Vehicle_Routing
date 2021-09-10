using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ObjectBehaviour : MonoBehaviour
{
    private const int SPEED = 10;

//initalise pre-existing objects in Unity Scene
    [SerializeField] GameObject stop1;
    [SerializeField] GameObject stop2;
    [SerializeField] GameObject stop3;
    [SerializeField] GameObject stop4;
    [SerializeField] GameObject bus;

    GameObject stop;

    Vector3 currentStop = new Vector3(0f, 0f, 0f);
    Vector3 currentPosition;

    int i = 0;
    private List<GameObject> _objectList = new List<GameObject>();



    // Start is called before the first frame update
    void Start()
    {
        //Add GameObjects to List of GameObjects
        _objectList.Add(stop1);
        _objectList.Add(stop2);
        _objectList.Add(stop3);
        _objectList.Add(stop4);
    }

    // Update is called once per frame
    void Update()
    {
        //Start of Scene Initalizer
        if (i == 0)
        {
            stop = _objectList[i];
                i++;
        }
        
        //set current position and Move towards current destination
        currentPosition = bus.transform.position;

        float step = SPEED * Time.deltaTime;

        transform.position = Vector3.MoveTowards(transform.position, stop.transform.position, step);


        //Set new stop in list, else close editor
        if (Vector3.Distance(transform.position, stop.transform.position) < 0.001f)
        {

            stop = _objectList[i];
            i++;

            if (i == 4)
            {
                UnityEditor.EditorApplication.isPlaying = false;
            }
        }

    }

}
