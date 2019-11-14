using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TranslateController : MonoBehaviour, IDragHandler
{
    public Transform CuttingPlane;
    public Vector3 Axis;
    public GameObject UIImage;

    public Transform Back;
    public Transform Front;

    private Vector3 baseRotation;
    private Canvas mCanvas;

    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;

    private static TranslateController activeController = null;

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

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 arrowStart = Camera.main.WorldToScreenPoint(Front.position);
        Vector2 arrowEnd = Camera.main.WorldToScreenPoint(Back.position);

        if (Mathf.Abs(arrowStart.x - arrowEnd.x) > Mathf.Abs(arrowStart.y - arrowEnd.y))//arrow more horizontal
        {
            if (arrowStart.x < arrowEnd.x)//arrow pointing left
            { 
                CuttingPlane.transform.Translate(Axis * eventData.delta.x * Time.deltaTime);
            }
            else //arrow pointing right
            {
                CuttingPlane.transform.Translate(Axis * -eventData.delta.x * Time.deltaTime);
            }
        }
        else //arrow more vertical
        {
            if (arrowStart.y < arrowEnd.y)//arrow pointing left
            {
                CuttingPlane.transform.Translate(Axis * eventData.delta.y * Time.deltaTime);
            }
            else //arrow pointing right
            {
                CuttingPlane.transform.Translate(Axis * -eventData.delta.y * Time.deltaTime);
            }
        }
    }
}
