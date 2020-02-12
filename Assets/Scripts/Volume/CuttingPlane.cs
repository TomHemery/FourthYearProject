using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingPlane : MonoBehaviour
{
    void Update()
    {
        VolumeManager.Instance.SetPlane(GetPlanePosition(), GetPlaneNormal());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.CompareTag("VolumeCube")) {
            VolumeManager.Instance.SetDoSlicing(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.transform.CompareTag("VolumeCube"))
        {
            VolumeManager.Instance.SetDoSlicing(false);
        }
    }

    public Vector3 GetPlanePosition() {
        return transform.position;
    }

    public Vector3 GetPlaneNormal() {
        return transform.forward;
    }
}
