using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RouteSolver;
using UnityEditor;

public class MenuScript : MonoBehaviour
{
    [SerializeField]private string _targetsceneName;
    [SerializeField] private Text selectedStrategy;

    public void OnEnable()
    {
        selectedStrategy.text = PlayerPrefs.GetString("RoutingStrategy");
    }

    public void setNonSpecialAgents()
    {
        PlayerPrefs.SetInt("NonSpecialAgentCount", PlayerPrefs.GetInt("AgentCount") - PlayerPrefs.GetInt("SpecialAgentCount"));
    }

    public void loadGameScene()
    {
        if (PlayerPrefs.GetString("RoutingStrategy") != null && PlayerPrefs.GetString("RoutingStrategy") != "")
        {
            setNonSpecialAgents();
            UnityEngine.SceneManagement.SceneManager.LoadScene(_targetsceneName);
        }
    }

    public void loadGameSceneGreedy1()
    {
        PlayerPrefs.SetString("SpecialRoute", "False");
        PlayerPrefs.SetString("RoutingStrategy", "Greedy");
        //  selectedStrategy.text = "Greedy";
        selectedStrategy.text = PlayerPrefs.GetString("RoutingStrategy");
    }

    public void loadGameSceneGreedy2()
    {
        PlayerPrefs.SetString("SpecialRoute", "False");
        //setNonSpecialAgents();
        //UnityEngine.SceneManagement.SceneManager.LoadScene(_targetsceneName);
        PlayerPrefs.SetString("RoutingStrategy", "Alt Greedy");
        //selectedStrategy.text = "Alt Greedy";
        selectedStrategy.text = PlayerPrefs.GetString("RoutingStrategy");
    }

    public void loadGameSceneGeneticAlgorithm()
    {
        //setNonSpecialAgents();
        PlayerPrefs.SetString("SpecialRoute", "True");
        //UnityEngine.SceneManagement.SceneManager.LoadScene(_targetsceneName);
        PlayerPrefs.SetString("RoutingStrategy", "Genetic");
        //selectedStrategy.text = "Genetic";
        selectedStrategy.text = PlayerPrefs.GetString("RoutingStrategy");
    }

    public void loadGameSceneKMeans1()
    {
        PlayerPrefs.SetString("SpecialRoute", "False");
        //setNonSpecialAgents();
        //UnityEngine.SceneManagement.SceneManager.LoadScene(_targetsceneName);
        PlayerPrefs.SetString("RoutingStrategy", "Cluster\nGreedy");
        //selectedStrategy.text = "Cluster\nGreedy1";
        selectedStrategy.text = PlayerPrefs.GetString("RoutingStrategy");
    }

    public void loadGameSceneKMeans2()
    {
        PlayerPrefs.SetString("SpecialRoute", "False");
        //setNonSpecialAgents();
        //UnityEngine.SceneManagement.SceneManager.LoadScene(_targetsceneName);
        PlayerPrefs.SetString("RoutingStrategy", "Cluster\nAlt Greedy");
        //selectedStrategy.text = "Cluster\nGreedy2";
        selectedStrategy.text = PlayerPrefs.GetString("RoutingStrategy");
    }

    public void loadGameSceneKMeansGA()
    {
        PlayerPrefs.SetString("SpecialRoute", "False");
        //setNonSpecialAgents();
        //UnityEngine.SceneManagement.SceneManager.LoadScene(_targetsceneName);
        PlayerPrefs.SetString("RoutingStrategy", "Cluster\nGenetic");
        //selectedStrategy.text = "Cluster\nGenetic";
        selectedStrategy.text = PlayerPrefs.GetString("RoutingStrategy");
    }

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
}
