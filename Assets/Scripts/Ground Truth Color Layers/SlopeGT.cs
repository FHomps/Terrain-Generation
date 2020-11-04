using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlopeGT : GroundTruthLayer {

    public float minSlope = 0f;
    public float maxSlope = 1f;

    public override void Generate(bool reallocate) {
        BaseTerrain t = gameObject.GetComponentInParent<BaseTerrain>();
        if (reallocate || values == null)
            values = new Color[t.resolution, t.resolution];

        /* 
         * The slope of a triangle is calculated as sqrt(a² + b²) / c, where a, b and c are obtained from the triangle's plane's equation ax+by+cz=d (with z being the vertical axis)
         * Since d does not intervene in the slope calculation, it can be set arbitrarily so that c is always 1, simplifying calculations.
         * For each vertex, its "slope" is calculated from the average of the slopes of two triangles. Those triangles are shaped from the center vertex C as well as
         * its four directly neighbouring vertices T (top), R (right), L (left), and B (bottom). This way, all triangles are used once and only once in the global computation.
         * Obtaining the planar equation is greatly simplified thanks to the relative x and y coordinates of the vertices being known in advance.
         * This results in the slope calculation only needing the z (height) coordinate of each vertex.
         */
        for (int i = 1; i < t.resolution - 1; i++) {
            for (int j = 1; j < t.resolution - 1; j++) {
                float zC = t.WIPVertices[i * t.resolution + j].y;
                float zL = t.WIPVertices[i * t.resolution + j - 1].y;
                float zT = t.WIPVertices[(i - 1) * t.resolution + j].y;
                float zR = t.WIPVertices[i * t.resolution + j + 1].y;
                float zB = t.WIPVertices[(i + 1) * t.resolution + j].y;

                float k = ((Mathf.Sqrt(Tools.Square(zL - zC) + Tools.Square(zC - zT)) + Mathf.Sqrt(Tools.Square(zC - zR) + Tools.Square(zB - zC))) / 2 * t.resolution / t.size - minSlope) / (maxSlope - minSlope);
                values[i, j] = Color.Lerp(Color.green, Color.red, k);
            }

            //Border-filling 1
            values[i, 0] = values[i, 1];
            values[i, t.resolution - 1] = values[i, t.resolution - 2];
        }

        //Border-filling 2
        for (int j = 0; j < t.resolution; j++) {
            values[0, j] = values[1, j];
            values[t.resolution - 1, j] = values[t.resolution - 2, j];
        }
    }
}