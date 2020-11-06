using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using UnityEngine;

[DisallowMultipleComponent]
public class BaseTerrain : MonoBehaviour
{
    public Shader unlitShader;
    public Shader litShader;

    private int old_resolution;
    public int resolution = 1000;
    private float old_size;
    public float size = 100;

    public bool groundTruthMode = false;
    private bool old_groundTruthMode;

    bool needsFullMeshUpdate = false; //Set to true by changing mesh resolution or size, and also at the start of rendering

    //Reference to the vertices array while it is being updated, so that intermediary operations may be applied
    //Ex.: ground truth based on slope or elevation at a certain point in the computation of vertices
    public Vector3[] WIPVertices = null;

    Vector3[] InitializeVertices() {
        Vector3[] vertices = new Vector3[Tools.Square(resolution)];
        float xstart = -size / 2;
        float zstart = -size / 2;
        float spacing = size / (resolution - 1);

        for (int i = 0; i < vertices.Length; i++) {
            int col = i % resolution;
            int row = i / resolution;
            vertices[i] = new Vector3(xstart + col * spacing, 0, zstart + row * spacing);
        }

        return vertices;
    }

    int[] InitializeTriangles() {
        int spaces = resolution - 1;
        int[] triangles = new int[6 * Tools.Square(spaces)];

        for (int i = 0; i < Tools.Square(spaces); i++) {
            int col = i % spaces;
            int row = i / spaces;
            int index = i * 6;
            triangles[index] = row * resolution + col;
            triangles[index + 1] = triangles[index] + resolution;
            triangles[index + 2] = triangles[index] + 1;
            triangles[index + 3] = triangles[index + 2];
            triangles[index + 4] = triangles[index + 1];
            triangles[index + 5] = triangles[index + 4] + 1;
        }

        return triangles;
    }

    void Start() {
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        meshFilter.mesh = mesh;

        needsFullMeshUpdate = true;

        old_resolution = resolution;
        old_size = size;
        old_groundTruthMode = groundTruthMode;

        gameObject.GetComponent<MeshRenderer>().material.shader = groundTruthMode ? unlitShader : litShader;
    }

