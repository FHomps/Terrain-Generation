using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseIndicator : MonoBehaviour {
    public Shader unlitShader;

    GameObject mainSphere = null;
    GameObject diffSphere = null;

    private void Start() {
        mainSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        diffSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        mainSphere.transform.position = new Vector3(20, 200, 20);
        mainSphere.transform.localScale = Vector3.one * 20;
        mainSphere.GetComponent<MeshRenderer>().material.color = Color.red;
        diffSphere.transform.position = new Vector3(20, 200, 20);
        diffSphere.transform.localScale = Vector3.one * 20;
        diffSphere.GetComponent<MeshRenderer>().material.color = Color.blue;
    }

    void Update() {
        bool left = Input.GetMouseButtonDown(0);
        bool right = Input.GetMouseButtonDown(1);

        if (left || right) {
            if (left)
                mainSphere.SetActive(true);
            else
                diffSphere.SetActive(true);

            BaseTerrain t = GetComponentInParent<BaseTerrain>();

            Vector3 pos = Input.mousePosition;
            pos.z = Camera.main.transform.position.y;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(pos);
            pos = worldPos + new Vector3(t.size, 0, t.size) / 2;
            pos = pos / t.size * t.resolution;
            int i = Mathf.RoundToInt(pos.z);
            int j = Mathf.RoundToInt(pos.x);
            if (i < 0 || i >= t.resolution || j < 0 || j >= t.resolution) {
                if (left) {
                    mainSphere.SetActive(false);
                    mainSphere.transform.position -= new Vector3(0, mainSphere.transform.position.y, 0);
                }
                else {
                    diffSphere.SetActive(false);
                    diffSphere.transform.position -= new Vector3(0, diffSphere.transform.position.y, 0);
                }
                return;
            }
            float height = t.WIPVertices[i * t.resolution + j].y;
            worldPos.y = height;

            if (left)
                mainSphere.transform.position = worldPos;
            else
                diffSphere.transform.position = worldPos;

            string y1 = mainSphere.activeSelf ? mainSphere.transform.position.y.ToString("F1") : "?";
            string y2 = diffSphere.activeSelf ? diffSphere.transform.position.y.ToString("F1") : "?";
            string delta = (mainSphere.activeSelf && diffSphere.activeSelf) ? (mainSphere.transform.position.y - diffSphere.transform.position.y).ToString("F1") : "?";

            Debug.Log(y1 + '\t' + y2 + "\tΔ " + delta);
        }
    }
}
