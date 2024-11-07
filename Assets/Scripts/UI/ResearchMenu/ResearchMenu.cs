using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ResearchMenu : MonoBehaviour
{
    public GameObject researchPanel;
    public Button researchOpenButton;
    public Image researchPanelCloseButton;

    public Image medicalResearchButton;
    public Image patientResearchButton;
    public Image hospitalResearchButton;

    public GameObject medicalResearch;
    public GameObject patientResearch;
    public GameObject hospitalResearch;

    public GameObject researchItem_1;
    public GameObject researchItem_2;
    public GameObject researchItem_3;

    private Color originalColor;
    private Color hoverColor = new Color(0.92f, 0.92f, 0.92f); // 어두워지는 색
    private Color clickColor = new Color(0.8f, 0.8f, 0.8f); // 클릭 시 더 어두운 색

    private Image currentSelectedButton; // 현재 선택된 버튼

    // Start is called before the first frame update
    void Awake()
    {
        researchPanel = Assign(researchPanel, "ResearchPanel");
        researchOpenButton = Assign(researchOpenButton, "ResearchOpenButton");
        researchPanelCloseButton = Assign(researchPanelCloseButton, "ResearchPanelCloseButton");
        medicalResearchButton = Assign(medicalResearchButton, "MedicalResearchButton");
        patientResearchButton = Assign(patientResearchButton, "PatientResearchButton");
        hospitalResearchButton = Assign(hospitalResearchButton, "HospitalResearchButton");
        medicalResearch = Assign(medicalResearch, "MedicalResearch");
        patientResearch = Assign(patientResearch, "PatientResearch");
        hospitalResearch = Assign(hospitalResearch, "HospitalResearch");
        researchItem_1 = Assign(researchItem_1, "ResearchItem_1");
        researchItem_2 = Assign(researchItem_2, "ResearchItem_2");
        researchItem_3 = Assign(researchItem_3, "ResearchItem_3");

        // 버튼 클릭 시 패널 활성화 설정
        researchOpenButton.onClick.AddListener(OpenResearchPanel);

        // 이미지(닫기 버튼) 클릭 이벤트 설정
        AddEventTrigger(researchPanelCloseButton.gameObject, EventTriggerType.PointerClick, CloseResearchPanel);

        // 각 연구 버튼 클릭 시 연구 변경
        AddEventTrigger(medicalResearchButton.gameObject, EventTriggerType.PointerClick, (data) => ChangeResearch("Medical", medicalResearchButton));
        AddEventTrigger(patientResearchButton.gameObject, EventTriggerType.PointerClick, (data) => ChangeResearch("Patient", patientResearchButton));
        AddEventTrigger(hospitalResearchButton.gameObject, EventTriggerType.PointerClick, (data) => ChangeResearch("Hospital", hospitalResearchButton));

        // 각 버튼에 마우스 오버 및 클릭 효과 추가
        AddHoverEffect(medicalResearchButton);
        AddHoverEffect(patientResearchButton);
        AddHoverEffect(hospitalResearchButton);
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

    // 연구 패널을 여는 함수
    private void OpenResearchPanel()
    {
        if (researchPanel != null)
        {
            medicalResearch.SetActive(true);
            patientResearch.SetActive(false);
            hospitalResearch.SetActive(false);
            researchPanel.SetActive(true);
            researchItem_1.SetActive(false);
            researchItem_2.SetActive(false);
            researchItem_3.SetActive(false);
            Time.timeScale = 0; // 게임 시간 멈춤
        }
    }

    // 연구 패널을 닫는 함수
    private void CloseResearchPanel(BaseEventData data)
    {
        if (researchPanel != null)
        {
            researchPanel.SetActive(false);
            Time.timeScale = 1; // 게임 시간 재개
        }
    }

    // 연구 전환 함수
    private void ChangeResearch(string researchType, Image selectedButton)
    {
        // 연구 타입에 따라 오브젝트 활성화/비활성화
        switch (researchType)
        {
            case "Medical":
                medicalResearch.SetActive(true);
                patientResearch.SetActive(false);
                hospitalResearch.SetActive(false);
                break;
            case "Patient":
                medicalResearch.SetActive(false);
                patientResearch.SetActive(true);
                hospitalResearch.SetActive(false);
                break;
            case "Hospital":
                medicalResearch.SetActive(false);
                patientResearch.SetActive(false);
                hospitalResearch.SetActive(true);
                break;
        }

        researchItem_1.SetActive(false);
        researchItem_2.SetActive(false);
        researchItem_3.SetActive(false);

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
}
