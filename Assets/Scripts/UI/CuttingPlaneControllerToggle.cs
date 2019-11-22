using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CuttingPlaneControllerToggle : MonoBehaviour, IPointerClickHandler
{
    public GameObject RotationController;
    public GameObject TranslationController;
    private ControlState currentState;
    private bool active = false;

    private void Awake()
    {
        currentState = ControlState.Rotate;
    }

    private void Start()
    {
        if (RotationController != null) RotationController.SetActive(false);
        if (TranslationController != null) TranslationController.SetActive(false);
        gameObject.GetComponent<Canvas>().worldCamera = Camera.main;
    }

    private void Update()
    {
        if (active)
        {
            if (Input.GetKeyUp(KeyCode.R))
            {
                currentState = ControlState.Rotate;
                if (RotationController != null) RotationController.SetActive(true);
                if (TranslationController != null) TranslationController.SetActive(false);
            }
            else if (Input.GetKeyUp(KeyCode.T))
            {
                currentState = ControlState.Translate;
                if (RotationController != null) RotationController.SetActive(false);
                if (TranslationController != null) TranslationController.SetActive(true);
            }

            else if (Input.GetKeyUp(KeyCode.Delete))
            {
                Deactivate();
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Activate();
    }

    private void Activate()
    {
        active = true;
        switch (currentState)
        {
            case ControlState.Rotate:
                if (RotationController != null) RotationController.SetActive(true);
                if (TranslationController != null) TranslationController.SetActive(false);
                break;
            case ControlState.Translate:
                if (RotationController != null) RotationController.SetActive(false);
                if (TranslationController != null) TranslationController.SetActive(true);
                break;
        }
    }

    private void Deactivate()
    {
        active = false;
        if (RotationController != null) RotationController.SetActive(false);
        if (TranslationController != null) TranslationController.SetActive(false);
    }

    private enum ControlState { 
        Rotate,
        Translate
    }
}
