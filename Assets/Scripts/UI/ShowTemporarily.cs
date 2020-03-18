using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowTemporarily : MonoBehaviour
{
    public float lifeTime = 2.0f;
    private float cooldown = 0.0f;


    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        cooldown += Time.deltaTime;
        if (cooldown > lifeTime) {
            cooldown = 0.0f;
            gameObject.SetActive(false);
        }
    }

    public void Show() {
        gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        cooldown = 0.0f;
        gameObject.SetActive(false);
    }
}
