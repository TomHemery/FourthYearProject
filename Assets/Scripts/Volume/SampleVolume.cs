using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleVolume : MonoBehaviour
{
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
        Vector3 between;
        float test;
        //don't sample the volume if it is occluded by the occlusion plane
        if (mVolumeBehaviour.DoOcclusion)
        {
            between = pos - mVolumeBehaviour.OcclusionPlanePos;
            test = Vector3.Dot(between, mVolumeBehaviour.OcclusionPlaneNormal);
            if (test <= 0) return 0;
        }

        if (mVolumeBehaviour.DoCutting)
        {
            //don't sample the volume if it is occluded by a cutting plane 
            for (int i = 0; i < mVolumeBehaviour.numActiveCuttingPlanes; i++) {
                Vector3 planePos = mVolumeBehaviour.CuttingPlanePositions[i];
                Vector3 planeNormal = mVolumeBehaviour.CuttingPlaneNormals[i];
                between = pos - planePos;
                test = Vector3.Dot(between, planeNormal);
                if (test <= 0) return 0;
            }
            
            
        }

        return SampleUnconditionallyAt(pos, neighbourhood);
    }

    /// <summary>
    /// Samples the volume attached to this game object (NB: O(neighbourhood^3))
    /// </summary>
    /// <param name="pos">position relative to the center of this object to be sampled </param>
    /// <param name="neighbourhood">how wide to make the neighbourhood (1 = +/- 1 in each x y and z)</param>
    /// <returns>float density -> average density of the specified position and neighbourhood</returns>
    public float SampleUnconditionallyAt(Vector3 pos, int neighbourhood = 0) {
        RelativePos = pos - half;
        //map relative position to an absolute texture position
        TexPos = new Vector3(RelativePos.x * VolumeBehaviour.VolumeTexture.width, RelativePos.y * VolumeBehaviour.VolumeTexture.height, RelativePos.z * VolumeBehaviour.VolumeTexture.depth);

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
                    Color c = VolumeBehaviour.VolumeTexture.GetPixel(Mathf.FloorToInt(TexPos.x + x), Mathf.FloorToInt(TexPos.y + y), Mathf.FloorToInt(TexPos.z + z));
                    value = (value * (numSamples - 1) + (c.r * 0.3f + c.g * 0.59f + c.b * 0.11f)) / numSamples;
                }
            }
        }

        //don't sample if we find that brightness is less than the rendering threshold set in the volume manager 
        if (value < mVolumeBehaviour.Threshold) value = 0;

        return value;
    }

    public Color SampleColourAt(Vector3 pos) {
        RelativePos = pos - half;
        TexPos = new Vector3(RelativePos.x * VolumeBehaviour.VolumeTexture.width, RelativePos.y * VolumeBehaviour.VolumeTexture.height, RelativePos.z * VolumeBehaviour.VolumeTexture.depth);
        return VolumeBehaviour.VolumeTexture.GetPixel(Mathf.FloorToInt(TexPos.x), Mathf.FloorToInt(TexPos.y), Mathf.FloorToInt(TexPos.z));
    }

    public static float Map(float value, float start1, float stop1, float start2, float stop2)
    {
        return start2 + (stop2 - start2) * ((value - start1) / (stop1 - start1));
    }
}
