using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RouteSolver;
using UnityEditor;

public class MenuScript : MonoBehaviour
{
    [SerializeField]private string _targetsceneName;

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
