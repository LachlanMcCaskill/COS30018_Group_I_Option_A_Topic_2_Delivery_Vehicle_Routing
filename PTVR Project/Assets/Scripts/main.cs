using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using RouteSolver;

public class main : MonoBehaviour
{
	// there is a List<GameObject> for each vehicle
	List<List<GameObject>> _routes;
	List<Color> _colors = new List<Color>
	{
		Color.red,
		Color.green,
		Color.blue,
		Color.yellow,
		Color.cyan,
	};

    // Start is called before the first frame update
    void Start()
    {
		TransportNetwork network = GameObject.Find("Network").GetComponent<TransportNetwork>();
		network.InitialiseNetwork();

        //create the route solver
        IRouteSolver rs = new GreedyRouteSolver();

        //create the passengers
        List<Passenger> passengersToTransport = new List<Passenger>();
        foreach(GameObject destination in network.Destinations)
        {
			//Debug.Log(destination.name);
            passengersToTransport.Add(new Passenger(destination));
        }

        // create the transportAgents (buses)
        List<TransportAgent> transportAgents = new List<TransportAgent>();
        transportAgents.Add(new TransportAgent(5));
        transportAgents.Add(new TransportAgent(5));
        transportAgents.Add(new TransportAgent(6));

		// solve routes
		Vector3 start = network.DepotDestination.transform.position;
		network.PrintNetwork();
        List<Vector3> points = network.DestinationPoints;
		int vehicleCount = transportAgents.Count();
        List<Route> routes = rs.Solve(start, points, vehicleCount);
		_routes = routes.Select(route => network.ConvertRouteToDestinations(route)).ToList();

		foreach(List<GameObject> objs in _routes)
		{
			print("Route: " + string.Join(", ", objs.Select(obj => obj.name)));
		}
    }

	private void Update()
	{
		/*for (int i=0; i<_routes.Count; i++)
		{
			List<GameObject> route = _routes[i];
			Color color = _colors[i];

			IEnumerable<Tuple<GameObject,GameObject>> pairs = route.Zip(route.Skip(1), Tuple.Create);
			foreach((GameObject lhs, GameObject rhs) in pairs)
			{
				Debug.DrawLine(lhs.transform.position, rhs.transform.position, color);
			}
		}*/
	}
}
