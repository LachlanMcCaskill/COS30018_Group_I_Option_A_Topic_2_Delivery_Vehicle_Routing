using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

/*public class Route
{
    public Stack<GameObject> Stops;

    public float TotalDistance;

    public Route(GameObject startingLocation)
    {
        Stops.Push(startingLocation);
    }

    public Route()
    {
        Stops.Push(GameObject.Find("Depot"));
    }
}*/

[System.Serializable]
public class Route
{
    public Stack<Vector3> Destinations = new Stack<Vector3>();

    public Route(Vector3 start)
    {
        Destinations.Push(start);
    }

    public float TotalDistance => Destinations
        .Zip(Destinations.Skip(1), Tuple.Create)
        .Aggregate(0.0f, (acc, val) => acc + Vector3.Distance(val.Item1, val.Item2));

    public void PrintRoute()
    {
        //Print route with gameObject name
    }
}