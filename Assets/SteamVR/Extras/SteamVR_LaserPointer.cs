//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//Edited by Thomas Hemery to make it actually usable
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace Valve.VR.Extras
{
    public class SteamVR_LaserPointer : MonoBehaviour
    {
        public SteamVR_Behaviour_Pose pose;

        public SteamVR_Action_Boolean interactWithUI = SteamVR_Input.GetBooleanAction("InteractUI");

        public bool active = true;
        public Color color;
        public float thickness = 0.002f;
        public float scaleIncrease = 3.0f;
        public Color clickColor = Color.green;
        public GameObject holder;
        public GameObject pointer;
        bool isActive = true;
        public bool addRigidBody = false;
        public Transform reference;

        public event PointerEventHandler PointerIn;
        public event PointerEventHandler PointerOut;
        public event PointerEventHandler PointerClick;
        public event PointerEventHandler PointerDown;
        public event PointerEventHandler PointerUp;
        public event PointerEventHandler PointerDrag;
        public event PointerEventHandler PointerBeginDrag;
        public event PointerEventHandler PointerEndDrag;

        private bool dragging = false;

        Transform previousContact = null;


        private void Start()
        {
            if (pose == null)
                pose = GetComponent<SteamVR_Behaviour_Pose>();
            if (pose == null)
                Debug.LogError("No SteamVR_Behaviour_Pose component found on this object", this);

            if (interactWithUI == null)
                Debug.LogError("No ui interaction action has been set on this component.", this);

            holder = new GameObject();
            holder.transform.parent = this.transform;
            holder.transform.localPosition = Vector3.zero;
            holder.transform.localRotation = Quaternion.identity;

            pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pointer.transform.parent = holder.transform;
            pointer.transform.localScale = new Vector3(thickness, thickness, 100f);
            pointer.transform.localPosition = new Vector3(0f, 0f, 50f);
            pointer.transform.localRotation = Quaternion.identity;
            BoxCollider collider = pointer.GetComponent<BoxCollider>();
            if (addRigidBody)
            {
                if (collider)
                {
                    collider.isTrigger = true;
                }
                Rigidbody rigidBody = pointer.AddComponent<Rigidbody>();
                rigidBody.isKinematic = true;
            }
            else
            {
                if (collider)
                {
                    Object.Destroy(collider);
                }
            }
            Material newMaterial = new Material(Shader.Find("Unlit/Color"));
            newMaterial.SetColor("_Color", color);
            pointer.GetComponent<MeshRenderer>().material = newMaterial;
        }

        public virtual void OnPointerIn(PointerEventArgs e)
        {
            PointerIn?.Invoke(this, e);
        }

        public virtual void OnPointerClick(PointerEventArgs e)
        {
            PointerClick?.Invoke(this, e);
        }

        public virtual void OnPointerOut(PointerEventArgs e)
        {
            PointerOut?.Invoke(this, e);
        }

        public virtual void OnPointerDown(PointerEventArgs e) {
            PointerDown?.Invoke(this, e);
        }

        public virtual void OnPointerUp(PointerEventArgs e)
        {
            PointerUp?.Invoke(this, e);
        }

        public virtual void OnPointerDrag(PointerEventArgs e)
        {
            PointerDrag?.Invoke(this, e);
        }

        public virtual void OnBeginDrag(PointerEventArgs e) {
            PointerBeginDrag?.Invoke(this, e);
        }

        public virtual void OnEndDrag(PointerEventArgs e)
        {
            PointerEndDrag?.Invoke(this, e);
        }


        private void Update()
        {
            if (active != isActive)
            {
                isActive = active;
                pointer.SetActive(isActive);
            }
            if (!isActive)
            {   
                if (dragging)
                {
                    PointerEventArgs argsEvent = new PointerEventArgs
                    {
                        fromInputSource = pose.inputSource,
                        distance = 0,
                        flags = 0,
                        target = previousContact,
                        position = Vector3.zero
                    };
                    OnPointerUp(argsEvent);

                    dragging = false;
                    OnEndDrag(argsEvent);
                }
                return;
            }

            float dist = 100f;


            Ray raycast = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            bool bHit = Physics.Raycast(raycast, out hit);

            //flags in and out (only if we have changed object, prevents starting event multiple times)
            if (previousContact && previousContact != hit.transform)
            {
                PointerEventArgs args = new PointerEventArgs();
                args.fromInputSource = pose.inputSource;
                args.distance = 0f;
                args.flags = 0;
                args.target = previousContact;
                args.position = previousContact.position;
                OnPointerOut(args);
                previousContact = null;
            }
            if (bHit && previousContact != hit.transform)
            {
                PointerEventArgs argsIn = new PointerEventArgs();
                argsIn.fromInputSource = pose.inputSource;
                argsIn.distance = hit.distance;
                argsIn.flags = 0;
                argsIn.target = hit.transform;
                OnPointerIn(argsIn);
                previousContact = hit.transform;
            }
            if (!bHit)
            {
                previousContact = null;
            }
            if (bHit && hit.distance < 100f)
            {
                dist = hit.distance;
            }

            if (bHit && interactWithUI.GetStateUp(pose.inputSource))
            {
                PointerEventArgs argsEvent = new PointerEventArgs
                {
                    fromInputSource = pose.inputSource,
                    distance = hit.distance,
                    flags = 0,
                    target = hit.transform,
                    position = hit.point
                };
                OnPointerUp(argsEvent);
                OnPointerClick(argsEvent);

                dragging = false;
                OnEndDrag(argsEvent);
            }

            if (bHit && interactWithUI.GetStateDown(pose.inputSource))
            {
                PointerEventArgs argsEvent = new PointerEventArgs
                {
                    fromInputSource = pose.inputSource,
                    distance = hit.distance,
                    flags = 0,
                    target = hit.transform,
                    position = hit.point
                };
                OnPointerDown(argsEvent);

                dragging = true;
                OnBeginDrag(argsEvent);
            }

            if (bHit && dragging) {
                PointerEventArgs argsDrag = new PointerEventArgs
                {
                    fromInputSource = pose.inputSource,
                    distance = hit.distance,
                    flags = 0,
                    target = hit.transform,
                    position = hit.point
                };
                OnPointerDrag(argsDrag);
            }

            if (interactWithUI != null && interactWithUI.GetState(pose.inputSource))
            {
                pointer.transform.localScale = new Vector3(thickness * scaleIncrease, thickness * scaleIncrease, dist);
                pointer.GetComponent<MeshRenderer>().material.color = clickColor;
            }
            else
            {
                pointer.transform.localScale = new Vector3(thickness, thickness, dist);
                pointer.GetComponent<MeshRenderer>().material.color = color;
            }
            pointer.transform.localPosition = new Vector3(0f, 0f, dist / 2f);
        }
    }

    public struct PointerEventArgs
    {
        public SteamVR_Input_Sources fromInputSource;
        public uint flags;
        public float distance;
        public Transform target;
        public Vector3 position;
    }

    public delegate void PointerEventHandler(object sender, PointerEventArgs e);
}