using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TransportNetwork : MonoBehaviour
{
    public GameObject agentPrefab;
	public GameObject StopPrefab;
    public GameObject passengerPrefab;
    //  public int StopCount = 16;
    public GameObject[] Destinations { get; private set; }
    public GameObject DepotDestination { get; private set; }
    public GameObject passengerList;

    public int GetStopCount
    {
        get
        {
            return PlayerPrefs.GetInt("Points");
        }
    }

    public void Start()
    {
        DepotDestination = GameObject.Find("Depot");
        if (PlayerPrefs.GetString("Mode") == "Generate")
        {
            Debug.Log("Mode: Generating new environment.    PlayerPrefs.GetString(Mode) = " + PlayerPrefs.GetString("Mode"));
            CreateRandomPoints();
            CreatePassengers();
            CreateTransportAgents();
        }
        else
        {
            Debug.Log("Mode: Loading from file.    PlayerPrefs.GetString(Mode) = " + PlayerPrefs.GetString("Mode"));
            FindObjectOfType<SceneDataBehaviour>().Load();
            Destinations = GameObject.FindGameObjectsWithTag("Stop");
            //  StopCount = Destinations.Length;
        }
    }

    private void CreatePassengers()
    {
        for (int i = 0; i < Destinations.Length; i++)
        {
            GameObject passenger = Instantiate(passengerPrefab, passengerList.transform);
            passenger.GetComponent<Passenger>().GenerateFromPreferences(Destinations[i]);
            passenger.name = "Passenger " + (i + 1).ToString();
        }
    }

    public GameObject CreateStop(int i)
    {
        Vector3 randomPosition = new Vector3(Random.Range(0f, 100f), 0f, Random.Range(0f, 100f));
        GameObject destinationToAdd = Instantiate(StopPrefab, randomPosition, Quaternion.identity, transform);
        destinationToAdd.name = "Stop " + (i + 1).ToString();

        //Debug.Log("Randomize: Creating stop at  " + randomPosition);
        return destinationToAdd;
    }

    public void CreateRandomPoints()
    {
        foreach (GameObject s in GameObject.FindGameObjectsWithTag("Stop"))
        {
            DestroyImmediate(s.gameObject);
        }
        foreach (GameObject s in GameObject.FindGameObjectsWithTag("Passenger"))
        {
            DestroyImmediate(s.gameObject);
        }
        //if there are no stops in the scene, create them randomly
        if (StopPrefab != null)
        {
            Destinations = new GameObject[GetStopCount];
            for (int i = 0; i < GetStopCount; i++)
            {
                //Vector3 randomPosition = new Vector3(Random.Range(0f, 100f), 0f, Random.Range(0f, 100f));
                //GameObject destinationToAdd = Instantiate(StopPrefab, randomPosition, Quaternion.identity);
                //destinationToAdd.name = (i + 1).ToString();
                Destinations[i] = CreateStop(i);
            }
        }
        else
        {
            Log.Info("Could not find a prefab to generate a network with.");
        }
    }

    private void CreateTransportAgents()
    {
        foreach (TransportAgent a in FindObjectsOfType<TransportAgent>())
        {
            Destroy(a.gameObject);
        }

        for (int i = 0; i < PlayerPrefs.GetInt("AgentCount"); i++)
        {
            CreateAgent();
        }
    }

    public void CreateAgent()
    {
        GameObject newAgent = Instantiate(agentPrefab);
        newAgent.GetComponent<TransportAgent>().GenerateFromPreferences();
    }

    public List<Vector3> DestinationPoints => Destinations.Select(destination => destination.transform.position).ToList();

	public Route CreateRouteFromPlan(RoutePlan routePlan) 
	{
        Route route = new Route();

		// add depot to stack as start
		route.Destinations.Push(DepotDestination);
       
        //  to make multiple trips by one agent possible, ie returning to depot between trips, we need to add the depot to the array from which destinations are assigned
        GameObject[] withDepot = Destinations.Concat(new GameObject[] { DepotDestination }).ToArray();

        // add destinations to stack
        IEnumerable<GameObject> routeObjs = routePlan.Destinations.Select(point => withDepot.First(destination => destination.transform.position == point));
		foreach (GameObject obj in routeObjs)
		{
            route.Destinations.Push(obj);
		}

		// add depot to stack as end
		route.Destinations.Push(DepotDestination);

		return route;
	}

	public void PrintNetwork()
	{
		foreach(GameObject g in Destinations)
		{
			Log.Info(g.name);
		}
		if(Destinations.Length == 0)
		{
			Log.Info("No destinations to print!");
		}
	}
}
