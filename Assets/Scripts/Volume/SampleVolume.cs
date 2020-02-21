using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleVolume : MonoBehaviour
{
    Texture3D tex = null;
    public Vector3 TexPos { get; private set; } = new Vector3(0,0,0);
    public Vector3 RelativePos { get; private set; } = new Vector3(0, 0, 0);

    private static readonly Vector3 half = new Vector3(0.5f, 0.5f, 0.5f);

    private VolumeBehaviour mVolumeBehaviour;

    private void Awake()
    {
        mVolumeBehaviour = GetComponent<VolumeBehaviour>();
    }

    /// <summary>
    /// Samples the volume attached to this game object (NB: O(neighbourhood^3))
    /// </summary>
    /// <param name="pos">position relative to the center of this object to be sampled </param>
    /// <param name="neighbourhood">how wide to make the neighbourhood (1 = +/- 1 in each x y and z)</param>
    /// <returns>float density -> average density of the specified position and neighbourhood</returns>
    public float SampleVolumeDensityAt(Vector3 pos, int neighbourhood = 0) {
        //get texture and cache it if we haven't already
        if (tex == null) tex = (Texture3D)gameObject.GetComponent<Renderer>().material.GetTexture(VolumeBehaviour.MAIN_TEXTURE_TAG);

        RelativePos = pos - half;

        //don't sample the volume if it is occluded by the cutting plane
        Vector3 between = pos - mVolumeBehaviour.OcclusionPlanePos;
        float test = Vector3.Dot(between, mVolumeBehaviour.OcclusionPlaneNormal);
        if (test <= 0) return 0;

        //map relative position to an absolute texture position
        TexPos = new Vector3(RelativePos.x * tex.width, RelativePos.y * tex.height, RelativePos.z * tex.depth);

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
                    Color c = tex.GetPixel(Mathf.FloorToInt(TexPos.x + x), Mathf.FloorToInt(TexPos.y + y), Mathf.FloorToInt(TexPos.z + z));
                    value = (value * (numSamples - 1) + (c.r*0.3f + c.g*0.59f + c.b*0.11f)) / numSamples;
                }
            }
        }

        //don't sample if we find that brightness is less than the rendering threshold set in the volume manager 
        if (value < mVolumeBehaviour.Threshold) value = 0;

        return value;
    }

    public static float Map(float value, float start1, float stop1, float start2, float stop2)
    {
        return start2 + (stop2 - start2) * ((value - start1) / (stop1 - start1));
    }
}
