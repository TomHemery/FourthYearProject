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

    private Vector3 cameraStartPosition;
    private Quaternion cameraStartRotation;

    private List<Vector3> cuttingPlaneStartPositions;
    private List<Quaternion> cuttingPlaneStartRotations;

    private float startDensity;
    private float startSpacing;

    private void Start()
    {
        cameraStartPosition = MainCamera != null ? MainCamera.transform.position : Vector3.zero;
        cameraStartRotation = MainCamera != null ? MainCamera.transform.rotation : Quaternion.identity;
        startDensity = DensitySlider.value;
        startSpacing = SpacingSlider.value;
    }

    public void CacheCuttingPlanes(){
        cuttingPlaneStartPositions = new List<Vector3>();
        cuttingPlaneStartRotations = new List<Quaternion>();
        
        foreach (GameObject g in VolumeManager.Instance.VolumeCubes) {
            cuttingPlaneStartPositions.Add(g.transform.position);
            cuttingPlaneStartRotations.Add(g.transform.rotation);
        }
    }

    public void OnResetViewButtonPressed() {
        MainCamera.transform.position = cameraStartPosition;
        MainCamera.transform.rotation = cameraStartRotation;

        DensitySlider.value = startDensity;
        SpacingSlider.value = startSpacing;

        if (cuttingPlaneStartRotations != null && cuttingPlaneStartPositions != null) {
            for (int i = 0; i < VolumeManager.Instance.VolumeCubes.Count; i++) {
                VolumeManager.Instance.VolumeCubes[i].transform.position = cuttingPlaneStartPositions[i];
                VolumeManager.Instance.VolumeCubes[i].transform.rotation = cuttingPlaneStartRotations[i];
            }
        }
    }
}
