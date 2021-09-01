using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MessageSystem;

public class destinationMessage : Message
{
    public GameObject destination;
}

public class Passenger : MonoBehaviour
{
    private GameObject _intendedDestination;

    public Passenger(GameObject destinationToSet)
    {
        _intendedDestination = destinationToSet;
    }

    public void sendDestination()
    {
        Debug.Log("I want to go to " + _intendedDestination.name);	

		// send an introduction message
		MessageBoard.SendMessage
        (
            new destinationMessage
            {
                destination = _intendedDestination,
            }
        );
    }
}
