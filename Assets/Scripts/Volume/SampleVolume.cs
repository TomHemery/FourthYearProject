using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleVolume : MonoBehaviour
{
    Texture3D tex = null;
    public Vector3 TexPos { get; private set; } = new Vector3(0,0,0);
    public Vector3 RelativePos { get; private set; } = new Vector3(0, 0, 0);

    /// <summary>
    /// Samples the volume attached to this game object (NB: O(neighbourhood^3))
    /// </summary>
    /// <param name="pos">position relative to the center of this object to be sampled </param>
    /// <param name="neighbourhood">how wide to make the neighbourhood (1 = +/- 1 in each x y and z)</param>
    /// <returns>float density -> average density of the specified position and neighbourhood</returns>
    public float SampleVolumeDensityAt(Vector3 pos, int neighbourhood = 0) {
        //get texture and cache it if we haven't already
        if (tex == null) tex = (Texture3D)gameObject.GetComponent<Renderer>().material.GetTexture(VolumeManager.Instance.MaterialTextureName);

        RelativePos = pos;
        Transform cubeTransform = VolumeManager.Instance.VolumeCube.transform;

        //map relative position to an absolute texture position
        TexPos = new Vector3((int)Map(pos.x, -1 * cubeTransform.localScale.x / 2, cubeTransform.localScale.x / 2, 0, tex.width),
                             (int)Map(pos.y, -1 * cubeTransform.localScale.y / 2, cubeTransform.localScale.y / 2, 0, tex.height),
                             (int)Map(pos.z, -1 * cubeTransform.localScale.z / 2, cubeTransform.localScale.z / 2, 0, tex.depth));

        //sample the texture at the relative point (average the neighbourhood)
        float value = 0.0f;
        int numSamples = 0;
        //ouch don't set neighbourhood too high or this will take a while
        for (int x = -neighbourhood; x <= neighbourhood; x++) 
        {
            for (int y = -neighbourhood; y <= neighbourhood; y++)
            {
                for (int z = -neighbourhood; z <= neighbourhood; z++)
                {
                    numSamples++;
                    Color c = tex.GetPixel(Mathf.FloorToInt(TexPos.x), Mathf.FloorToInt(TexPos.y), Mathf.FloorToInt(TexPos.z));
                    value = (value * (numSamples - 1) + (c.r + c.g + c.b) / 3) / numSamples;
                }
            }
        }
        //Color c = tex.GetPixel(Mathf.FloorToInt(TexPos.x), Mathf.FloorToInt(TexPos.y), Mathf.FloorToInt(TexPos.z));
        return value;
    }

    public static float Map(float value, float start1, float stop1, float start2, float stop2)
    {
        return start2 + (stop2 - start2) * ((value - start1) / (stop1 - start1));
    }
}
