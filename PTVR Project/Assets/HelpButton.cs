using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpButton : MonoBehaviour
{
    public GameObject helpText;
    public GameObject menuPanel;

    public void ToggleHelp()
    {
        helpText.SetActive(!helpText.activeSelf);
    }
}
