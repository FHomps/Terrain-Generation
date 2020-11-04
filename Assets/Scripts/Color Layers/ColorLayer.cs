using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorLayer : TerrainLayer {

    public Color[,] values;
    public ElevationLayer mask;

    public virtual bool IsGroundTruth() { return false; }

    public override void Generate(bool reallocate) {
        BaseTerrain t = gameObject.GetComponentInParent<BaseTerrain>();
        if (reallocate || values == null)
            values = new Color[t.resolution, t.resolution];
    }
}
