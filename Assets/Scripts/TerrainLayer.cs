using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TerrainLayer : MonoBehaviour
{
    public string identifier = "";
    public bool active = true;

    public bool shouldRegenerate = false;

    public abstract void Generate(bool reallocate = false);

    public virtual bool PropagateDependencies() { return false; } //Checks if dependencies need to be regenerated, if so ticks shouldRegenerate and returns true

    protected virtual void OnValidate() {
        shouldRegenerate = true;
    }
}
