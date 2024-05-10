using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public int width = 256; // Width of the noise map
    public int height = 256; // Height of the noise map
    public float scale = 20f; // Scale of the noise map
    public float offsetX = 0f; // Offset for X-axis
    public float offsetY = 0f; // Offset for Y-axis

    [SerializeField] private GameObject cloud;

    private Renderer renderer;

    public void GenerateMap()
    {
        renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = GenerateNoiseMap();
    }

    Texture2D GenerateNoiseMap()
    {
        Texture2D texture = new Texture2D(width, height);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float xCoord = (float)x / width * scale + offsetX;
                float yCoord = (float)y / height * scale + offsetY;

                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                Color color = new Color(sample, sample, sample);
                texture.SetPixel(x, y, color);

                if (sample > 0.7f)
                    _ = Instantiate(cloud, new Vector3(x, transform.position.y, y), Quaternion.identity);
            }
        }

        texture.Apply();
        return texture;
    }
}
