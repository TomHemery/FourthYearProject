using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingPlane : MonoBehaviour
{
    private RectTransform mRectTransform;

    private Material mat;

    private void Awake()
    {
        mRectTransform = gameObject.GetComponent<RectTransform>();
        mat = transform.root.GetComponent<Renderer>().material; 
    }

    void Update()
    {
        ApplyToMaterial();
    }

    public void ApplyToMaterial() {
        mat.SetVector(VolumeManager.PLANE_NORMAL_TAG, GetPlaneNormal());
        mat.SetVector(VolumeManager.PLANE_POSITION_TAG, GetPlanePosition());
    }

    public Vector4 GetPlanePosition() {
        return transform.localPosition + new Vector3(0.5f, 0.5f, 0.5f);
    }

    public Vector4 GetPlaneNormal() {
        return transform.forward;
    }
}
