using System.Collections.Generic;
using UnityEngine;
using MessageSystem;
using RouteSolver;
using System.Linq;
using UnityEngine.SceneManagement;

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
//<<<<<<< HEAD
        if (PlayerPrefs.HasKey("RoutingStrategy"))
        {
            switch (PlayerPrefs.GetString("RoutingStrategy"))
            {
                case "Greedy1":
                    _routeSolver = new GreedyRouteSolver();
                    break;
                case "Greedy2":
                    _routeSolver = new AlternativeGreedyRouteSolver();
                    break;
                case "KMeans1":
                    _routeSolver = new KMeansClusterRouteSolver(new GreedyRouteSolver());
                    break;
                case "KMeans2":
                    _routeSolver = new KMeansClusterRouteSolver(new AlternativeGreedyRouteSolver());
                    break;
                case "GA":
                    _routeSolver = new GeneticRouteSolver();
                    break;
                case "KMeansGA":
                    _routeSolver = new KMeansClusterRouteSolver(new GeneticRouteSolver());
                    break;
                default:
                    break;
            }

            _transportNetwork = GameObject.Find("Network").GetComponent<TransportNetwork>();

            RouteAgents();
        }
        else
        {
            Debug.Log("Player prefs not set. If you're seeing this in unity, start the game from the menu scene");
        }
//=======
//        _routeSolver = new GreedyRouteSolver();
//        //  _routeSolver = new GeneticRouteSolver();
//          //_routeSolver = new KMeansClusterRouteSolver(new GeneticRouteSolver());
//        _transportNetwork = GameObject.Find("Network").GetComponent<TransportNetwork>();
//		RouteAgents();
//>>>>>>> save-load
    }

    private void RouteAgents()
    {
        Log.Info("Sending route to agents...");
        Route[] routes = CreateRoutes();
        if (routes != null)
        {
            SendRoutes(routes);
            _generatedRoute = true;
        }
    }

    private void SendRoutes(Route[] routes)
    {
        for (int i = 0; i < _transportAgents.Count; i++)
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
        if (SceneManager.GetActiveScene().name == "Routing" && _transportNetwork.DepotDestination != null)
        {
            int transportAgentCount = _transportAgents.Count;
            if (transportAgentCount == 0) return new Route[] { };

            Vector3 start = _transportNetwork.DepotDestination.transform.position;
            List<Vector3> points = new List<Vector3>();

            List<DestinationMessage> destinations = new List<DestinationMessage>();

            if (_passengerData.Count > 0)
            {
                foreach (DestinationMessage p in _passengerData)
                {
                    destinations.Add(p);

                    if (!points.Contains(p.destination.transform.position))
                    {
                        points.Add(p.destination.transform.position);
                        Debug.Log("Added: " + p.destination.transform.position.ToString() + " to destinations.");
                    }
                }

                List<RoutePlan> routePlans = _routeSolver.Solve(start, points, _transportAgents, destinations);
                return routePlans.Select(routePlan => _transportNetwork.CreateRouteFromPlan(routePlan)).ToArray();
            }
            else
            {
                Debug.Log("No passengers for which to create routes.");
                return null;
            }
        }
        else
        {
            Debug.Log("Routing only possible in routing scene with a transport network with a non-null depot variable.");
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

        //  regenerate route when an agent retires
        //  this causes routes to be regenerated when you press back or quit
        //  if (_generatedRoute) RouteAgents(); 
    }

    private void OnPassengerIntroduction(DestinationMessage message)
    {
        _passengerData.Add(message);
    }
}
