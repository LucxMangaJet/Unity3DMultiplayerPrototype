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
    [SerializeField] float steepnessTransitionFactor;
    [SerializeField] AnimationCurve grassTypeTransition;
    float[,] heightmap;

    [NaughtyAttributes.Button]
    public void RegenerateTerrain()
    {
        if (data == null) return;

        var random = new System.Random(seed);
        heightmap = GenerateHeights(random);
        GenerateSplatmap(random);

        data.SetHeights(0, 0, heightmap);
    }

    private void GenerateSplatmap(System.Random random)
    {
        int res = data.alphamapResolution;
        float[,,] splatmap = new float[res, res, 3];


        for (int x = 0; x < res; x++)
        {
            for (int y = 0; y < res; y++)
            {
                float xPercent = (float)x / res;
                float yPercent = (float)y / res;
                float steepness = Mathf.Clamp01((GetSteepnessAt(xPercent, yPercent) - 1.5f) * 0.5f);

                float grass = 1 - steepness;
                float h = grassTypeTransition.Evaluate(GetHeightAt(xPercent, yPercent));

                splatmap[x, y, 0] = grass * h;
                splatmap[x, y, 1] = steepness;
                splatmap[x, y, 2] = grass * (1 - h);
            }
        }

        data.SetAlphamaps(0, 0, splatmap);
    }

    private float GetHeightAt(float x, float y)
    {
        float res = data.heightmapResolution;
        int xCoord = Mathf.FloorToInt(x * res);
        int yCoord = Mathf.FloorToInt(y * res);
        return heightmap[xCoord, yCoord];
    }

    private float GetSteepnessAt(float x, float y)
    {
        if (x > 1 || y > 1 || x < 0 || y < 0) return 0;

        float res = data.heightmapResolution;

        int xCoord = Mathf.FloorToInt(x * res);
        int yCoord = Mathf.FloorToInt(y * res);

        float hp = heightmap[xCoord, yCoord];
        float hpr = heightmap[xCoord + 1, yCoord];
        float hpu = heightmap[xCoord, yCoord + 1];
        float hpru = heightmap[xCoord + 1, yCoord + 1];

        float angle = Mathf.Abs(Mathf.Asin((hpr + hpu + hpru) * 0.33333f - hp) * res);
        return angle;
    }

    private float[,] GenerateHeights(System.Random random)
    {
        int res = data.heightmapResolution;
        float[,] heightmap = new float[res, res];

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
        return heightmap;
    }
}
