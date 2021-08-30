using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransportNetwork : MonoBehaviour
{
    private Stop _start;
    private List<Stop> _destinations;

    TransportNetwork()
    {
        GameObject[] tempList = GameObject.FindGameObjectsWithTag("Stop");
        foreach(GameObject g in tempList)
        {
            _destinations.Add(g.GetComponent<Stop>());
        }
        _start = GameObject.Find("Depot").GetComponent<Stop>();
    }
}
