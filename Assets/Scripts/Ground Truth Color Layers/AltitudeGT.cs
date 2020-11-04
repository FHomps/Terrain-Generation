using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltitudeGT : GroundTruthLayer {

    public float minAltitude = 0f;
    public float maxAltitude = 1f;

    public override void Generate(bool reallocate) {
        BaseTerrain t = gameObject.GetComponentInParent<BaseTerrain>();
        if (reallocate || values == null)
            values = new Color[t.resolution, t.resolution];

        for (int i = 0; i < t.resolution; i++) {
            for (int j = 0; j < t.resolution; j++) {
                float k = (t.WIPVertices[i * t.resolution + j].y - minAltitude) / (maxAltitude - minAltitude);
                values[i, j] = Color.Lerp(Color.green, Color.red, k);
            }
        }
    }
}