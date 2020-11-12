using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Settings : MonoBehaviour {
    void Awake() {
        Application.targetFrameRate = 20;
    }
}
