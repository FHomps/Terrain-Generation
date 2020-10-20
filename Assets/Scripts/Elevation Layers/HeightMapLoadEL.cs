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
    public float pixelSize = 1f;

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

        //Where i and j end up when translated to map coordinates
        int[] iCoords = new int[t.resolution];
        int[] jCoords = new int[t.resolution];

        for (int i = 0; i < t.resolution; i++) {
            iCoords[i] = Mathf.RoundToInt(lines / 2 - (centerPosition.y + t.size / 2 - i * spacing) / pixelSize);
            jCoords[i] = Mathf.RoundToInt(lineSamples / 2 - (centerPosition.x + t.size / 2 - i * spacing) / pixelSize);
        }

        float min = float.MaxValue;
        float max = float.MinValue;

        for (int i = 0; i < t.resolution; i++) {
            for (int j = 0; j < t.resolution; j++) {
                values[i, j] = float.MinValue;
            }
        }

        for (int i = 0; i < t.resolution; i++) {
            if (iCoords[i] < 0 || iCoords[i] >= lines)
                continue;
            br.BaseStream.Seek(startOffset + iCoords[i] * lineSamples * 4, SeekOrigin.Begin);
            byte[] line = br.ReadBytes(lineSamples * 4);
            for (int j = 0; j < t.resolution; j++) {
                if (jCoords[j] < 0 || jCoords[j] >= lineSamples)
                    continue;
                float f = BitConverter.ToSingle(line, jCoords[j] * 4);
                if (Mathf.Abs(f) < 10000) {
                    values[i, j] = f;
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
}
