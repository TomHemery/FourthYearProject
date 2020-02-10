using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestVolumeSampling : MonoBehaviour
{
    private Text text;
    public ControllerCollisionBehaviour collisionBehaviour;
    private void Awake()
    {
        text = GetComponent<Text>();
    }
    // Update is called once per frame
    void Update()
    {
        text.text = "" + collisionBehaviour.Density + 
            (
                collisionBehaviour.LastVolumeSampler == null ? "" : 
                ("\n" + collisionBehaviour.LastVolumeSampler.RelativePos + "\n" + collisionBehaviour.LastVolumeSampler.TexPos)
            );
    }
}
