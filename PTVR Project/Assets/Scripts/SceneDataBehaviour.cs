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
        // Load();  moved this to the masterroutingagent
	}

	public void Save()
	{
		SceneDataSerialized sceneData = new SceneDataSerialized
		{
			Stops = FindStops(),
            TransportAgents = FindTransportAgents(),
			Passengers = FindPassengers(),
		};
		string content = JsonConvert.SerializeObject(sceneData, Formatting.Indented);

        
		File.WriteAllText(Application.dataPath + "/SceneData.json", content);
        
		// helper functions

		SceneDataSerialized.Stop[] FindStops() => GameObject
			.FindGameObjectsWithTag("Stop")
			.Select(stop => new SceneDataSerialized.Stop
			{
				Name = stop.name,
                PositionX = stop.transform.position.x,
                PositionY = stop.transform.position.y,
                PositionZ = stop.transform.position.z,
                //Position = new Vector3(stop.transform.position.x, stop.transform.position.y, stop.transform.position.z),
            })
			.ToArray();

		SceneDataSerialized.TransportAgent[] FindTransportAgents() => GameObject
			.FindObjectsOfType<TransportAgent>()
			.Select(agent => new SceneDataSerialized.TransportAgent
			{
				Name = agent.name,
				Capacity = agent.Capacity,
                Special = agent.Special,    // added special quality
			})
			.ToArray();

        SceneDataSerialized.Passenger[] FindPassengers() => GameObject
            .FindObjectsOfType<Passenger>()
            .Select(passenger => new SceneDataSerialized.Passenger
            {
                Name = passenger.name,
                Destination = passenger.IntendedDestination.name,
                Special = passenger.Special,   // added special quality
            })
			.ToArray();
	}

	public void Load()
	{
        string sceneDataString = File.ReadAllText(Application.dataPath + "/SceneData.json");
		SceneDataSerialized sceneData = JsonConvert.DeserializeObject<SceneDataSerialized>(sceneDataString);

        int agentCount = 0;
        int specialAgentCount = 0;
        int capacity = 0;
        int passengerCount = 0;
        int specialPassengerCount = 0;

        foreach (SceneDataSerialized.Stop fileStop in sceneData.Stops)
		{
            Vector3 position = new Vector3(fileStop.PositionX, fileStop.PositionY, fileStop.PositionZ); // passing a vector3 to SceneDataSerialized
            GameObject obj = GameObject.Instantiate(StopPrefab, position, Quaternion.identity, Network.transform);
            obj.transform.position = position;
            obj.name = fileStop.Name;
		}

        // reverse this to maintain order
        for (int i = sceneData.Passengers.Length; i > 0; i--)
        {
            passengerCount++;
            if (sceneData.Passengers[i - 1].Special)
            {
                specialPassengerCount++;
            }

            Passenger p = GameObject.Instantiate(PassengerPrefab, Vector3.zero, Quaternion.identity, PassengerList.transform).GetComponent<Passenger>();
            p.LoadFromFile(GameObject.Find(sceneData.Passengers[i - 1].Destination), sceneData.Passengers[i - 1].Special, sceneData.Passengers[i - 1].Name);  // created method to load
        }
        //foreach (SceneDataSerialized.Passenger filePassenger in sceneData.Passengers)
        //{
        //    passengerCount++;
        //    if (filePassenger.Special)
        //    {
        //        specialPassengerCount++;
        //    }

        //    Passenger p = GameObject.Instantiate(PassengerPrefab, Vector3.zero, Quaternion.identity, PassengerList.transform).GetComponent<Passenger>();
        //    p.LoadFromFile(GameObject.Find(filePassenger.Destination), filePassenger.Special, filePassenger.Name);  // created method to load
        //    //p.IntendedDestination = GameObject.Find(passenger.Destination);
        //    //p.SendDestination();
        //}


        // reverse this to maintain order
        for (int i = sceneData.TransportAgents.Length; i > 0; i--)
        {
            capacity = sceneData.TransportAgents[i-1].Capacity; // all agents same capacity
            agentCount++;
            if (sceneData.TransportAgents[i - 1].Special)
            {
                specialAgentCount++;
            }
            
            TransportAgent a = GameObject.Instantiate(TransportAgentPrefab, Vector3.zero, Quaternion.identity).GetComponent<TransportAgent>();
            a.LoadFromFile(sceneData.TransportAgents[i - 1].Special, sceneData.TransportAgents[i - 1].Capacity);  // created method to load
        }
  //      foreach (SceneDataSerialized.TransportAgent fileTransportAgent in sceneData.TransportAgents)
		//{
  //          capacity = fileTransportAgent.Capacity; // all agents same capacity
  //          agentCount++;
  //          if (fileTransportAgent.Special)
  //          {
  //              specialAgentCount++;
  //          }
  //          TransportAgent a = GameObject.Instantiate(TransportAgentPrefab, Vector3.zero,  Quaternion.identity).GetComponent<TransportAgent>();
  //          a.LoadFromFile(fileTransportAgent.Special, fileTransportAgent.Capacity);  // created method to load
  //          //a.name = transportAgent.Name;
  //          //a.Capacity = transportAgent.Capacity;
  //      }

        //  get varaibles to display

        PlayerPrefs.SetInt("AgentCount", agentCount);
        PlayerPrefs.SetInt("SpecialAgentCount", specialAgentCount);
        PlayerPrefs.SetInt("Capacity", capacity);
        PlayerPrefs.SetInt("Points", passengerCount);
        PlayerPrefs.SetInt("SpecialPoints", specialPassengerCount);
        FindObjectOfType<UIHandler>().PrintVariables();
    }

	private struct SceneDataSerialized
	{
		public Stop[] Stops;  
		public TransportAgent[] TransportAgents;
		public Passenger[] Passengers;

		public struct Stop
		{
			public string Name;
            public float PositionX;
            public float PositionY;
            public float PositionZ;
            //public Vector3 Position;
		}

		public struct TransportAgent
		{
			public string Name;
			public int Capacity;
            public bool Special;
		}

		public struct Passenger
		{
			public string Name;
			public string Destination;
            public bool Special;
        }
	}
}
