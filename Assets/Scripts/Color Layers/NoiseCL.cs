using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseCL : ColorLayer {

    public FastNoiseSIMDUnity noise = null;

    public Color lowColor = Color.black;
    public Color highColor = Color.white;

    public override bool PropagateDependencies() {
        if (!shouldRegenerate && noise != null && noise.modified) {
            shouldRegenerate = true;
            return true;
        }
        return false;
    }

    public override void Generate(bool reallocate) {
        BaseTerrain t = gameObject.GetComponentInParent<BaseTerrain>();
        if (reallocate || values == null)
            values = new Color[t.resolution, t.resolution];

        if (noise == null) {
            return;
        }

        float[] set = noise.fastNoiseSIMD.GetNoiseSet(0, 0, 0, t.resolution, 1, t.resolution, t.size / t.resolution);

        for (int i = 0; i < t.resolution; i++) {
            for (int j = 0; j < t.resolution; j++) {
                values[i, j] = Color.Lerp(lowColor, highColor, set[i + j * t.resolution] + 0.5f);
            }
        }
    }
}