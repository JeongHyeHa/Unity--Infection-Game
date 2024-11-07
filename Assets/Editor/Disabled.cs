using UnityEditor;
using UnityEngine;

public class Disabled : EditorWindow
{
    // 에디터 창을 생성할 수 있는 메뉴 항목 추가
    [MenuItem("Tools/Disable MeshRenderer on IsInfection")]
    public static void DisableMeshRendererOnInfectionObjects()
    {
        // 경로에 있는 프리팹들을 가져옴
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Resources/Prefabs/Outpatient" });

        foreach (string prefabGuid in prefabGuids)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(prefabGuid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (prefab != null)
            {
                // IsInfection 오브젝트를 찾음
                Transform isInfectionTransform = prefab.transform.Find("IsInfection");
                if (isInfectionTransform != null)
                {
                    // MeshRenderer 컴포넌트를 찾고, 있으면 비활성화
                    MeshRenderer meshRenderer = isInfectionTransform.GetComponent<MeshRenderer>();
                    if (meshRenderer != null)
                    {
                        // MeshRenderer를 비활성화함
                        meshRenderer.enabled = false;
                        Debug.Log($"Disabled MeshRenderer on {prefab.name}/IsInfection");

                        // 프리팹 변경 사항 저장
                        PrefabUtility.SavePrefabAsset(prefab);
                    }
                    else
                    {
                        Debug.LogWarning($"MeshRenderer not found on IsInfection in prefab: {prefab.name}");
                    }
                }
                else
                {
                    Debug.LogWarning($"IsInfection object not found in prefab: {prefab.name}");
                }
            }
        }

        Debug.Log("MeshRenderer disabling process completed.");
    }
}
