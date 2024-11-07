using TMPro;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// 게임 종료 팝업창 UI 관리 스크립트
public class ExitGame : MonoBehaviour
{
    public Image yesButton;
    public Image noButton;
    public CanvasGroup exitCanvasGroup;
    public MainMenuController mainMenuController;

    public float hoverScale = 1.1f;     // 확대 비율
    public float scaleDuration = 0.1f;  // 확대/축소 애니메이션 시간
    public float fadeInDuration = 1.0f; // 페이드아웃 시간

    void Start()
    {
        yesButton = Assign(yesButton, "YesButton");
        noButton = Assign(noButton, "NoButton");
        exitCanvasGroup = Assign(exitCanvasGroup, "SplashCoverImage");
        mainMenuController = Assign(mainMenuController, "MainMenuCanvas");

        // 각 텍스트에 클릭 이벤트 및 마우스 오버 이벤트 추가
        AddEventTrigger(yesButton, OnYesClicked);
        AddEventTrigger(noButton, OnNoClicked);
    }

    // 자동 할당 코드
    private T Assign<T>(T obj, string objectName) where T : Object
    {
        if (obj == null)
        {
            GameObject foundObject = GameObject.Find(objectName);
            if (foundObject != null)
            {
                if (typeof(Component).IsAssignableFrom(typeof(T))) obj = foundObject.GetComponent(typeof(T)) as T;
                else if (typeof(GameObject).IsAssignableFrom(typeof(T))) obj = foundObject as T;
            }
            if (obj == null) Debug.LogError($"{objectName} 를 찾을 수 없습니다.");
        }
        return obj;
    }

    // 텍스트 클릭 시 이벤트 추가
    private void AddEventTrigger(Image image, UnityEngine.Events.UnityAction action)
    {
        EventTrigger trigger = image.gameObject.GetComponent<EventTrigger>() ?? image.gameObject.AddComponent<EventTrigger>();
        var entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
        entry.callback.AddListener((eventData) => action());
        trigger.triggers.Add(entry);
    }

    // "예" 버튼 클릭 시
    private void OnYesClicked()
    {
        StartCoroutine(FadeInAndQuit());   // 페이드인과 함께 종료
    }

    // "아니오" 버튼 클릭 시
    private void OnNoClicked()
    {
        mainMenuController.ClosePopup(gameObject); // 팝업 닫기 호출
    }

    // 페이드인 후 게임 종료
    private IEnumerator FadeInAndQuit()
    {
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            exitCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
            yield return null;
        }

        exitCanvasGroup.alpha = 0f;
        Application.Quit(); // 게임 종료
    }
}
