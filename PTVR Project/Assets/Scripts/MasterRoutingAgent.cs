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
	private bool _generatedRoute = false;

	private void OnEnable()
	{
		MessageBoard.ListenForMessage<TransportAgentIntroductionMessage>(OnTransportAgentIntroduction);
		MessageBoard.ListenForMessage<TransportAgentRetirementMessage>(OnTransportAgentRetirement);
	}

	private void OnDisable()
	{
		MessageBoard.StopListeningForMessage<TransportAgentIntroductionMessage>(OnTransportAgentIntroduction);
		MessageBoard.StopListeningForMessage<TransportAgentRetirementMessage>(OnTransportAgentRetirement);
	}

    private void Start()
    {
		_routeSolver = new GreedyRouteSolver();
		_transportNetwork = GameObject.Find("Network").GetComponent<TransportNetwork>();
		RouteAgents();
    }

	private void RouteAgents()
	{
		Log.Info("Sending route to agents...");
		Stack<GameObject>[] routes = CreateRoutes();
		SendRoutes(routes);
		_generatedRoute = true;
	}

    private void SendRoutes(Stack<GameObject>[] routes)
    {
		for (int i=0; i<_transportAgents.Count; i++)
		{
			TransportAgentIntroductionMessage agent = _transportAgents[i];
			Stack<GameObject> route = routes[i];

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

    private Stack<GameObject>[] CreateRoutes()
    {
		int transportAgentCount = _transportAgents.Count;
		if (transportAgentCount == 0) return new Stack<GameObject>[]{};
		Vector3 start = _transportNetwork.DepotDestination.transform.position;
        List<Vector3> points = _transportNetwork.DestinationPoints;
        List<Route> routes = _routeSolver.Solve(start, points, transportAgentCount);
		return routes.Select(route => _transportNetwork.ConvertRouteToDestinations(route)).ToArray();
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
}
