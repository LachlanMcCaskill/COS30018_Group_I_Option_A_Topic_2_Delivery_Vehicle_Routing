using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class RoutePlan
{
    public Stack<Vector3> Destinations = new Stack<Vector3>();

    public float TotalDistance => Destinations
        .Zip(Destinations.Skip(1), Tuple.Create)
        .Aggregate(0.0f, (acc, val) => acc + Vector3.Distance(val.Item1, val.Item2));
}