    void Update() {
        bool shouldUpdateVertices = false;
        bool shouldUpdateColors = (groundTruthMode == false && old_groundTruthMode == true);
        bool shouldUpdateGroundTruth = (groundTruthMode == true && old_groundTruthMode == false);
        old_groundTruthMode = groundTruthMode;

        if (shouldUpdateColors) {
            gameObject.GetComponent<MeshRenderer>().material.shader = litShader;
        }
        else if (shouldUpdateGroundTruth) {
            gameObject.GetComponent<MeshRenderer>().material.shader = unlitShader;
        }

        bool possibleUnpropagatedRegen;
        do {
            possibleUnpropagatedRegen = false;
            foreach (TerrainLayer layer in gameObject.GetComponentsInChildren<TerrainLayer>()) {
                if (layer.PropagateDependencies())
                    possibleUnpropagatedRegen = true;
            }
        } while (possibleUnpropagatedRegen);

        foreach (TerrainLayer layer in gameObject.GetComponentsInChildren<TerrainLayer>()) {
            if (layer is GroundTruthLayer && shouldUpdateVertices) { // All ground truth layers to load after a regenerating EL should also be regenerated
                layer.shouldRegenerate = true;
                if (groundTruthMode)
                    shouldUpdateGroundTruth = true;
            }
            else if (layer.shouldRegenerate) {
                if (layer is ElevationLayer || layer is ECLayer)
                    shouldUpdateVertices = true;
                if (layer is ColorLayer || layer is ECLayer)
                    shouldUpdateColors = true;
                if (layer is GroundTruthLayer)
                    shouldUpdateGroundTruth = true;
            }
        }

        foreach (FastNoiseSIMDUnity noise in gameObject.GetComponentsInChildren<FastNoiseSIMDUnity>()) {
            if (noise.modified)
                noise.modified = false;
        }

        if (shouldUpdateVertices || shouldUpdateColors || shouldUpdateGroundTruth || needsFullMeshUpdate) {
            MeshFilter filter = gameObject.GetComponent<MeshFilter>();
            Mesh mesh = filter.mesh;

            if (needsFullMeshUpdate) {
                mesh.Clear();

                WIPVertices = InitializeVertices();
                Color[] colors = new Color[Tools.Square(resolution)];

                foreach (TerrainLayer layer in gameObject.GetComponentsInChildren<TerrainLayer>()) {
                    layer.Generate(true);
                    layer.shouldRegenerate = false;
                    if (!layer.active)
                        continue;
                    if (layer is ElevationLayer eLayer) {
                        if (eLayer.mask != null) {
                            for (int i = 0; i < resolution; i++) {
                                for (int j = 0; j < resolution; j++) {
                                    WIPVertices[i * resolution + j].y += eLayer.mask.values[i, j] * eLayer.values[i, j];
                                }
                            }
                        }
                        else {
                            for (int i = 0; i < resolution; i++) {
                                for (int j = 0; j < resolution; j++) {
                                    WIPVertices[i * resolution + j].y += eLayer.values[i, j];
                                }
                            }
                        }
                    }
                    else if (groundTruthMode == false && layer is ColorLayer cLayer) {
                        if (cLayer.mask != null) {
                            for (int i = 0; i < resolution; i++) {
                                for (int j = 0; j < resolution; j++) {
                                    colors[i * resolution + j] = Tools.OverlayColors(colors[i * resolution + j], cLayer.values[i, j], cLayer.mask.values[i, j]);
                                }
                            }
                        }
                        else {
                            for (int i = 0; i < resolution; i++) {
                                for (int j = 0; j < resolution; j++) {
                                    colors[i * resolution + j] = Tools.OverlayColors(colors[i * resolution + j], cLayer.values[i, j]);
                                }
                            }
                        }
                    }
                    else if (groundTruthMode == false && layer is ECLayer ecLayer) {
                        if (ecLayer.mask != null) {
                            for (int i = 0; i < resolution; i++) {
                                for (int j = 0; j < resolution; j++) {
                                    WIPVertices[i * resolution + j].y += ecLayer.mask.values[i, j] * ecLayer.elevationValues[i, j];
                                    colors[i * resolution + j] = Tools.OverlayColors(colors[i * resolution + j], ecLayer.colorValues[i, j], ecLayer.mask.values[i, j]);
                                }
                            }
                        }
                        else {
                            for (int i = 0; i < resolution; i++) {
                                for (int j = 0; j < resolution; j++) {
                                    WIPVertices[i * resolution + j].y += ecLayer.elevationValues[i, j];
                                    colors[i * resolution + j] = Tools.OverlayColors(colors[i * resolution + j], ecLayer.colorValues[i, j]);
                                }
                            }
                        }
                    }
                    else if (groundTruthMode == true && layer is GroundTruthLayer gtLayer) {
                        for(int i = 0; i < resolution; i++) {
                            for (int j = 0; j < resolution; j++) {
                                colors[i * resolution + j] = Tools.OverlayColors(colors[i * resolution + j], gtLayer.values[i, j]);
                            }
                        }
                    }
                }

                WIPVertices[0].y = 150;
                WIPVertices[resolution - 1].y = 300;

                mesh.vertices = WIPVertices;
                mesh.triangles = InitializeTriangles();
                mesh.RecalculateNormals();
                mesh.colors = colors;
                mesh.RecalculateTangents();

                needsFullMeshUpdate = false;
            }

            else {
                WIPVertices = InitializeVertices();
                Color[] colors = new Color[Tools.Square(resolution)];

                foreach (TerrainLayer layer in gameObject.GetComponentsInChildren<TerrainLayer>()) {
                    if (layer.shouldRegenerate) {
                        layer.Generate(false);
                        layer.shouldRegenerate = false;
                    }
                    if (!layer.active)
                        continue;
                    if ((shouldUpdateVertices || shouldUpdateGroundTruth) && layer is ElevationLayer eLayer) {
                        if (eLayer.mask != null) {
                            for (int i = 0; i < resolution; i++) {
                                for (int j = 0; j < resolution; j++) {
                                    WIPVertices[i * resolution + j].y += eLayer.mask.values[i, j] * eLayer.values[i, j];
                                }
                            }
                        }
                        else {
                            for (int i = 0; i < resolution; i++) {
                                for (int j = 0; j < resolution; j++) {
                                    WIPVertices[i * resolution + j].y += eLayer.values[i, j];
                                }
                            }
                        }
                    }
                    else if (groundTruthMode == false && shouldUpdateColors && layer is ColorLayer cLayer) {
                        if (cLayer.mask != null) {
                            for (int i = 0; i < resolution; i++) {
                                for (int j = 0; j < resolution; j++) {
                                    colors[i * resolution + j] = Tools.OverlayColors(colors[i * resolution + j], cLayer.values[i, j], cLayer.mask.values[i, j]);
                                }
                            }
                        }
                        else {
                            for (int i = 0; i < resolution; i++) {
                                for (int j = 0; j < resolution; j++) {
                                    colors[i * resolution + j] = Tools.OverlayColors(colors[i * resolution + j], cLayer.values[i, j]);
                                }
                            }
                        }
                    }
                    else if (layer is ECLayer ecLayer) {
                        if (shouldUpdateVertices || shouldUpdateGroundTruth) {
                            if (ecLayer.mask != null) {
                                for (int i = 0; i < resolution; i++) {
                                    for (int j = 0; j < resolution; j++) {
                                        WIPVertices[i * resolution + j].y += ecLayer.mask.values[i, j] * ecLayer.elevationValues[i, j];
                                    }
                                }
                            }
                            else {
                                for (int i = 0; i < resolution; i++) {
                                    for (int j = 0; j < resolution; j++) {
                                        WIPVertices[i * resolution + j].y += ecLayer.elevationValues[i, j];
                                    }
                                }
                            }
                        }
                        if (groundTruthMode == false && shouldUpdateColors) {
                            if (ecLayer.mask != null) {
                                for (int i = 0; i < resolution; i++) {
                                    for (int j = 0; j < resolution; j++) {
                                        colors[i * resolution + j] = Tools.OverlayColors(colors[i * resolution + j], ecLayer.colorValues[i, j], ecLayer.mask.values[i, j]);
                                    }
                                }
                            }
                            else {
                                for (int i = 0; i < resolution; i++) {
                                    for (int j = 0; j < resolution; j++) {
                                        colors[i * resolution + j] = Tools.OverlayColors(colors[i * resolution + j], ecLayer.colorValues[i, j]);
                                    }
                                }
                            }
                        }
                    }
                    else if (groundTruthMode == true && shouldUpdateGroundTruth && layer is GroundTruthLayer gtLayer) {
                        for (int i = 0; i < resolution; i++) {
                            for (int j = 0; j < resolution; j++) {
                                colors[i * resolution + j] = Tools.OverlayColors(colors[i * resolution + j], gtLayer.values[i, j]);
                            }
                        }
                    }
                }

                if (shouldUpdateVertices) {
                    mesh.vertices = WIPVertices;
                    mesh.RecalculateNormals();
                }
                if (shouldUpdateColors || shouldUpdateGroundTruth) {
                    mesh.colors = colors;
                }
            }

            filter.mesh = mesh;
        }
    }

    void OnValidate() {
        if (resolution != old_resolution || size != old_size)
            needsFullMeshUpdate = true;

        if (resolution > 2048) {
            Debug.LogWarning("Dangerously high resolution, reverting to 2048");
            resolution = 2048;
        }

        old_resolution = resolution;
        old_size = size;
    }
}
