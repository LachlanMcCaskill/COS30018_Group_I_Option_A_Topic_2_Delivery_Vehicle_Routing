using UnityEngine;
using MessageSystem;

public class DestinationMessage : Message
{
    public GameObject destination;
}

public class Passenger : MonoBehaviour
{
    public GameObject IntendedDestination;

    public Passenger(GameObject destinationToSet)
    {
        IntendedDestination = destinationToSet;
    }

    private void OnEnable()
	{
		if (IntendedDestination != null)
		{
			SendDestination();
		}
	}

    public void SendDestination()
    {
        Debug.Log("My name is "+gameObject.name+" and I want to go to point" + IntendedDestination.name + ".");	

		// send an introduction message
		MessageBoard.SendMessage
        (
            new DestinationMessage
            {
                destination = IntendedDestination,
            }
        );
    }
}
