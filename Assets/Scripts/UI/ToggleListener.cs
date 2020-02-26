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
        foreach (VolumeBehaviour v in VolumeBehaviour.AllRenderingVolumes) v.SetRenderRed(red.isOn);
    }
    public void HandleGreenCheckBox()
    {
        foreach (VolumeBehaviour v in VolumeBehaviour.AllRenderingVolumes) v.SetRenderGreen(green.isOn);
    }

    public void HandleBlueCheckBox()
    {
        foreach (VolumeBehaviour v in VolumeBehaviour.AllRenderingVolumes) v.SetRenderBlue(blue.isOn);
    }
}
