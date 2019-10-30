﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class VolumeManager : MonoBehaviour
{
    public static VolumeManager Instance { private set; get; } = null;

    public Texture3D Volume { get; private set; }
    private Texture3D VolumeCopy;
    private Material cubeMaterial;
    private GameObject renderCube;
    private Texture2D[] slices;

    [SerializeField] public string MaterialTextureName;
    public GameObject rendererPrefab;
    public string sourceFolderName;

    private float Density = 1;
    private int SamplingQuality = 64;

    private const string DENSITY_TAG = "_Density";
    private const string SAMPLE_QUALITY_TAG = "_SamplingQuality";
    private const string RED_TAG = "_Red";
    private const string GREEN_TAG = "_Green";
    private const string BLUE_TAG = "_Blue";
    private const string X_CROP_TAG = "_XCrop";

    private void Awake()
    {
        Debug.Log("Volume manager on awake");
        if (Instance == null) Instance = this;

        slices = Resources.LoadAll("Volumetric Data/" + sourceFolderName, typeof(Texture2D)).Cast<Texture2D>().ToArray();

        Volume = CreateTexture3D(slices);
        VolumeCopy = new Texture3D(Volume.width, Volume.height, Volume.depth, TextureFormat.RGBA32, false);
        Graphics.CopyTexture(Volume, VolumeCopy);

        renderCube = Instantiate(rendererPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        cubeMaterial = renderCube.GetComponent<Renderer>().material;
        cubeMaterial.SetTexture(MaterialTextureName, Volume);

        cubeMaterial.SetFloat(DENSITY_TAG, Density);
        cubeMaterial.SetInt(SAMPLE_QUALITY_TAG, SamplingQuality);

        cubeMaterial.SetInt(RED_TAG, 1);
        cubeMaterial.SetInt(BLUE_TAG, 1);
        cubeMaterial.SetInt(GREEN_TAG, 1);
        cubeMaterial.SetInt(X_CROP_TAG, Volume.width);
    }

    public void SetDensity(float d)
    {
        Density = d;
        cubeMaterial.SetFloat(DENSITY_TAG, d);
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

    public void SetXScale(float scale) {
        Transform t;
        t = renderCube.transform;
        t.localScale = new Vector3(scale, t.localScale.y, t.localScale.z);
    }

    public void SetXCrop(float crop) {
        cubeMaterial.SetFloat(X_CROP_TAG, crop);
    }

    //Creates one 3D texture with all colour information
    Texture3D CreateTexture3D(Texture2D [] imageStack)
    {
        int imageWidth = imageStack[0].width;
        int imageHeight = imageStack[0].height;
        Color[] colorArray = new Color[imageWidth * imageHeight * imageStack.Length];
        Texture3D texture = new Texture3D(imageStack[0].width, imageStack[1].height, imageStack.Length, TextureFormat.RGBA32, false);
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