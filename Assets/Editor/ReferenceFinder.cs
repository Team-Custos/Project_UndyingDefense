using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 삭제/교체하려는 오브젝트(및 그 하위 전체)를 다른 어떤 컴포넌트가 참조하고 있는지
/// 찾아주는 에디터 툴.
///
/// 사용 목적: UI 프리팹을 하나로 통합하기 전에, 기존 "UI" / "UIPrefab" 오브젝트를
/// 삭제해도 되는지, 다른 스크립트(GameManager, 카메라, 전투 로직 등)가 그 하위의
/// 특정 버튼/텍스트/이미지 등을 참조하고 있는지 미리 확인.
///
/// 사용법:
/// 1. 확인하려는 씬을 연다 (훈련장 / 금산성 / 남한산성 / 남원성 각각).
/// 2. Hierarchy에서 삭제하려는 UI 루트 오브젝트를 선택.
/// 3. 메뉴 Tools > Reference Finder > Find In Open Scenes 실행
///    (프리팹 에셋까지 포함해서 찾고 싶으면 Find In Open Scenes + All Prefabs 실행)
/// 4. Console 창에 결과 출력. 각 줄을 더블클릭하면 해당 오브젝트로 바로 이동.
///
/// 주의:
/// - "Find In Open Scenes"는 현재 열려 있는 씬만 검사합니다. 4개 씬 각각 열어서 돌려야 합니다.
/// - "+ All Prefabs"는 Assets 폴더의 모든 프리팹 에셋도 함께 검사합니다(프로젝트 크기에 따라 시간이 걸릴 수 있음).
/// - 이미 참조가 깨진(Missing) 상태는 당연히 잡을 수 없으니, 삭제하기 "전"에 실행해야 의미가 있습니다.
/// </summary>
public static class ReferenceFinder
{
    [MenuItem("Tools/Reference Finder/Find In Open Scenes")]
    public static void FindInOpenScenes()
    {
        var targets = CollectTargets();
        if (targets == null) return;

        var sb = new StringBuilder();
        int count = ScanOpenScenes(targets, sb);
        Report(count, sb);
    }

    [MenuItem("Tools/Reference Finder/Find In Open Scenes + All Prefabs")]
    public static void FindInOpenScenesAndPrefabs()
    {
        var targets = CollectTargets();
        if (targets == null) return;

        var sb = new StringBuilder();
        int count = ScanOpenScenes(targets, sb);
        count += ScanAllPrefabs(targets, sb);
        Report(count, sb);
    }

    // ---------- 내부 구현 ----------

    private static HashSet<Object> CollectTargets()
    {
        var selected = Selection.gameObjects;
        if (selected.Length == 0)
        {
            Debug.LogWarning("[ReferenceFinder] 먼저 Hierarchy(또는 Project)에서 검사할 오브젝트를 선택하세요.");
            return null;
        }

        var targets = new HashSet<Object>();
        foreach (var go in selected)
        {
            foreach (var t in go.GetComponentsInChildren<Transform>(true))
            {
                targets.Add(t.gameObject);
                foreach (var comp in t.GetComponents<Component>())
                {
                    if (comp != null) targets.Add(comp);
                }
            }
        }

        Debug.Log($"[ReferenceFinder] 대상 오브젝트/컴포넌트 {targets.Count}개(하위 포함)를 기준으로 검색합니다...");
        return targets;
    }

    private static int ScanOpenScenes(HashSet<Object> targets, StringBuilder sb)
    {
        int found = 0;
        for (int s = 0; s < SceneManager.sceneCount; s++)
        {
            var scene = SceneManager.GetSceneAt(s);
            if (!scene.isLoaded) continue;

            foreach (var root in scene.GetRootGameObjects())
            {
                found += ScanHierarchy(root, targets, sb, $"[Scene:{scene.name}]");
            }
        }
        return found;
    }

    private static int ScanAllPrefabs(HashSet<Object> targets, StringBuilder sb)
    {
        int found = 0;
        var guids = AssetDatabase.FindAssets("t:Prefab");
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var prefabRoot = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefabRoot == null) continue;

            found += ScanHierarchy(prefabRoot, targets, sb, $"[Prefab:{path}]");
        }
        return found;
    }

    private static int ScanHierarchy(GameObject root, HashSet<Object> targets, StringBuilder sb, string contextLabel)
    {
        int found = 0;
        foreach (var t in root.GetComponentsInChildren<Transform>(true))
        {
            foreach (var comp in t.GetComponents<Component>())
            {
                if (comp == null) continue; // Missing script
                if (targets.Contains(comp)) continue; // 자기 자신은 스킵

                var fieldPaths = FindObjectReferencesIn(comp, targets);
                foreach (var fieldPath in fieldPaths)
                {
                    found++;
                    string line = $"{contextLabel} {GetPath(comp.gameObject)}  <{comp.GetType().Name}>.{fieldPath}";
                    sb.AppendLine(line);
                    Debug.Log("[ReferenceFinder] " + line, comp);
                }
            }
        }
        return found;
    }

    private static List<string> FindObjectReferencesIn(Component comp, HashSet<Object> targets)
    {
        var result = new List<string>();
        var so = new SerializedObject(comp);
        var prop = so.GetIterator();
        bool enterChildren = true;

        while (prop.NextVisible(enterChildren))
        {
            enterChildren = true;

            if (prop.propertyType == SerializedPropertyType.ObjectReference)
            {
                var value = prop.objectReferenceValue;
                if (value != null && targets.Contains(value))
                {
                    result.Add(prop.propertyPath);
                }
            }
        }

        return result;
    }

    private static string GetPath(GameObject go)
    {
        string path = go.name;
        var t = go.transform.parent;
        while (t != null)
        {
            path = t.name + "/" + path;
            t = t.parent;
        }
        return path;
    }

    private static void Report(int count, StringBuilder sb)
    {
        if (count == 0)
        {
            Debug.Log("[ReferenceFinder] 참조를 찾지 못했습니다. (검사 범위를 벗어난 씬/프리팹이 있는지 확인하세요)");
        }
        else
        {
            Debug.Log($"[ReferenceFinder] 총 {count}건의 참조를 찾았습니다:\n{sb}");
        }
    }
}
