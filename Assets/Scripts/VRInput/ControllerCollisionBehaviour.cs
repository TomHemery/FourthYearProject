using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ControllerCollisionBehaviour : MonoBehaviour
{
    public SteamVR_Action_Vibration hapticAction;
    public SteamVR_Input_Sources input;

    public GameObject LastVolumeSampled { get; private set; } = null;
    public SampleVolume LastVolumeSampler { get; private set; } = null;
    public bool TouchingVolume { get; private set; } = false;
    public bool DoHaptics = true;

    private readonly float threshold = 0.05f;

    public float Density { get; private set; } = 0f;

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.CompareTag("VolumeCube"))
        {
            Vector3 relativePos = transform.position - other.transform.position;
            Density = other.gameObject.GetComponent<SampleVolume>().SampleVolumeDensityAt(relativePos, 1);
            LastVolumeSampler = other.gameObject.GetComponent<SampleVolume>();
            LastVolumeSampled = other.gameObject;
            if (Density > threshold)
            {
                TouchingVolume = true;
                if (DoHaptics) hapticAction.Execute(0, 0.01f, Density * 50 + 60, Density * 100, input);
            }
            else
                TouchingVolume = false;
        }
    }
}
