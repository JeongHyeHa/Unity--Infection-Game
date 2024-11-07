using UnityEngine;
using UnityEditor;
using System.IO;

public class PrefabCreator : EditorWindow
{
    [MenuItem("Tools/Process Prefabs and Assign IconCanvas")]
    public static void ProcessPrefabsAndAssignIconCanvas()
    {
        // 프리팹 경로 설정
        string prefabPath = "Assets/Resources/Prefabs/Outpatient";

        // 해당 경로의 모든 프리팹 파일 로드
        string[] prefabFiles = Directory.GetFiles(prefabPath, "*.prefab", SearchOption.AllDirectories);

        // 각 프리팹 처리
        foreach (string prefabFile in prefabFiles)
        {
            // 프리팹 로드
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabFile);
            if (prefab == null)
            {
                Debug.LogError("Failed to load prefab: " + prefabFile);
                continue;
            }

            // Prefab의 모든 컴포넌트 가져오기
            Component[] components = prefab.GetComponents<Component>();
            bool hasChanges = false;

            // Missing Script 제거
            for (int i = components.Length - 1; i >= 0; i--)
            {
                if (components[i] == null) // Missing Script가 있는 경우
                {
                    GameObjectUtility.RemoveMonoBehavioursWithMissingScript(prefab);
                    hasChanges = true;
                    Debug.Log("Missing script removed from: " + prefab.name);
                }
            }

            // IconManager 스크립트가 없다면 추가
            IconManager iconManager = prefab.GetComponent<IconManager>();
            if (iconManager == null)
            {
                iconManager = prefab.AddComponent<IconManager>();
                hasChanges = true;
                Debug.Log("IconManager script added to: " + prefab.name);
            }

            // IconCanvas라는 오브젝트를 찾음
            Transform iconCanvasTransform = prefab.transform.Find("IconCanvas");
            if (iconCanvasTransform != null)
            {
                GameObject iconCanvas = iconCanvasTransform.gameObject;

                // IconManager의 IconCanvas 필드에 IconCanvas 오브젝트를 할당
                if (iconManager.iconCanvas == null)
                {
                    iconManager.iconCanvas = iconCanvas;
                    hasChanges = true;
                    Debug.Log("IconCanvas assigned to IconManager in: " + prefab.name);
                }
            }
            else
            {
                Debug.LogWarning("IconCanvas not found in prefab: " + prefab.name);
            }

            // Prefab에 변경 사항이 있으면 저장
            if (hasChanges)
            {
                // 프리팹 적용
                PrefabUtility.SavePrefabAsset(prefab);
                Debug.Log("Prefab updated: " + prefab.name);
            }
        }

        Debug.Log("Prefab processing and IconCanvas assignment completed.");
    }
}
