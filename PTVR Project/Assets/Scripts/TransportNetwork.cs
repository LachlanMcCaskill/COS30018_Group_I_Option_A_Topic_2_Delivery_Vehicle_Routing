using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TransportNetwork : MonoBehaviour
{
	public GameObject StopPrefab;
	public int StopCount = 16;
    public GameObject[] Destinations { get; private set; }
    public GameObject DepotDestination { get; private set; }

	public void InitialiseNetwork()
	{
		//Debug.Log("Initialised Transport Network Values");
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
			Debug.Log("Could not find any stops in the scene and could not find a prefab to generate a network with.");
		}
		else
		{
			StopCount = Destinations.Length;
		}
	}

    public TransportNetwork()
    {
		
    }

	public List<Vector3> DestinationPoints => Destinations.Select(destination => destination.transform.position).ToList();

	public List<GameObject> ConvertRouteToDestinations(Route route) 
	{
		List<GameObject> destinations = route.Destinations
			.Select(point => Destinations.First(destination => destination.transform.position == point))
			.ToList();
		
		// add begining and end
		destinations.Insert(0, DepotDestination);
		destinations.Add(DepotDestination);

		return destinations;
	}

	public void PrintNetwork()
	{
		foreach(GameObject g in Destinations)
		{
			Debug.Log(g.name);
		}
		if(Destinations.Length == 0)
		{
			Debug.Log("No destinations to print!");
		}
	}
}
