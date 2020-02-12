using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ControllerCollisionBehaviour : MonoBehaviour
{
    public SteamVR_Action_Vibration hapticAction;
    public SteamVR_Input_Sources input;
    private Light indicatorLight;

    public bool TouchingSomething { get; private set; } = false;
    public GameObject TouchedObject { get; private set; } = null;
    public bool DoHaptics = true;

    private readonly float threshold = 0.05f;

    private Color touchColor = new Color(0, 1, 1);
    private Color noTouchColor = new Color(0, 0.5f, 0.5f);

    private void Awake()
    {
        indicatorLight = GetComponentInChildren<Light>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.CompareTag("VolumeCube")) {
            TouchingSomething = true;
            TouchedObject = other.gameObject;
            indicatorLight.color = touchColor;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.CompareTag("VolumeCube"))
        {
            Vector3 relativePos = transform.position - other.transform.position;
            float density = other.gameObject.GetComponent<SampleVolume>().SampleVolumeDensityAt(relativePos, 1);

            if (density > threshold)
            {
                TouchedObject = other.gameObject;
                TouchingSomething = true;
                indicatorLight.color = touchColor;
                if (DoHaptics) hapticAction.Execute(0, 0.01f, density * 50 + 60, density * 100, input);
            }
            else
            {
                TouchingSomething = false;
                TouchedObject = null;
                indicatorLight.color = noTouchColor;
            }
        }
        else {
            TouchedObject = other.gameObject;
            TouchingSomething = true;
            indicatorLight.color = touchColor;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        TouchingSomething = false;
        TouchedObject = null;
        indicatorLight.color = noTouchColor;
    }
}
