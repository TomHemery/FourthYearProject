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
        VolumeLoader.Instance.SetRenderRed(red.isOn);
    }
    public void HandleGreenCheckBox()
    {
        VolumeLoader.Instance.SetRenderGreen(green.isOn);
    }

    public void HandleBlueCheckBox()
    {
        VolumeLoader.Instance.SetRenderBlue(blue.isOn);
    }
}
