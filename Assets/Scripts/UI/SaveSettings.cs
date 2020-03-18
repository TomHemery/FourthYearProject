using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSettings : MonoBehaviour
{
    public ShowTemporarily savedPrompt;

    public void SaveVolumeSettingsToXML() {
        Debug.Log("Saving settings");
        SettingsManager.SaveSettingsToXML(VolumeBehaviour.CurrentVolumeName, VolumeBehaviour.Settings);
        savedPrompt.Show();
    }
}
