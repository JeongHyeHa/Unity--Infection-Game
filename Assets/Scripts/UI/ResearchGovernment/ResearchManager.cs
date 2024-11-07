using System.Collections;
using System.Linq;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ResearchManager : MonoBehaviour
{
    //public GameObject research;                   // 연구 건물 버튼
    //public CalendarManager calendarManager;       // CalendarManager 스크립트 참조
    //public GovernmentManager govManager;          // governmentManager 스크립트 참조
    //public ProgressBar progressBar;               // ProgressBar 스크립트 참조
    //public PanelManager panelManager;             // PanelManager 스크립트 참조
    //public DictionaryManager dictManager;         // DictionaryManager 스크립트 참조
    //public CurrentMoney currentMoneyManager;      // CurrentMoney 스크립트 참조
    //public monthlyReportUI;       // MonthlyReportUI 스크립트 참조
    //public GameObject selectPanel;
    //public Button[] researchLevelButton;
    //public Button selectCloseButton;
    //public float researchDuration = 300f;         // 연구 소요 시간 (초)
    //public int maxCompletedResearchLevel = 0;     // 완료된 최고 연구 레벨 (초기값 0)
    //public int maxDictionaryLevel = 0;            // 원인균 최고 잠금 해제 레벨(초기값 0 (=CRE))
    //private bool isResearchActive = false;        // 연구 진행 여부
    //private bool isResearchButtonActive = false;  // 연구실 클릭 가능 여부  
    //private bool isTreatmentDeveloped = false;    // 치료제 개발 여부
    //private bool isVaccineDeveloped = false;      // 백신 개발 여부
    //private int selectedResearchLevel = 1;        // 현재 연구 레벨

    //void Start()
    //{
    //    InitializeElements();
    //    UpdateResearchButtons();                     // 버튼 초기 설정
    //    selectPanel.SetActive(false);
    //    StartCoroutine(CheckResearchAvailability()); // 연구 가능 여부 체크 시작 
    //}

    //private void InitializeElements()
    //{
    //    researchLevelButton = Enumerable.Range(1, 3)
    //        .Select(i => Assign<Button>(null, $"ResearchLevel{i}Button"))
    //        .ToArray();

    //    research = Assign(research, "Research");
    //    calendarManager = Assign(calendarManager, "InfoCanvas");
    //    govManager = Assign(govManager, "Government");
    //    progressBar = Assign(progressBar, "ChargeProgress");
    //    panelManager = Assign(panelManager, "PanelManager");
    //    dictManager = Assign(dictManager, "DictionaryManager");
    //    currentMoneyManager = Assign(currentMoneyManager, "CurrentMoneyManager");
    //    selectPanel = Assign(selectPanel, "SelectPanel");
    //    selectCloseButton = Assign(selectCloseButton, "SelectCloseButton");
    //    monthlyReportUI = FindObjectOfType<MonthlyReportUI>();

    //    // 버튼 클릭 이벤트 할당
    //    for (int i = 0; i < researchLevelButton.Length; i++)
    //    {
    //        int lev = i;    //클로저 문제 해결방안
    //        researchLevelButton[i].onClick.AddListener(() => OnResearchLevelSelected(lev + 1));
    //    }
    //    selectCloseButton.onClick.AddListener(() => OnClosePanelButton());
    //}

    //// 연구 가능 여부 체크
    //IEnumerator CheckResearchAvailability()
    //{
    //    while (true)
    //    {
    //        if (calendarManager != null && calendarManager.currentDay >= 18)  // 18일 이후로 연구 가능  
    //        {
    //            isResearchButtonActive = true;
    //            Debug.Log("연구 건물 버튼이 활성화되었습니다.");
    //            break;
    //        }
    //        yield return new WaitForSeconds(1); // 1초 간격으로 체크
    //    }
    //}

    //// 3D 오브젝트 클릭 이벤트 처리
    //void OnMouseDown()
    //{
    //    if (!govManager.IsGovernmentComplete())
    //    {
    //        panelManager.ShowWarning("warning", "질병관리청에 신고가 완료되어야 연구가 가능합니다.");
    //        return;
    //    }

    //    if (!isResearchButtonActive)
    //    {
    //        panelManager.ShowWarning("warning", "연구는 18일 이후에 가능합니다.");
    //        return;
    //    }

    //    if (govManager.IsAnyPanelActive())
    //    {
    //        return;
    //    }

    //    //연구를 시작할 수 있는 모든 조건을 만족하면
    //    selectPanel.SetActive(true);
    //}

    //// 연구 레벨 선택 시 호출
    //void OnResearchLevelSelected(int level)
    //{
    //    if (isResearchActive)
    //    {
    //        panelManager.ShowWarning("warning", "이미 진행 중인 연구가 있습니다.");
    //        selectPanel.SetActive(false);
    //        return;
    //    }

    //    if (currentMoneyManager.CurrentMoneyGetter < researchCost(level))
    //    {
    //        panelManager.ShowWarning("money", $"돈이 부족합니다. (현재 레벨 {level})(연구 비용: {researchCost(level)}ID)");
    //        selectPanel.SetActive(false);
    //        return;
    //    }

    //    if (maxCompletedResearchLevel > 0 && level == 1)   //레벨1의 연구가 끝나면 자유롭게 원인균 열람 가능
    //    {
    //        dictManager.OpenDictionaryPanel();
    //        selectPanel.SetActive(false);
    //        return;
    //    }

    //    // 연구 비용 차감
    //    currentMoneyManager.CurrentMoneyGetter -= researchCost(level);
    //    monthlyReportUI.AddExpense(researchCost(level));        // 월말표 갱신 
    //    Debug.Log($"레벨{level}: 연구 비용 {researchCost(level)}ID가 차감되었습니다. 남은 돈: {currentMoneyManager.CurrentMoneyGetter}ID");


    //    selectedResearchLevel = level; // 선택한 연구 레벨 저장
    //    selectPanel.SetActive(false);
    //    StartResearch();
    //}

    ////연구 시작
    //public void StartResearch()
    //{
    //    isResearchActive = true;            //연구 진행 여부
    //    progressBar.StartFilling(selectedResearchLevel, OnResearchComplete);    // 진행바 시작
    //    StartCoroutine(ResearchTimeout());  // 5분 후 연구 종료
    //}

    //private int researchCost(int level)  //연구 선택지 UI 만들 경우 삭제
    //{
    //    return level switch
    //    {
    //        1 => 500000,
    //        2 => 1000000,
    //        _ => 1500000,
    //    };
    //}

    //// 연구 완료 시 실행되는 함수
    //private void OnResearchComplete()
    //{
    //    isResearchActive = false;

    //    //각 레벨 별 기능 수행
    //    switch (selectedResearchLevel)
    //    {
    //        case 1:
    //            dictManager.OpenDictionaryPanel();
    //            selectPanel.SetActive(false);
    //            break;
    //        case 2:
    //            panelManager.ShowWarning("check", "축하합니다! 치료제 개발에 성공하셨습니다.");
    //            isTreatmentDeveloped = true;
    //            break;
    //        case 3:
    //            panelManager.ShowWarning("check", "축하합니다! 백신 개발에 성공하셨습니다.");
    //            isVaccineDeveloped = true;
    //            break;
    //    }

    //    // 연구 완료 후 현재 연구 레벨을 최대 완료된 연구 레벨로 업데이트
    //    maxCompletedResearchLevel = Mathf.Max(maxCompletedResearchLevel, selectedResearchLevel);

    //    UpdateResearchButtons();    // 연구 완료 후 버튼 상태 업데이트
    //    progressBar.ResetProgress();
    //}

    //// 연구 종료 코루틴
    //IEnumerator ResearchTimeout()
    //{
    //    yield return new WaitForSeconds(researchDuration); // 5분 대기

    //    isResearchActive = false;       //연구 진행 여부
    //    panelManager.ShowWarning("check", "연구가 종료되었습니다.");
    //}

    //// 연구 레벨 버튼의 활성화/비활성화 상태 업데이트
    //private void UpdateResearchButtons()
    //{
    //    researchLevelButton[0].interactable = true; // 레벨 1은 항상 가능
    //    researchLevelButton[1].interactable = maxCompletedResearchLevel >= 1;
    //    researchLevelButton[2].interactable = maxCompletedResearchLevel >= 2;
    //}

    ////닫기 버튼 눌렀을 때
    //private void OnClosePanelButton()
    //{
    //    selectPanel.SetActive(false);
    //}

    //// 치료제 개발 여부 확인
    //public bool IsTreatmentDeveloped() => isTreatmentDeveloped;

    //// 백신 개발 여부 확인
    //public bool IsVaccineDeveloped() => isVaccineDeveloped;

    //// 자동 할당 코드 
    //private T Assign<T>(T obj, string objectName) where T : Object
    //{
    //    if (obj == null)
    //    {
    //        GameObject foundObject = GameObject.Find(objectName);
    //        if (foundObject != null)
    //        {
    //            if (typeof(Component).IsAssignableFrom(typeof(T))) obj = foundObject.GetComponent(typeof(T)) as T;
    //            else if (typeof(GameObject).IsAssignableFrom(typeof(T))) obj = foundObject as T;
    //        }
    //        if (obj == null) Debug.LogError($"{objectName} 를 찾을 수 없습니다.");
    //    }
    //    return obj;
    //}
}
