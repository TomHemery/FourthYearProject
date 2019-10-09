using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeLoader : MonoBehaviour
{
    public Texture2D[] slices;
    Texture3D texture;
    private Material cubeMaterial;

    [SerializeField] public string materialTexture;

    private void Awake()
    {
        cubeMaterial = gameObject.GetComponent<Renderer>().material;
        Debug.Log("Found material on cube: " + cubeMaterial);
    }

    void Start()
    {
        texture = CreateTexture3D(slices);
        Debug.Log("Setting texture on material: " + materialTexture);
        cubeMaterial.SetTexture(materialTexture, texture);
    }

    Texture3D CreateTexture3D(Texture2D [] imageStack)
    {
        int imageWidth = imageStack[0].width;
        int imageHeight = imageStack[0].height;
        Color[] colorArray = new Color[imageWidth * imageHeight * imageStack.Length];
        texture = new Texture3D(imageStack[0].width, imageStack[1].height, imageStack.Length, TextureFormat.RGBA32, true);
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