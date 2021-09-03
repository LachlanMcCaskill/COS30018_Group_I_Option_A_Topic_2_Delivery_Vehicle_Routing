using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class RouteSolver : MonoBehaviour
{
    public TransportNetwork _transportNetwork;

    public RouteSolver()
    {
        _transportNetwork = new TransportNetwork();
    }  

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void passengerSetup()
    {

    }

    public List<Route> Solve(Vector3 start, List<Vector3> points, int vehicleCount)
    {
        List<Route> result = new List<Route>();

        for (int i=0; i<vehicleCount; i++) 
        {
            result.Add(new Route(start));
        }

        // while there are still points avaliable
        while (result.Count > 0)
        {
            // pick route with lowest total distance
            Route route = result.Aggregate((cur, next) => cur.TotalDistance <= next.TotalDistance ? cur : next);

            // get the last point the route traveled to
            Vector3 currentPoint = route.Destinations.Peek();

            // find the nearest avaliable point to that point
            Vector3 nearestPoint = points
                .Select(p => Tuple.Create(p, Vector3.Distance(currentPoint, p)))
                .Aggregate((cur, next) => cur.Item2 <= next.Item2 ? cur : next)
                .Item1;

            // remove that point from the avaliable points list
            points.Remove(nearestPoint);

            // add that point to the route
            route.Destinations.Push(nearestPoint);
        }

        return result;
    }

    public void PrintSolutionToTerminal()
    {
        Debug.Log("Printing Solution to DRV Problem");
        
    }
}
