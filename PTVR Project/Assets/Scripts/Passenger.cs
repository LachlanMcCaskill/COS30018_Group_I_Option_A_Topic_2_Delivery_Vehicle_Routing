using UnityEngine;
using MessageSystem;

public class DestinationMessage : Message
{
    public GameObject destination;
}

public class Passenger : MonoBehaviour
{
    public GameObject IntendedDestination;

    //set intended destination based on the value set in the inspector
    public Passenger(GameObject destinationToSet)
    {
        IntendedDestination = destinationToSet;
    }

    //send destination when enabled if it has one
    private void OnEnable()
	{
		if (IntendedDestination != null)
		{
			SendDestination();
		}
	}

    //Send destination to message board for master routing agent to read
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
