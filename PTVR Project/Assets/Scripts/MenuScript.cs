using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RouteSolver;
using UnityEditor;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    [SerializeField]private string _targetsceneName;

    [SerializeField] private GameObject _agentCountText;
    [SerializeField] private GameObject _agentCapacityText;
    [SerializeField] private GameObject _pointCountText;
    [SerializeField] private GameObject _randomizeText;

    private void Start()
    {
        PlayerPrefs.SetInt("Points", 16);
        PlayerPrefs.SetInt("Capacity", 6);
        PlayerPrefs.SetInt("AgentCount", 2);
        PlayerPrefs.SetInt("Randomize", 0);

        _agentCountText.GetComponent<Text>().text = PlayerPrefs.GetInt("AgentCount").ToString();
        _agentCapacityText.GetComponent<Text>().text = PlayerPrefs.GetInt("Capacity").ToString();
        _pointCountText.GetComponent<Text>().text = PlayerPrefs.GetInt("Points").ToString();
        _randomizeText.GetComponent<Text>().text = "OFF";
    }

    public void Randomize()
    {
        if (PlayerPrefs.GetInt("Randomize") == 0)
        {
            PlayerPrefs.SetInt("Randomize", 1);
            _randomizeText.GetComponent<Text>().text = "ON";
        }
        else 
        {
            PlayerPrefs.SetInt("Randomize", 0);
            _randomizeText.GetComponent<Text>().text = "OFF";
        }
    }

    public void increasePoints()
    {
        int currentCount = PlayerPrefs.GetInt("Points");
        PlayerPrefs.SetInt("Points", currentCount + 1);
        PlayerPrefs.SetInt("Randomize", 1);
        _randomizeText.GetComponent<Text>().text = "ON";
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
            PlayerPrefs.SetInt("Randomize", 1);
            _randomizeText.GetComponent<Text>().text = "ON";
            PlayerPrefs.SetInt("Points", currentCount - 1);
        }
        _pointCountText.GetComponent<Text>().text = PlayerPrefs.GetInt("Points").ToString();
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
        _agentCountText.GetComponent<Text>().text = PlayerPrefs.GetInt("AgentCount").ToString();
    }

    public void loadGameSceneGreedy1()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(_targetsceneName);
        PlayerPrefs.SetString("RoutingStrategy", "Greedy1");
    }

    public void loadGameSceneGreedy2()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(_targetsceneName);
        PlayerPrefs.SetString("RoutingStrategy", "Greedy2");
    }

    public void loadGameSceneGeneticAlgorithm()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(_targetsceneName);
        PlayerPrefs.SetString("RoutingStrategy", "GA");
    }

    public void loadGameSceneKMeans1()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(_targetsceneName);
        PlayerPrefs.SetString("RoutingStrategy", "KMeans1");
    }

    public void loadGameSceneKMeans2()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(_targetsceneName);
        PlayerPrefs.SetString("RoutingStrategy", "KMeans2");
    }

    public void loadGameSceneKMeansGA()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(_targetsceneName);
        PlayerPrefs.SetString("RoutingStrategy", "KMeansGA");
    }

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
