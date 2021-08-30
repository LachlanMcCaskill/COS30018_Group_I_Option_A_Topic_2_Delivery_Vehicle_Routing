using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransportNetwork : MonoBehaviour
{
    private Stop _start;
    public List<Stop> _destinations;

    public TransportNetwork()
    {
        GameObject[] tempList = GameObject.FindGameObjectsWithTag("Stop");
        foreach(GameObject g in tempList)
        {
            _destinations.Add(g.GetComponent<Stop>());
        }
        _start = GameObject.Find("Depot").GetComponent<Stop>();
    }

    public void passengerSetup(List<Passenger> passengers)
    {
        _start.addPassengers(passengers);
    }
}
