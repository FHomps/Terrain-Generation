using System;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class CratersEL : ElevationLayer
{
    public int seed = 0;
    public uint craters = 10;

    [Range(0, 1)]
    public float variationRange = 0f;
    //Crater attributes are 2-dimensional vectors:
    //X: Main scale: base attribute value
    //Y: Variation influence: attribute incremental multiplier at variation 1
    public Vector2 radius = new Vector2(100f, 0f);
    public Vector2 holeDepth = new Vector2(10f, 0f);
    public Vector2 holeSteepness = new Vector2(1f, 0f);
    public Vector2 rimHeight = new Vector2(4f, 0f);
    public Vector2 rimSteepness = new Vector2(1f, 0f);
    public Vector2 smoothFactor = new Vector2(.5f, 0f);

    public float noiseScale = 0.01f;
    public float noiseStrength = 0.2f;

    public override void Generate(bool reallocate) {
        ProceduralTerrain t = gameObject.GetComponentInParent<ProceduralTerrain>();
        if (reallocate || values == null)
            values = new float[t.resolution, t.resolution];
        else {
            for (int i = 0; i < t.resolution; i++) {
                for (int j = 0; j < t.resolution; j++) {
                    values[i, j] = 0;
                }
            }
        }        

        Random.InitState(seed);

        FastNoiseSIMD noise = new FastNoiseSIMD(seed);
        noise.SetNoiseType(FastNoiseSIMD.NoiseType.Perlin);
        float[] noiseSet = noise.GetNoiseSet(0, 0, 0, t.resolution, 1, t.resolution, t.size / t.resolution / noiseScale);

        for (int i = 0; i < craters; i++) {
            float variation = Random.Range(-variationRange, variationRange);

            float r = Tools.Variate(radius, variation);
            float hd = Tools.Variate(holeDepth, variation);
            float hs = Tools.Variate(holeSteepness, variation);
            float rh = Tools.Variate(rimHeight, variation);
            float rs = Tools.Variate(rimSteepness, variation);
            float sf = Tools.Variate(smoothFactor, variation);

            Vector2 craterPos = new Vector2(Random.value, Random.value) * t.size;
            float fullRadius = r + Mathf.Sqrt(rh / rs);
            int minx = Math.Max(0, Mathf.FloorToInt((craterPos.x - fullRadius) / t.size * t.resolution));
            int miny = Math.Max(0, Mathf.FloorToInt((craterPos.y - fullRadius) / t.size * t.resolution));
            int maxx = Math.Min(t.resolution, Mathf.CeilToInt((craterPos.x + fullRadius) / t.size * t.resolution));
            int maxy = Math.Min(t.resolution, Mathf.CeilToInt((craterPos.y + fullRadius) / t.size * t.resolution));
            for (int x = minx; x < maxx; x++) {
                for (int y = miny; y < maxy; y++) {
                    float dist = (new Vector2(x, y) / t.resolution * t.size - craterPos).magnitude + noiseSet[x + t.resolution * y] * noiseStrength;

                    // Adapted from S. Lague - https://github.com/SebLague/Solar-System
                    float hole = (Tools.Square(dist / r) - 1) * (hd + rh) * hs + rh;
                    float rimX = Mathf.Min(dist - fullRadius, 0);
                    float rim = rs * Tools.Square(rimX);

                    float craterShape = Tools.SmoothMax(hole, -hd, sf);
                    craterShape = Tools.SmoothMin(craterShape, rim, sf);
                    values[x, y] += craterShape;
                }
            }
        }

    }
}
