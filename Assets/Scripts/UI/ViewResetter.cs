using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewResetter : MonoBehaviour
{
    public Button ResetViewButton;
    public Camera MainCamera;
    public Slider DensitySlider;
    public Slider SpacingSlider;

    public GameObject CuttingPlane { get; private set; } = null;

    private Vector3 cameraStartPosition;
    private Quaternion cameraStartRotation;

    private Vector3 cuttingPlaneStartPosition;
    private Quaternion cuttingPlaneStartRotation;

    private float startDensity;
    private float startSpacing;

    private void Start()
    {
        cameraStartPosition = MainCamera != null ? MainCamera.transform.position : Vector3.zero;
        cameraStartRotation = MainCamera != null ? MainCamera.transform.rotation : Quaternion.identity;
        startDensity = DensitySlider.value;
        startSpacing = SpacingSlider.value;
    }

    public void SetCuttingPlane(GameObject cp) {
        cuttingPlaneStartPosition = cp.transform.position;
        cuttingPlaneStartRotation = cp.transform.rotation;
        CuttingPlane = cp;
    }

    public void OnResetViewButtonPressed() {
        MainCamera.transform.position = cameraStartPosition;
        MainCamera.transform.rotation = cameraStartRotation;

        DensitySlider.value = startDensity;
        SpacingSlider.value = startSpacing;

        if (CuttingPlane != null) {
            CuttingPlane.transform.position = cuttingPlaneStartPosition;
            CuttingPlane.transform.rotation = cuttingPlaneStartRotation;
        }
    }
}
