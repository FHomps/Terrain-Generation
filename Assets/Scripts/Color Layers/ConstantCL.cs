using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantCL : ColorLayer {

    public Color color = Color.gray;

    public override void Generate(bool reallocate) {
        BaseTerrain t = gameObject.GetComponentInParent<BaseTerrain>();
        if (reallocate || values == null)
            values = new Color[t.resolution, t.resolution];

        for (int i = 0; i < t.resolution; i++) {
            for (int j = 0; j < t.resolution; j++) {
                values[i, j] = color;
            }
        }
    }
}