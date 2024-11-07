using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TutorialButtonPress : TutorialBase
{
    [SerializeField] private Button buttonToPress; // 일반 버튼
    [SerializeField] private Image closeButton;    // 예외적으로 처리할 Image 버튼
    private bool isButtonPressed = false;
    private bool tutorialCompleted = false;

    public override void Enter()
    {
        // 상태 초기화
        isButtonPressed = false;
        tutorialCompleted = false;

        // 일반 버튼에 클릭 이벤트 리스너 추가
        if (buttonToPress != null)
        {
            buttonToPress.onClick.AddListener(OnButtonPressed);
        }
        else
        {
            //Debug.LogError("Button not assigned in the inspector!");
        }

        // 예외적인 Image에 대해서는 EventTrigger로 처리
        if (closeButton != null)
        {
            AddEventTrigger(closeButton.gameObject, EventTriggerType.PointerClick, OnImageButtonPressed);
        }
        else
        {
            //Debug.LogError("Close button not assigned in the inspector!");
        }
    }

    public override void Execute(TutorialController controller)
    {
        // 버튼이 눌리고 튜토리얼이 아직 완료되지 않은 경우에만 실행
        if (isButtonPressed && !tutorialCompleted)
        {
            tutorialCompleted = true;

            StartCoroutine(controller.Delay());
        }
    }

    public override void Exit()
    {
        // 리스너 제거 및 상태 초기화
        if (buttonToPress != null)
        {
            buttonToPress.onClick.RemoveListener(OnButtonPressed);
        }

        // EventTrigger를 통한 예외 처리도 해제할 수 있지만, 이 예제에서는 생략
        isButtonPressed = false;
        tutorialCompleted = false;
    }

    // 일반 버튼이 눌렸을 때 처리
    private void OnButtonPressed()
    {
        if (!tutorialCompleted)
        {
            isButtonPressed = true;
            Debug.Log("Button pressed!" + buttonToPress);
        }
    }

    // 예외적으로 Image 버튼이 눌렸을 때 처리
    private void OnImageButtonPressed(BaseEventData eventData)
    {
        if (!tutorialCompleted)
        {
            isButtonPressed = true;
            Debug.Log("Image button (close) pressed!" + closeButton);
        }
    }

    // 이벤트 트리거를 추가하는 함수 (Image 전용)
    private void AddEventTrigger(GameObject target, EventTriggerType eventType, System.Action<BaseEventData> action)
    {
        EventTrigger trigger = target.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = target.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = eventType;
        entry.callback = new EventTrigger.TriggerEvent();
        entry.callback.AddListener((data) => { action(data); });

        trigger.triggers.Add(entry);
    }
}
