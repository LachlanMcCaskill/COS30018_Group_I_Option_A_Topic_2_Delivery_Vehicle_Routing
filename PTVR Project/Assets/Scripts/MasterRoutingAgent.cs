using System.Collections.Generic;
using UnityEngine;
using MessageSystem;
using RouteSolver;
using System.Linq;

public class MasterRoutingAgent : MonoBehaviour
{
    private IRouteSolver _routeSolver;
    private TransportNetwork _transportNetwork;
	private List<TransportAgentIntroductionMessage> _transportAgents = new List<TransportAgentIntroductionMessage>();
	private List<DestinationMessage> _passengerData = new List<DestinationMessage>();
	private bool _generatedRoute = false;

	private void OnEnable()
	{
		MessageBoard.ListenForMessage<DestinationMessage>(OnPassengerIntroduction);
		MessageBoard.ListenForMessage<TransportAgentIntroductionMessage>(OnTransportAgentIntroduction);
		MessageBoard.ListenForMessage<TransportAgentRetirementMessage>(OnTransportAgentRetirement);
	}

	private void OnDisable()
	{
		MessageBoard.StopListeningForMessage<TransportAgentIntroductionMessage>(OnTransportAgentIntroduction);
		MessageBoard.StopListeningForMessage<TransportAgentRetirementMessage>(OnTransportAgentRetirement);
		//MessageBoard.StopListeningForMessage<DestinationMessage>(OnPassengerIntroduction);
	}

    private void Start()
    {
		//_routeSolver = new GreedyRouteSolver();
		_routeSolver = new AlternativeGreedyRouteSolver();
		_transportNetwork = GameObject.Find("Network").GetComponent<TransportNetwork>();
		RouteAgents();
    }

	private void RouteAgents()
	{
		Log.Info("Sending route to agents...");
		Route[] routes = CreateRoutes();
		SendRoutes(routes);
		_generatedRoute = true;
	}

    private void SendRoutes(Route[] routes)
    {
		for (int i=0; i<_transportAgents.Count; i++)
		{
			TransportAgentIntroductionMessage agent = _transportAgents[i];
			Route route = routes[i];

			MessageBoard.SendMessage
			(
				new TransportAgentRouteMessage
				{
					TransportAgentId = agent.TransportAgentId,
					Route = route,
				}
			);
		}
    }

    private Route[] CreateRoutes()
    {
		int transportAgentCount = _transportAgents.Count;
		if (transportAgentCount == 0) return new Route[]{};

		Vector3 start = _transportNetwork.DepotDestination.transform.position;
		List<Vector3> points = new List<Vector3>();
		if(_passengerData.Count > 0)
		{
			foreach(DestinationMessage p in _passengerData)
			{
				if(!points.Contains(p.destination.transform.position))
				{
					points.Add(p.destination.transform.position);
					Debug.Log("Added: "+p.destination.transform.position.ToString()+" to destinations.");
				}
			}
			List<RoutePlan> routePlans = _routeSolver.Solve(start, points, _transportAgents);
			return routePlans.Select(routePlan => _transportNetwork.CreateRouteFromPlan(routePlan)).ToArray();
		}
		else
		{
			Debug.Log("No passengers for which to create routes for.");
			return null;
		}
    }

	private void OnTransportAgentIntroduction(TransportAgentIntroductionMessage message)
	{
		_transportAgents.Add(message);

		// if agents already exist and have been given routes, regenerate the
		// routes so there is one for the new agent
		if (_generatedRoute) RouteAgents();
	}

	private void OnTransportAgentRetirement(TransportAgentRetirementMessage message)
	{
		_transportAgents.RemoveAll(agent => agent.TransportAgentId == message.TransportAgentId);

		// regenerate route when an agent retires
		if (_generatedRoute) RouteAgents();
	}

	private void OnPassengerIntroduction(DestinationMessage message)
	{
		_passengerData.Add(message);
	}
}
