using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevationLayer : TerrainLayer {

    public float[,] values;
    public ElevationLayer mask;

    public override void Generate(bool reallocate) {
        ProceduralTerrain t = gameObject.GetComponentInParent<ProceduralTerrain>();
        if (reallocate || values == null)
            values = new float[t.resolution, t.resolution];
    }
}
