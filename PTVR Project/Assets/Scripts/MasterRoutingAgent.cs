using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MessageSystem;

public class MasterRoutingAgent : MonoBehaviour
{
    private RouteSolver _routeSolver;
    private TransportNetwork _transportNetwork;
    private List<TransportAgent> _transportAgent;

    private void SendRoutes()
    {
        foreach(TransportAgent t in _transportAgent)
        {
            //assign the correct route to every single transport agent
        }
    }

    private void CreateRoutes()
    {

    }

    private float CalculateDistance(Vector3 point1, Vector3 point2)
    {
        float result = new float();
        return result;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
