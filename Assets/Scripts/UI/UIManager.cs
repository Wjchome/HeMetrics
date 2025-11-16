using FairyGUI;
using UnityEngine;

public class UIManager:MonoBehaviour
{
    public GameObject bondUI;
    public BondUI bondShow;
    public void Init()
    {
       GComponent ui = bondUI.GetComponent<UIPanel>().ui;
       if (ui != null)
       {
           bondShow = ui as BondUI;
           bondShow.Init();
       }
       else
       {
           Debug.LogError("BondUI: Failed to get UI component from UIPanel");
       }
    }

}

