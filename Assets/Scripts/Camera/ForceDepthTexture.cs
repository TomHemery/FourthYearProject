using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceDepthTexture : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
    }
}
