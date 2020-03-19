using UnityEngine;
public class CreateRandomData
{
    public static Texture3D CreateRandomTexture(int dims) {
        Texture3D result = new Texture3D(dims, dims, dims, TextureFormat.RGBA32, false);
        Color32[] pixels = result.GetPixels32();

        for (int i = 0; i < pixels.Length; i++) {
            pixels[i] = new Color32(getRandomByte(), getRandomByte(), getRandomByte(), getRandomByte());
        }

        result.SetPixels32(pixels);
        result.Apply();
        return result;
    }

    private static byte getRandomByte() {
        return (byte)(Random.value * byte.MaxValue);
    }
}
