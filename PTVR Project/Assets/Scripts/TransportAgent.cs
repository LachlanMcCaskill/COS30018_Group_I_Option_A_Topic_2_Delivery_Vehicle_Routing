using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MessageSystem;


public class CapacityMessage : Message
{
    public int capacity;
    public int specialCapacity;
}

public class TransportAgent : MonoBehaviour
{
    string type;
    private List<Passenger> _passengers;
    private int _capacity;
    private int _specialCapacity;
    //private Route currentRoute = new Route();

    // these are used to generate transports
    //  public int GetCapacity()
    //  {
    //      return _capacity;
    //  }
    //  
    //  public int GetSpecialCapacity()
    //  {
    //      return _specialCapacity;
    //  }

    public static int LargeCapacity()
    {
        return 6;
    }

    public static int SmallCapacity()
    {
        return 4;
    }

    public static int LargeSpecialCapacity()
    {
        return 1;
    }

    public static int SmallSpecialCapacity()
    {
        return 1;
    }

    public TransportAgent(string _type) // int capacity
    {
        type = _type;
        if (type == "large")
        {
            _capacity = LargeCapacity();
            _specialCapacity = LargeSpecialCapacity();
        }
        else if (type == "small")
        {
            _capacity = SmallCapacity();
            _specialCapacity = SmallSpecialCapacity();
        }
        else
        {
            Debug.Log("Transport Agent type [" + _type + "] not recognised");
        }
    }

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
        Debug.Log("I can hold " + _capacity + " passengers including " + _specialCapacity + " special passengers.");	

		// send an introduction message
		MessageBoard.SendMessage
        (
            new CapacityMessage
            {
                capacity = _capacity,
            }
        );
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
