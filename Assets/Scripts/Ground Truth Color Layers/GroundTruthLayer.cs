using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTruthLayer : TerrainLayer
{

    public Color[,] values;

    public override void Generate(bool reallocate = false) {
        ProceduralTerrain t = gameObject.GetComponentInParent<ProceduralTerrain>();
        if (reallocate || values == null)
            values = new Color[t.resolution, t.resolution];
    }
}
