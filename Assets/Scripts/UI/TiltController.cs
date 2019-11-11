using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TiltController : MonoBehaviour
{
    public Transform CuttingPlane;
    public Vector3 Axis;
    public Vector3 ZeroAngle;
    public GameObject UIImage;

    private Vector3 baseRotation;
    private Canvas mCanvas;

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
        //Fetch the Event System from the Scene
        m_EventSystem = GetComponent<EventSystem>();
        baseRotation = CuttingPlane.localRotation.eulerAngles;
        mCanvas.worldCamera = Camera.main;
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
                    if (activeController == null) Debug.Log("Active controller = " + this);
                    activeController = this;
                    float angle = Vector3.Angle(ZeroAngle, result.worldPosition);

                    Debug.Log("Angle: " + angle);
                    Debug.Log("World position: " + result.worldPosition);

                    if (Axis.x != 0)
                    {
                        if (result.worldPosition.x < 0) angle = -angle;
                    }
                    else if (Axis.y != 0)
                    {
                        if (result.worldPosition.z > 0) angle = -angle;
                    }
                    else if (Axis.z != 0)
                    {
                        if (result.worldPosition.z < 0) angle = -angle;
                    }

                    CuttingPlane.rotation = Quaternion.Euler(Axis * angle + baseRotation);
                }
            }
        }
        else if (activeController == this)
        {
            activeController = null;
            Debug.Log("Active controller removed");
        }
        else
            baseRotation = CuttingPlane.localRotation.eulerAngles;
    }
}
