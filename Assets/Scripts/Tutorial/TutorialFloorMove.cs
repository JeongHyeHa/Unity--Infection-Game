using UnityEngine;
using System.Collections;

public class TutorialFloorMove : TutorialBase
{
    private CameraController cameraController;
    private bool isFloorChanged = false;

    public override void Enter()
    {
        // CameraController 인스턴스를 찾음
        cameraController = FindObjectOfType<CameraController>();
        if (cameraController == null)
        {
            Debug.LogError("CameraController not found!");
            return;
        }

        Debug.Log("Wait for the floor index to change to 2.");
    }

    public override void Execute(TutorialController controller)
    {
        // floorIndex가 2로 바뀌는지 확인
        if (cameraController.currentFloorIndex == 2 && !isFloorChanged)
        {
            isFloorChanged = true;

            cameraController.StartCoroutine(controller.Delay());
        }
    }

    public override void Exit()
    {
        // 상태 초기화
        isFloorChanged = false;
    }
}
