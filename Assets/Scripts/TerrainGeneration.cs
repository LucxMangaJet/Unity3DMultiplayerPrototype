using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public struct NoiseLayer
{
    public float Scale;
    public float Intensity;
}


public class TerrainGeneration : MonoBehaviour
{
    [SerializeField] int seed;
    [SerializeField] TerrainData data;

    [SerializeField] NoiseLayer[] noiseLayers;

    [SerializeField] float safeZoneHeight = 0.3f;
    [SerializeField] AnimationCurve safezoneTransitionCurve;

    [NaughtyAttributes.Button]
    public void RegenerateTerrain()
    {

        var random = new System.Random(seed);
        int res = data.heightmapResolution;
        int halfRes = res / 2;
        float[,] heightmap = new float[res, res];


        Vector2 center = new Vector2(halfRes, halfRes);
        float offsetX = (float)random.NextDouble();
        float offsetY = (float)random.NextDouble();

        float totalnoiselayersIntensity = noiseLayers.Sum((x) => x.Intensity);


        for (int x = 0; x < res; x++)
        {
            for (int y = 0; y < res; y++)
            {
                float xPercent = (float)x / res;
                float yPercent = (float)y / res;

                float distanceFromCenter = Vector2.Distance(new Vector2(0.5f, 0.5f), new Vector2(xPercent, yPercent));
                float noise = 0;

                for (int i = 0; i < noiseLayers.Length; i++)
                {
                    var layer = noiseLayers[i];
                    noise += Mathf.PerlinNoise(offsetX + xPercent * layer.Scale, offsetY + yPercent * layer.Scale) * (layer.Intensity / totalnoiselayersIntensity);
                }

                float height = Mathf.Lerp(safeZoneHeight, noise, safezoneTransitionCurve.Evaluate(distanceFromCenter));

                heightmap[x, y] = height;
            }
        }

        data.SetHeights(0, 0, heightmap);


    }





}
