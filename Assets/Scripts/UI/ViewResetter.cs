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

    private float startDensity;
    private float startThreshold;
    private float startQuality;

    private void Awake()
    {
        startDensity = DensitySlider.value;
        startQuality = QualitySlider.value;
        startThreshold = ThresholdSider.value;

        occlusionPlaneStartPosition = occlusionPlane.transform.position;
        occlusionPlaneStartRotation = occlusionPlane.transform.rotation;
    }

    public void OnResetViewButtonPressed() {
        DensitySlider.value = startDensity;
        QualitySlider.value = startQuality;
        ThresholdSider.value = startThreshold;
        occlusionPlane.transform.position = occlusionPlaneStartPosition;
        occlusionPlane.transform.rotation = occlusionPlaneStartRotation;
        foreach (VolumeBehaviour v in VolumeBehaviour.AllVolumes) Destroy(v.gameObject);
        GameObject newVol = Instantiate(VolumeCubePrefab);
        newVol.GetComponent<VolumeBehaviour>().LoadVolume(VolumeBehaviour.CurrentVolumeName);

        RedToggle.isOn = true;
        BlueToggle.isOn = true;
        GreenToggle.isOn = true;
    }
}
