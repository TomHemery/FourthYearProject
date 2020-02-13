using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StopAllSliderInput : MonoBehaviour
{
    private Slider[] allSliders;
    public void Start()
    {
        allSliders = GetComponentsInChildren<Slider>();
    }
    public void StopAllSliders() {
        PointerEventData e = new PointerEventData(EventSystem.current);
        foreach (Slider s in allSliders) {
            s.OnPointerUp(e);
        }
    }
}
