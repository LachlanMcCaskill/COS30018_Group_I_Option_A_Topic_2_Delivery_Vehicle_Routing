using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using MessageSystem;

public class UIHandler : MonoBehaviour
{
    private List<TransportAgentCostMessage> _routeCosts = new List<TransportAgentCostMessage>();
    [SerializeField] private GameObject costPanel;
    private List<GameObject> _costPanels = new List<GameObject>();
    [SerializeField] private Text totalText;
    private float totalCostValue = 0f;
    [SerializeField] private GameObject layoutGroup;

    [SerializeField] private GameObject variablePanel;
    [SerializeField] private Text agentCountText;
    [SerializeField] private Text specialAgentCountText;
    [SerializeField] private Text capacityText;
    [SerializeField] private Text passengerCountText;
    [SerializeField] private Text specialPassengerCountText;

    private void OnEnable()
    {
        totalText.text = "00.00";
        MessageBoard.ListenForMessage<TransportAgentCostMessage>(OnCostMessageRecieved);
        PrintVariables();
    }

    public void PrintVariables()
    {
        agentCountText.text = PlayerPrefs.GetInt("AgentCount").ToString();
        specialAgentCountText.text = PlayerPrefs.GetInt("SpecialAgentCount").ToString();
        capacityText.text = PlayerPrefs.GetInt("Capacity").ToString();
        passengerCountText.text = PlayerPrefs.GetInt("Points").ToString();
        specialPassengerCountText.text = PlayerPrefs.GetInt("SpecialPoints").ToString();
    }

    private void OnDisable()
    {
        MessageBoard.StopListeningForMessage<TransportAgentCostMessage>(OnCostMessageRecieved);
    }

    private void DisplayRouteCosts()
    {
        foreach (TransportAgentCostMessage c in _routeCosts)
        {
            CreateCostPanel(c.cost, c.routeColour);
        }
    }

    private void CreateCostPanel(float cost, Color routeColour)
    {
        CostPanel newCostPanel = GameObject.Instantiate(costPanel, layoutGroup.transform).GetComponent<CostPanel>();
        newCostPanel.cost = cost.ToString("0.00");
        newCostPanel.routeColour = routeColour;
        totalCostValue += cost;
        totalText.text = totalCostValue.ToString("0.00");
    }

    private void OnCostMessageRecieved(TransportAgentCostMessage m)
    {
        _routeCosts.Add(m);
        CreateCostPanel(m.cost, m.routeColour);
    }

    // NOTE(Andrew): this is probably no longer necessary?
    // private void OnCostMessageStopListening(TransportAgentCostMessage m)
    // {
    // 	 _routeCosts.Remove(m);
    // }

    //button methods
    public void QuitApp()
    {
        //if (EditorApplication.isPlaying)
        //{
        //    EditorApplication.isPlaying = false;
        //}
        //else
        //{
            Application.Quit();
        //}
        //Debug.Log("Debug: Exit button pressed.");
    }

    public void back()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }

    private void OnCostMessageStopListening(TransportAgentCostMessage m)
    {
        _routeCosts.Remove(m);
    }
}
