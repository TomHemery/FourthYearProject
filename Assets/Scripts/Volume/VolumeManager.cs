using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

public class VolumeManager : MonoBehaviour
{
    public static VolumeManager Instance { private set; get; } = null;

    public Texture3D Volume { get; private set; }
    private Material cubeMaterial;
    public GameObject VolumeCube { get; private set; } = null;
    private Texture2D[] slices;
    private CuttingPlane cuttingPlane;

    [SerializeField] public string MaterialTextureName;
    public GameObject rendererPrefab;
    public string defaultFolderName;
    public ViewResetter viewResetter;
    public Vector3 instantiatePosition;
    public float volumeScale = 0.5f;

    public float Density { get; private set; } = 1;
    public float Threshold { get; private set; } = 0.0f;
    public int SamplingQuality { get; private set; } = 64;
    public Vector3 PlanePos { get; private set; } = new Vector3(-100, 0, 0);
    public Vector3 PlaneNormal { get; private set; } = new Vector3();
    public bool DoSlicing { get; private set; } = false;

    private const string DENSITY_TAG = "_Density";
    private const string SAMPLE_QUALITY_TAG = "_SamplingQuality";
    private const string THRESHOLD_TAG = "_Threshold";
    private const string RED_TAG = "_Red";
    private const string GREEN_TAG = "_Green";
    private const string BLUE_TAG = "_Blue";
    private const string PLANE_POSITION_TAG = "_PlanePos";
    private const string PLANE_NORMAL_TAG = "_PlaneNormal";
    private const string DO_SLICING_TAG = "_DoSlicing";

    public const string VOLUMETRIC_DATA_PATH = "Volumetric Data/";
    public const string CACHE_PATH = "Assets/Resources/VolumeCache/";
    public const string CACHE_PATH_SHORT = "VolumeCache/";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        if (defaultFolderName != "")LoadVolume(defaultFolderName);
    }

    public void LoadVolume(string name) {
        
        if (VolumeCube != null) Destroy(VolumeCube);

        //look for a cached instance of volume
        Volume = Resources.Load<Texture3D>(CACHE_PATH_SHORT + name);
        //if none exists then create it
        if (Volume == null)
        {
            slices = Resources.LoadAll(VOLUMETRIC_DATA_PATH + name, typeof(Texture2D)).Cast<Texture2D>().ToArray();
            Volume = CreateTexture3D(slices);
            AssetDatabase.CreateAsset(Volume, CACHE_PATH + name + ".asset");
        }

        VolumeCube = Instantiate(rendererPrefab, instantiatePosition, Quaternion.identity);
        VolumeCube.transform.localScale = new Vector3(volumeScale, volumeScale, volumeScale);
        cubeMaterial = VolumeCube.GetComponent<Renderer>().material;
        cubeMaterial.SetTexture(MaterialTextureName, Volume);

        cubeMaterial.SetFloat(DENSITY_TAG, Density);
        cubeMaterial.SetFloat(THRESHOLD_TAG, 0.0f);
        cubeMaterial.SetInt(SAMPLE_QUALITY_TAG, SamplingQuality);

        cubeMaterial.SetInt(RED_TAG, 1);
        cubeMaterial.SetInt(BLUE_TAG, 1);
        cubeMaterial.SetInt(GREEN_TAG, 1);

        SetDoSlicing(true);

        //could uncomment this line if memory usage is an issue, but it's much much faster if you don't 
        //Resources.UnloadUnusedAssets();
    }

    public void SetDensity(float d)
    {
        Density = d;
        cubeMaterial.SetFloat(DENSITY_TAG, d);
        
    }

    public void SetThreshold(float t) {
        Threshold = t;
        cubeMaterial.SetFloat(THRESHOLD_TAG, t);
    }

    public void SetQuality(int q)
    {
        SamplingQuality = q;
        cubeMaterial.SetFloat(SAMPLE_QUALITY_TAG, q); 
    }

    public void SetRenderRed(bool r) 
    {
        cubeMaterial.SetInt(RED_TAG, r ? 1 : 0);
    }
    public void SetRenderGreen(bool g)
    {
        cubeMaterial.SetInt(GREEN_TAG, g ? 1 : 0);
    }

    public void SetRenderBlue(bool b)
    {
        cubeMaterial.SetInt(BLUE_TAG, b ? 1 : 0);
    }

    public void SetPlane(Vector3 planePos, Vector3 normal) {
        PlanePos = VolumeCube.transform.InverseTransformPoint(planePos);
        PlaneNormal = VolumeCube.transform.InverseTransformDirection(normal);
        cubeMaterial.SetVector(PLANE_POSITION_TAG, PlanePos);
        cubeMaterial.SetVector(PLANE_NORMAL_TAG, PlaneNormal);
    }

    public void SetDoSlicing(bool doSlicing) {
        cubeMaterial.SetInt(DO_SLICING_TAG, doSlicing ? 1 : 0);
        DoSlicing = doSlicing;
    }

    //Creates one 3D texture with all colour information
    Texture3D CreateTexture3D(Texture2D [] imageStack)
    {
        int imageWidth = imageStack[0].width;
        int imageHeight = imageStack[0].height;
        Color[] colorArray = new Color[imageWidth * imageHeight * imageStack.Length];
        Texture3D texture = new Texture3D(imageStack[0].width, imageStack[1].height, imageStack.Length, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Trilinear;
        Texture2D slice;
        for (int i = 0; i <imageStack.Length; i++) {
            slice = imageStack[i];
            for (int x = 0; x < imageWidth; x++) {
                for (int y = 0; y < imageHeight; y++) { 
                    colorArray[x + (y * imageWidth) + (i * imageHeight * imageWidth)] = slice.GetPixel(x, y);
                }
            }
        }
        texture.SetPixels(colorArray);
        texture.Apply();
        return texture;
    }
}