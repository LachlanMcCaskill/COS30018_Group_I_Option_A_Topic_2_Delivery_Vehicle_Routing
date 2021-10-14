using System.Collections.Generic;
using UnityEngine;
using MessageSystem;
using System;
using System.Linq;

public class TransportAgent : MonoBehaviour
{
	public float Speed;

    private List<Passenger> _passengers;
    private int _capacity = 6;  // agents weren't assigned capacity
    private Route _route = null;
    private Route _movementRoute = null;
	private Color _color;
	private Lerped<Vector3> _target = new Lerped<Vector3>(Vector3.zero, 0.0f, Easing.EaseInOut);

	private bool messageSent = false;

	private void OnEnable()
	{
		MessageBoard.ListenForMessage<TransportAgentRouteMessage>(OnTransportAgentRouteMessage);
		SendIntroductionMessage();
		_color = _colors.Pop();
	}

	private void OnDisable()
	{
		MessageBoard.StopListeningForMessage<TransportAgentRouteMessage>(OnTransportAgentRouteMessage);
		SendRetirementMessage();
		_colors.Push(_color);
	}

	private void Update()
	{
		UpdateMovement();
		DebugDrawRoute();
	}

	private void UpdateMovement()
	{
		bool hasRoute = _movementRoute != null;
		bool hasReachedDestination = _target.InterpolationComplete;
		bool moreDestinationsExist = _movementRoute.Destinations.Count != 0;

		if (hasRoute && hasReachedDestination && moreDestinationsExist)
		{
			Vector3 next = _movementRoute.Destinations.Pop().transform.position;
			float distance = Vector3.Distance(_target.Value, next);
			_target.Value = next;
			_target.DurationSeconds = (1/Speed) * distance;
		}

		transform.position = _target.Value;
	}

	private void DebugDrawRoute()
	{
		if (_route != null)
		{
			IEnumerable<Tuple<GameObject,GameObject>> pairs = _route.Destinations.Zip(_route.Destinations.Skip(1), Tuple.Create);
			foreach((GameObject lhs, GameObject rhs) in pairs)
			{
				Debug.DrawLine(lhs.transform.position, rhs.transform.position, _color);
			}
			if(!messageSent)
			{
				DisplayCost();
			}
		}
	}

	private void DisplayCost()
	{
		SendCostMessage(_route.TotalDistance); //this can be any numeric value
		messageSent = true;
	}

	private void SendIntroductionMessage()
	{
		MessageBoard.SendMessage
		(
			new TransportAgentIntroductionMessage
			{
				TransportAgentId = GetInstanceID(),
				Capacity = _capacity,
			}		
		);
	}

	private void SendRetirementMessage()
	{
		MessageBoard.SendMessage
		(
			new TransportAgentRetirementMessage
			{
				TransportAgentId = GetInstanceID(),
			}		
		);
	}

	private void SendCostMessage(float routecost)
	{
		MessageBoard.SendMessage
		(
			new TransportAgentCostMessage
			{
				routeColour = _color,
				cost = routecost,
			}
		);
	}

    private void AddPassenger(Passenger p)
    {
        _passengers.Add(p);
    }

	private void OnTransportAgentRouteMessage(TransportAgentRouteMessage message)
	{
		_target.Value = GameObject.Find("Depot").transform.position;
		_target.DurationSeconds = 0.0f;
		if (message.TransportAgentId == GetInstanceID())
		{
			_route = message.Route.Copy();
			_movementRoute = message.Route.Copy();
		}
	}

	private static Stack<Color> _colors = new Stack<Color>
	(
		new Color[] {
			Color.cyan,
			Color.yellow,
			Color.blue,
			Color.green,
			Color.red,
		}
	);
}

public class TransportAgentIntroductionMessage : Message
{
	public int TransportAgentId;
	public int Capacity;
}

public class TransportAgentRetirementMessage : Message
{
	public int TransportAgentId;
}

public class TransportAgentRouteMessage : Message
{
	public int TransportAgentId;
	public Route Route;
}

public class TransportAgentCostMessage : Message
{
	public float cost;
	public Color routeColour;
}
