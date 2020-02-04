using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ControllerCollisionBehaviour : MonoBehaviour
{
    public SteamVR_Action_Vibration hapticAction;
    public SteamVR_Input_Sources input;

    float threshold = 0.2f;

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.CompareTag("VolumeCube"))
        {
            Vector3 relativePos = other.transform.position - transform.position;
            float density = other.gameObject.GetComponent<SampleVolume>().SampleVolumeDensityAt(relativePos);
            if (density > threshold) {
                hapticAction.Execute(0, 0.05f, 100, density * 175, input);
            }
        }
    }
}
