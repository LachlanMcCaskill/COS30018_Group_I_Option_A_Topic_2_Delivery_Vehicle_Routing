using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouteSolver : MonoBehaviour
{
    private TransportNetwork _transportNetwork = new TransportNetwork();    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void passengerSetup()
    {

    }

    List<Route> Solve()
    {
        List<Route> result = new List<Route>();
        return result;
    }

    public void PrintSolutionToTerminal()
    {
        Debug.Log("Printing Solution to DRV Problem");
    }
}
