using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using RouteSolver;

public class main : MonoBehaviour
{
    public int GetStopCount()
    {
        return stopCount;
    }

    public int GetPassengerCount()
    {
        return passengerCount;
    }

    public int GetSpecialPassengerCount()
    {
        return passengerCount;
    }

    public int GetTransportAgentCount()
    {
        return transportAgentCount;
    }

    // Variables
    private int stopCount;
    private int passengerCount;
    private int specialPassengerCount;
    private int transportAgentCount;
    private int largeTransportAgentCount;

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
        List<Passenger> passengersToTransport = PassengerGeneration(network);

        // create the transportAgents (buses)
        List<TransportAgent> transportAgents = TransportGeneration();

//  <<<<<<< Updated upstream
		// solve routes
		Vector3 start = network.DepotDestination.transform.position;
		network.PrintNetwork();

//  >>>>>>> Stashed changes
        List<Vector3> points = network.DestinationPoints;
		int vehicleCount = transportAgents.Count();
        List<Route> routes = rs.Solve(start, points, vehicleCount);
		_routes = routes.Select(route => network.ConvertRouteToDestinations(route)).ToList();

		foreach(List<GameObject> objs in _routes)
		{
			print("Route: " + string.Join(", ", objs.Select(obj => obj.name)));
		}

        print("Passengers: " + passengerCount + " including " + specialPassengerCount + " special passengers.");
        print("Transport Agents: " + transportAgentCount+ " including " + largeTransportAgentCount + " large Transports.");
        int totalCapacity = transportAgentCount * TransportAgent.SmallCapacity() + largeTransportAgentCount * (TransportAgent.LargeCapacity() - TransportAgent.SmallCapacity());
        int specialCapacity = transportAgentCount * TransportAgent.SmallSpecialCapacity() + largeTransportAgentCount * (TransportAgent.LargeSpecialCapacity() - TransportAgent.SmallSpecialCapacity());
        print("Total Capacity: " + totalCapacity + " including capacity for " + specialCapacity + " special passengers");
    }

    private List<Passenger> PassengerGeneration(TransportNetwork network)
    {
        List<Passenger> passengersToTransport = new List<Passenger>();

        // currently creates exactly one passenger per destination
        // special passengers have a random possibility of being generated
        foreach (GameObject destination in network.Destinations)
        {
            //Debug.Log(destination.name);
            Passenger newPassenger = new Passenger(destination);
            passengersToTransport.Add(newPassenger);

            passengerCount++;
            if (newPassenger.IsSpecial()) specialPassengerCount++;
        }
        return passengersToTransport;
    }

    private List<TransportAgent> TransportGeneration()
    {
        List<TransportAgent> transportAgents = new List<TransportAgent>();

        int curCapacity = 0;
        int curSpecialCapacity = 0;

        // make enough transport agents for all passengers
        // need to have both enough capacity and special capacity
        while (curCapacity < passengerCount || curSpecialCapacity < specialPassengerCount)
        {
            TransportAgent newTransport;
            transportAgentCount++;

            if (UnityEngine.Random.Range(0, 1f) < 0.35)
            {
                newTransport = new TransportAgent("large");
                curCapacity += TransportAgent.LargeCapacity();
                curSpecialCapacity += TransportAgent.LargeSpecialCapacity();
                largeTransportAgentCount++;
            }
            else
            {
                newTransport = new TransportAgent("small");
                curCapacity += TransportAgent.SmallCapacity();
                curSpecialCapacity += TransportAgent.SmallSpecialCapacity();
            }

            transportAgents.Add(newTransport);
        }

        //  transportAgents.Add();  //  new TransportAgent(4)
        //  transportAgents.Add();  //  new TransportAgent(8)

        return transportAgents;
    }

	private void Update()
	{
		for (int i=0; i<_routes.Count; i++)
		{
			List<GameObject> route = _routes[i];
			Color color = _colors[i%_colors.Count];

			IEnumerable<Tuple<GameObject,GameObject>> pairs = route.Zip(route.Skip(1), Tuple.Create);
			foreach((GameObject lhs, GameObject rhs) in pairs)
			{
				Debug.DrawLine(lhs.transform.position, rhs.transform.position, color);
			}
		}
	}
}
