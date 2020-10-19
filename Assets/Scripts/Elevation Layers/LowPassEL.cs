using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowPassEL : ElevationLayer {
    
    public float radius = 1f;

    public ElevationLayer baseLayer;

    public override bool PropagateDependencies() {
        if (!shouldRegenerate && baseLayer != null && baseLayer.shouldRegenerate) {
            shouldRegenerate = true;
            return true;
        }
        return false;
    }

    private float reflectGet(float[,] array, int i, int j) {
        if (i < 0)
            i = -i;
        else {
            int w = array.GetLength(0) - 1;
            if (i > w)
                i = w - (i - w);
        }
        if (j < 0)
            j = -j;
        else {
            int h = array.GetLength(1) - 1;
            if (j > h)
                j = h - (j - h);
        }
        return array[i, j];
    }

    public override void Generate(bool reallocate) {
        ProceduralTerrain t = gameObject.GetComponentInParent<ProceduralTerrain>();
        if (reallocate || values == null)
            values = new float[t.resolution, t.resolution];
        else {
            for (int i = 0; i < t.resolution; i++) { for (int j = 0; j < t.resolution; j++) { values[i, j] = 0; } }
        }

        if (baseLayer == null || baseLayer.values == null)
            return;

        if (radius <= 0) {
            for (int i = 0; i < t.resolution; i++) {
                for (int j = 0; j < t.resolution; j++) {
                    values[i, j] = baseLayer.values[i, j];
                }
            }
            return;
        }

        float squaredSigma = Tools.Square(radius / 3);
        float spacing = t.size / t.resolution;
        int drad = Mathf.CeilToInt(radius / t.size * t.resolution);
        if (drad > 10)
            drad = 10;
        int kres = 2 * drad + 1;
        float[,] kernel = new float[kres, kres];
        float weightSum = 0;
        for (int i = 0; i < kres; i++) {
            for (int j = 0; j < kres; j++) {
                kernel[i, j] = Tools.Gauss2D(spacing * (i - drad), spacing * (j - drad), squaredSigma);
                weightSum += kernel[i, j];
            }
        }
        float newWeightSum = 0;
        for (int i = 0; i < kres; i++) {
            for (int j = 0; j < kres; j++) {
                kernel[i, j] /= weightSum;
                newWeightSum += kernel[i, j];
            }
        }

        for (int i = 0; i < t.resolution; i++) {
            for (int j = 0; j < t.resolution; j++) {
                for (int x = 0; x < kres; x++) {
                    for (int y = 0; y < kres; y++) {
                        values[i, j] += kernel[x, y] * reflectGet(baseLayer.values, i - drad + x, j - drad + y);
                    }
                }
            }
        }

    }
}
