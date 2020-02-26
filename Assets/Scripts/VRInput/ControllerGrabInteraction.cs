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
    public SteamVR_Action_Vibration hapticAction;

    public Transform GrabbedTransform { get; private set; } = null;

    private static ControllerGrabInteraction leftHandInteraction;
    private static ControllerGrabInteraction rightHandInteraction;

    private ControllerGrabInteraction otherHandInteraction;

    private static bool doTwoHandedGrab = false;

    private readonly float shakeThreshold = 0.45f;
    private readonly float ripThreshold = 0.6f;

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

    private void Update()
    {
        if (doTwoHandedGrab && this == rightHandInteraction)
        {
            GrabbedTransform.position = (transform.position + otherHandInteraction.transform.position) / 2;

            if (GrabbedTransform.gameObject.CompareTag("VolumeCube"))//if we are grabbing a volume cube
            {
                float dist = Vector3.Distance(transform.position, otherHandInteraction.transform.position);
                if (dist > ripThreshold)
                {
                    GrabbedTransform.GetComponent<VolumeBehaviour>().CuttingPlaneTransform.LookAt(transform.position);
                    GameObject newHalf = GrabbedTransform.gameObject.GetComponent<VolumeBehaviour>().Split();

                    if (Vector3.Distance(newHalf.transform.position, transform.position) < Vector3.Distance(newHalf.transform.position, otherHandInteraction.transform.position)){
                        GrabbedTransform.SetParent(otherHandInteraction.transform);
                        newHalf.transform.SetParent(transform);
                        GrabbedTransform = newHalf.transform;
                    }
                    else {
                        GrabbedTransform.SetParent(transform);
                        newHalf.transform.SetParent(otherHandInteraction.transform);
                        otherHandInteraction.GrabbedTransform = newHalf.transform;
                    }
                    doTwoHandedGrab = false;
                }
                else if (dist > shakeThreshold)
                {
                    //shake dem hands
                    float scale = (dist - shakeThreshold) / (ripThreshold - shakeThreshold);
                    hapticAction.Execute(0, scale * 0.1f, 150, scale, inputSource);
                    hapticAction.Execute(0, scale * 0.1f, 150, scale, otherHandInteraction.inputSource);
                }
            }
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
        //if the trigger is pulled and we are touching a volume then grab the volume   
        if (newState && collisionBehaviour.TouchedObject != null)
        {
            GrabbedTransform = collisionBehaviour.TouchedObject.transform;
            collisionBehaviour.DoHaptics = false;
            if (otherHandInteraction.GrabbedTransform != GrabbedTransform)
            {
                GrabbedTransform.SetParent(transform);
            }
            else
            {
                doTwoHandedGrab = true;
                GrabbedTransform.SetParent(null);
            }
        }
        //if we are holding a volume and we let go of the trigger then let go of it
        else if (!newState && GrabbedTransform != null) {
            if (doTwoHandedGrab) //if we were grabbing it with two hands
            {
                doTwoHandedGrab = false; //grab it with the other hand only
                GrabbedTransform.SetParent(otherHandInteraction.transform);
            }
            else { //if we were grabbing it with one hand 
                GrabbedTransform.SetParent(null); 
            }
            GrabbedTransform = null;
            collisionBehaviour.DoHaptics = true;
        }
    }
}