using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR.Extras;

public class LaserPointerEventWrapper : MonoBehaviour
{
    private SteamVR_LaserPointer steamVrLaserPointer;

    private void Awake()
    {
        steamVrLaserPointer = gameObject.GetComponent<SteamVR_LaserPointer>();
        steamVrLaserPointer.PointerIn += OnPointerIn;
        steamVrLaserPointer.PointerOut += OnPointerOut;
        steamVrLaserPointer.PointerClick += OnPointerClick;
    }

    private void OnPointerClick(object sender, PointerEventArgs e)
    {

        Debug.Log("Pointer clicked! Target: " + e.target);
        IPointerClickHandler clickHandler = e.target.GetComponent<IPointerClickHandler>();
        if (clickHandler == null)
        {
            return;
        }


        clickHandler.OnPointerClick(new PointerEventData(EventSystem.current));
    }

    private void OnPointerOut(object sender, PointerEventArgs e)
    {
        Debug.Log("Pointer out! Target: " + e.target);
        IPointerExitHandler pointerExitHandler = e.target.GetComponent<IPointerExitHandler>();

        if (pointerExitHandler != null) pointerExitHandler.OnPointerExit(new PointerEventData(EventSystem.current));
    }

    private void OnPointerIn(object sender, PointerEventArgs e)
    {
        Debug.Log("Pointer in! Target: " + e.target);
        IPointerEnterHandler pointerEnterHandler = e.target.GetComponent<IPointerEnterHandler>();

        if (pointerEnterHandler != null) pointerEnterHandler.OnPointerEnter(new PointerEventData(EventSystem.current));
    }
}
