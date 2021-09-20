using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TransportNetwork : MonoBehaviour
{
	public GameObject StopPrefab;
	public int StopCount = 16;
    public GameObject[] Destinations { get; private set; }
    public GameObject DepotDestination { get; private set; }

	public void Start()
	{
		DepotDestination = GameObject.Find("Depot");
		Destinations = GameObject.FindGameObjectsWithTag("Stop");

		//if there are no stops in the scene, create them randomly
		if(Destinations.Length == 0 && StopPrefab != null)
		{
			Destinations = new GameObject[StopCount];
			for(int i = 0; i<StopCount; i++)
			{
				Vector3 randomPosition = new Vector3(Random.Range(0f, 100f), 0f, Random.Range(0f, 100f));
				GameObject destinationToAdd = Instantiate(StopPrefab, randomPosition, Quaternion.identity);
				destinationToAdd.name = (i+1).ToString();
				Destinations[i] = destinationToAdd;
			}
		}
		else if(Destinations.Length == 0 && StopPrefab == null)
		{
			Log.Info("Could not find any stops in the scene and could not find a prefab to generate a network with.");
		}
		else
		{
			StopCount = Destinations.Length;
		}
	}

	public List<Vector3> DestinationPoints => Destinations.Select(destination => destination.transform.position).ToList();

	public Route CreateRouteFromPlan(RoutePlan routePlan) 
	{
		Route route = new Route();

		// add depot to stack as start
		route.Destinations.Push(DepotDestination);
		
		// add destinations to stack
		IEnumerable<GameObject> routeObjs = routePlan.Destinations.Select(point => Destinations.First(destination => destination.transform.position == point));
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
