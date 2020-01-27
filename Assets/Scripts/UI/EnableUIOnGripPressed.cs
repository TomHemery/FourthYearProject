using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.Extras;

public class EnableUIOnGripPressed : MonoBehaviour
{
    public SteamVR_Input_Sources inputSource;
    public SteamVR_Action_Boolean gripButton;
    public GameObject uiTarget;
    public SteamVR_LaserPointer laserPointerTarget;

    // Start is called before the first frame update
    void Start()
    {
        if(uiTarget != null) uiTarget.SetActive(false);
        if (laserPointerTarget != null) laserPointerTarget.active = false;
    }

    private void OnEnable()
    {
        if (gripButton != null) {
            gripButton.AddOnChangeListener(OnGripChanged, inputSource);
        }
    }

    private void OnDisable()
    {
        if (gripButton != null)
        {
            gripButton.RemoveOnChangeListener(OnGripChanged, inputSource);
        }
    }

    void OnGripChanged(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
    {
        if (uiTarget != null) uiTarget.SetActive(newState);
        if (laserPointerTarget != null) laserPointerTarget.active = newState;
    }
}
