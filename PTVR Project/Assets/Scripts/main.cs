using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class main : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //create the route solver
        RouteSolver rs = new RouteSolver();
        //create the passengers
        List<Passenger> passengersToTransport = new List<Passenger>();
        foreach(GameObject g in rs._transportNetwork._destinations)
        {
            passengersToTransport.Add(new Passenger(g));
        }
        //create the transportAgents (buses)
        List<TransportAgent> transportAgents = new List<TransportAgent>();
        transportAgents.Add(new TransportAgent(5));
        transportAgents.Add(new TransportAgent(5));
        transportAgents.Add(new TransportAgent(6));
        List<Vector3> points = rs._transportNetwork._destinations.Select(obj => obj.transform.position).ToList();
        rs.Solve(rs._transportNetwork._start.transform.position, points, transportAgents.Count());
    }
}
