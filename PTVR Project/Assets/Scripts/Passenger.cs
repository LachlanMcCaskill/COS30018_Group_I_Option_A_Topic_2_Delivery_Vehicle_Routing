using UnityEngine;
using MessageSystem;

public class DestinationMessage : Message
{
    public GameObject destination;
}

public class Passenger : MonoBehaviour
{
    [SerializeField]private GameObject _intendedDestination;

    //public Passenger(GameObject destinationToSet)
    //{
    //    _intendedDestination = destinationToSet;
    //}

    public void SetDestination(GameObject destination)
    {
        _intendedDestination = destination;
		sendDestination();
    }

    private void OnEnable()
	{
        if (_intendedDestination != null && PlayerPrefs.GetInt("Randomize") == 0)
        {
            sendDestination();
        }
	}

    public void sendDestination()
    {
        Debug.Log("My name is "+gameObject.name+" and I want to go to point" + _intendedDestination.name + ".");	

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
