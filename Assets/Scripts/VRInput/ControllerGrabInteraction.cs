using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

//allows each controller to grab an object with a collider, one per controller
public class ControllerGrabInteraction : MonoBehaviour
{
    public SteamVR_Input_Sources inputSource;
    public SteamVR_Action_Boolean triggerPressed;
    public ControllerCollisionBehaviour collisionBehaviour;

    public Transform GrabbedTransform { get; private set; } = null;

    private static ControllerGrabInteraction leftHandInteraction;
    private static ControllerGrabInteraction rightHandInteraction;

    private ControllerGrabInteraction otherHandInteraction;

    private void Awake()
    {
        if (inputSource == SteamVR_Input_Sources.LeftHand)
        {
            leftHandInteraction = this;
        }
        else if (inputSource == SteamVR_Input_Sources.RightHand)
        {
            rightHandInteraction = this;
        }
        else 
        {
            Debug.LogError("Non hand input assigned to controller volume interaction script!");
        }
    }

    private void Start()
    {
        otherHandInteraction = rightHandInteraction == this ? leftHandInteraction : rightHandInteraction;
    }

    private void OnEnable()
    {
        if (triggerPressed != null)
        {
            triggerPressed.AddOnChangeListener(OnTriggerChanged, inputSource);
        }
    }

    private void OnDisable()
    {
        if (triggerPressed != null)
        {
            triggerPressed.RemoveOnChangeListener(OnTriggerChanged, inputSource);
        }
    }

    void OnTriggerChanged(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState) {
        //if the trigger is pulled and we are touching a volume then grab the volume with this hand 
        if (newState && collisionBehaviour.TouchingSomething && otherHandInteraction.GrabbedTransform != collisionBehaviour.TouchedObject)
        {
            GrabbedTransform = collisionBehaviour.TouchedObject.transform;
            collisionBehaviour.DoHaptics = false;
            GrabbedTransform.SetParent(transform);
        }
        else
        {
            if (GrabbedTransform is object)
            {
                GrabbedTransform.SetParent(null);
                GrabbedTransform = null;
            }
            collisionBehaviour.DoHaptics = true;
        }
    }
}