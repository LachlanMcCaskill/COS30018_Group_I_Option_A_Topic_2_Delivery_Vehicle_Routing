using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransportAgent : MonoBehaviour
{
    private List<Passenger> _passengers;
    private int _capacity;
    private Route currentRoute;

    private void TravelToNextDestination()
    {
        //Move toward the next stop in the route
    }

    private void AddPassenger(Passenger p)
    {
        _passengers.Add(p);
    }

    private void ArriveAtStop()
    {
        //stop at the current stop in the route and drop off passengers
    }

    private void AssignRoute(Route r)
    {
        //set the route of this agent
    }

    private void SendCapacity()
    {
        //access the messaging system to send a messgae to the master routing agent containg information about this agent's capacity
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
