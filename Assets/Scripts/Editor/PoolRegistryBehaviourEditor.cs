
using UnityEditor;
using UnityEngine;
using Utility;
using Utility.Pool;

[CustomEditor(typeof(PoolRegistryBehaviour))]
public class PoolRegistryBehaviourEditor : Editor
{
    private bool _foldout = true;

    public override void OnInspectorGUI()
    {
        var targetComp = (PoolRegistryBehaviour)target;

        DrawDefaultInspector();
        EditorGUILayout.Space(8);

        if (targetComp.ShowOnlyInPlayMode && !Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Play Mode에서만 풀 정보를 표시하도록 설정되어 있습니다.", MessageType.Info);
            return;
        }

        var pools = PoolRegistry.Pools;

        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField($"Registered Pools: {pools.Count}", EditorStyles.boldLabel);
            if (GUILayout.Button("Refresh", GUILayout.Width(80)))
                Repaint();
        }

        _foldout = EditorGUILayout.Foldout(_foldout, "Pools", true);
        if (!_foldout) return;

        if (pools.Count == 0)
        {
            EditorGUILayout.HelpBox("등록된 풀이 없습니다. (GameObjectPool<T> 생성 시 자동 등록됩니다)", MessageType.Warning);
            return;
        }

        EditorGUILayout.Space(4);

        for (int i = 0; i < pools.Count; i++)
        {
            var p = pools[i];
            if (p == null) continue;

            using (new EditorGUILayout.VerticalScope("box"))
            {
                EditorGUILayout.LabelField($"[{i}] {p.PoolTypeName}", EditorStyles.boldLabel);

                // Prefab / PrefabDefinition 표시
                var prefabGO = p.PrefabGameObject;
                GameObject prefabDef = null;

                if (prefabGO != null)
                    prefabDef = PrefabUtility.GetCorrespondingObjectFromSource(prefabGO);

                EditorGUILayout.ObjectField("Prefab (Component)", p.PrefabObject, typeof(Object), false);
                EditorGUILayout.ObjectField("Prefab (GameObject)", prefabGO, typeof(GameObject), false);

                if (prefabDef != null && prefabDef != prefabGO)
                    EditorGUILayout.ObjectField("PrefabDefinition", prefabDef, typeof(GameObject), false);
                else
                    EditorGUILayout.LabelField("PrefabDefinition", "(same as prefab)");

                // Asset path (있으면)
                var assetObj = (Object)(prefabDef != null ? prefabDef : prefabGO);
                if (assetObj != null)
                {
                    var path = AssetDatabase.GetAssetPath(assetObj);
                    if (!string.IsNullOrEmpty(path))
                        EditorGUILayout.LabelField("Asset Path", path);
                }

                // Parent
                EditorGUILayout.ObjectField("Default Parent", p.DefaultParent, typeof(Transform), true);

                EditorGUILayout.Space(4);

                // Counts
                EditorGUILayout.LabelField("Counts", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("CountAll", p.CountAll.ToString());
                EditorGUILayout.LabelField("Active", p.CountActive.ToString());
                EditorGUILayout.LabelField("Inactive", p.CountInactive.ToString());
                EditorGUILayout.LabelField("MaxSize", p.MaxSize.ToString());
                EditorGUILayout.LabelField("DefaultCapacity", p.DefaultCapacity.ToString());
                EditorGUILayout.LabelField("CollectionCheck", p.CollectionCheck ? "True" : "False");

                // 간단 진행바(활성 비율)
                float ratio = (p.CountAll <= 0) ? 0f : (float)p.CountActive / p.CountAll;
                Rect r = GUILayoutUtility.GetRect(1, 18);
                EditorGUI.ProgressBar(r, ratio, $"Active Ratio: {(ratio * 100f):0.0}%");

                // 편의 버튼
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (prefabGO != null && GUILayout.Button("Ping Prefab"))
                        EditorGUIUtility.PingObject(prefabGO);

                    if (prefabDef != null && GUILayout.Button("Ping PrefabDefinition"))
                        EditorGUIUtility.PingObject(prefabDef);
                }
            }
        }

        // 플레이 중이면 실시간 갱신
        if (Application.isPlaying)
            Repaint();
    }
}
