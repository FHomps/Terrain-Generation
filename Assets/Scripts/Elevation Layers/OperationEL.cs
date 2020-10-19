using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperationEL : ElevationLayer
{
    public enum ModType {
        Add, Substract, Increment, Decrement, Complement, Multiply, Divide, Inverse, Absolute
    }

    public ModType type;

    public ElevationLayer layer1;
    public ElevationLayer layer2;

    public override bool PropagateDependencies() {
        if (!shouldRegenerate && (layer1 != null && layer1.shouldRegenerate) || (layer2 != null && layer2.shouldRegenerate)) {
            shouldRegenerate = true;
            return true;
        }
        return false;
    }

    public override void Generate(bool reallocate) {
        ProceduralTerrain t = gameObject.GetComponentInParent<ProceduralTerrain>();
        if (reallocate || values == null)
            values = new float[t.resolution, t.resolution];
        
        switch (type) {
        case ModType.Add:
            for (int i = 0; i < t.resolution; i++) { for (int j = 0; j < t.resolution; j++) { values[i, j] = layer1.values[i, j] + layer2.values[i, j]; } }
            break;
        case ModType.Substract:
            for (int i = 0; i < t.resolution; i++) { for (int j = 0; j < t.resolution; j++) { values[i, j] = layer1.values[i, j] - layer2.values[i, j]; } }
            break;
        case ModType.Multiply:
            for (int i = 0; i < t.resolution; i++) { for (int j = 0; j < t.resolution; j++) { values[i, j] = layer1.values[i, j] * layer2.values[i, j]; } }
            break;
        case ModType.Divide:
            for (int i = 0; i < t.resolution; i++) { for (int j = 0; j < t.resolution; j++) { values[i, j] = layer1.values[i, j] / layer2.values[i, j]; } }
            break;
        case ModType.Increment:
            for (int i = 0; i < t.resolution; i++) { for (int j = 0; j < t.resolution; j++) { values[i, j] = layer1.values[i, j] + 1; } }
            break;
        case ModType.Decrement:
            for (int i = 0; i < t.resolution; i++) { for (int j = 0; j < t.resolution; j++) { values[i, j] = layer1.values[i, j] - 1; } }
            break;
        case ModType.Complement:
            for (int i = 0; i < t.resolution; i++) { for (int j = 0; j < t.resolution; j++) { values[i, j] = 1 - layer1.values[i, j]; } }
            break;
        case ModType.Inverse:
            for (int i = 0; i < t.resolution; i++) { for (int j = 0; j < t.resolution; j++) { values[i, j] = 1 / layer1.values[i, j]; } }
            break;
        case ModType.Absolute:
            for (int i = 0; i < t.resolution; i++) { for (int j = 0; j < t.resolution; j++) { values[i, j] = Mathf.Abs(layer1.values[i, j]); } }
            break;
        }
    }
}
