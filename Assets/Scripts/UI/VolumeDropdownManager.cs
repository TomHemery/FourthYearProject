using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Dropdown;

public class VolumeDropdownManager : MonoBehaviour
{

    public Dropdown dropdown;

    void Awake()
    {
        string name = VolumeBehaviour.defaultFileName;
        int index = -1;
        OptionData option;
        for (int i = 0; i < dropdown.options.Count; i++) {
            option = dropdown.options[i];
            if (option.text == name) {
                index = i;
            }
        }
        if (index >= 0)
        {
            dropdown.value = index;
        }
        OnSelectOption();
    }

    public void OnSelectOption() {
        foreach (VolumeBehaviour v in VolumeBehaviour.AllVolumes) v.LoadVolume(dropdown.options[dropdown.value].text);
    }

}