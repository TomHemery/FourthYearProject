using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

public class VolumeBehaviour : MonoBehaviour
{
    public static List<VolumeBehaviour> AllRenderingVolumes { get; private set; } = null;
    public static string CurrentVolumeName { get; private set; } = null;

    public Texture3D VolumeTexture { get; private set; }
    private Material mMaterial;
    private Texture2D[] slices;

    public static readonly string defaultFileName = "Head";

    public float Density { get; private set; } = 1;
    public float Threshold { get; private set; } = 0.0f;
    public int SamplingQuality { get; private set; } = 64;
    public Vector3 OcclusionPlanePos { get; private set; } = new Vector3(-100, 0, 0);
    public Vector3 OcclusionPlaneNormal { get; private set; } = new Vector3();

    public Vector3 CuttingPlanePos = new Vector3();
    public Vector3 CuttingPlaneNormal { get; private set; } = new Vector3();
    public bool DoOcclusion { get; private set; } = false;
    public bool DoCutting { get; private set; } = false;

    public bool RenderRed { get; private set; } = true;
    public bool RenderGreen { get; private set; } = true;
    public bool RenderBlue { get; private set; } = true;

    [HideInInspector]
    public Transform CuttingPlaneTransform;

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
    private const string CUTTING_PLANE_TAG = "_CuttingPlanePos";
    private const string CUTTING_NORMAL_TAG = "_CuttingPlaneNormal";
    private const string DO_CUTTING_TAG = "_DoCutting";

    public const string VOLUMETRIC_DATA_PATH = "Volumetric Data/";
    public const string CACHE_PATH = "Assets/Resources/VolumeCache/";
    public const string CACHE_PATH_SHORT = "VolumeCache/";

    private void Awake()
    {
        CuttingPlaneTransform = transform.GetChild(0);
        if (AllRenderingVolumes == null) AllRenderingVolumes = new List<VolumeBehaviour>();
        AllRenderingVolumes.Add(this);
    }

    private void OnDestroy()
    {
        AllRenderingVolumes.Remove(this);
    }

    private void Update()
    {
        if (DoCutting) SetCuttingPlane(CuttingPlaneTransform.position, CuttingPlaneTransform.forward);
    }

    public void LoadVolume(string name)
    {
        CurrentVolumeName = name;
        //look for a cached instance of volume
        VolumeTexture = Resources.Load<Texture3D>(CACHE_PATH_SHORT + name);
        //if none exists then create it
        if (VolumeTexture == null)
        {
            slices = Resources.LoadAll(VOLUMETRIC_DATA_PATH + name, typeof(Texture2D)).Cast<Texture2D>().ToArray();
            VolumeTexture = CreateTexture3D(slices);
            AssetDatabase.CreateAsset(VolumeTexture, CACHE_PATH + name + ".asset");
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

        //could uncomment this line if memory usage is an issue, but it's much much faster if you don't 
        //Resources.UnloadUnusedAssets();
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

    public void SetCuttingPlane(Vector3 planePos, Vector3 planeNormal) {
        CuttingPlanePos = transform.InverseTransformPoint(planePos);
        CuttingPlaneNormal = transform.InverseTransformDirection(planeNormal);
        mMaterial.SetVector(CUTTING_PLANE_TAG, CuttingPlanePos);
        mMaterial.SetVector(CUTTING_NORMAL_TAG, CuttingPlaneNormal);
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

    public GameObject Split() {
        GameObject clone = Instantiate(gameObject);

        clone.GetComponent<VolumeBehaviour>().LoadVolume(CurrentVolumeName);
        clone.GetComponent<VolumeBehaviour>().CuttingPlaneTransform.Rotate(0, 180, 0);

        Vector3 offset = CuttingPlaneTransform.forward * 0.1f;
        clone.transform.position -= offset;
        transform.position += offset;

        SetDoCutting(true);
        VolumeBehaviour cloneBehaviour = clone.GetComponent<VolumeBehaviour>();
        cloneBehaviour.SetDoCutting(true);

        cloneBehaviour.SetRenderRed(RenderRed);
        cloneBehaviour.SetRenderGreen(RenderGreen);
        cloneBehaviour.SetRenderBlue(RenderBlue);

        cloneBehaviour.SetThreshold(Threshold);
        cloneBehaviour.SetDensity(Density);
        cloneBehaviour.SetQuality(SamplingQuality);

        clone.GetComponent<VolumeBehaviour>().SetDoCutting(true);

        return clone;
    }
}
