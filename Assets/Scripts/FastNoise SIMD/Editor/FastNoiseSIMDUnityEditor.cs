using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FastNoiseSIMDUnity))]
public class FastNoiseSIMDUnityEditor : Editor
{
	private bool Refresh<T>(ref T param, T newValue) {
		if (!param.Equals(newValue)) {
			param = newValue;
			return true;
        }
		return false;
    }

	public override void OnInspectorGUI()
	{
		FastNoiseSIMDUnity fastNoiseSIMDUnity = ((FastNoiseSIMDUnity)target);
		FastNoiseSIMD fastNoiseSIMD = fastNoiseSIMDUnity.fastNoiseSIMD;

		bool changed = false;

		changed |= Refresh(ref fastNoiseSIMDUnity.noiseName, EditorGUILayout.TextField("Name", fastNoiseSIMDUnity.noiseName));

		changed |= Refresh(ref fastNoiseSIMDUnity.generalSettingsFold, EditorGUILayout.Foldout(fastNoiseSIMDUnity.generalSettingsFold, "General Settings"));

		if (fastNoiseSIMDUnity.generalSettingsFold)
		{
			changed |= Refresh(ref fastNoiseSIMDUnity.noiseType, (FastNoiseSIMD.NoiseType)EditorGUILayout.EnumPopup("Noise Type", fastNoiseSIMDUnity.noiseType));
			changed |= Refresh(ref fastNoiseSIMDUnity.seed, EditorGUILayout.IntField("Seed", fastNoiseSIMDUnity.seed));
			changed |= Refresh(ref fastNoiseSIMDUnity.scale, EditorGUILayout.FloatField("Global Scale", fastNoiseSIMDUnity.scale));
			fastNoiseSIMD.SetNoiseType(fastNoiseSIMDUnity.noiseType);
			fastNoiseSIMD.SetSeed(fastNoiseSIMDUnity.seed);
			ProceduralTerrain t = fastNoiseSIMDUnity.GetComponentInParent<ProceduralTerrain>();
			fastNoiseSIMD.SetFrequency(1.0f / fastNoiseSIMDUnity.scale);

			Vector3 axisScales = EditorGUILayout.Vector3Field("Axis Scales", fastNoiseSIMDUnity.axisScales);
			if (axisScales.x == 0) axisScales.x = 1.0f;
			if (axisScales.y == 0) axisScales.y = 1.0f;
			if (axisScales.z == 0) axisScales.z = 1.0f;
			changed |= Refresh(ref fastNoiseSIMDUnity.axisScales, axisScales);
			fastNoiseSIMD.SetAxisScales(fastNoiseSIMDUnity.axisScales.x, fastNoiseSIMDUnity.axisScales.y,
				fastNoiseSIMDUnity.axisScales.z);
		}

		changed |= Refresh(ref fastNoiseSIMDUnity.fractalSettingsFold, EditorGUILayout.Foldout(fastNoiseSIMDUnity.fractalSettingsFold, "Fractal Settings"));

		if (fastNoiseSIMDUnity.fractalSettingsFold)
		{
			changed |= Refresh(ref fastNoiseSIMDUnity.fractalType, (FastNoiseSIMD.FractalType)EditorGUILayout.EnumPopup("Fractal Type", fastNoiseSIMDUnity.fractalType));
			changed |= Refresh(ref fastNoiseSIMDUnity.octaves, EditorGUILayout.IntSlider("Octaves", fastNoiseSIMDUnity.octaves, 2, 9));
			changed |= Refresh(ref fastNoiseSIMDUnity.lacunarity, EditorGUILayout.FloatField("Lacunarity", fastNoiseSIMDUnity.lacunarity));
			changed |= Refresh(ref fastNoiseSIMDUnity.gain, EditorGUILayout.FloatField("Gain", fastNoiseSIMDUnity.gain));
			fastNoiseSIMD.SetFractalType(fastNoiseSIMDUnity.fractalType);
			fastNoiseSIMD.SetFractalOctaves(fastNoiseSIMDUnity.octaves);
			fastNoiseSIMD.SetFractalLacunarity(fastNoiseSIMDUnity.lacunarity);
			fastNoiseSIMD.SetFractalGain(fastNoiseSIMDUnity.gain);
		}

		changed |= Refresh(ref fastNoiseSIMDUnity.cellularSettingsFold, EditorGUILayout.Foldout(fastNoiseSIMDUnity.cellularSettingsFold, "Cellular Settings"));

		if (fastNoiseSIMDUnity.cellularSettingsFold)
		{
			changed |= Refresh(ref fastNoiseSIMDUnity.cellularReturnType, (FastNoiseSIMD.CellularReturnType)EditorGUILayout.EnumPopup("Return Type", fastNoiseSIMDUnity.cellularReturnType));
			fastNoiseSIMD.SetCellularReturnType(fastNoiseSIMDUnity.cellularReturnType);

			if (fastNoiseSIMDUnity.cellularReturnType == FastNoiseSIMD.CellularReturnType.NoiseLookup)
			{
				changed |= Refresh(ref fastNoiseSIMDUnity.cellularNoiseLookupType, (FastNoiseSIMD.NoiseType)EditorGUILayout.EnumPopup("Noise Lookup Type", fastNoiseSIMDUnity.cellularNoiseLookupType));
				changed |= Refresh(ref fastNoiseSIMDUnity.cellularNoiseLookupFrequency, EditorGUILayout.FloatField("Noise Lookup Frequency", fastNoiseSIMDUnity.cellularNoiseLookupFrequency));
				fastNoiseSIMD.SetCellularNoiseLookupType(fastNoiseSIMDUnity.cellularNoiseLookupType);
				fastNoiseSIMD.SetCellularNoiseLookupFrequency(fastNoiseSIMDUnity.cellularNoiseLookupFrequency);
			}
			changed |= Refresh(ref fastNoiseSIMDUnity.cellularDistanceFunction, (FastNoiseSIMD.CellularDistanceFunction)EditorGUILayout.EnumPopup("Distance Function", fastNoiseSIMDUnity.cellularDistanceFunction));
			fastNoiseSIMD.SetCellularDistanceFunction(fastNoiseSIMDUnity.cellularDistanceFunction);

			changed |= Refresh(ref fastNoiseSIMDUnity.cellularDistanceIndex0, EditorGUILayout.IntSlider("Distance2 Index 0", Mathf.Min(fastNoiseSIMDUnity.cellularDistanceIndex0, fastNoiseSIMDUnity.cellularDistanceIndex1 - 1), 0, 2));
			changed |= Refresh(ref fastNoiseSIMDUnity.cellularDistanceIndex1, EditorGUILayout.IntSlider("Distance2 Index 1", fastNoiseSIMDUnity.cellularDistanceIndex1, 1, 3));
			fastNoiseSIMD.SetCellularDistance2Indicies(fastNoiseSIMDUnity.cellularDistanceIndex0, fastNoiseSIMDUnity.cellularDistanceIndex1);

			changed |= Refresh(ref fastNoiseSIMDUnity.cellularJitter, EditorGUILayout.Slider("Cell Jitter", fastNoiseSIMDUnity.cellularJitter, 0f, 1f));
			fastNoiseSIMD.SetCellularJitter(fastNoiseSIMDUnity.cellularJitter);
		}

		changed |= Refresh(ref fastNoiseSIMDUnity.perturbSettingsFold, EditorGUILayout.Foldout(fastNoiseSIMDUnity.perturbSettingsFold, "Perturb Settings"));

		if (fastNoiseSIMDUnity.perturbSettingsFold)
		{
			changed |= Refresh(ref fastNoiseSIMDUnity.perturbType, (FastNoiseSIMD.PerturbType)EditorGUILayout.EnumPopup("Perturb Type", fastNoiseSIMDUnity.perturbType));
			changed |= Refresh(ref fastNoiseSIMDUnity.perturbAmp, EditorGUILayout.FloatField("Amp", fastNoiseSIMDUnity.perturbAmp));
			changed |= Refresh(ref fastNoiseSIMDUnity.perturbFrequency, EditorGUILayout.FloatField("Frequency", fastNoiseSIMDUnity.perturbFrequency));
			fastNoiseSIMD.SetPerturbType(fastNoiseSIMDUnity.perturbType);
			fastNoiseSIMD.SetPerturbAmp(fastNoiseSIMDUnity.perturbAmp);
			fastNoiseSIMD.SetPerturbFrequency(fastNoiseSIMDUnity.perturbFrequency);

			if (fastNoiseSIMDUnity.perturbType == FastNoiseSIMD.PerturbType.GradientFractal ||
				fastNoiseSIMDUnity.perturbType == FastNoiseSIMD.PerturbType.GradientFractal_Normalise)
			{
				changed |= Refresh(ref fastNoiseSIMDUnity.perturbOctaves, EditorGUILayout.IntSlider("Fractal Octaves", fastNoiseSIMDUnity.perturbOctaves, 2, 9));
				changed |= Refresh(ref fastNoiseSIMDUnity.perturbLacunarity, EditorGUILayout.FloatField("Fractal Lacunarity", fastNoiseSIMDUnity.perturbLacunarity));
				changed |= Refresh(ref fastNoiseSIMDUnity.perturbGain, EditorGUILayout.FloatField("Fractal Gain", fastNoiseSIMDUnity.perturbGain));
				fastNoiseSIMD.SetPerturbFractalOctaves(fastNoiseSIMDUnity.perturbOctaves);
				fastNoiseSIMD.SetPerturbFractalLacunarity(fastNoiseSIMDUnity.perturbLacunarity);
				fastNoiseSIMD.SetPerturbFractalGain(fastNoiseSIMDUnity.perturbGain);
			}
			if (fastNoiseSIMDUnity.perturbType == FastNoiseSIMD.PerturbType.Normalise ||
				fastNoiseSIMDUnity.perturbType == FastNoiseSIMD.PerturbType.Gradient_Normalise ||
				fastNoiseSIMDUnity.perturbType == FastNoiseSIMD.PerturbType.GradientFractal_Normalise)
			{
				changed |= Refresh(ref fastNoiseSIMDUnity.perturbNormaliseLength, EditorGUILayout.FloatField("Normalise Length", fastNoiseSIMDUnity.perturbNormaliseLength));
				fastNoiseSIMD.SetPerturbNormaliseLength(fastNoiseSIMDUnity.perturbNormaliseLength);
			}
		}

		if (GUILayout.Button("Reset"))
		{
			fastNoiseSIMD.SetSeed(fastNoiseSIMDUnity.seed = 1337);
			fastNoiseSIMDUnity.scale = 100f;
			fastNoiseSIMD.SetFrequency(1.0f / fastNoiseSIMDUnity.scale);
			fastNoiseSIMD.SetNoiseType(fastNoiseSIMDUnity.noiseType = FastNoiseSIMD.NoiseType.Simplex);

			fastNoiseSIMD.SetFractalOctaves(fastNoiseSIMDUnity.octaves = 3);
			fastNoiseSIMD.SetFractalLacunarity(fastNoiseSIMDUnity.lacunarity = 2.0f);
			fastNoiseSIMD.SetFractalGain(fastNoiseSIMDUnity.gain = 0.5f);
			fastNoiseSIMD.SetFractalType(fastNoiseSIMDUnity.fractalType = FastNoiseSIMD.FractalType.FBM);

			fastNoiseSIMD.SetCellularDistanceFunction(
				fastNoiseSIMDUnity.cellularDistanceFunction = FastNoiseSIMD.CellularDistanceFunction.Euclidean);
			fastNoiseSIMD.SetCellularReturnType(
				fastNoiseSIMDUnity.cellularReturnType = FastNoiseSIMD.CellularReturnType.CellValue);

			fastNoiseSIMD.SetCellularNoiseLookupType(fastNoiseSIMDUnity.cellularNoiseLookupType = FastNoiseSIMD.NoiseType.Simplex);
			fastNoiseSIMD.SetCellularNoiseLookupFrequency(fastNoiseSIMDUnity.cellularNoiseLookupFrequency = 0.2f);

			fastNoiseSIMD.SetCellularDistance2Indicies(fastNoiseSIMDUnity.cellularDistanceIndex0 = 0, fastNoiseSIMDUnity.cellularDistanceIndex1 = 1);
			fastNoiseSIMD.SetCellularJitter(fastNoiseSIMDUnity.cellularJitter = 0.45f);

			fastNoiseSIMD.SetPerturbType(fastNoiseSIMDUnity.perturbType = FastNoiseSIMD.PerturbType.None);
			fastNoiseSIMD.SetPerturbAmp(fastNoiseSIMDUnity.perturbAmp = 1.0f);
			fastNoiseSIMD.SetPerturbFrequency(fastNoiseSIMDUnity.perturbFrequency = 0.5f);

			fastNoiseSIMD.SetPerturbFractalOctaves(fastNoiseSIMDUnity.perturbOctaves = 3);
			fastNoiseSIMD.SetPerturbFractalLacunarity(fastNoiseSIMDUnity.perturbLacunarity = 2.0f);
			fastNoiseSIMD.SetPerturbFractalGain(fastNoiseSIMDUnity.perturbGain = 0.5f);

			changed = true;
		}

		fastNoiseSIMDUnity.modified |= changed;
	}

