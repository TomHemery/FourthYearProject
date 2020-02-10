using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSetup : MonoBehaviour
{ 
    private void Start()
    {
        Camera.main.depthTextureMode = DepthTextureMode.Depth;
    }
}
