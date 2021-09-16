using System.Collections.Generic;
using UnityEngine;
using MessageSystem;
using System;
using System.Linq;

public class TransportAgent : MonoBehaviour
{
    private List<Passenger> _passengers;
    private int _capacity;
    private Stack<GameObject> _currentRoute = null;
	private Color _color;

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
		DebugDrawRoute();
	}

	private void DebugDrawRoute()
	{
		if (_currentRoute != null)
		{
			IEnumerable<Tuple<GameObject,GameObject>> pairs = _currentRoute.Zip(_currentRoute.Skip(1), Tuple.Create);
			foreach((GameObject lhs, GameObject rhs) in pairs)
			{
				Debug.DrawLine(lhs.transform.position, rhs.transform.position, _color);
			}
		}
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

    private void AddPassenger(Passenger p)
    {
        _passengers.Add(p);
    }

	private void OnTransportAgentRouteMessage(TransportAgentRouteMessage message)
	{
		if (message.TransportAgentId == GetInstanceID())
		{
			_currentRoute = message.Route;
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
	public Stack<GameObject> Route;
}
