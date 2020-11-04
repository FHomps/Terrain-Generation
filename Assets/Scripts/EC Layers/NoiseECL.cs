using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseECL : ECLayer {
    public FastNoiseSIMDUnity noise = null;

    public Color lowColor = Color.black;
    public Color highColor = Color.white;

    public float elevationStrength = 1.0f;

    public override bool PropagateDependencies() {
        if (!shouldRegenerate && noise != null && noise.modified) {
            shouldRegenerate = true;
            return true;
        }
        return false;
    }

    public override void Generate(bool reallocate) {
        base.Generate(reallocate);
        BaseTerrain t = gameObject.GetComponentInParent<BaseTerrain>();

        if (noise == null) {
            return;
        }

        float[] set = noise.fastNoiseSIMD.GetNoiseSet(0, 0, 0, t.resolution, 1, t.resolution, t.size / t.resolution);

        for (int i = 0; i < t.resolution; i++) {
            for (int j = 0; j < t.resolution; j++) {
                colorValues[i, j] = Color.Lerp(lowColor, highColor, set[i + j * t.resolution] + 0.5f);
                elevationValues[i, j] = elevationStrength * set[i + j * t.resolution];
            }
        }
    }
}
