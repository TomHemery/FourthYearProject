using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class VolumeLoader : MonoBehaviour
{
    
    private Texture3D[] textures;
    private Material cubeMaterial;
    private GameObject renderCube;
    private Texture2D[] slices;

    [SerializeField] public string MaterialTextureName;
    public int RendererSpacing = 2;
    public bool SplitRGB = true;
    public GameObject rendererPrefab;
    public string sourceFolderName;

    private float Density = 1;
    private int SamplingQuality = 64;

    public static VolumeLoader Instance { private set; get; } = null;

    private const string DENSITY_TAG = "_Density";
    private const string SAMPLE_QUALITY_TAG = "_SamplingQuality";
    private const string RED_TAG = "_Red";
    private const string GREEN_TAG = "_Green";
    private const string BLUE_TAG = "_Blue";

    private void Awake()
    {
        if (Instance == null) Instance = this;

        slices = Resources.LoadAll("Volumetric Data/" + sourceFolderName, typeof(Texture2D)).Cast<Texture2D>().ToArray();

        textures = CreateTexture3D(slices);

        renderCube = Instantiate(rendererPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        cubeMaterial = renderCube.GetComponent<Renderer>().material;
        cubeMaterial.SetTexture(MaterialTextureName, textures[0]);
        cubeMaterial.SetFloat(DENSITY_TAG, Density);
        cubeMaterial.SetInt(SAMPLE_QUALITY_TAG, SamplingQuality);

        cubeMaterial.SetInt(RED_TAG, 1);
        cubeMaterial.SetInt(BLUE_TAG, 1);
        cubeMaterial.SetInt(GREEN_TAG, 1);
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

    //Creates one 3D texture with all colour information
    Texture3D [] CreateTexture3D(Texture2D [] imageStack)
    {
        int imageWidth = imageStack[0].width;
        int imageHeight = imageStack[0].height;
        Color[] colorArray = new Color[imageWidth * imageHeight * imageStack.Length];
        Texture3D texture = new Texture3D(imageStack[0].width, imageStack[1].height, imageStack.Length, TextureFormat.RGBA32, true);
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
        return new Texture3D[] { texture };
    }

    //creates one 3D texture per channel (R, G, B);
    Texture3D[] CreateRGBTexture3D(Texture2D [] imageStack) {
        Texture3D[] result = new Texture3D[3];
        for(int i = 0; i < result.Length; i++){
            int imageWidth = imageStack[0].width;
            int imageHeight = imageStack[0].height;
            Color[] colorArray = new Color[imageWidth * imageHeight * imageStack.Length];
            result[i] = new Texture3D(imageStack[0].width, imageStack[1].height, imageStack.Length, TextureFormat.RGBA32, true);
            Texture2D slice;
            for (int z = 0; z < imageStack.Length; z++)
            {
                slice = imageStack[z];
                for (int x = 0; x < imageWidth; x++)
                {
                    for (int y = 0; y < imageHeight; y++)
                    {
                        Color c = slice.GetPixel(x, y);
                        switch (i) {
                            case 0:
                                c.g = 0;
                                c.b = 0;
                                break;
                            case 1:
                                c.r = 0;
                                c.b = 0;
                                break;
                            case 2:
                                c.r = 0;
                                c.g = 0;
                                break;
                        }
                        colorArray[x + (y * imageWidth) + (z * imageHeight * imageWidth)] = c;
                    }
                }
            }
            result[i].SetPixels(colorArray);
            result[i].Apply();
        }
        return result;
    }
}