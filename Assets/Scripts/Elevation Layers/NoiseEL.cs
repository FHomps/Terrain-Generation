using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseEL : ElevationLayer {

    public FastNoiseSIMDUnity noise = null;

    public float strength = 1.0f;

    public override bool PropagateDependencies() {
        if (!shouldRegenerate && noise != null && noise.modified) {
            shouldRegenerate = true;
            return true;
        }
        return false;
    }

    public override void Generate(bool reallocate) {
        ProceduralTerrain t = gameObject.GetComponentInParent<ProceduralTerrain>();
        if (reallocate || values == null)
            values = new float[t.resolution, t.resolution];

        if (noise == null) {
            return;
        }

        float[] set = noise.fastNoiseSIMD.GetNoiseSet(0, 0, 0, t.resolution, 1, t.resolution, t.size / t.resolution);

        for (int i = 0; i < t.resolution; i++) {
            for (int j = 0; j < t.resolution; j++) {
                values[i, j] = strength * set[i + j * t.resolution];
            }
        }
    }
}
