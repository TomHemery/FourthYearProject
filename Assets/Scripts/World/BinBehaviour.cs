using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinBehaviour : MonoBehaviour
{
    public ControllerGrabInteraction leftHandGrab;
    public ControllerGrabInteraction rightHandGrab;

    public Color normalColour;
    public Color hoverColour;
    private Material m_material;

    private void Awake()
    {
        m_material = GetComponent<Renderer>().material;
        m_material.color = normalColour;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("VolumeCube")) {
            if (leftHandGrab.GrabbedTransform != other.transform &&
                rightHandGrab.GrabbedTransform != other.transform) 
            {
                Destroy(other.gameObject);
                m_material.color = normalColour;
            }
            else {
                m_material.color = hoverColour;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        m_material.color = normalColour;
    }
}
