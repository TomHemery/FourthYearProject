using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleVolume : MonoBehaviour
{
    Texture3D tex = null;

    private void Start()
    {
        
    }
    public float SampleVolumeDensityAt(Vector3 pos) {
        //get texture and cache it if we haven't already
        if (tex == null) tex = (Texture3D)gameObject.GetComponent<Renderer>().material.GetTexture(VolumeManager.Instance.MaterialTextureName);

        Transform cubeTransform = VolumeManager.Instance.VolumeCube.transform;
        //map relative position to an absolute texture position
        pos.x = Map(pos.x, -1 * cubeTransform.localScale.x / 2, cubeTransform.localScale.x / 2, 0, tex.width);
        pos.y = Map(pos.y, -1 * cubeTransform.localScale.y / 2, cubeTransform.localScale.y / 2, 0, tex.height);
        pos.z = Map(pos.z, -1 * cubeTransform.localScale.z / 2, cubeTransform.localScale.z / 2, 0, tex.depth);
        //sample the texture at the specific point
        Color c = tex.GetPixel(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
        return (c.r + c.g + c.b) / 3;
    }

    public static float Map(float value, float start1, float stop1, float start2, float stop2)
    {
        return start2 + (stop2 - start2) * ((value - start1) / (stop1 - start1));
    }
}
