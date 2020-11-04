using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ECLayer : TerrainLayer {

    public Color[,] colorValues = null;
    public float[,] elevationValues = null;
    public ElevationLayer mask;

    public override void Generate(bool reallocate) {
        BaseTerrain t = gameObject.GetComponentInParent<BaseTerrain>();
        if (reallocate || colorValues == null || elevationValues == null) {
            colorValues = new Color[t.resolution, t.resolution];
            elevationValues = new float[t.resolution, t.resolution];
        }
    }
}
