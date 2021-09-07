using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TransportNetwork : MonoBehaviour
{
    public GameObject depotDestination;
    public GameObject[] Destinations;
	public GameObject stopPrefab;
	public int stopCount = 16;

	public void Start()
	{
		depotDestination = GameObject.Find("Depot");
        Destinations = GameObject.FindGameObjectsWithTag("Stop");

		//if there are no stops in the scene, create them randomly
		if(Destinations.Length == 0 && stopPrefab != null)
		{
			for(int i = 0; i<stopCount; i++)
			{
				Vector3 randomPosition = new Vector3(Random.Range(0f, 100f), 0f, Random.Range(0f, 100f));
				Instantiate(stopPrefab, randomPosition, Quaternion.identity);
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
}
