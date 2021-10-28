using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class CostPanel : MonoBehaviour
{
    public string cost;
    public Color routeColour;
    [SerializeField]private GameObject costPanel;
    [SerializeField]private GameObject routeColourPanel;

    //store the values of the child components so they can be referred to as variables
    private void Start()
    {
        costPanel.GetComponent<Text>().text = cost;
        routeColourPanel.GetComponent<Text>().color = routeColour;
    }
}