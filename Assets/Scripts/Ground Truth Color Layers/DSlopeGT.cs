using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DSlopeGT : GroundTruthLayer {

    public float slopeThreshold = 0.5f;

    public override void Generate(bool reallocate) {
        ProceduralTerrain t = gameObject.GetComponentInParent<ProceduralTerrain>();
        if (reallocate || values == null)
            values = new Color[t.resolution, t.resolution];

        for (int i = 1; i < t.resolution - 1; i++) {
            for (int j = 1; j < t.resolution - 1; j++) {
                float zC = t.WIPVertices[i * t.resolution + j].y;
                float zL = t.WIPVertices[i * t.resolution + j - 1].y;
                float zT = t.WIPVertices[(i - 1) * t.resolution + j].y;
                float zR = t.WIPVertices[i * t.resolution + j + 1].y;
                float zB = t.WIPVertices[(i + 1) * t.resolution + j].y;

                float slope = (Mathf.Sqrt(Tools.Square(zL - zC) + Tools.Square(zC - zT)) + Mathf.Sqrt(Tools.Square(zC - zR) + Tools.Square(zB - zC))) / 2 * t.resolution / t.size;
                values[i, j] = slope > slopeThreshold ? Color.red : Color.green;
            }

            //Border-filling 1
            values[i, 0] = values[i, 1];
            values[i, t.resolution - 1] = values[i, t.resolution - 2];
        }

        //Border-filling 2
        for (int j = 0; j < t.resolution; j++) {
            values[0, j] = values[1, j];
            values[t.resolution - 1, j] = values[t.resolution - 2, j];
        }
    }
}