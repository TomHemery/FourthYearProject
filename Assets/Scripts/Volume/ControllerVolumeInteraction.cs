using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ControllerVolumeInteraction : MonoBehaviour
{
    public SteamVR_Input_Sources inputSource;
    public SteamVR_Action_Boolean triggerPressed;
    public ControllerCollisionBehaviour collisionBehaviour;

    public Transform GrabbedTransform { get; private set; } = null;
    private Vector3 offset;

    private static ControllerVolumeInteraction leftHandInteraction;
    private static ControllerVolumeInteraction rightHandInteraction;
    private static Vector3 prevDirection = Vector3.zero;

    private ControllerVolumeInteraction otherHandInteraction;

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

    /*private void Update()
    {
        if(GrabbedTransform is object){ //we gripped something 
            if (otherHandInteraction.GrabbedTransform == GrabbedTransform)
            { //both hands are grabbing this object only move it for now 
                if (this == rightHandInteraction) { //this update will happen for both hands, but we only need to update the volume transform once (arbitrary to do it from the right hand here)
                    GrabbedTransform.position = (transform.position + otherHandInteraction.transform.position) / 2; //the volume's position is equal to the average of both hand positions (between)
                }
                //update the offset so if we let go with one hand the volume won't jump around 
                offset = GrabbedTransform.position - transform.position;
            }
            else
            { //one hand is grabbing this object
                prevDirection.Set(0, 0, 0);
                GrabbedTransform.position = transform.position + offset; //the volume's position is equal to 
            }
        }
    }
    */

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
        if (newState && collisionBehaviour.TouchingVolume && otherHandInteraction.GrabbedTransform != collisionBehaviour.LastVolumeSampled.transform)
        {
            GrabbedTransform = collisionBehaviour.LastVolumeSampled.transform;
            offset = GrabbedTransform.position - transform.position;
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