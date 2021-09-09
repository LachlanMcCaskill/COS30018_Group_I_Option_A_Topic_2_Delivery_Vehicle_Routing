using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TransportNetwork : MonoBehaviour
{
    public GameObject depotDestination;
    public GameObject[] Destinations;
	public GameObject stopPrefab;
	public int stopCount = 16;

	public void InitialiseNetwork()
	{
		//Debug.Log("Initialised Transport Network Values");
		depotDestination = GameObject.Find("Depot");
        Destinations = GameObject.FindGameObjectsWithTag("Stop");

		//if there are no stops in the scene, create them randomly
		if(Destinations.Length == 0 && stopPrefab != null)
		{
			Destinations = new GameObject[stopCount];
			for(int i = 0; i<stopCount; i++)
			{
				Vector3 randomPosition = new Vector3(Random.Range(0f, 100f), 0f, Random.Range(0f, 100f));
				GameObject destinationToAdd = Instantiate(stopPrefab, randomPosition, Quaternion.identity);
				destinationToAdd.name = (i+1).ToString();
				Destinations[i] = destinationToAdd;
			}
		}
		else if(Destinations.Length == 0 && stopPrefab == null)
		{
			Debug.Log("Could not find any stops in the scene and could not find a prefab to generate a network with.");
		}
		else
		{
			stopCount = Destinations.Length;
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
		destinations.Insert(0, depotDestination);
		destinations.Add(depotDestination);

		return destinations;
	}

	public void printNetwork()
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
