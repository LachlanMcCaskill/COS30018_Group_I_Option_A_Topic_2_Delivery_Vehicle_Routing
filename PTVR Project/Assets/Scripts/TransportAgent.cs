
using System.Collections.Generic;
using UnityEngine;
using MessageSystem;
using System;
using System.Linq;

public class TransportAgent : MonoBehaviour
{
    public float Speed;

    private List<Passenger> _passengers;

    [SerializeField] private int _capacity;
    [SerializeField] private bool _special;

    public bool Special
    {
        get
        {
            return _special;
        }
        set
        {
            _special = value;
        }
    }

    public int Capacity
    {
        get
        {
            return _capacity;
        }
        set
        {
            _capacity = value;
        }
    }

    private Route _route = null;
    private Route _movementRoute = null;
    private Color _color;
    private Lerped<Vector3> _target = new Lerped<Vector3>(Vector3.zero, 0.0f, Easing.EaseInOut);

	private LineRenderer _lineMap;
	private GameObject _depot;


    [SerializeField] private Mesh _sphere;
    [SerializeField] private Mesh _cube;

    //  [SerializeField] private Mesh _;

    private bool messageSent = false;

    public void LoadFromFile(bool special, int capacity)
    {
        _special = special;
        setShape();
        SetCapacity(capacity);
        //SetCapacity(PlayerPrefs.GetInt("Capacity"));
        MessageBoard.ListenForMessage<TransportAgentRouteMessage>(OnTransportAgentRouteMessage);
        SendIntroductionMessage();
        _color = _colors.Pop();
		_depot = GameObject.Find("Depot");
		InitializeLineRenderer();
    }

    public void GenerateFromPreferences()
    {
        GetSpecial();
        setShape();
        SetCapacity(PlayerPrefs.GetInt("Capacity"));
        MessageBoard.ListenForMessage<TransportAgentRouteMessage>(OnTransportAgentRouteMessage);
        SendIntroductionMessage();
        _color = _colors.Pop();
		_depot = GameObject.Find("Depot");
		InitializeLineRenderer();
    }

    //private void OnEnable()
    //{
    //}
	
	private void InitializeLineRenderer()
	{
		_lineMap = GetComponent<LineRenderer>();
		_lineMap.startColor = _color;
		_lineMap.endColor = _color;
	}

    public void SetCapacity(int capacity)
    {
        _capacity = capacity;
    }

    public void GetSpecial()
    {
        if (PlayerPrefs.GetString("SpecialRoute") == "True")
        {
            //  non-special agents need to be assigned first for reasons related to assigning special destinations, should resolve this a different way though
            int notSpecialCount = PlayerPrefs.GetInt("NonSpecialAgentCount");
            if (notSpecialCount > 0)
            {
                _special = false;
            }
            else _special = true;
            PlayerPrefs.SetInt("NonSpecialAgentCount", notSpecialCount - 1);
        }
        else _special = false;
    }

    private void setShape()
    {
        Mesh mesh;
        if (_special)
        {
            mesh = _sphere;
            gameObject.GetComponent<MeshRenderer>().material.color = Color.yellow;
        }
        else
        {
            mesh = _cube;
            gameObject.GetComponent<MeshRenderer>().material.color = Color.blue;
        }

        gameObject.GetComponent<MeshFilter>().mesh = mesh;
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
            _target.DurationSeconds = (1 / Speed) * distance;
        }

        transform.position = _target.Value;
    }

    private void DebugDrawRoute()
    {
        if (_route != null)
        {
            IEnumerable<Tuple<GameObject, GameObject>> pairs = _route.Destinations.Zip(_route.Destinations.Skip(1), Tuple.Create);
            foreach ((GameObject lhs, GameObject rhs) in pairs)
            {
                Debug.DrawLine(lhs.transform.position, rhs.transform.position, _color);
            }
            if (!messageSent)
            {
                DisplayCost();
            }
			DrawLine();
        }
    }

    private void DisplayCost()
    {
        SendCostMessage(_route.TotalDistance); //this can be any numeric value
        messageSent = true;
    }

	private void DrawLine()
	{
		Vector3[] points = _route.Destinations.Select(route => route.transform.position).ToArray();
		_lineMap.positionCount = points.Length + 2;
		_lineMap.SetPosition(0, _depot.transform.position);
		for (int i=0; i<points.Length; i++)
		{
			_lineMap.SetPosition(i+1, points[i]);
		}
		_lineMap.SetPosition(points.Length + 1, _depot.transform.position);
	}

    private void SendIntroductionMessage()
    {
        MessageBoard.SendMessage
        (
            new TransportAgentIntroductionMessage
            {
                TransportAgentId = GetInstanceID(),
                Capacity = _capacity,
                Special = _special,
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
    public bool Special;
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
