using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera mainCamera; // 메인 카메라
    private CameraHandler cameraHandler; // CameraHandler 인스턴스

    [System.Serializable]
    public struct FloorView
    {
        public Transform topViewTransform; // 탑뷰 트랜스폼
        public Transform quarterViewTransform; // 쿼터뷰 트랜스폼
    }

    public FloorView[] floorViews; // 각 층의 뷰 설정
    public int currentFloorIndex = 0; // 현재 선택된 층 인덱스
    private bool isTopView = true; // 현재 뷰 모드 (탑뷰 여부)

    public string[] floorLayerNames; // 각 층별 내부 레이어 이름 배열
    public string[] stairsLayerNames; // 각 층별 계단 레이어 이름 배열

    public Transform cameraPositionParent; // CameraPosition 오브젝트를 할당

    private float defaultZoomSpeed;
    private float defaultMoveSpeed;
    private float defaultEdgeMoveSpeed;
    private float defaultMaxXDistanceFromCenter;
    private float defaultMaxZDistanceFromCenter;

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        cameraHandler = mainCamera.GetComponent<CameraHandler>();
        if (cameraHandler == null)
        {
            Debug.LogError("CameraHandler not found on the main camera!");
            return;
        }

        AssignCameraPositions(); // 카메라 위치 할당

        SaveDefaultCameraSettings();
        MoveToFloor(0); // 초기 층 설정
    }

    private void AssignCameraPositions()
    {
        // TopView와 QuarterView의 트랜스폼을 찾음
        Transform topViewParent = cameraPositionParent.Find("TopView");
        Transform quarterViewParent = cameraPositionParent.Find("QuarterView");

        if (topViewParent == null || quarterViewParent == null)
        {
            Debug.LogError("TopView or QuarterView parent not found!");
            return;
        }

        // floorViews 배열 초기화
        floorViews = new FloorView[topViewParent.childCount];

        // 각 층의 TopView와 QuarterView를 할당
        for (int i = 0; i < floorViews.Length; i++)
        {
            floorViews[i].topViewTransform = topViewParent.GetChild(i);
            floorViews[i].quarterViewTransform = quarterViewParent.GetChild(i);
        }
    }

    private void SaveDefaultCameraSettings()
    {
        defaultZoomSpeed = cameraHandler.zoomSpeed;
        defaultEdgeMoveSpeed = cameraHandler.edgeMoveSpeed;
        defaultMoveSpeed = cameraHandler.moveSpeed;
        defaultMaxXDistanceFromCenter = cameraHandler.maxXDistanceFromCenter;
        defaultMaxZDistanceFromCenter = cameraHandler.maxZDistanceFromCenter;
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    ToggleViewMode(); // 'T' 키를 눌러 뷰 모드 토글
        //}
    }

    public void MoveToFloor(int floorIndex)
    {
        if (floorIndex < 0 || floorIndex >= floorViews.Length)
        {
            Debug.LogError("Invalid floor index");
            return;
        }

        currentFloorIndex = floorIndex;
        SetView();
        ApplyCameraSettings(floorIndex);
    }

    public void ToggleViewMode()
    {
        isTopView = !isTopView;
        SetView(); // 뷰 모드 변경
    }

    private void SetView()
    {
        if (isTopView)
        {
            SetTopView(); // 탑뷰 설정
            //cameraHandler.SetViewMode(CameraHandler.ViewMode.TopView); // 카메라 핸들러에 탑뷰 모드 설정
        }
        else
        {
            SetQuarterView(); // 쿼터뷰 설정
            //cameraHandler.SetViewMode(CameraHandler.ViewMode.QuarterView); // 카메라 핸들러에 쿼터뷰 모드 설정
        }

        // 중심 위치 업데이트
        Vector3 newCenterPosition = isTopView ? floorViews[currentFloorIndex].topViewTransform.position : floorViews[currentFloorIndex].quarterViewTransform.position;
        cameraHandler.SetCenterPosition(newCenterPosition); // 카메라 핸들러에 중심 위치 설정
    }

    private void SetTopView()
    {
        mainCamera.transform.position = floorViews[currentFloorIndex].topViewTransform.position; // 메인 카메라의 위치를 탑뷰 위치로 설정
        mainCamera.transform.rotation = floorViews[currentFloorIndex].topViewTransform.rotation; // 메인 카메라의 회전을 탑뷰 회전으로 설정
    }

    private void SetQuarterView()
    {
        mainCamera.transform.position = floorViews[currentFloorIndex].quarterViewTransform.position; // 메인 카메라의 위치를 쿼터뷰 위치로 설정
        mainCamera.transform.rotation = floorViews[currentFloorIndex].quarterViewTransform.rotation; // 메인 카메라의 회전을 쿼터뷰 회전으로 설정
    }

    private void ApplyCameraSettings(int floorIndex)
    {
        if (floorIndex == 10) // 옥상일 경우 특수 설정
        {
            cameraHandler.zoomSpeed = 20.0f;
            cameraHandler.edgeMoveSpeed = 32.0f;
            cameraHandler.moveSpeed = 32.0f;
            cameraHandler.maxXDistanceFromCenter = 80.0f;
            cameraHandler.maxZDistanceFromCenter = 80.0f;
        }
        else
        {
            cameraHandler.zoomSpeed = defaultZoomSpeed;
            cameraHandler.edgeMoveSpeed = defaultEdgeMoveSpeed;
            cameraHandler.moveSpeed = defaultMoveSpeed;
            cameraHandler.maxXDistanceFromCenter = defaultMaxXDistanceFromCenter;
            cameraHandler.maxZDistanceFromCenter = defaultMaxZDistanceFromCenter;
        }

        // 카메라 CullingMask 설정

        int layerMask = 0;

        // 해당 층의 내부 레이어 설정
        if (floorIndex < floorLayerNames.Length)
        {
            int floorLayer = LayerMask.NameToLayer(floorLayerNames[floorIndex]);
            if (floorLayer != -1)
            {
                layerMask |= 1 << floorLayer;
            }
            else
            {
                Debug.LogWarning($"Layer not found: {floorLayerNames[floorIndex]}");
            }
        }

        // 해당 층의 계단 레이어 설정
        int stairsLayerIndex = floorIndex / 2;
        if (stairsLayerIndex < stairsLayerNames.Length)
        {
            int stairsLayer = LayerMask.NameToLayer(stairsLayerNames[stairsLayerIndex]);
            if (stairsLayer != -1)
            {
                layerMask |= 1 << stairsLayer;
            }
            else
            {
                Debug.LogWarning($"Stairs layer not found: {stairsLayerNames[stairsLayerIndex]}");
            }
        }

        // UI 및 기타 필요한 기본 레이어 추가
        int uiLayer = LayerMask.NameToLayer("Default");
        if (uiLayer != -1)
        {
            layerMask |= 1 << uiLayer;
        }
        else
        {
            Debug.LogWarning("UI layer not found");
        }

        // 카메라에 Culling Mask 적용
        mainCamera.cullingMask = layerMask;
    }
}
