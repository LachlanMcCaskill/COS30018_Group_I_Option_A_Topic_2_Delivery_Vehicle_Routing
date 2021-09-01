using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransportNetwork : MonoBehaviour
{
    private GameObject _start;
    public List<GameObject> _destinations;

    public TransportNetwork()
    {
        GameObject[] tempList = GameObject.FindGameObjectsWithTag("Stop");
        foreach(GameObject g in tempList)
        {
            _destinations.Add(g);
        }
        _start = GameObject.Find("Depot");
    }
}
