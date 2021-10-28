using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MessageSystem;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    private List<TransportAgentCostMessage> _routeCosts = new List<TransportAgentCostMessage>();
    [SerializeField] private GameObject costPanel;
    private List<GameObject> _costPanels = new List<GameObject>();
    [SerializeField]private GameObject layoutGroup;
    [SerializeField]private Text totalText;
    private float totalCostValue = 0f;

    //Check message board for costs to display
    private void OnEnable()
    {
        MessageBoard.ListenForMessage<TransportAgentCostMessage>(OnCostMessageRecieved);
    }

    //stop listening for costs to display
    private void OnDisable()
    {
        MessageBoard.StopListeningForMessage<TransportAgentCostMessage>(OnCostMessageStopListening);
    }

    // Reset the sum of all route costs to 0 on startup
    void Start()
    {
        totalText.text = "00.00";
    }

    // Update the total cost every frame
    void Update()
    {
        totalText.text = totalCostValue.ToString("0.00");   
    }

    //create all cost panels
    private void DisplayRouteCosts()
    {
        foreach(TransportAgentCostMessage c in _routeCosts)
        {
            CreateCostPanel(c.cost, c.routeColour);
        }
    }

    //create a new UI panel that displays the cost of a route
    private void CreateCostPanel(float cost, Color routeColour)
    {
        CostPanel newCostPanel = GameObject.Instantiate(costPanel, layoutGroup.transform).GetComponent<CostPanel>();
        newCostPanel.cost = cost.ToString("0.00");
        newCostPanel.routeColour = routeColour;
        totalCostValue += cost;
    }

    private void OnCostMessageRecieved(TransportAgentCostMessage m)
    {
        _routeCosts.Add(m);
        CreateCostPanel(m.cost, m.routeColour);
    }

    private void OnCostMessageStopListening(TransportAgentCostMessage m)
    {
        _routeCosts.Remove(m);
    }

    //Quit the application
    public void QuitApp()
    {
        if (EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = false;
        }
        else
        {
            Application.Quit();
        }
        Debug.Log("Debug: Exit button pressed.");
    }
}
