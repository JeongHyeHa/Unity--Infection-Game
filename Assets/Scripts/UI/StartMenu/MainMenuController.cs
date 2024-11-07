using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class MainMenuController : MonoBehaviour
{
    // 메뉴 텍스트 버튼 4개 참조
    public TextMeshProUGUI loginMenu;
    public TextMeshProUGUI signupMenu; // LoadGameText를 AccountCreationText로 변경
    public TextMeshProUGUI settingMenu;
    public TextMeshProUGUI ExitGameMenu;

    public GameObject loginCanvas;
    public GameObject signupCanvas;
    public GameObject settingWindow;
    public GameObject exitGameCanvas;

    public float hoverScale = 1.1f;     // 확대 비율
    public float scaleDuration = 0.1f;  // 확대/축소 애니메이션 시간
    private bool isPopupActive = false; // 팝업 활성 상태를 확인하는 변수

    private void Start()
    {
        loginMenu = Assign(loginMenu, "LoginMenu");
        signupMenu = Assign(signupMenu, "SignUpMenu");
        settingMenu = Assign(settingMenu, "OptionMenu");
        ExitGameMenu = Assign(ExitGameMenu, "ExitGameMenu");
        loginCanvas = Assign(loginCanvas, "LoginCanvas");
        signupCanvas = Assign(signupCanvas, "SignUpCanvas");
        settingWindow = Assign(settingWindow, "SettingWindow");
        exitGameCanvas = Assign(exitGameCanvas, "ExitGameCanvas");

        //// Canvas를 올바른 순서로 배치
        //loginCanvas.transform.SetAsLastSibling();
        //signupCanvas.transform.SetAsLastSibling();
        //exitGameCanvas.transform.SetAsLastSibling();

        // 각 텍스트에 클릭 이벤트 및 마우스 오버 이벤트 추가
        AddEventTrigger(loginMenu, OnStartGameClicked);
        AddEventTrigger(signupMenu, OnAccountCreationClicked);
        AddEventTrigger(settingMenu, OnSettingsClicked);
        AddEventTrigger(ExitGameMenu, OnQuitGameClicked);

        // 텍스트 영역 들어오고 나가는 이벤트 추가
        AddHoverEvent(loginMenu);
        AddHoverEvent(signupMenu);
        AddHoverEvent(settingMenu);
        AddHoverEvent(ExitGameMenu);

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

    // 텍스트 클릭 시 이벤트 발생 구현
    private void AddEventTrigger(TextMeshProUGUI text, UnityEngine.Events.UnityAction action)
    {
        EventTrigger trigger = text.gameObject.GetComponent<EventTrigger>() ?? text.gameObject.AddComponent<EventTrigger>();
        var entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
        entry.callback.AddListener((eventData) => { if (!isPopupActive) action(); });
        trigger.triggers.Add(entry);
    }

    // 텍스트 영역과 마우스 포인터 간 상호작용
    private void AddHoverEvent(TextMeshProUGUI text)
    {
        EventTrigger trigger = text.gameObject.GetComponent<EventTrigger>() ?? text.gameObject.AddComponent<EventTrigger>();

        // 포인터가 텍스트 영역 내로 들어왔을 때
        var pointerEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        pointerEnter.callback.AddListener((eventData) => { if (!isPopupActive) OnPointerEnter(text); });
        trigger.triggers.Add(pointerEnter);

        // 포인터가 텍스트 영역 외부로 벗어났을 때
        var pointerExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        pointerExit.callback.AddListener((eventData) => { if (!isPopupActive) OnPointerExit(text); });
        trigger.triggers.Add(pointerExit);
    }

    // 포인터가 텍스트 영역 내에 있을 때는 글자 크기가 커짐 (강조효과)
    private void OnPointerEnter(TextMeshProUGUI text)
    {
        StartCoroutine(ScaleText(text, hoverScale, scaleDuration));
    }
    private void OnPointerExit(TextMeshProUGUI text)
    {
        StartCoroutine(ScaleText(text, 1f, scaleDuration));
    }
    private IEnumerator ScaleText(TextMeshProUGUI text, float targetScale, float duration)
    {
        Vector3 initialScale = text.transform.localScale;
        Vector3 targetScaleVector = new Vector3(targetScale, targetScale, targetScale);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            text.transform.localScale = Vector3.Lerp(initialScale, targetScaleVector, elapsed / duration);
            yield return null;
        }

        text.transform.localScale = targetScaleVector;
    }

    // 로그인 메뉴 클릭
    private void OnStartGameClicked()
    {
        Debug.Log("Start Game Clicked");
        loginCanvas.SetActive(true);    // 로그인 창 실행
        isPopupActive = true; // 팝업 활성 상태로 설정
    }

    // 회원가입 메뉴 클릭
    private void OnAccountCreationClicked()
    {
        Debug.Log("Account Creation Clicked");
        signupCanvas.SetActive(true);  // 회원가입 창 실행
        isPopupActive = true; // 팝업 활성 상태로 설정
    }

    // 설정 메뉴 클릭
    private void OnSettingsClicked()
    {
        Debug.Log("Settings Clicked");
        settingWindow.SetActive(true);// 설정 창 실행
    }

    // 게임종료 메뉴 클릭
    private void OnQuitGameClicked()
    {
        Debug.Log("Quit Game Clicked");
        exitGameCanvas.SetActive(true); // 게임종료 확인 창 실행
        isPopupActive = true; // 팝업 활성 상태로 설정
    }

    // ExitGame 팝업 종료 버튼
    // private 인 isPopupActive 관리 위해 여기서 사용
    public void ClosePopup(GameObject popupCanvas)
    {
        popupCanvas.SetActive(false);
        isPopupActive = false; // 팝업 비활성 상태로 설정
    }

    // 팝업 해제되면 메인메뉴 버튼 다시 클릭 가능하도록 사용
    public void EnableMenuInteraction()
    {
        isPopupActive = false;
    }
}
