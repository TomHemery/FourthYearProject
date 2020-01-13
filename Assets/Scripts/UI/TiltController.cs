using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TiltController : MonoBehaviour
{
    public Transform CuttingPlane;
    public GameObject UIImage;
    public char RotationAxis;

    private Canvas mCanvas;
    private Vector3 zeroAngle;
    private bool zeroSet = false;
    private Vector3 localAxis;

    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;

    private static TiltController activeController = null;

    private void Awake()
    {
        mCanvas = GetComponent<Canvas>();
    }

    void Start()
    {
        //Fetch the Raycaster from the GameObject (the Canvas)
        m_Raycaster = GetComponent<GraphicRaycaster>();
        m_Raycaster.ignoreReversedGraphics = false;
        //Fetch the Event System from the Scene
        m_EventSystem = GetComponent<EventSystem>();
        mCanvas.worldCamera = Camera.main;
        localAxis = RotationAxis == 'X' ? CuttingPlane.forward : RotationAxis == 'Y' ? CuttingPlane.up : CuttingPlane.right;
    }

    void Update()
    {
        //Check if the left Mouse button is clicked
        if ((activeController == this || activeController == null) && Input.GetKey(KeyCode.Mouse0))
        {
            //Set up the new Pointer Event
            m_PointerEventData = new PointerEventData(m_EventSystem);
            //Set the Pointer Event Position to that of the mouse position
            m_PointerEventData.position = Input.mousePosition;

            //Create a list of Raycast Results
            List<RaycastResult> results = new List<RaycastResult>();

            //Raycast using the Graphics Raycaster and mouse click position
            m_Raycaster.Raycast(m_PointerEventData, results);

            foreach (RaycastResult result in results)
            {
                if (result.gameObject == UIImage)
                {
                    activeController = this;
                    if (!zeroSet)
                    { 
                        zeroAngle = result.worldPosition;
                        zeroSet = true;
                    }
                    else {
                        float angle = Vector3.SignedAngle(zeroAngle, result.worldPosition, localAxis);
                        if (Vector3.Dot(transform.forward, Camera.main.transform.forward) < 0) angle = -angle;
                        CuttingPlane.Rotate(localAxis, angle);
                        zeroAngle = result.worldPosition;
                    }
                }
            }
        }
        else if (activeController == this)
        {
            activeController = null;
            zeroSet = false;
        }
    }
}
