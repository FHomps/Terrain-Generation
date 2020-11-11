using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CratersECL : ECLayer {
    public int seed = 0;
    public uint craters = 10;
    public bool poissonSampling = true;

    public bool enableVariation = true;
    public float variationShapeFactor = 0f;
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

    public FastNoiseSIMDUnity ridgeColorNoise;
    public Vector2 ridgeColorValue = new Vector2(.8f, 0f);
    public Vector2 ridgeColorAlpha = new Vector2(.8f, .3f);
    public Vector2 ridgeColorSpread = new Vector2(120f, 0f);
    public Vector2 ridgeColorDecay = new Vector2(2f, 0f);
    public Vector2 ridgeColorNoiseStrength = new Vector2(.5f, 0f);

    public FastNoiseSIMDUnity holeColorNoise;
    public Vector2 holeColorValue = new Vector2(.8f, 0f);
    public Vector2 holeColorAlpha = new Vector2(.8f, .3f);
    public Vector2 holeColorNoiseStrength = new Vector2(.5f, 0f);

    public override bool PropagateDependencies() {
        if (!shouldRegenerate && ridgeColorNoise != null && ridgeColorNoise.modified) {
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

        float[] colorNoiseSet = ridgeColorNoise.fastNoiseSIMD.GetNoiseSet(0, 0, 0, t.resolution, 1, t.resolution, t.size / t.resolution);

        foreach (Vector2 craterPos in craterPositions) {
            float variation = 0; 
            if (enableVariation) {
                variation = Mathf.Pow(Random.Range(0f, 1f), Mathf.Exp(-variationShapeFactor / 5)) * 2 - 1;
            }

            float r = Tools.Variate(radius, variation);
            float hd = Tools.Variate(holeDepth, variation);
            float hs = Tools.Variate(holeSteepness, variation);
            float rh = Tools.Variate(rimHeight, variation);
            float rs = Tools.Variate(rimSteepness, variation);
            float sf = Tools.Variate(smoothFactor, variation);

            float v = Tools.Variate(ridgeColorValue, variation);
            float a = Tools.Variate(ridgeColorAlpha, variation);
            float spr = Tools.Variate(ridgeColorSpread, variation);
            float dc = Tools.Variate(ridgeColorDecay, variation);
            float cnstr = Tools.Variate(ridgeColorNoiseStrength, variation);

            float elevationRadius = r + Mathf.Sqrt(rh / rs);
            float colorRadius = r + spr;
            float affectedRadius = Mathf.Max(elevationRadius, colorRadius);
            int minx = Math.Max(0, Mathf.FloorToInt((craterPos.x - affectedRadius) / t.size * t.resolution));
            int miny = Math.Max(0, Mathf.FloorToInt((craterPos.y - affectedRadius) / t.size * t.resolution));
            int maxx = Math.Min(t.resolution, Mathf.CeilToInt((craterPos.x + affectedRadius) / t.size * t.resolution));
            int maxy = Math.Min(t.resolution, Mathf.CeilToInt((craterPos.y + affectedRadius) / t.size * t.resolution));

            for (int x = minx; x < maxx; x++) {
                for (int y = miny; y < maxy; y++) {
                    float dist = (new Vector2(x, y) / t.resolution * t.size - craterPos).magnitude + shapeNoiseSet[x + t.resolution * y] * shapeNoiseStrength;

                    // Adapted from S. Lague - https://github.com/SebLague/Solar-System
                    float hole = (Tools.Square(dist / r) - 1) * (hd + rh) * hs + rh;
                    float rimX = Mathf.Min(dist - elevationRadius, 0);
                    float rim = rs * Tools.Square(rimX);

                    float craterShape = Tools.SmoothMax(hole, -hd, sf);
                    craterShape = Tools.SmoothMin(craterShape, rim, sf);
                    elevationValues[x, y] += craterShape;

                    float distToRidge = Mathf.Abs(dist - r);
                    Color pixelColor = new Color(v, v, v, a);
                    pixelColor.a += colorNoiseSet[x + t.resolution * y] * cnstr;
                    pixelColor.a *= Mathf.Pow(Mathf.Max(0, 1 - distToRidge / spr), dc);

                    colorValues[x, y] = Tools.OverlayColors(colorValues[x, y], pixelColor);

                }
            }
        }

    }
}
