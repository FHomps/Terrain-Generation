using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DAltitudeGT : GroundTruthLayer {

    public float altitudeThreshold = 1f;

    public override void Generate(bool reallocate) {
        ProceduralTerrain t = gameObject.GetComponentInParent<ProceduralTerrain>();
        if (reallocate || values == null)
            values = new Color[t.resolution, t.resolution];

        for (int i = 0; i < t.resolution; i++) {
            for (int j = 0; j < t.resolution; j++) {
                values[i, j] = t.WIPVertices[i * t.resolution + j].y > altitudeThreshold ? Color.red : Color.green;
            }
        }
    }
}