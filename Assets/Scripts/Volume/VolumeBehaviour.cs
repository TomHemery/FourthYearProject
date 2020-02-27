using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using UnityEditor;
public class VolumeBehaviour : MonoBehaviour
{
    public static List<VolumeBehaviour> AllRenderingVolumes { get; private set; } = null;
    public static string CurrentVolumeName { get; private set; } = null;

    public static Texture3D VolumeTexture { get; private set; }
    private Material mMaterial;
    private Texture2D[] slices;

    public static readonly string defaultFileName = "Head";

    public float Density { get; private set; } = 1;
    public float Threshold { get; private set; } = 0.0f;
    public int SamplingQuality { get; private set; } = 64;
    public Vector3 OcclusionPlanePos { get; private set; } = new Vector3(-100, 0, 0);
    public Vector3 OcclusionPlaneNormal { get; private set; } = new Vector3();

    public bool DoOcclusion { get; private set; } = false;
    public bool DoCutting { get; private set; } = false;

    public bool RenderRed { get; private set; } = true;
    public bool RenderGreen { get; private set; } = true;
    public bool RenderBlue { get; private set; } = true;

    public const string MAIN_TEXTURE_TAG = "_MainTex";
    private const string DENSITY_TAG = "_Density";
    private const string SAMPLE_QUALITY_TAG = "_SamplingQuality";
    private const string THRESHOLD_TAG = "_Threshold";
    private const string RED_TAG = "_Red";
    private const string GREEN_TAG = "_Green";
    private const string BLUE_TAG = "_Blue";
    private const string OCCLUSION_POS_TAG = "_OcclusionPlanePos";
    private const string OCCLUSION_NORMAL_TAG = "_OcclusionPlaneNormal";
    private const string DO_OCCLUSION_TAG = "_DoOcclusion";
    private const string CUTTING_PLANE_TAG = "_CuttingPlanePositions";
    private const string CUTTING_NORMAL_TAG = "_CuttingPlaneNormals";
    private const string DO_CUTTING_TAG = "_DoCutting";
    private const string NUM_CUTTING_PLANES_TAG = "_NumCuttingPlanes";

    public const string VOLUMETRIC_DATA_PATH = "/VolumetricData/";
    public const string CACHE_PATH = "/VolumetricCache/";

    private const int MAX_CUTTING_PLANES = 5;
    [HideInInspector]
    public int numActiveCuttingPlanes = 0;
    [HideInInspector]
    public Transform [] cuttingPlaneTransforms = new Transform[MAX_CUTTING_PLANES];

