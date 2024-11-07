using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PolicyMenu : MonoBehaviour
{
    public PolicyResearch policyResearch;

    public GameObject policyPanel;
    public Button policyOpenButton;
    public Image policyCloseButton;

    public Image gearTabButton;
    public Image patientTabButton;
    public Image researchTabButton;

    public GameObject gearTab;
    public GameObject patientTab;
    public GameObject researchTab;

    private Color originalColor;
    private Color hoverColor = new Color(0.92f, 0.92f, 0.92f); // 어두워지는 색
    private Color clickColor = new Color(0.8f, 0.8f, 0.8f); // 클릭 시 더 어두운 색

    private Image currentSelectedButton; // 현재 선택된 버튼

    private CameraHandler cameraHandler;

    // Start is called before the first frame update
    void Awake()
    {
        cameraHandler = FindObjectOfType<CameraHandler>();

        policyPanel = Assign(policyPanel, "PolicyPanel");
        policyOpenButton = Assign(policyOpenButton, "PolicyOpenButton");
        policyCloseButton = Assign(policyCloseButton, "PolicyCloseButton");
        gearTabButton = Assign(gearTabButton, "GearTabButton");
        patientTabButton = Assign(patientTabButton, "PatientTabButton");
        researchTabButton = Assign(researchTabButton, "ResearchTabButton");
        gearTab = Assign(gearTab, "GearTab");
        patientTab = Assign(patientTab, "PatientTab");
        researchTab = Assign(researchTab, "ResearchTab");

        // 버튼 클릭 시 패널 활성화 설정
        policyOpenButton.onClick.AddListener(OpenPolicyPanel);

        // 이미지(닫기 버튼) 클릭 이벤트 설정
        AddEventTrigger(policyCloseButton.gameObject, EventTriggerType.PointerClick, ClosePolicyPanel);

        // 각 연구 버튼 클릭 시 연구 변경
        AddEventTrigger(gearTabButton.gameObject, EventTriggerType.PointerClick, (data) => ChangePolicy("Gear", gearTabButton));
        AddEventTrigger(patientTabButton.gameObject, EventTriggerType.PointerClick, (data) => ChangePolicy("Patient", patientTabButton));
        AddEventTrigger(researchTabButton.gameObject, EventTriggerType.PointerClick, (data) => ChangePolicy("Research", researchTabButton));

        // 각 버튼에 마우스 오버 및 클릭 효과 추가
        AddHoverEffect(gearTabButton);
        AddHoverEffect(patientTabButton);
        AddHoverEffect(researchTabButton);
    }

    // 정책 창 오픈 함수
    private void OpenPolicyPanel()
    {
        if (policyPanel != null)
        {
            // Gear 탭을 기본 선택 상태로 설정
            ChangePolicy("Gear", gearTabButton);
            currentSelectedButton = gearTabButton;

            policyPanel.SetActive(true);
            Time.timeScale = 0f; // 게임 시간 멈춤
            cameraHandler.isPolicyMenuOpen = true; // 정책 창이 열렸음을 설정

        }

    }


    // 정책 창 닫는 함수
    private void ClosePolicyPanel(BaseEventData data)
    {
        if (policyPanel != null)
        {
            policyPanel.SetActive(false);
            Time.timeScale = 1; // 게임 재개
            cameraHandler.isPolicyMenuOpen = false; // 정책 창이 닫혔음을 설정

        }
    }

    // 정책 버튼 클릭 시 해당 정책으로 변경하는 함수
    private void ChangePolicy(string policyType, Image selectedButton)
    {
        switch (policyType)
        {
            case "Gear":
                gearTab.SetActive(true);
                patientTab.SetActive(false);
                researchTab.SetActive(false);
                break;
            case "Patient":
                gearTab.SetActive(false);
                patientTab.SetActive(true);
                researchTab.SetActive(false);
                break;
            case "Research":
                gearTab.SetActive(false);
                patientTab.SetActive(false);
                researchTab.SetActive(true);
                policyResearch.OpenResearchPanel();
                policyResearch.medicineUsePanel.SetActive(false);
                policyResearch.vaccineUsePanel.SetActive(false);
                break;
        }

        // 이전에 선택된 버튼의 색상을 원래대로 복원
        if (currentSelectedButton != null)
        {
            currentSelectedButton.color = originalColor;
        }

        // 새로운 선택된 버튼의 색상을 클릭 색상으로 변경
        currentSelectedButton = selectedButton;
        currentSelectedButton.color = clickColor;
    }

    // 마우스 오버와 클릭 효과 추가 함수
    private void AddHoverEffect(Image button)
    {
        originalColor = button.color;

        AddEventTrigger(button.gameObject, EventTriggerType.PointerEnter, (data) => OnHover(button));
        AddEventTrigger(button.gameObject, EventTriggerType.PointerExit, (data) => OnHoverExit(button));
    }

    // 마우스 오버 시 호출되는 함수
    private void OnHover(Image button)
    {
        // 선택된 버튼이 아닐 때만 호버 색상을 적용
        if (button != currentSelectedButton)
        {
            button.color = hoverColor;
        }
    }

    // 마우스 오버 종료 시 호출되는 함수
    private void OnHoverExit(Image button)
    {
        // 선택된 버튼이 아닐 때만 원래 색상으로 복원
        if (button != currentSelectedButton)
        {
            button.color = originalColor;
        }
    }

    // 이벤트 트리거를 추가하는 함수
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

    // 자동 오브젝트 할당 함수
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
}