	public override bool HasPreviewGUI()
	{
		return true;
	}

	public override GUIContent GetPreviewTitle()
	{
		return new GUIContent("FastNoiseSIMD Unity - " + ((FastNoiseSIMDUnity)target).noiseName);
	}

	public override void DrawPreview(Rect previewArea)
	{
		FastNoiseSIMDUnity fastNoiseSIMDUnity = ((FastNoiseSIMDUnity)target);
		FastNoiseSIMD fastNoiseSIMD = fastNoiseSIMDUnity.fastNoiseSIMD;

		Texture2D tex = new Texture2D((int)previewArea.width, (int)previewArea.height);
		Color32[] pixels = new Color32[tex.width * tex.height];

		Vector3[] vectors = new Vector3[tex.width * tex.height];

		int index = 0;
		for (int y = 0; y < tex.height; y++)
		{
			for (int x = 0; x < tex.width; x++)
			{
				vectors[index++] = new Vector3(x, y);
			}
		}

		float[] noiseSet = new float[vectors.Length];
		fastNoiseSIMD.FillNoiseSetVector(noiseSet, new FastNoiseSIMD.VectorSet(vectors));

		float min = Single.MaxValue;
		float max = Single.MinValue;
		for (int i = 0; i < noiseSet.Length; i++)
		{
			min = Mathf.Min(min, noiseSet[i]);
			max = Mathf.Max(max, noiseSet[i]);
		}

		float scale = 255f / (max - min);

		for (int i = 0; i < noiseSet.Length; i++)
		{
			byte noise = (byte)Mathf.Clamp((noiseSet[i] - min) * scale, 0f, 255f);
			pixels[i] = new Color32(noise, noise, noise, 255);
		}

		tex.SetPixels32(pixels);
		tex.Apply();
		GUI.DrawTexture(previewArea, tex, ScaleMode.StretchToFill, false);
	}
}