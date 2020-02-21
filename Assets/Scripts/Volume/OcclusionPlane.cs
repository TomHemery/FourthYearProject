using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OcclusionPlane : MonoBehaviour
{
    void Update()
    {
        foreach (VolumeBehaviour v in VolumeBehaviour.AllVolumes) v.SetOcclusionPlane(GetPlanePosition(), GetPlaneNormal());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.CompareTag("VolumeCube")) {
            other.gameObject.GetComponent<VolumeBehaviour>().SetDoOcclusion(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.transform.CompareTag("VolumeCube"))
        {
            other.gameObject.GetComponent<VolumeBehaviour>().SetDoOcclusion(false);
        }
    }

    public Vector3 GetPlanePosition() {
        return transform.position;
    }

    public Vector3 GetPlaneNormal() {
        return transform.forward;
    }
}
