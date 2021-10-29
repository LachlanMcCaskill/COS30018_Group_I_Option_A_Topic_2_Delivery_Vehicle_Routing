using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VariableScript : MonoBehaviour
{
    //  this script manages the UI that lets the user set configuration variables and stores that information for the Transport Network to read

    [SerializeField] private GameObject _agentCountText;
    [SerializeField] private GameObject _specialAgentCountText;
    [SerializeField] private GameObject _agentCapacityText;
    [SerializeField] private GameObject _pointCountText;
    [SerializeField] private GameObject _specialPointCountText;
    [SerializeField] private GameObject _randomizeText;

    public void resetVariables()
    {
        //  remove special functions from routes that do not implement it
        PlayerPrefs.SetInt("AgentCount", 2);
        PlayerPrefs.SetInt("SpecialAgentCount", 1);
        PlayerPrefs.SetInt("Capacity", 6);
        PlayerPrefs.SetInt("Points", 16);
        PlayerPrefs.SetInt("SpecialPoints", 8);
        PlayerPrefs.SetString("Mode", "Generate");
    }

    public void resetText()
    {
        _agentCountText.GetComponent<Text>().text = PlayerPrefs.GetInt("AgentCount").ToString();
        _specialAgentCountText.GetComponent<Text>().text = PlayerPrefs.GetInt("SpecialAgentCount").ToString();
        _agentCapacityText.GetComponent<Text>().text = PlayerPrefs.GetInt("Capacity").ToString();
        _pointCountText.GetComponent<Text>().text = PlayerPrefs.GetInt("Points").ToString();
        _specialPointCountText.GetComponent<Text>().text = PlayerPrefs.GetInt("SpecialPoints").ToString();

        if (PlayerPrefs.GetString("Mode") == "Generate")
        {
            _randomizeText.GetComponent<Text>().text = "Generate";
        }
        else _randomizeText.GetComponent<Text>().text = "Load File";

    }

    private void Start()
    {
        resetText();
        //  resetVariables();
    }

    public void ChangeMode()
    {
        if (PlayerPrefs.GetString("Mode") == "Load")
        {
            PlayerPrefs.SetString("Mode", "Generate");
            _randomizeText.GetComponent<Text>().text = "Generate";
        }
        else
        {
            PlayerPrefs.SetString("Mode", "Load");
            _randomizeText.GetComponent<Text>().text = "Load File";
        }
    }

    public void increasePoints()
    {
        int currentCount = PlayerPrefs.GetInt("Points");
        PlayerPrefs.SetInt("Points", currentCount + 1);
        _pointCountText.GetComponent<Text>().text = PlayerPrefs.GetInt("Points").ToString();
    }

    public void decreasePoints()
    {
        int currentCount = PlayerPrefs.GetInt("Points");
        if (currentCount <= 1)
        {
            PlayerPrefs.SetInt("Points", 1);
        }
        else
        {
            PlayerPrefs.SetInt("Points", currentCount - 1);
        }

        if (PlayerPrefs.GetInt("Points") < PlayerPrefs.GetInt("SpecialPoints"))
        {
            PlayerPrefs.SetInt("SpecialPoints", PlayerPrefs.GetInt("Points"));
            _specialPointCountText.GetComponent<Text>().text = PlayerPrefs.GetInt("SpecialPoints").ToString();
        }
        _pointCountText.GetComponent<Text>().text = PlayerPrefs.GetInt("Points").ToString();
    }

    public void increaseSpecialPoints()
    {
        int currentCount = PlayerPrefs.GetInt("SpecialPoints");
        if (currentCount < PlayerPrefs.GetInt("Points"))
        {
            PlayerPrefs.SetInt("SpecialPoints", currentCount + 1);
        }
        else
        {
            PlayerPrefs.SetInt("SpecialPoints", PlayerPrefs.GetInt("Points"));
        }
        _specialPointCountText.GetComponent<Text>().text = PlayerPrefs.GetInt("SpecialPoints").ToString();
    }

    public void decreaseSpecialPoints()
    {
        int currentCount = PlayerPrefs.GetInt("SpecialPoints");
        if (currentCount <= 0)
        {
            PlayerPrefs.SetInt("SpecialPoints", 0);
        }
        else
        {
            PlayerPrefs.SetInt("SpecialPoints", currentCount - 1);
        }
        _specialPointCountText.GetComponent<Text>().text = PlayerPrefs.GetInt("SpecialPoints").ToString();
    }

    public void increasCapacity()
    {
        int currentCount = PlayerPrefs.GetInt("Capacity");
        PlayerPrefs.SetInt("Capacity", currentCount + 1);
        _agentCapacityText.GetComponent<Text>().text = PlayerPrefs.GetInt("Capacity").ToString();
    }

    public void decreaseCapacity()
    {
        int currentCount = PlayerPrefs.GetInt("Capacity");
        if (currentCount <= 1)
        {
            PlayerPrefs.SetInt("Capacity", 1);
        }
        else PlayerPrefs.SetInt("Capacity", currentCount - 1);
        _agentCapacityText.GetComponent<Text>().text = PlayerPrefs.GetInt("Capacity").ToString();
    }

    public void increaseSpecialAgentCount()
    {
        int currentCount = PlayerPrefs.GetInt("SpecialAgentCount");
        if (currentCount < PlayerPrefs.GetInt("AgentCount"))
        {
            PlayerPrefs.SetInt("SpecialAgentCount", currentCount + 1);
        }
        else
        {
            PlayerPrefs.SetInt("SpecialAgentCount", PlayerPrefs.GetInt("AgentCount"));
        }
        _specialAgentCountText.GetComponent<Text>().text = PlayerPrefs.GetInt("SpecialAgentCount").ToString();
    }

    public void decreaseSpecialAgentCount()
    {
        int currentCount = PlayerPrefs.GetInt("SpecialAgentCount");
        if (currentCount <= 0)
        {
            PlayerPrefs.SetInt("SpecialAgentCount", 0);
        }
        else
        {
            PlayerPrefs.SetInt("SpecialAgentCount", currentCount - 1);
        }
        _specialAgentCountText.GetComponent<Text>().text = PlayerPrefs.GetInt("SpecialAgentCount").ToString();
    }

    public void increaseAgentCount()
    {
        int currentCount = PlayerPrefs.GetInt("AgentCount");
        PlayerPrefs.SetInt("AgentCount", currentCount + 1);
        _agentCountText.GetComponent<Text>().text = PlayerPrefs.GetInt("AgentCount").ToString();
    }

    public void decreaseAgentCount()
    {
        int currentCount = PlayerPrefs.GetInt("AgentCount");
        if (currentCount <= 1)
        {
            PlayerPrefs.SetInt("AgentCount", 1);
        }
        else PlayerPrefs.SetInt("AgentCount", currentCount - 1);

        if (PlayerPrefs.GetInt("AgentCount") < PlayerPrefs.GetInt("SpecialAgentCount"))
        {
            PlayerPrefs.SetInt("SpecialAgentCount", PlayerPrefs.GetInt("AgentCount"));
            _specialAgentCountText.GetComponent<Text>().text = PlayerPrefs.GetInt("SpecialAgentCount").ToString();
        }
        _agentCountText.GetComponent<Text>().text = PlayerPrefs.GetInt("AgentCount").ToString();
    }
}
