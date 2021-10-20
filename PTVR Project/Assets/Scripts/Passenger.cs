using UnityEngine;
using MessageSystem;

public class DestinationMessage : Message
{
    public Vector3 Position
    {
        get
        {
            return destination.transform.position;
        }
    }

    public GameObject destination;
    public bool special;
}

public class Passenger : MonoBehaviour
{
    [SerializeField]private GameObject _intendedDestination;
    [SerializeField]private bool _special;

    [SerializeField] private Mesh _cylinder;
    [SerializeField] private Mesh _cube;

    //public Passenger(GameObject destinationToSet)
    //{
    //    _intendedDestination = destinationToSet;
    //}

    private void getSpecial()
    {
        if (PlayerPrefs.GetString("SpecialRoute") == "True")
        {
            int specialCount = PlayerPrefs.GetInt("SpecialPoints");
            if (specialCount > 0)
            {
                _special = true;
                PlayerPrefs.SetInt("SpecialPoints", specialCount - 1);
            }
            else _special = false;
        }
        else _special = false;
    }

    public void SetDestination(GameObject destination)
    {
        getSpecial();
        _intendedDestination = destination;
        setSpecialDestination();
        sendDestination();
    }

    public void setSpecialDestination()
    {
        Mesh mesh;
        if (_special)
        {
            _intendedDestination.GetComponent<MeshRenderer>().material.color = Color.red;
            mesh = _cylinder;
        }
        else
        {
            _intendedDestination.GetComponent<MeshRenderer>().material.color = Color.green;
            mesh = _cube;
        }

        _intendedDestination.GetComponent<MeshFilter>().mesh = mesh;
    }

    //   private void OnEnable()
    //{
    //       if (_intendedDestination != null && PlayerPrefs.GetInt("Randomize") == 0)
    //       {
    //           sendDestination();
    //       }
    //}

    public void sendDestination()
    {
        string log = "My name is " + gameObject.name;
        if (_special)
        {
            log += ". I am a special passenger";
        }
        log += ". I want to go to " + _intendedDestination.name + ".";
        Debug.Log(log);

        // send an introduction message
        MessageBoard.SendMessage
        (
            new DestinationMessage
            {
                destination = _intendedDestination,
                special = _special,
            }
        );
    }
}
