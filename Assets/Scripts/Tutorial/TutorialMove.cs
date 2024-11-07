using UnityEngine;
using UnityEngine.UI;

public class TutorialMove : TutorialBase
{
    private float moveDistance = 3.0f; // 이동해야 할 거리
    private Vector3 initialPosition;
    private Vector3 firstPosition;

    private CameraHandler cameraHandler;

    [SerializeField] private GameObject KeyImage;        // 키 이미지 UI
    [SerializeField] private Toggle CheckToggle_W; // 체크박스
    [SerializeField] private Toggle CheckToggle_S; // 체크박스
    [SerializeField] private Toggle CheckToggle_A; // 체크박스
    [SerializeField] private Toggle CheckToggle_D; // 체크박스

    public override void Enter()
    {
        // 카메라 핸들러를 찾음
        cameraHandler = FindObjectOfType<CameraHandler>();
        if (cameraHandler == null)
        {
            Debug.LogError("CameraHandler not found!");
            return;
        }

        // 카메라 초기 위치 저장
        initialPosition = cameraHandler.mainCamera.transform.position;
        firstPosition = cameraHandler.mainCamera.transform.position;

        // UI 초기 상태 설정
        KeyImage.SetActive(true);
        CheckToggle_W.isOn = false;
        CheckToggle_S.isOn = false;
        CheckToggle_A.isOn = false;
        CheckToggle_D.isOn = false;

        // 튜토리얼 시작 안내
        Debug.Log("Move the camera in all directions (W, A, S, D) by " + moveDistance + " units to continue.");
    }

    public override void Execute(TutorialController controller)
    {
        if (cameraHandler == null) return;

        Vector3 currentPosition = cameraHandler.mainCamera.transform.position;
        Vector3 distanceMoved = currentPosition - initialPosition;

        // 상하좌우 이동 감지 - 설정한 이동 거리 이상 이동했을 때만 true
        if (!CheckToggle_W.isOn && distanceMoved.z <= -moveDistance)
        {
            CheckToggle_W.isOn = true;
            initialPosition = cameraHandler.mainCamera.transform.position;
            Debug.Log("Moved up enough.");
        }
        if (!CheckToggle_S.isOn && distanceMoved.z >= moveDistance)
        {
            CheckToggle_S.isOn = true;
            initialPosition = cameraHandler.mainCamera.transform.position;
            Debug.Log("Moved down enough.");
        }
        if (!CheckToggle_A.isOn && distanceMoved.x >= moveDistance)
        {
            CheckToggle_A.isOn = true;
            initialPosition = cameraHandler.mainCamera.transform.position;
            Debug.Log("Moved left enough.");
        }
        if (!CheckToggle_D.isOn && distanceMoved.x <= -moveDistance)
        {
            CheckToggle_D.isOn = true;
            initialPosition = cameraHandler.mainCamera.transform.position;
            Debug.Log("Moved right enough.");
        }

        // 모든 방향으로 설정된 거리만큼 이동했을 때 다음 튜토리얼로 이동
        if (CheckToggle_W.isOn && CheckToggle_S.isOn && CheckToggle_A.isOn && CheckToggle_D.isOn)
        {
            Debug.Log("All directions moved enough. Proceeding to the next tutorial.");
            //StartCoroutine(controller.Delay());
            controller.SetNextTutorial();
        }
    }

    public override void Exit()
    {
        // 튜토리얼 종료 시의 로직을 여기에 추가할 수 있습니다.
        // UI 요소 비활성화
        KeyImage.SetActive(false);
        CheckToggle_W.isOn = false;
        CheckToggle_S.isOn = false;
        CheckToggle_A.isOn = false;
        CheckToggle_D.isOn = false;
    }
}
