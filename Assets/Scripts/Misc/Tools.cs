﻿using System;
using UnityEngine;

public static class Tools { 
    public static int Square(int i) {
        return i * i;
    }

    public static float Square(float f) {
        return f * f;
    }

    public static int Cube(int i) {
        return i * i * i;
    }

    public static float Cube(float f) {
        return f * f * f;
    }

    public static int IPow(int i, uint pow) {
        int result = 1;
        while (pow > 0) {
            result *= i;
            pow--;
        }
        return result;
    }

    public static float IPow(float f, uint pow) {
        float result = 1f;
        while (pow > 0) {
            result *= f;
            pow--;
        }
        return result;
    }

    // From https://www.iquilezles.org/www/articles/smin/smin.htm
    public static float SmoothMin(float a, float b, float k) {
        k = Mathf.Max(0, k);
        float h = Mathf.Max(0, Mathf.Min(1, (b - a + k) / (2 * k)));
        return a * h + b * (1 - h) - k * h * (1 - h);
    }

    public static float SmoothMax(float a, float b, float k) {
        k = Mathf.Min(0, -k);
        float h = Mathf.Max(0, Mathf.Min(1, (b - a + k) / (2 * k)));
        return a * h + b * (1 - h) - k * h * (1 - h);
    }

    public static float RandRange(Vector2 rangedFloat) {
        return UnityEngine.Random.Range(rangedFloat.x - rangedFloat.y / 2, rangedFloat.x + rangedFloat.y / 2);
    }

    public static float Gauss2D(float x, float y, float sigmaSquared) {
        return 1 / (2 * Mathf.PI * sigmaSquared) * Mathf.Exp(-(Square(x) + Square(y)) / (2 * sigmaSquared));
    }

    public static Color OverlayColors(Color bottom, Color top) {
        Color result = new Color();
        result.a = top.a + (1 - top.a) * bottom.a;
        result.r = top.a * top.r + (1 - top.a) * bottom.a * bottom.r;
        result.g = top.a * top.g + (1 - top.a) * bottom.a * bottom.g;
        result.b = top.a * top.b + (1 - top.a) * bottom.a * bottom.b;
        return result;
    }

    public static Color OverlayColors(Color bottom, Color top, float topMask) {
        Color result = new Color();
        top.a = top.a * topMask;
        result.a = top.a + (1 - top.a) * bottom.a;
        result.r = top.a * top.r + (1 - top.a) * bottom.a * bottom.r;
        result.g = top.a * top.g + (1 - top.a) * bottom.a * bottom.g;
        result.b = top.a * top.b + (1 - top.a) * bottom.a * bottom.b;
        return result;
    }
}