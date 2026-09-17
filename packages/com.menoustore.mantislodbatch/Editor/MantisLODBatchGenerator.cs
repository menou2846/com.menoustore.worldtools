using System.Collections.Generic;
using System.Linq;
using MantisLODEditor;
using UnityEditor;
using UnityEngine;
using MenouStore.License;

public class MantisLODBatchGenerator : EditorWindow
{
    private const string ProductId = "default";
    private const string OutputFolder = "Assets/GeneratedLODs";

    private float lod1Quality = 50f;
    private float lod2Quality = 20f;
    private float lod0Height = 0.5f;
    private float lod1Height = 0.15f;
    private float lod2Height = 0.02f;
    private bool protectBoundary = true;
    private bool protectShape = true;

    [MenuItem("Tools/軽量化検証/Mantis LODを一括生成")]
    private static void Open()
    {
        if (!LicenseAuth.IsAuthenticated(ProductId))
        {
            LicenseAuth.OpenAuthWindow(ProductId);
            return;
        }

        GetWindow<MantisLODBatchGenerator>("Mantis LOD 一括生成");
    }

    private void OnGUI()
    {
        if (!LicenseAuth.IsAuthenticated(ProductId))
        {
            EditorGUILayout.HelpBox("認証が必要です。一度ウィンドウを閉じてメニューから開き直してください。", MessageType.Warning);
            if (GUILayout.Button("認証する"))
            {
                LicenseAuth.OpenAuthWindow(ProductId);
                Close();
            }
            return;
        }

        EditorGUILayout.LabelField(
            "選択中のオブジェクト(MeshRendererが無ければ子階層を自動探索)に\nMantis LODでLOD1/LOD2メッシュを生成し、LODGroupを設定します。",
            EditorStyles.wordWrappedLabel);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("メッシュ品質", EditorStyles.boldLabel);
        lod1Quality = EditorGUILayout.Slider("LOD1 品質 (%)", lod1Quality, 1f, 99f);
        lod2Quality = EditorGUILayout.Slider("LOD2 品質 (%)", lod2Quality, 1f, 99f);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("切替しきい値(画面占有率)", EditorStyles.boldLabel);
        lod0Height = EditorGUILayout.Slider("LOD0 → LOD1", lod0Height, 0f, 1f);
        lod1Height = EditorGUILayout.Slider("LOD1 → LOD2", lod1Height, 0f, 1f);
        lod2Height = EditorGUILayout.Slider("LOD2 → カリング", lod2Height, 0f, 1f);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("簡略化オプション", EditorStyles.boldLabel);
        protectBoundary = EditorGUILayout.Toggle("境界を保護 (Protect Boundary)", protectBoundary);
        protectShape = EditorGUILayout.Toggle("三角形の形を保護 (Beautiful Triangles)", protectShape);

        EditorGUILayout.Space();
        if (GUILayout.Button("選択中のオブジェクトに生成", GUILayout.Height(32)))
        {
            GenerateForSelection();
        }
    }

    private void GenerateForSelection()
    {
        var targets = Selection.gameObjects;
        if (targets.Length == 0)
        {
            EditorUtility.DisplayDialog("Mantis LOD 一括生成", "Hierarchyでオブジェクトを選択してください。", "OK");
            return;
        }

        if (!AssetDatabase.IsValidFolder(OutputFolder))
            AssetDatabase.CreateFolder("Assets", "GeneratedLODs");

        var renderers = new List<MeshRenderer>();
        foreach (var go in targets)
        {
            if (go.GetComponent<LODGroup>() != null)
            {
                Debug.LogWarning($"[スキップ] {go.name} は既にLODGroupを持っています。");
                continue;
            }

            var ownRenderer = go.GetComponent<MeshRenderer>();
            if (ownRenderer != null && go.GetComponent<MeshFilter>() != null)
            {
                renderers.Add(ownRenderer);
            }
            else
            {
                renderers.AddRange(go.GetComponentsInChildren<MeshRenderer>(true)
                    .Where(r => r.GetComponent<MeshFilter>() != null && r.GetComponent<LODGroup>() == null));
            }
        }

        int done = 0;
        foreach (var mr in renderers)
        {
            if (ProcessRenderer(mr))
                done++;
        }

        AssetDatabase.SaveAssets();
        EditorUtility.DisplayDialog("Mantis LOD 一括生成", $"{done}件のオブジェクトにLODGroupを生成しました。\n生成メッシュ: {OutputFolder}", "OK");
    }

    private bool ProcessRenderer(MeshRenderer mr)
    {
        var go = mr.gameObject;
        var mf = go.GetComponent<MeshFilter>();
        var originalMesh = mf.sharedMesh;
        if (originalMesh == null)
        {
            Debug.LogWarning($"[スキップ] {go.name} にメッシュがありません。");
            return false;
        }

        try
        {
            var lod1Mesh = SimplifyMesh(originalMesh, lod1Quality, go.name + "_LOD1");
            var lod2Mesh = SimplifyMesh(originalMesh, lod2Quality, go.name + "_LOD2");

            var lod1Go = CreateLodChild(go, "LOD1", lod1Mesh, mr.sharedMaterials);
            var lod2Go = CreateLodChild(go, "LOD2", lod2Mesh, mr.sharedMaterials);

            var lodGroup = go.AddComponent<LODGroup>();
            var lods = new[]
            {
                new LOD(lod0Height, new[] { mr }),
                new LOD(lod1Height, new[] { lod1Go.GetComponent<MeshRenderer>() }),
                new LOD(lod2Height, new[] { lod2Go.GetComponent<MeshRenderer>() }),
            };
            lodGroup.SetLODs(lods);
            lodGroup.RecalculateBounds();

            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[{go.name}] LOD生成に失敗しました: {e}");
            return false;
        }
    }

    private Mesh SimplifyMesh(Mesh originalMesh, float quality, string assetName)
    {
        var workingMesh = Object.Instantiate(originalMesh);
        var mantisMeshes = new[] { new Mantis_Mesh { mesh = workingMesh } };

        MantisLODEditorUtility.PrepareSimplify(mantisMeshes);
        MantisLODEditorUtility.Simplify(mantisMeshes, protectBoundary, false, false, false, protectShape, false, 10);
        MantisLODEditorUtility.SetQuality(mantisMeshes, quality);

        var resultMesh = Object.Instantiate(workingMesh);
        resultMesh.name = assetName;

        MantisLODEditorUtility.FinishSimplify(mantisMeshes, true, false);
        Object.DestroyImmediate(workingMesh);

        var path = AssetDatabase.GenerateUniqueAssetPath($"{OutputFolder}/{assetName}.asset");
        AssetDatabase.CreateAsset(resultMesh, path);
        return resultMesh;
    }

    private GameObject CreateLodChild(GameObject parent, string label, Mesh mesh, Material[] materials)
    {
        var child = new GameObject(label);
        child.transform.SetParent(parent.transform, false);
        var mf = child.AddComponent<MeshFilter>();
        mf.sharedMesh = mesh;
        var mr = child.AddComponent<MeshRenderer>();
        mr.sharedMaterials = materials;
        return child;
    }
}
