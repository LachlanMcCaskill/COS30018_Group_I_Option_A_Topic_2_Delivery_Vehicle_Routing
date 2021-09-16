using UnityEngine;
using MessageSystem;

public class DestinationMessage : Message
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
