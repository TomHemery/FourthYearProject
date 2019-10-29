using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleListener : MonoBehaviour
{
    public Toggle red;
    public Toggle green;
    public Toggle blue;
    public void HandleRedCheckBox() 
    {
        VolumeManager.Instance.SetRenderRed(red.isOn);
    }
    public void HandleGreenCheckBox()
    {
        VolumeManager.Instance.SetRenderGreen(green.isOn);
    }

    public void HandleBlueCheckBox()
    {
        VolumeManager.Instance.SetRenderBlue(blue.isOn);
    }
}