    public Vector4[] CuttingPlanePositions { get; private set; } = new Vector4[MAX_CUTTING_PLANES];
    public Vector4[] CuttingPlaneNormals { get; private set; } = new Vector4[MAX_CUTTING_PLANES];

    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++) {
            cuttingPlaneTransforms[i] = transform.GetChild(i);
        }

        if (AllRenderingVolumes == null) AllRenderingVolumes = new List<VolumeBehaviour>();
        AllRenderingVolumes.Add(this);
    }

    private void OnDestroy()
    {
        AllRenderingVolumes.Remove(this);
    }

    private void Update()
    {
        if (DoCutting) {
            SetCuttingPlanes();
        }
    }

    public void LoadVolume(string name)
    {
        if (CurrentVolumeName != name)
        {
            CurrentVolumeName = name;
            VolumeTexture = LoadFromDisk(name);
        }

        mMaterial = GetComponent<Renderer>().material;
        mMaterial.SetTexture(MAIN_TEXTURE_TAG, VolumeTexture);

        mMaterial.SetFloat(DENSITY_TAG, Density);
        mMaterial.SetFloat(THRESHOLD_TAG, Threshold);
        mMaterial.SetInt(SAMPLE_QUALITY_TAG, SamplingQuality);

        mMaterial.SetInt(RED_TAG, 1);
        mMaterial.SetInt(BLUE_TAG, 1);
        mMaterial.SetInt(GREEN_TAG, 1);

        SetDoOcclusion(true);
    }

    Texture3D LoadFromDisk(string name) {
        string path = Directory.GetCurrentDirectory() + VOLUMETRIC_DATA_PATH + name;
        string[] slicePaths = Directory.GetFiles(path);

        if (slicePaths.Length > 0) {

            System.Drawing.Image test = System.Drawing.Image.FromFile(slicePaths[0]);
            Vector3Int dims = new Vector3Int(test.Width, test.Height, slicePaths.Length);
            
            Texture2D[] slices = new Texture2D[slicePaths.Length];
            for (int i = 0; i < slicePaths.Length; i++) {
                slices[i] = new Texture2D(dims.x, dims.y);
                slices[i].LoadImage(File.ReadAllBytes(slicePaths[i]));
            }

            return CreateTexture3D(slices);
        }
        return null;
    }

    public void SetDensity(float d)
    {
        Density = d;
        mMaterial.SetFloat(DENSITY_TAG, d);

    }

    public void SetThreshold(float t)
    {
        Threshold = t;
        mMaterial.SetFloat(THRESHOLD_TAG, t);
    }

    public void SetQuality(int q)
    {
        SamplingQuality = q;
        mMaterial.SetFloat(SAMPLE_QUALITY_TAG, q);
    }

    public void SetRenderRed(bool r)
    {
        RenderRed = r;
        mMaterial.SetInt(RED_TAG, r ? 1 : 0);
    }
    public void SetRenderGreen(bool g)
    {
        RenderGreen = g;
        mMaterial.SetInt(GREEN_TAG, g ? 1 : 0);
    }

    public void SetRenderBlue(bool b)
    {
        RenderBlue = b;
        mMaterial.SetInt(BLUE_TAG, b ? 1 : 0);
    }

    public void SetOcclusionPlane(Vector3 planePos, Vector3 planeNormal)
    {
        OcclusionPlanePos = transform.InverseTransformPoint(planePos);
        OcclusionPlaneNormal = transform.InverseTransformDirection(planeNormal);
        mMaterial.SetVector(OCCLUSION_POS_TAG, OcclusionPlanePos);
        mMaterial.SetVector(OCCLUSION_NORMAL_TAG, OcclusionPlaneNormal);
    }

    public void SetCuttingPlanes() {
        for (int i = 0; i < numActiveCuttingPlanes; i++)
        {
            CuttingPlanePositions[i] = transform.InverseTransformPoint(cuttingPlaneTransforms[i].position);
            CuttingPlaneNormals[i] = transform.InverseTransformDirection(cuttingPlaneTransforms[i].forward);
        }

        mMaterial.SetVectorArray(CUTTING_PLANE_TAG, CuttingPlanePositions);
        mMaterial.SetVectorArray(CUTTING_NORMAL_TAG, CuttingPlaneNormals);
        mMaterial.SetInt(NUM_CUTTING_PLANES_TAG, numActiveCuttingPlanes);
    }

    public void SetDoOcclusion(bool doOcclusion)
    {
        mMaterial.SetInt(DO_OCCLUSION_TAG, doOcclusion ? 1 : 0);
        DoOcclusion = doOcclusion;
    }

    public void SetDoCutting(bool doCutting) 
    {
        mMaterial.SetInt(DO_CUTTING_TAG, doCutting ? 1 : 0);
        DoCutting = doCutting;
    }

    //Creates one 3D texture with all colour information
    Texture3D CreateTexture3D(Texture2D[] imageStack)
    {
        int imageWidth = imageStack[0].width;
        int imageHeight = imageStack[0].height;
        Color[] colorArray = new Color[imageWidth * imageHeight * imageStack.Length];
        Texture3D texture = new Texture3D(imageStack[0].width, imageStack[1].height, imageStack.Length, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Trilinear;
        Texture2D slice;
        for (int i = 0; i < imageStack.Length; i++)
        {
            slice = imageStack[i];
            for (int x = 0; x < imageWidth; x++)
            {
                for (int y = 0; y < imageHeight; y++)
                {
                    colorArray[x + (y * imageWidth) + (i * imageHeight * imageWidth)] = slice.GetPixel(x, y);
                }
            }
        }
        texture.SetPixels(colorArray);
        texture.Apply();
        return texture;
    }

    public GameObject Split(Vector3 target) {
        if (numActiveCuttingPlanes < MAX_CUTTING_PLANES)
        {
            cuttingPlaneTransforms[numActiveCuttingPlanes].LookAt(target);

            GameObject clone = Instantiate(gameObject);

            VolumeBehaviour cloneBehaviour = clone.GetComponent<VolumeBehaviour>();
            cloneBehaviour.LoadVolume(CurrentVolumeName);

            SetDoCutting(true);
            cloneBehaviour.SetDoCutting(true);
            numActiveCuttingPlanes++;
            cloneBehaviour.numActiveCuttingPlanes++;

            cloneBehaviour.cuttingPlaneTransforms[numActiveCuttingPlanes - 1].forward = -cuttingPlaneTransforms[numActiveCuttingPlanes - 1].forward;

            cloneBehaviour.SetRenderRed(RenderRed);
            cloneBehaviour.SetRenderGreen(RenderGreen);
            cloneBehaviour.SetRenderBlue(RenderBlue);

            cloneBehaviour.SetThreshold(Threshold);
            cloneBehaviour.SetDensity(Density);
            cloneBehaviour.SetQuality(SamplingQuality);

            SetCuttingPlanes();
            cloneBehaviour.SetCuttingPlanes();

            return clone;
        }
        return null;
    }
}
