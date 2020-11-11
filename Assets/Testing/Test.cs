using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {
    public Color bottom;
    public Color top;
    public Color overlay;

    private void OnValidate() {
        overlay = Tools.OverlayColors(bottom, top);
    }
}