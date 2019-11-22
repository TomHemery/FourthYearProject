using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Dropdown;

public class VolumeDropdownManager : MonoBehaviour
{

    public Dropdown dropdown;
    bool skipChange = false;


    void Start()
    {
        string name = VolumeManager.Instance.defaultFolderName;
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
            skipChange = true;
            dropdown.value = index;
        }
    }

    public void OnSelectOption() {
        if (skipChange) skipChange = false;
        else VolumeManager.Instance.LoadVolume(dropdown.options[dropdown.value].text);
    }

}