using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TransportNetwork
{
    public GameObject Start;
    public GameObject[] Destinations;

    public TransportNetwork()
    {
        Start = GameObject.Find("Depot");
        Destinations = GameObject.FindGameObjectsWithTag("Stop");
    }

	public List<Vector3> DestinationPoints => Destinations.Select(destination => destination.transform.position).ToList();

	public List<GameObject> ConvertRouteToDestinations(Route route) 
	{
		List<GameObject> destinations = route.Destinations
			.Select(point => Destinations.First(destination => destination.transform.position == point))
			.ToList();
		
		// add begining and end
		destinations.Insert(0, Start);
		destinations.Add(Start);

		return destinations;
	}
}
