﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingPlane : MonoBehaviour
{
    private RectTransform mRectTransform;
    private Transform parentTransform;

    private void Awake()
    {
        mRectTransform = gameObject.GetComponent<RectTransform>();
        parentTransform = mRectTransform.parent;
    }

    void Update()
    {
        VolumeManager.Instance.SetPlane(GetPlanePosition(), GetPlaneNormal());
    }

    public Vector4 GetPlanePosition() {
        return transform.localPosition + new Vector3(0.5f, 0.5f, 0.5f);
    }

    public Vector4 GetPlaneNormal() {
        return transform.forward;
    }
}