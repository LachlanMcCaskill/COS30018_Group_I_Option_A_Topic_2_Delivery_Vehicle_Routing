using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MessageSystem;

public class UIHandler : MonoBehaviour
{
    private List<TransportAgentCostMessage> _routeCosts = new List<TransportAgentCostMessage>();

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

    private void OnCostMessageRecieved(TransportAgentCostMessage m)
    {
        _routeCosts.Add(m);
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
