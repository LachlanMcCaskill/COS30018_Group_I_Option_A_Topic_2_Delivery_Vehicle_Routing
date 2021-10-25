using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InSysGUIBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject TransportAgent;
    public GameObject[] TAarray;
    public List<GameObject> TAList = new List<GameObject>();
    public GameObject tempTA;

    public int i = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void AddAgent()
    {
        if (i > 10)
        {
            return;
        }
        i++;
        //TAarray[i] = GameObject.Instantiate(TransportAgent);
        TAList.Add(GameObject.Instantiate(TransportAgent));     
    }

    public void RemoveAgent()
    {
        if (i < 1)
        {
            return;
        }
        //tempTA = TAarray[i];
        //Destroy(tempTA);
        tempTA = TAList[i];
        TAList.RemoveAt(i);
        Destroy(tempTA);
        i--;
        
    }
}
