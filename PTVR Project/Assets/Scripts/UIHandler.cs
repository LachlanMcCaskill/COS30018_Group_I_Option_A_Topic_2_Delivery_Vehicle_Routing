using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MessageSystem;

public class UIHandler : MonoBehaviour
{
    private List<TransportAgentCostMessage> _routeCosts = new List<TransportAgentCostMessage>();
    [SerializeField] private GameObject costPanel;
    private List<GameObject> _costPanels = new List<GameObject>();
    [SerializeField]private GameObject layoutGroup;

    private void OnEnable()
    {
        MessageBoard.ListenForMessage<TransportAgentCostMessage>(OnCostMessageRecieved);
    }

    private void OnDisable()
    {
        MessageBoard.StopListeningForMessage<TransportAgentCostMessage>(OnCostMessageStopListening);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void DisplayRouteCosts()
    {
        foreach(TransportAgentCostMessage c in _routeCosts)
        {
            CreateCostPanel(c.cost, c.routeColour);
        }
    }

    private void CreateCostPanel(float cost, Color routeColour)
    {
        CostPanel newCostPanel = GameObject.Instantiate(costPanel, layoutGroup.transform).GetComponent<CostPanel>();
        newCostPanel.cost = cost.ToString("0.00");
        newCostPanel.routeColour = routeColour;
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

    //button methods
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
