using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class SettingWindow : MonoBehaviour
{
    public GameObject settingWindow;
    public Image closeButton;

    public GameObject overLayUI;

    void Start()
    {
        settingWindow = Assign(settingWindow, "SettingWindow");
        closeButton = Assign(closeButton, "SettingPanelCloseButton");
        overLayUI = Assign(overLayUI, "OverLayUI");

        // 이미지 버튼 클릭 이벤트 메서드 등록 (필요에 따라 추가 기능 구현)
        SetupButton(closeButton, OnCloseButtonClick);
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


    void SetupButton(Image image, System.Action onClick)
    {
        EventTrigger trigger = image.gameObject.AddComponent<EventTrigger>();

        // 클릭 시 메서드 호출
        EventTrigger.Entry pointerClick = new EventTrigger.Entry();
        pointerClick.eventID = EventTriggerType.PointerClick;
        pointerClick.callback.AddListener((eventData) => { onClick(); });
        trigger.triggers.Add(pointerClick);
    }


    void OnCloseButtonClick()
    {
        IsAbleManager.Instance.CloseWindow(settingWindow);
        // 패널 비활성화
        settingWindow.SetActive(false);
        overLayUI.SetActive(false);
    }
}
