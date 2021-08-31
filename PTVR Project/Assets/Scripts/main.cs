using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class main : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //create the route solver
        RouteSolver rs = new RouteSolver();
        //create the passengers
        List<Passenger> passengersToTransport = new List<Passenger>();
        foreach(Stop s in rs._transportNetwork._destinations)
        {
            passengersToTransport.Add(new Passenger(s));
        }
        //create the transportAgents (buses)
        List<TransportAgent> transportAgents = new List<TransportAgent>();
        transportAgents.Add(new TransportAgent(5));
        transportAgents.Add(new TransportAgent(5));
        transportAgents.Add(new TransportAgent(5));
    }
}
