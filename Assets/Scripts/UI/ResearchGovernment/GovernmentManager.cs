using UnityEngine;
using System.Collections;

public class GovernmentManager : MonoBehaviour
{
    //public GameObject government;               //질병관리청 건물 버튼
    //public ProgressBar progressBar;             // ProgressBar 스크립트
    //public PanelManager panelManager;           // PanelManager 스크립트 참조
    //public DictionaryManager dictManager;       //DictionaryManager 스크립트 참조
    //public ResearchManager researchManager;            //ResearchManager 스크립트 참조
    //public GoToGame gotoGame;
    //public MinimapController minimapController;
    //public GameObject windowPanel;
    //public int currentStage = 1;                    //현재 게임 난이도  //통합 시 UI화면에서 결정한 Stage값을 가져와야 함
    //public bool isGovernmentComplete = false;   //질병관리청 신고 완료 여부
    //private bool isDiseaseControlButtonSelected = false;  //질병관리청 버튼 선택 여부
    //private string currentDisease;  // 현재 질병 이름

    //void Start()
    //{
    //    // 오브젝트 자동 할당
    //    government = Assign(government, "Government");
    //    progressBar = Assign(progressBar, "ChargeProgress");
    //    panelManager = Assign(panelManager, "PanelManager");
    //    dictManager = Assign(dictManager, "DictionaryManager");
    //    researchManager = Assign(researchManager, "Research");
    //    windowPanel = Assign(windowPanel, "WindowPanel");

    //    if (currentStage == 2)
    //    {
    //        currentDisease = "COVID-19";
    //    }
    //    else
    //    {
    //        currentDisease = "CURE";
    //    }

    //    // OutpatientCreator의 원래 최대 외래 환자 수 저장
    //    //originalMaxOutpatients = Managers.ObjectPooling.maxOfOutpatient; (통합 시 주석 해제)
    //    //dictManager.dictionaryPanel.SetActive(false);
    //}

    //// 질병관리청 클릭 이벤트
    //void OnMouseDown()
    //{
    //    if (!gotoGame.isStart)
    //    {
    //        return;
    //    }

    //    if (!IsAbleManager.Instance.CanOpenNewWindow())
    //    {
    //        return;
    //    }

    //    if (IsAnyPanelActive())
    //    {
    //        return;
    //    }

    //    OnGovernmentButtonClicked();
    //}

    ////패널 활성화 여부 관리
    //public bool IsAnyPanelActive()
    //{
    //    return researchManager.selectPanel.activeSelf || panelManager.newsPanel.activeSelf || dictManager.dictionaryPanel.activeSelf;
    //}

    ////질병관리청 건물을 처음 눌렀을 때만 작동하도록
    //public void OnGovernmentButtonClicked()
    //{
    //    if (!isDiseaseControlButtonSelected)
    //    {
    //        progressBar.StartFilling(4, OtherLevelComplete);
    //        isDiseaseControlButtonSelected = true;
    //    }
    //    else
    //    {
    //        panelManager.ShowWarning("warning", "이미 신고한 상태입니다.");
    //    }

    //}

    ////질병관리청 검사 진행바가 완료됐을 경우 진행할 함수
    //public void OtherLevelComplete()
    //{
    //    panelManager.ShowWarning("check", "질병관리청에 신고가\n완료되었습니다. \n진단법, 치료제, 백신 연구를\n시작할 수 있습니다.");
    //    progressBar.ResetProgress();  //검색바 초기화
    //    isGovernmentComplete = true;

    //    //레벨2 && 원인균이 COVID-19일 경우
    //    if (currentDisease == "COVID-19")
    //    {
    //        StartCoroutine(COVIDWarningPanel());
    //    }
    //    else
    //    {
    //        // 조건이 만족하지 않는 경우 원래 최대 외래 환자 수로 복원
    //        //Managers.ObjectPooling.maxOfOutpatient = originalMaxOutpatients;  (통합 시 주석 해제)
    //    }
    //}

    //private IEnumerator COVIDWarningPanel()
    //{
    //    yield return new WaitForSeconds(1f);
    //    panelManager.ShowWarning("warning", "현재 원인 균 'COVID-19'는 전염성이 높습니다. \r\n환자들이 병원 방문을 회피합니다.\r\n(외래 진료 30%↓)");
    //    DecreaseOutpatientVisitRate(30);  //외래 진료 30% 감소 기능 처리
    //}

    //// 외래 진료율 감소 처리 함수
    //private void DecreaseOutpatientVisitRate(int percentage)
    //{
    //    Debug.Log($"외래 진료율이 {percentage}% 감소되었습니다.");  //통합 시 삭제
    //    // 외래 환자 최대 수를 30% 줄이기
    //    //Managers.ObjectPooling.maxOfOutpatient = Mathf.FloorToInt(originalMaxOutpatients * (1 - percentage / 100f));  (통합 시 주석 해제)
    //    //Debug.Log($"외래 환자 수가 {percentage}% 감소되었습니다. 새로운 최대 환자 수: {Managers.ObjectPooling.maxOfOutpatient}");  (통합 시 주석 해제)
    //}

    //// 질병청 선고 완료 여부 반환
    //public bool IsGovernmentComplete() => isGovernmentComplete;

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
