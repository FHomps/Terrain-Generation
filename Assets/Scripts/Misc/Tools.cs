using System;
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

    public static float Variate(Vector2 variableFloat, float variation) {
        return variableFloat.x * (1 + variation * variableFloat.y);
    }

    public static float Gauss2D(float x, float y, float sigmaSquared) {
        return 1 / (2 * Mathf.PI * sigmaSquared) * Mathf.Exp(-(Square(x) + Square(y)) / (2 * sigmaSquared));
    }

    public static Color OverlayColors(Color bottom, Color top) {
        Color result = new Color();
        result.a = top.a + (1 - top.a) * bottom.a;
        result.r = top.a * top.r + (1 - top.a) * bottom.r;
        result.g = top.a * top.g + (1 - top.a) * bottom.g;
        result.b = top.a * top.b + (1 - top.a) * bottom.b;
        return result;
    }

    public static Color OverlayColors(Color bottom, Color top, float topMask) {
        Color result = new Color();
        float maskedTopA = top.a * topMask;
        result.a = maskedTopA + (1 - maskedTopA) * bottom.a;
        result.r = maskedTopA * top.r + (1 - maskedTopA) * bottom.r;
        result.g = maskedTopA * top.g + (1 - maskedTopA) * bottom.g;
        result.b = maskedTopA * top.b + (1 - maskedTopA) * bottom.b;
        return result;
    }

    private static float startTime = 0;
    private static float time = 0;
    private static float subTime = 0;
    private static string clockString = "";

    public static void StartClock() {
        clockString = "Started Clock\n";
        time = Time.realtimeSinceStartup;
        startTime = time;
        subTime = time;
    }

    public static void AddClockStop(string message) {
        float delta = Time.realtimeSinceStartup - time;
        clockString += delta.ToString("F4") + " - " + message + '\n';

        time = Time.realtimeSinceStartup;
        subTime = time;
    }

    public static void AddSubClockStop(string message) {
        float delta = Time.realtimeSinceStartup - subTime;
        clockString += "    " + delta.ToString("F4") + " - " + message + '\n';

        subTime = Time.realtimeSinceStartup;
    }

    public static void EndClock(string message = "End") {
        AddClockStop(message);
        float totalDelta = Time.realtimeSinceStartup - startTime;
        clockString += totalDelta.ToString("F4") + " - " + "Total";
        Debug.Log(clockString);
    }
}