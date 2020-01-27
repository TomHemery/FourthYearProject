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

        steamVrLaserPointer.PointerUp += OnPointerUp;
        steamVrLaserPointer.PointerDown += OnPointerDown;

        steamVrLaserPointer.PointerDrag += OnPointerDrag;
        steamVrLaserPointer.PointerBeginDrag += OnPointerBeginDrag;
        steamVrLaserPointer.PointerEndDrag += OnPointerEndDrag;
    }

    private void OnPointerClick(object sender, PointerEventArgs e)
    {
        IPointerClickHandler clickHandler = e.target.GetComponent<IPointerClickHandler>();
        clickHandler?.OnPointerClick(new PointerEventData(EventSystem.current));
    }

    private void OnPointerUp(object sender, PointerEventArgs e)
    {
        IPointerUpHandler upHandler = e.target.GetComponent<IPointerUpHandler>();
        upHandler?.OnPointerUp(new PointerEventData(EventSystem.current));
    }

    private void OnPointerDown(object sender, PointerEventArgs e) {
        IPointerDownHandler downHandler = e.target.GetComponent<IPointerDownHandler>();
        downHandler?.OnPointerDown(new PointerEventData(EventSystem.current));
    }

    private void OnPointerBeginDrag(object sender, PointerEventArgs e) {
        IBeginDragHandler dragHandler = e.target.GetComponent<IBeginDragHandler>();
        PointerEventData dat = new PointerEventData(EventSystem.current);
        dat.position = e.position; 
        dragHandler?.OnBeginDrag(dat);
    }

    private void OnPointerEndDrag(object sender, PointerEventArgs e) {
        IEndDragHandler dragHandler = e.target.GetComponent<IEndDragHandler>();
        PointerEventData dat = new PointerEventData(EventSystem.current);
        dat.position = e.position;
        dragHandler?.OnEndDrag(dat);
    }

    private void OnPointerDrag(object sender, PointerEventArgs e) {
        IDragHandler dragHandler = e.target.GetComponent<IDragHandler>();
        PointerEventData dat = new PointerEventData(EventSystem.current);
        dat.position = e.position;
        dragHandler?.OnDrag(dat);
    }

    private void OnPointerOut(object sender, PointerEventArgs e)
    {
        IPointerExitHandler pointerExitHandler = e.target.GetComponent<IPointerExitHandler>();
        pointerExitHandler?.OnPointerExit(new PointerEventData(EventSystem.current));
    }

    private void OnPointerIn(object sender, PointerEventArgs e)
    {
        IPointerEnterHandler pointerEnterHandler = e.target.GetComponent<IPointerEnterHandler>();
        pointerEnterHandler?.OnPointerEnter(new PointerEventData(EventSystem.current));
    }
}
