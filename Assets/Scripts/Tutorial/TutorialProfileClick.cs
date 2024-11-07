using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class TutorialProfileClick : TutorialBase
{
    private bool isClicked = false;

    public override void Enter()
    {
        Debug.Log("Click on any profile to proceed.");
    }

    public override void Execute(TutorialController controller)
    {
        // 마우스 클릭을 감지하여 EventSystem을 사용해 UI 클릭 확인
        if (Input.GetMouseButtonDown(0))
        {
            // UI 클릭 감지
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // 클릭된 UI 오브젝트 감지
                PointerEventData pointerData = new PointerEventData(EventSystem.current)
                {
                    position = Input.mousePosition
                };

                // RaycastAll을 사용하여 모든 UI 오브젝트 검사
                var results = new System.Collections.Generic.List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, results);

                foreach (var result in results)
                {
                    // 클릭된 오브젝트에서 ProfileClickHandler 또는 부모 오브젝트에서 ProfileClickHandler 찾기
                    ProfileClickHandler profileClickHandler = result.gameObject.GetComponentInParent<ProfileClickHandler>();
                    if (profileClickHandler != null && !isClicked)
                    {
                        Debug.Log($"Profile clicked: {profileClickHandler.personID}");
                        isClicked = true;
                        StartCoroutine(controller.Delay());  // 클릭 후 딜레이 처리
                    }
                    else
                    {
                        Debug.Log("Clicked object does not have ProfileClickHandler.");
                    }
                }
            }
        }
    }

    public override void Exit()
    {
        // 필요 시 종료 로직 추가
    }
}
