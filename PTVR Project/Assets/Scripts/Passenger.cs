using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MessageSystem;

public class DestinationMessage : Message
{
    public GameObject destination;
}

public class Passenger : MonoBehaviour
{
    string type;
    private GameObject _intendedDestination;

    public bool IsSpecial()
    {
        return type == "special";
    }

    public Passenger(GameObject destinationToSet)
    {
        _intendedDestination = destinationToSet;

        if (Random.Range(0, 1f) < 0.15f)
        {
            type = "special";
        }
        else type = "normal";
    }

    public void sendDestination()
    {
        Debug.Log("I want to go to point" + _intendedDestination.name + ".");	

		// send an introduction message
		MessageBoard.SendMessage
        (
            new DestinationMessage
            {
                destination = _intendedDestination,
            }
        );
    }
}
