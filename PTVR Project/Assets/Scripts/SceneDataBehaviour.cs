using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

class SceneDataBehaviour : MonoBehaviour
{
	public GameObject StopPrefab;
	public GameObject TransportAgentPrefab;
	public GameObject PassengerPrefab;

	private GameObject Network;
	private GameObject PassengerList;

	private void Awake()
	{
		Network = GameObject.Find("Network");
		PassengerList = GameObject.Find("PassengerList");
		Load();
	}

	private void Save()
	{
		SceneDataSerialized sceneData = new SceneDataSerialized
		{
			Stops = FindStops(),
			TransportAgents = FindTransportAgents(),
			Passengers = FindPassengers(),
		};
		string content = JsonConvert.SerializeObject(sceneData, Formatting.Indented);
		File.WriteAllText("SceneData.json", content);

		// helper functions

		SceneDataSerialized.Stop[] FindStops() => GameObject
			.FindGameObjectsWithTag("Stop")
			.Select(stop => new SceneDataSerialized.Stop
			{
				Name = stop.name,
				Position = stop.transform.position,
			})
			.ToArray();

		SceneDataSerialized.TransportAgent[] FindTransportAgents() => GameObject
			.FindObjectsOfType<TransportAgent>()
			.Select(agent => new SceneDataSerialized.TransportAgent
			{
				Name = agent.name,
				Capacity = agent.Capacity,
			})
			.ToArray();

		SceneDataSerialized.Passenger[] FindPassengers() => GameObject
			.FindObjectsOfType<Passenger>()
			.Select(passenger => new SceneDataSerialized.Passenger
			{
				Name = passenger.name,
				Destination = passenger.IntendedDestination.name,
			})
			.ToArray();
	}

	private void Load()
	{
		string sceneDataString = File.ReadAllText("SceneData.json");
		SceneDataSerialized sceneData = JsonConvert.DeserializeObject<SceneDataSerialized>(sceneDataString);

		foreach (SceneDataSerialized.Stop stop in sceneData.Stops)
		{
			GameObject obj = GameObject.Instantiate(StopPrefab, stop.Position, Quaternion.identity, Network.transform);
			obj.transform.position = stop.Position;
			obj.name = stop.Name;
		}

		foreach (SceneDataSerialized.Passenger passenger in sceneData.Passengers)
		{
			Passenger p = GameObject.Instantiate(PassengerPrefab, Vector3.zero, Quaternion.identity, PassengerList.transform).GetComponent<Passenger>();
			p.IntendedDestination = GameObject.Find(passenger.Destination);
			p.SendDestination();
		}

		foreach (SceneDataSerialized.TransportAgent transportAgent in sceneData.TransportAgents)
		{
			TransportAgent a = GameObject.Instantiate(TransportAgentPrefab, Vector3.zero,  Quaternion.identity).GetComponent<TransportAgent>();
			a.name = transportAgent.Name;
			a.Capacity = transportAgent.Capacity;
		}
	}

	private struct SceneDataSerialized
	{
		public Stop[] Stops;  
		public TransportAgent[] TransportAgents;
		public Passenger[] Passengers;

		public struct Stop
		{
			public string Name;
			public Vector3 Position;
		}

		public struct TransportAgent
		{
			public string Name;
			public int Capacity;
		}

		public struct Passenger
		{
			public string Name;
			public string Destination;
		}
	}
}
