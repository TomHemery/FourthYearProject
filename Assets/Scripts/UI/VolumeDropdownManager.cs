using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Dropdown;

public class VolumeDropdownManager : MonoBehaviour
{

    public Dropdown dropdown;

    int startIndex = 0;

    void Awake()
    {
        //look for volume folders
        string name = VolumeBehaviour.defaultFileName;
        string[] directories = Directory.GetDirectories(Directory.GetCurrentDirectory() + "/" + VolumeBehaviour.VOLUMETRIC_DATA_PATH);
        List<OptionData> options = new List<OptionData>();

        //build the options list
        string folder;
        
        for (int i = 0; i < directories.Length; i++) {
            folder = Path.GetFileNameWithoutExtension(directories[i]);
            if (folder == name) startIndex = i;
            options.Add(new OptionData(folder));
        }
        dropdown.AddOptions(options);

    }

    private void Start()
    {
        if (startIndex >= 0)
        {
            dropdown.value = startIndex;
        }
        OnSelectOption();
    }

    /// <summary>
    /// Loads the selected volume from the dropdown into all present volume renderers
    /// </summary>
    private void OnSelectOption() {
        ViewResetter.Instance.OnResetViewButtonPressed();
        foreach (VolumeBehaviour v in VolumeBehaviour.AllRenderingVolumes) v.LoadVolume(dropdown.options[dropdown.value].text);
    }

}