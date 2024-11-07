#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class RemoveMissingScriptsInFolder : MonoBehaviour
{
    [MenuItem("Tools/Remove Missing Scripts in Prefabs in Folder")]
    private static void RemoveMissingScriptsInPrefabsInFolder()
    {
        string folderPath = "Assets/Resources/Prefabs/Patient"; // 원하는 폴더 경로로 변경

        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });

        foreach (string guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            // EditPrefabContentsScope 사용하여 프리팹 수정
            using (var editScope = new PrefabUtility.EditPrefabContentsScope(path))
            {
                GameObject prefabRoot = editScope.prefabContentsRoot;

                // 'Missing Script'가 존재하는지 확인하고 제거
                int removedCount = RemoveMissingScripts(prefabRoot);
                if (removedCount > 0)
                {
                    Debug.Log($"Removed {removedCount} missing scripts from: {path}");
                }
            }
        }

        Debug.Log("Missing scripts removed from all prefabs in folder.");
    }

    // GameObject에서 'Missing Script' 컴포넌트를 제거하는 함수
    private static int RemoveMissingScripts(GameObject gameObject)
    {
        int removedCount = 0;
        Component[] components = gameObject.GetComponents<Component>();

        for (int i = components.Length - 1; i >= 0; i--)
        {
            if (components[i] == null) // Missing Script인 경우
            {
                Undo.RegisterCompleteObjectUndo(gameObject, "Remove Missing Script");
                GameObjectUtility.RemoveMonoBehavioursWithMissingScript(gameObject);
                removedCount++;
            }
        }

        // 모든 하위 게임 오브젝트도 검사
        foreach (Transform child in gameObject.transform)
        {
            removedCount += RemoveMissingScripts(child.gameObject);
        }

        return removedCount;
    }
}
#endif
