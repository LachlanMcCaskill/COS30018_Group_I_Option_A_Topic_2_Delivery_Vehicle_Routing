using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Route
{
    public Stack<GameObject> Destinations = new Stack<GameObject>();

	/// <summary>
	/// The accumulated distance between each of the destination points.
	/// </summary>
    public float TotalDistance => Destinations
        .Zip(Destinations.Skip(1), Tuple.Create)
        .Aggregate(0.0f, (acc, val) => acc + Vector3.Distance(val.Item1.transform.position, val.Item2.transform.position));

	public Route Copy()
	{
		Route route = new Route();
		route.Destinations = new Stack<GameObject>(Destinations);
		return route;
	}
}
