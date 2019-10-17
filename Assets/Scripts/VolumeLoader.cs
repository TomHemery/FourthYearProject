using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeLoader : MonoBehaviour
{
    public GameObject rendererPrefab;
    public Texture2D[] slices;
    private Texture3D[] textures;
    private Material[] materials;
    private GameObject[] renderCubes;

    [SerializeField] public string MaterialTextureName;
    public int RendererSpacing = 2;
    public bool SplitRGB = true;

    private float Density = 1;
    private int SamplingQuality = 64;

    public static VolumeLoader Instance { private set; get; } = null;

    private const string DENSITY_TAG = "_Density";
    private const string SAMPLE_QUALITY_TAG = "_SamplingQuality";

    private void Awake()
    {
        if (Instance == null) Instance = this;

        if (SplitRGB)
            textures = CreateRGBTexture3D(slices);
        else
            textures = CreateTexture3D(slices);

        materials = new Material[textures.Length];
        renderCubes = new GameObject[textures.Length];
        Debug.Log("Spawning prefabs");
        for (int i = 0; i < textures.Length; i++)
        {
            GameObject renderCube = Instantiate(rendererPrefab, new Vector3(i * RendererSpacing, 0, 0), Quaternion.identity);
            renderCubes[i] = renderCube;
            Material cubeMaterial = renderCube.GetComponent<Renderer>().material;
            cubeMaterial.SetTexture(MaterialTextureName, textures[i]);
            cubeMaterial.SetFloat(DENSITY_TAG, Density);
            cubeMaterial.SetInt(SAMPLE_QUALITY_TAG, SamplingQuality);
            materials[i] = cubeMaterial;
        }
    }

    void Start()
    {
        
    }

    public void SetDensity(float d)
    {
        Density = d;
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].SetFloat(DENSITY_TAG, d);
        }

    }

    public void SetQuality(int q)
    {
        SamplingQuality = q;
        for (int i = 0; i < textures.Length; i++)
        {
            materials[i].SetFloat(SAMPLE_QUALITY_TAG, q);
        }
    }

    public void SetXScale(float scale) {
        Transform t;
        
        for (int i = 0; i < renderCubes.Length; i++) {
            t = renderCubes[i].transform;
            t.localScale = new Vector3(scale, t.localScale.y, t.localScale.z);
        }
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