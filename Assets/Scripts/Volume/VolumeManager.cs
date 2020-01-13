﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

public class VolumeManager : MonoBehaviour
{
    public static VolumeManager Instance { private set; get; } = null;

    public Texture3D Volume { get; private set; }
    public List<GameObject> VolumeCubes { get; private set; } = null;
    private Texture2D[] slices;

    [SerializeField] public string MaterialTextureName;
    public GameObject rendererPrefab;
    public string defaultFolderName;
    public ViewResetter viewResetter;

    private float Density = 1;
    private int SamplingQuality = 64;

    public const string DENSITY_TAG = "_Density";
    public const string SAMPLE_QUALITY_TAG = "_SamplingQuality";
    public const string RED_TAG = "_Red";
    public const string GREEN_TAG = "_Green";
    public const string BLUE_TAG = "_Blue";
    public const string PLANE_POSITION_TAG = "_PlanePos";
    public const string PLANE_NORMAL_TAG = "_PlaneNormal";
    public const string REVERSE_PLANE_TAG = "_ReversePlaneSlicing";

    public const string VOLUMETRIC_DATA_PATH = "Volumetric Data/";
    public const string CACHE_PATH = "Assets/Resources/VolumeCache/";
    public const string CACHE_PATH_SHORT = "VolumeCache/";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        if(defaultFolderName != "")LoadVolume(defaultFolderName);
    }

    public void LoadVolume(string name) {
        if (VolumeCubes != null) foreach (GameObject g in VolumeCubes) Destroy(g);
        VolumeCubes = new List<GameObject>();
        //look for a cached instance of volume
        Volume = Resources.Load<Texture3D>(CACHE_PATH_SHORT + name);
        //if none exists then create it
        if (Volume == null)
        {
            slices = Resources.LoadAll(VOLUMETRIC_DATA_PATH + name, typeof(Texture2D)).Cast<Texture2D>().ToArray();
            Volume = CreateTexture3D(slices);
            AssetDatabase.CreateAsset(Volume, CACHE_PATH + name + ".asset");
        }

        VolumeCubes.Add(Instantiate(rendererPrefab, new Vector3(0, 0, 0), Quaternion.identity));
        Material cubeMaterial;
        cubeMaterial = VolumeCubes[0].GetComponent<Renderer>().material;
        cubeMaterial.SetTexture(MaterialTextureName, Volume);

        cubeMaterial.SetFloat(DENSITY_TAG, Density);
        cubeMaterial.SetInt(SAMPLE_QUALITY_TAG, SamplingQuality);

        cubeMaterial.SetInt(RED_TAG, 1);
        cubeMaterial.SetInt(BLUE_TAG, 1);
        cubeMaterial.SetInt(GREEN_TAG, 1);

        cubeMaterial.SetInt(REVERSE_PLANE_TAG, 1);

        CuttingPlane cuttingPlane = VolumeCubes[0].GetComponentInChildren<CuttingPlane>();
        cuttingPlane.transform.Translate(0, 0, 0.5f);
        cuttingPlane.ApplyToMaterial();

        viewResetter.CacheCuttingPlanes();

        //could uncomment this line if memory usage is an issue, but it's much much faster if you don't 
        //Resources.UnloadUnusedAssets();
    }

    public void SplitVolume() {
        if (Volume != null)
        {
            foreach (GameObject g in VolumeCubes) {
                Destroy(g);
            }
            VolumeCubes.Clear();

            GameObject left = Instantiate(rendererPrefab, new Vector3(0, 0, 0.2f), Quaternion.identity);
            GameObject right = Instantiate(rendererPrefab, new Vector3(0, 0, -0.2f), Quaternion.identity);
            VolumeCubes.Add(left);
            VolumeCubes.Add(right);
            Material cubeMaterial;
            for (int i = 0; i < VolumeCubes.Count(); i++) {
                cubeMaterial = VolumeCubes[i].GetComponent<Renderer>().material;
                cubeMaterial.SetTexture(MaterialTextureName, Volume);

                cubeMaterial.SetFloat(DENSITY_TAG, Density);
                cubeMaterial.SetInt(SAMPLE_QUALITY_TAG, SamplingQuality);

                cubeMaterial.SetInt(RED_TAG, 1);
                cubeMaterial.SetInt(BLUE_TAG, 1);
                cubeMaterial.SetInt(GREEN_TAG, 1);

                cubeMaterial.SetInt(REVERSE_PLANE_TAG, i == 0 ? 0 : 1);
                cubeMaterial.SetVector(PLANE_POSITION_TAG, VolumeCubes[i].GetComponentInChildren<CuttingPlane>().GetPlanePosition());
                cubeMaterial.SetVector(PLANE_NORMAL_TAG, VolumeCubes[i].GetComponentInChildren<CuttingPlane>().GetPlaneNormal());
            }
            viewResetter.CacheCuttingPlanes();
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.C))
        {
            SplitVolume();
        }
    }

    public void SetDensity(float d)
    {
        Density = d;
        foreach (GameObject g in VolumeCubes)
        {
            g.GetComponent<Renderer>().material.SetFloat(DENSITY_TAG, d);
        }
        
    }

    public void SetQuality(int q)
    {
        SamplingQuality = q;
        foreach (GameObject g in VolumeCubes)
        {
            g.GetComponent<Renderer>().material.SetFloat(SAMPLE_QUALITY_TAG, q);
        }
    }

    public void SetRenderRed(bool r) 
    {
        foreach (GameObject g in VolumeCubes)
        {
            g.GetComponent<Renderer>().material.SetInt(RED_TAG, r ? 1 : 0);
        }
    }
    public void SetRenderGreen(bool g)
    {
        foreach (GameObject ga in VolumeCubes)
        {
            ga.GetComponent<Renderer>().material.SetInt(GREEN_TAG, g ? 1 : 0);
        }
    }

    public void SetRenderBlue(bool b)
    {
        foreach (GameObject g in VolumeCubes)
        {
            g.GetComponent<Renderer>().material.SetInt(BLUE_TAG, b ? 1 : 0);
        }
    }

    public void SetXScale(float scale)
    {
        Transform t;
        foreach (GameObject g in VolumeCubes)
        {
            t = g.transform;
            t.localScale = new Vector3(scale, t.localScale.y, t.localScale.z);
        }
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