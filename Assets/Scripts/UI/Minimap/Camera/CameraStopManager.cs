using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; } // 싱글톤 인스턴스

    public CameraHandler cameraHandler; // CameraHandler 스크립트
    public List<GameObject> uiPanels; // UI 패널 리스트
    public Canvas questCanvas; // QuestCanvas의 Canvas 컴포넌트 참조

    void Start()
    {
        if (cameraHandler == null)
        {
            cameraHandler = FindObjectOfType<CameraHandler>();
            if (cameraHandler == null)
            {
                Debug.LogError("CameraHandler not found!");
            }
        }

        // QuestCanvas가 null인지 확인
        if (questCanvas == null)
        {
            Debug.LogError("QuestCanvas is not assigned!");
        }
    }

    void Update()
    {
        HandleUIAndCameraControl();
    }

    private void HandleUIAndCameraControl()
    {
        bool anyPanelActive = false;

        // UI 패널이 활성화되어 있는지 확인
        foreach (GameObject uiPanel in uiPanels)
        {
            if (uiPanel.activeSelf)
            {
                anyPanelActive = true;
                break;
            }
        }

        // QuestCanvas의 Canvas 컴포넌트가 활성화되어 있는지 확인
        if (questCanvas != null && questCanvas.enabled)
        {
            anyPanelActive = true;
        }

        // 카메라 핸들러를 활성화 또는 비활성화
        cameraHandler.enabled = !anyPanelActive;
    }
}
