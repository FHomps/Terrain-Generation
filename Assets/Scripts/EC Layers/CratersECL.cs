using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CratersECL : ECLayer {
    public int seed = 0;
    public uint craters = 10;
    public bool poissonSampling = true;

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

    public float shapeNoiseScale = 0.01f;
    public float shapeNoiseStrength = 0.2f;

    public FastNoiseSIMDUnity colorNoise;
    public Vector2 colorValue = new Vector2(.8f, 0f);
    public Vector2 colorAlpha = new Vector2(.8f, .3f);
    public Vector2 colorSpread = new Vector2(120f, 0f);
    public Vector2 colorDecay = new Vector2(2f, 0f);
    public Vector2 colorNoiseStrength = new Vector2(.5f, 0f);

    public override bool PropagateDependencies() {
        if (!shouldRegenerate && colorNoise != null && colorNoise.modified) {
            shouldRegenerate = true;
            return true;
        }
        return false;
    }

    public override void Generate(bool reallocate) {
        base.Generate(reallocate);
        BaseTerrain t = gameObject.GetComponentInParent<BaseTerrain>();
        if (!reallocate) {
            for (int i = 0; i < t.resolution; i++) {
                for (int j = 0; j < t.resolution; j++) {
                    elevationValues[i, j] = 0;
                    colorValues[i, j] = Color.clear;
                }
            }
        }

        Random.InitState(seed);

        List<Vector2> craterPositions;
        if (poissonSampling) {
            craterPositions = FastPoissonDiskSampling.Sampling(Vector2.zero, Vector2.one * t.size, t.size / Mathf.Sqrt(craters));

        }
        else {
            craterPositions = new List<Vector2>((int)craters);
            for (int i = 0; i < craters; i++) {
                craterPositions.Add(new Vector2(Random.value, Random.value) * t.size);
            }
        }

        FastNoiseSIMD shapeNoise = new FastNoiseSIMD(seed);
        shapeNoise.SetNoiseType(FastNoiseSIMD.NoiseType.Perlin);
        float[] shapeNoiseSet = shapeNoise.GetNoiseSet(0, 0, 0, t.resolution, 1, t.resolution, t.size / t.resolution / shapeNoiseScale);

        float[] colorNoiseSet = colorNoise.fastNoiseSIMD.GetNoiseSet(0, 0, 0, t.resolution, 1, t.resolution, t.size / t.resolution);

        foreach (Vector2 craterPos in craterPositions) {
            float variation = Random.Range(-variationRange, variationRange);

            float r = Tools.Variate(radius, variation);
            float hd = Tools.Variate(holeDepth, variation);
            float hs = Tools.Variate(holeSteepness, variation);
            float rh = Tools.Variate(rimHeight, variation);
            float rs = Tools.Variate(rimSteepness, variation);
            float sf = Tools.Variate(smoothFactor, variation);

            float v = Tools.Variate(colorValue, variation);
            float a = Tools.Variate(colorAlpha, variation);
            float spr = Tools.Variate(colorSpread, variation);
            float dc = Tools.Variate(colorDecay, variation);
            float cnstr = Tools.Variate(colorNoiseStrength, variation);

            float fullRadius = r + Mathf.Sqrt(rh / rs);
            int minx = Math.Max(0, Mathf.FloorToInt((craterPos.x - 1.5f * fullRadius) / t.size * t.resolution));
            int miny = Math.Max(0, Mathf.FloorToInt((craterPos.y - 1.5f * fullRadius) / t.size * t.resolution));
            int maxx = Math.Min(t.resolution, Mathf.CeilToInt((craterPos.x + 1.5f * fullRadius) / t.size * t.resolution));
            int maxy = Math.Min(t.resolution, Mathf.CeilToInt((craterPos.y + 1.5f * fullRadius) / t.size * t.resolution));
            for (int x = minx; x < maxx; x++) {
                for (int y = miny; y < maxy; y++) {
                    float dist = (new Vector2(x, y) / t.resolution * t.size - craterPos).magnitude + shapeNoiseSet[x + t.resolution * y] * shapeNoiseStrength;

                    // Adapted from S. Lague - https://github.com/SebLague/Solar-System
                    float hole = (Tools.Square(dist / r) - 1) * (hd + rh) * hs + rh;
                    float rimX = Mathf.Min(dist - fullRadius, 0);
                    float rim = rs * Tools.Square(rimX);

                    float craterShape = Tools.SmoothMax(hole, -hd, sf);
                    craterShape = Tools.SmoothMin(craterShape, rim, sf);
                    elevationValues[x, y] += craterShape;

                    float distToRidge = Mathf.Abs(dist - r);
                    Color pixelColor = new Color(v, v, v, a);
                    pixelColor.a *= 1 - (colorNoiseSet[x + t.resolution * y] + 0.5f) * cnstr;
                    pixelColor = Color.Lerp(Color.clear, pixelColor, Mathf.Pow(Mathf.Max(0, 1 - distToRidge / spr), dc));
                    
                    colorValues[x, y] = Tools.OverlayColors(colorValues[x, y], pixelColor);
                }
            }
        }

    }
}
