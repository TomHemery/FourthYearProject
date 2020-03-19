using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewResetter : MonoBehaviour
{
    public Button ResetViewButton;

    public Slider DensitySlider;
    public Slider QualitySlider;
    public Slider ThresholdSider;

    public Toggle RedToggle;
    public Toggle GreenToggle;
    public Toggle BlueToggle;

    public GameObject VolumeCubePrefab;

    private Vector3 occlusionPlaneStartPosition;
    private Quaternion occlusionPlaneStartRotation;
    public GameObject occlusionPlane;

    public static ViewResetter Instance { get; private set; } = null;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        occlusionPlaneStartPosition = occlusionPlane.transform.position;
        occlusionPlaneStartRotation = occlusionPlane.transform.rotation;
    }

    public void OnResetViewButtonPressed() {
        occlusionPlane.transform.position = occlusionPlaneStartPosition;
        occlusionPlane.transform.rotation = occlusionPlaneStartRotation;
        foreach (VolumeBehaviour v in VolumeBehaviour.AllRenderingVolumes) Destroy(v.gameObject);
        GameObject newVol = Instantiate(VolumeCubePrefab);
        newVol.GetComponent<VolumeBehaviour>().LoadVolume(VolumeBehaviour.CurrentVolumeName);
        newVol.GetComponent<VolumeBehaviour>().InitSettings();
        RedToggle.isOn = true;
        BlueToggle.isOn = true;
        GreenToggle.isOn = true;
    }
}
