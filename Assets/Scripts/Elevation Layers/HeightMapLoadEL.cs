using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Boo.Lang.Runtime.DynamicDispatching;
using System.Security.Cryptography;

public class HeightMapLoadEL : ElevationLayer {

    public string filename;
    public int lines = 11613;
    public int lineSamples = 6392;
    public int startOffset = 25568;

    public Vector2 centerPosition = Vector2.zero;
    private float old_pixelSize;
    public float pixelSize = 1f;

    private void Start() {
        old_pixelSize = pixelSize;
    }

    public override void Generate(bool reallocate) {
        ProceduralTerrain t = gameObject.GetComponentInParent<ProceduralTerrain>();
        if (reallocate || values == null)
            values = new float[t.resolution, t.resolution];

        BinaryReader br = null;
        
        try {
            br = new BinaryReader(File.Open(filename, FileMode.Open, FileAccess.Read));
        }
        catch (Exception e) {
            Debug.LogError("Couldn't load file " + filename + "\n" + e.Message);
            return;
        }

        Debug.Log("Successfully loaded file " + filename);

        float spacing = t.size / (t.resolution - 1);

        //Where i and j end up when translated to map coordinates, as floating coordinates (will be used in interpolation)
        
        int[] iCoords_floor = new int[t.resolution];
        int[] iCoords_ceil = new int[t.resolution];
        float[] iCoords_coeff = new float[t.resolution];
        int[] jCoords_floor = new int[t.resolution];
        int[] jCoords_ceil = new int[t.resolution];
        float[] jCoords_coeff = new float[t.resolution];

        for (int i = 0; i < t.resolution; i++) {
            float iCoord = (float)lines / 2 - (centerPosition.y + t.size / 2 - i * spacing) / pixelSize;
            float jCoord = (float)lineSamples / 2 - (centerPosition.x + t.size / 2 - i * spacing) / pixelSize;
            iCoords_floor[i] = Mathf.FloorToInt(iCoord);
            iCoords_ceil[i] = Mathf.CeilToInt(iCoord);
            iCoords_coeff[i] = iCoord - iCoords_floor[i];
            jCoords_floor[i] = Mathf.FloorToInt(jCoord);
            jCoords_ceil[i] = Mathf.CeilToInt(jCoord);
            jCoords_coeff[i] = jCoord - jCoords_floor[i];
        }

        float min = float.MaxValue;
        float max = float.MinValue;

        for (int i = 0; i < t.resolution; i++) {
            for (int j = 0; j < t.resolution; j++) {
                values[i, j] = float.MinValue;
            }
        }

        for (int i = 0; i < t.resolution; i++) {
            if (iCoords_floor[i] < 0 || iCoords_ceil[i] >= lines)
                continue;
            br.BaseStream.Seek(startOffset + iCoords_floor[i] * lineSamples * 4, SeekOrigin.Begin);
            byte[] upperLine = br.ReadBytes(lineSamples * 4);
            byte[] lowerLine = br.ReadBytes(lineSamples * 4);
            for (int j = 0; j < t.resolution; j++) {
                if (jCoords_floor[j] < 0 || jCoords_ceil[j] >= lineSamples)
                    continue;
                float topLeft = BitConverter.ToSingle(upperLine, jCoords_floor[j] * 4);
                float topRight = BitConverter.ToSingle(upperLine, jCoords_ceil[j] * 4);
                float botLeft = BitConverter.ToSingle(lowerLine, jCoords_floor[j] * 4);
                float botRight = BitConverter.ToSingle(lowerLine, jCoords_ceil[j] * 4);
                float x = iCoords_coeff[i];
                float y = jCoords_coeff[j];
                float f = botLeft * (1 - x) * (1 - y) + botRight * x * (1 - y) + topLeft * (1 - x) * y + topRight * x * y;
                if (Mathf.Abs(f) < 10000) {
                    values[t.resolution - i - 1, j] = f;
                    min = Mathf.Min(min, f);
                    max = Mathf.Max(max, f);
                }
            }
        }

        for (int i = 0; i < t.resolution; i++) {
            for (int j = 0; j < t.resolution; j++) {
                if (values[i, j] != float.MinValue) {
                    values[i, j] -= (min + max) / 2;
                }
                else {
                    values[i, j] = 0;
                }
            }
        }

        br.Close();
    }

    protected override void OnValidate() {
        base.OnValidate();

        if (old_pixelSize == 0)
            old_pixelSize = pixelSize;
        if (old_pixelSize != pixelSize && pixelSize != 0) {
            Debug.Log(pixelSize.ToString() + ' ' + old_pixelSize.ToString() + ' ' + centerPosition.ToString());
            centerPosition = centerPosition * pixelSize / old_pixelSize;
            old_pixelSize = pixelSize;
        }
    }
}
