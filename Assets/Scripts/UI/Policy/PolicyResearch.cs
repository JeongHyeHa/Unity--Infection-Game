using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Linq;

public class PolicyResearch : MonoBehaviour
{
    public GameObject researchTabButton_1;
    public GameObject researchTabButton_2;
    public GameObject researchTabButton_3;

    public GameObject researchTab_1;
    public GameObject researchTab_2;
    public GameObject researchTab_3;

    public Image researchLeftButton;
    public Image researchRightButton;

    public GameObject notResearchedTab;
    public Button researchStartButton;
    public TextMeshProUGUI researchingTimeText;
    public GameObject covidInfoTab;
    public GameObject CREInfoTab;
    public TextMeshProUGUI selectedLevel;
    public GameObject medicineLockPanel;
    public TextMeshProUGUI medicineTimeText;
    public GameObject vaccineLockPanel;
    public TextMeshProUGUI vaccineTimeText;
    public GameObject researchComplete;



    // 치료제 관련 변수
    public GameObject medicinePrefab; // 스크롤뷰에 넣을 프리팹
    public Transform medicineScrollViewContent; // 스크롤뷰의 Content 객체
    public GameObject medicineUsePanel; // 치료제 사용 패널
    public Image medicinePlusButton;    // 개수 +1 버튼
    public Image medicineMinusButton;   // 개수 -1 버튼
    public TextMeshProUGUI medicineCountText; // 개수 나타내는 텍스트
    public Button setMediCountZero;     // 설정 개수 0으로 변경
    public Button setMediCountTen;      // 설정 개수 10개 추가
    public Button setMediCountMax;      // 최대 개수로 설정
    public Button countDicisionButton;  // 결정버튼
    public Button usePanelCloseButton;

    private int medicineCount = 0; // 현재 설정된 치료제 사용 개수
    public TextMeshProUGUI medicineTotalPriceText; // 사용할 치료제만큼 얼마 사용할지 계산
    private Ward medSelectedWard; // 현재 선택된 병동 - 치료제 탭

    public GameObject vaccinePrefab;
    public Transform vaccineScrollViewContent;
    public GameObject vaccineUsePanel;
    public Image vaccinePlusButton;
    public Image vaccineMinusButton;
    public TextMeshProUGUI vaccineCountText;
    public Button setVaccineCountZero;
    public Button setVaccineCountTen;
    public Button setVaccineCountMax;
    public Button vaccineUseDicisionButton;
    public Button vaccineUsePanelClose;

    private int vaccineCount = 0;
    private Ward vaccineWard;

    private const int medicinePrice = 200; // 치료제 한 개당 가격
    private const int vaccinePrice = 80; // 백신 한 개당 가격

    private Coroutine researchCoroutine;


    private int currentTabIndex = 1; // 현재 탭 인덱스 (1번부터 시작)

    NewsController newsController;

    private string th_virus;            // 현 게임 바이러스 이름

    void Awake()
    {
        newsController = FindObjectOfType<NewsController>();

        researchTabButton_1 = Assign(researchTabButton_1, "ResearchTabButton_1");
        researchTabButton_2 = Assign(researchTabButton_2, "ResearchTabButton_2");
        researchTabButton_3 = Assign(researchTabButton_3, "ResearchTabButton_3");

        researchTab_1 = Assign(researchTab_1, "ResearchTab_1");
        researchTab_2 = Assign(researchTab_2, "ResearchTab_2");
        researchTab_3 = Assign(researchTab_3, "ResearchTab_3");

        notResearchedTab = Assign(notResearchedTab, "NotResearchedTab");
        researchStartButton = Assign(researchStartButton, "ResearchStartButton");
        researchingTimeText = Assign(researchingTimeText, "ResearchingTimeText");
        covidInfoTab = Assign(covidInfoTab, "COVIDInfoTab");
        CREInfoTab = Assign(CREInfoTab, "CREInfoTab");
        selectedLevel = Assign(selectedLevel, "SelectedLevel");
        medicineLockPanel = Assign(medicineLockPanel, "MedicineLockPanel");
        medicineTimeText = Assign(medicineTimeText, "MedicineTimeText");
        vaccineLockPanel = Assign(vaccineLockPanel, "VaccineLockPanel");
        vaccineTimeText = Assign(vaccineTimeText, "VaccineTimeText");
        researchComplete = Assign(researchComplete, "ResearchComplete");

        researchLeftButton = Assign(researchLeftButton, "ResearchLeftButton");
        researchRightButton = Assign(researchRightButton, "ResearchRightButton");

        medicineUsePanel = Assign(medicineUsePanel, "MedicineUsePanel");
        medicinePlusButton = Assign(medicinePlusButton, "MedicinePlusButton");
        medicineMinusButton = Assign(medicineMinusButton, "MedicineMinusButton");
        medicineCountText = Assign(medicineCountText, "MedicineCount");
        setMediCountZero = Assign(setMediCountZero, "SetMediCountZero");
        setMediCountTen = Assign(setMediCountTen, "SetMediCountTen");
        setMediCountMax = Assign(setMediCountMax, "SetMediCountMax");
        countDicisionButton = Assign(countDicisionButton, "CountDicisionButton");
        usePanelCloseButton = Assign(usePanelCloseButton, "UsePanelCloseButton");

        vaccineUsePanel = Assign(vaccineUsePanel, "VaccineUsePanel");
        vaccinePlusButton = Assign(vaccinePlusButton, "VaccinePlusButton");
        vaccineMinusButton = Assign(vaccineMinusButton, "VaccineMinusButton");
        vaccineCountText = Assign(vaccineCountText, "VaccineCount");
        setVaccineCountZero = Assign(setVaccineCountZero, "SetVaccineCountZero");
        setVaccineCountTen = Assign(setVaccineCountTen, "SetVaccineCountTen");
        setVaccineCountMax = Assign(setVaccineCountMax, "SetVaccineCountMax");
        vaccineUseDicisionButton = Assign(vaccineUseDicisionButton, "VaccineUseDicisionButton");
        vaccineUsePanelClose = Assign(vaccineUsePanelClose, "VaccineUsePanelClose");


        // Add SetMedicineCount button listeners
        setMediCountZero.onClick.AddListener(() => { SetMedicineCount(0); BtnSoundManager.Instance.PlayButtonSound(); });
        setMediCountTen.onClick.AddListener(() => { SetMedicineCount(medicineCount + 10); BtnSoundManager.Instance.PlayButtonSound(); });
        setMediCountMax.onClick.AddListener(() => { SetMedicineCount(GetCurrentWardInfectedCount()); BtnSoundManager.Instance.PlayButtonSound(); });
        countDicisionButton.onClick.AddListener(() => { ApplyMedicine(); BtnSoundManager.Instance.PlayButtonSound(); });
        usePanelCloseButton.onClick.AddListener(() => { CloseMedicineUsePanelWithoutSaving(); BtnSoundManager.Instance.PlayButtonSound(); });

        setVaccineCountZero.onClick.AddListener(() => { UpdateVaccineCount(0); BtnSoundManager.Instance.PlayButtonSound(); });
        setVaccineCountTen.onClick.AddListener(() => { UpdateVaccineCount(vaccineCount + 10); BtnSoundManager.Instance.PlayButtonSound(); });
        setVaccineCountMax.onClick.AddListener(() => { UpdateVaccineCount(GetCurrentWardUninfectedCount()); BtnSoundManager.Instance.PlayButtonSound(); });
        vaccineUseDicisionButton.onClick.AddListener(() => { ApplyVaccine(); BtnSoundManager.Instance.PlayButtonSound(); });
        vaccineUsePanelClose.onClick.AddListener(() => { CloseVaccineUsePanelWithoutSaving(); BtnSoundManager.Instance.PlayButtonSound(); });

        // 이벤트 트리거 추가
        AddEventTrigger(medicinePlusButton.gameObject, EventTriggerType.PointerClick, (data) => { UpdateMedicineCount(1); BtnSoundManager.Instance.PlayButtonSound(); });
        AddEventTrigger(medicineMinusButton.gameObject, EventTriggerType.PointerClick, (data) => { UpdateMedicineCount(-1); BtnSoundManager.Instance.PlayButtonSound(); });

        AddEventTrigger(vaccinePlusButton.gameObject, EventTriggerType.PointerClick, (data) => { UpdateVaccineCount(1); BtnSoundManager.Instance.PlayButtonSound(); });
        AddEventTrigger(vaccineMinusButton.gameObject, EventTriggerType.PointerClick, (data) => { UpdateVaccineCount(-1); BtnSoundManager.Instance.PlayButtonSound(); });

        AddEventTrigger(researchLeftButton.gameObject, EventTriggerType.PointerClick, (data) => { ChangeResearch("Left"); BtnSoundManager.Instance.PlayButtonSound(); });
        AddEventTrigger(researchRightButton.gameObject, EventTriggerType.PointerClick, (data) => { ChangeResearch("Right"); BtnSoundManager.Instance.PlayButtonSound(); });

        researchStartButton.onClick.AddListener(() => { StartResearch(); BtnSoundManager.Instance.PlayButtonSound(); MoneyManager.Instance.ResearchExpense(); });
    }

    // 첫 감염자 발생 시 연구 시작 가능
    public void FirstInfectedAppear()
    {
        researchStartButton.gameObject.SetActive(true);
    }

    // 연구 탭 버튼 클릭
    private void StartResearch()
    {
        // 연구 버튼에 대한 데이터 저장
        ResearchDBManager.Instance.AddResearchData(ResearchDBManager.ResearchMode.research, 1, 0, 1);

        // 연구 시작 버튼 클릭 시 코루틴 시작
        if (researchCoroutine != null)
        {
            StopCoroutine(researchCoroutine);
        }

        // 연구 시작 시 텍스트 초기화
        researchingTimeText.gameObject.SetActive(true);
        researchStartButton.gameObject.SetActive(false);
        researchCoroutine = StartCoroutine(ResearchCoroutine());
    }
    private IEnumerator ResearchCoroutine()
    {
        int remainingTime = 60; // 연구 시간 10초
        while (remainingTime > 0)
        {
            researchingTimeText.text = $"병원체 연구 중입니다...\n남은 연구 시간 : {remainingTime}초";
            yield return YieldInstructionCache.WaitForSecondsRealtime(1f);
            remainingTime--;
        }

        // 연구 완료 시 notResearchedTab 비활성화

        if (notResearchedTab != null)
            notResearchedTab.SetActive(false);

        researchingTimeText.gameObject.SetActive(false); // 텍스트 비활성화

        // selectedLevel에 따른 CREInfoTab과 CovidInfoTab 상태 설정
        string level = selectedLevel.text;
        if (level == "Easy")
        {
            CREInfoTab.SetActive(true);
            covidInfoTab.SetActive(false);
            th_virus = "CRE";
            newsController.TriggerPathogenResearchCompleteNews(th_virus);
        }
        else if (level == "Normal")
        {
            CREInfoTab.SetActive(false);
            covidInfoTab.SetActive(true);
            th_virus = "COVID-19";
            newsController.TriggerPathogenResearchCompleteNews(th_virus);
        }
        else if (level == "Hard")
        {
            CREInfoTab.SetActive(false);
            covidInfoTab.SetActive(true);
            th_virus = "COVID-19";
            newsController.TriggerPathogenResearchCompleteNews(th_virus);
        }

        // 연구 완료 알림 활성화
        if (researchComplete != null)
        {
            researchComplete.SetActive(true);
            StartCoroutine(DeactivateResearchCompleteAfterDelay());
        }

        StartCoroutine(UnlockMedicine());
    }

    private IEnumerator UnlockMedicine()
    {
        int remainingTime = 150;
        while (remainingTime > 0)
        {
            medicineTimeText.text = $"치료제 연구 중입니다...\n남은 연구 시간 : {remainingTime}초";
            yield return YieldInstructionCache.WaitForSecondsRealtime(1f);
            remainingTime--;
        }
        if (medicineLockPanel != null)
        {
            medicineLockPanel.SetActive(false); // 치료제 패널 해금
            newsController.TriggerCureResearchCompleteNews(th_virus);
            if (medicineLockPanel != null)
                medicineLockPanel.SetActive(false);
        }
        StartCoroutine(UnlockVaccine());
    }

    private IEnumerator UnlockVaccine()
    {

        int remainingTime = 300;
        while (remainingTime > 0)
        {
            vaccineTimeText.text = $"백신 연구 중입니다...\n남은 연구 시간 : {remainingTime}초";
            yield return YieldInstructionCache.WaitForSecondsRealtime(1f);
            remainingTime--;
        }
        if (vaccineLockPanel != null)
        {
            vaccineLockPanel.SetActive(false); // 백신 패널 해금
            newsController.TriggerVaccineResearchCompleteNews(th_virus);
            if (vaccineLockPanel != null)
                vaccineLockPanel.SetActive(false);
        }

        UpdateTabUI();
    }

    private IEnumerator DeactivateResearchCompleteAfterDelay()
    {
        float elapsedTime = 0f;
        while (elapsedTime < 5f)
        {
            researchComplete.SetActive(true);
            yield return YieldInstructionCache.WaitForSecondsRealtime(1f);
            elapsedTime += 1f;
        }

        researchComplete.SetActive(false); // 5초 후 비활성화
    }

    //
    // 치료제
    //
    // 치료제 사용창 오픈
    private void OpenMedicineUsePanel(Ward ward)
    {
        medSelectedWard = ward;
        medicineUsePanel.SetActive(true);
        medicineCount = 0;
        UpdateMedicineCountUI();
    }

    // 치료제 사용창 취소 로직
    private void CloseMedicineUsePanelWithoutSaving()
    {
        medicineUsePanel.SetActive(false);
    }

    // 치료제 사용량 설정값 조정
    private void UpdateMedicineCount(int change)
    {
        medicineCount += change;
        medicineCount = Mathf.Clamp(medicineCount, 0, GetCurrentWardInfectedCount());
        UpdateMedicineCountUI();
    }

    // 치료제 탭 UI 중 치료제 사용량 조절
    private void UpdateMedicineCountUI()
    {
        // 텍스트와 개수 설정
        medicineCountText.text = medicineCount.ToString();
        medicineTotalPriceText.text = $"{medicineCount}개 사용 : {medicineCount * 200} Sch";

        // 플러스, 마이너스 버튼의 활성화/비활성화 설정
        bool canIncrement = medicineCount < GetCurrentWardInfectedCount();
        bool canDecrement = medicineCount > 0;

        // 플러스 버튼
        medicinePlusButton.color = canIncrement ? Color.gray : new Color(1, 1, 1, 0.5f);
        medicinePlusButton.raycastTarget = canIncrement;

        // 마이너스 버튼
        medicineMinusButton.color = canDecrement ? Color.gray : new Color(1, 1, 1, 0.5f);
        medicineMinusButton.raycastTarget = canDecrement;
    }

    // 감염환자 수 계산 (치료제)
    private int GetCurrentWardInfectedCount()
    {
        return medSelectedWard != null ? medSelectedWard.inpatients.Count(p => p.infectionController.isInfected && p.isInCurrentWard && p.currentWard == medSelectedWard.WardName)
            + medSelectedWard.doctors.Count(p => p.infectionController.isInfected && p.isInCurrentWard && p.currentWard == medSelectedWard.WardName)
            + medSelectedWard.nurses.Count(p => p.infectionController.isInfected && p.isInCurrentWard && p.currentWard == medSelectedWard.WardName)
            + medSelectedWard.quarantinedPatients.Count(q => q.infectionController.isInfected && q.isInCurrentWard && q.currentWard == medSelectedWard.WardName)
            : 0;
    }

    // 치료제 사용 시 최대 최소값 조정
    private void SetMedicineCount(int count)
    {
        medicineCount = Mathf.Clamp(count, 0, GetCurrentWardInfectedCount());
        UpdateMedicineCountUI();
    }

    // 치료제 사용 로직 (치료제 사용 결정 버튼 클릭 시 실행)
    private void ApplyMedicine()
    {
        if (medSelectedWard == null || medicineCount == 0) return;

        int totalCost = medicineCount * medicinePrice;

        // 재화 확인 및 차감
        if (!MoneyManager.Instance.DeductMoney(totalCost))
        {
            Debug.LogWarning("Not enough money to use the selected amount of medicine.");
            return;
        }

        int remainingCount = medicineCount;

        // 감염된 환자 수 만큼 치료제를 사용
        foreach (var patient in medSelectedWard.inpatients.Where(p => p.infectionController.isInfected && p.isInCurrentWard && p.currentWard == medSelectedWard.WardName))
        {
            if (remainingCount <= 0) break;

            patient.personComponent.Recover();
            remainingCount--;
        }
        foreach (var patient in medSelectedWard.quarantinedPatients.Where(p => p.infectionController.isInfected && p.isInCurrentWard && p.currentWard == medSelectedWard.WardName))
        {
            if (remainingCount <= 0) break;

            patient.personComponent.Recover();
            remainingCount--;
        }
        foreach (var doctor in medSelectedWard.doctors.Where(p => p.infectionController.isInfected && p.isInCurrentWard && p.currentWard == medSelectedWard.WardName))
        {
            if (remainingCount <= 0) break;

            doctor.personComponent.Recover();
            remainingCount--;
        }

        foreach (var nurse in medSelectedWard.nurses.Where(p => p.infectionController.isInfected && p.isInCurrentWard && p.currentWard == medSelectedWard.WardName))
        {
            if (remainingCount <= 0) break;

            nurse.personComponent.Recover();
            remainingCount--;
        }


        // 연구 버튼에 대한 데이터 저장
        ResearchDBManager.Instance.AddResearchData(ResearchDBManager.ResearchMode.research, 3, medSelectedWard.num + 1, medicineCount);

        medicineCount = 0; // 사용한 후 설정된 사용량 초기화
        medicineUsePanel.SetActive(false);
        UpdateMedicineCountUI();
        UpdateTabUI();
        PopulateMedicineWard();
        Debug.Log($"Medicine applied. Total cost: {totalCost}");
    }


    //
    // 백신
    //
    // 백신 사용창 취소 버튼 로직
    private void CloseVaccineUsePanelWithoutSaving()
    {
        vaccineUsePanel.SetActive(false);
    }

    private void OpenVaccineUsePanel(Ward ward)
    {
        vaccineWard = ward;
        vaccineUsePanel.SetActive(true);

        // 초기 vaccineCount를 1로 설정
        vaccineCount = 0;
        UpdateVaccineCountUI();
    }

    private int GetAffordableVaccineCount()
    {
        int currentMoney = MoneyManager.Instance.currentMoneyManager.CurrentMoneyGetter;
        return currentMoney / vaccinePrice;
    }

    private void UpdateVaccineCountUI()
    {
        vaccineCountText.text = vaccineCount.ToString();

        int eligibleNPCCount = GetCurrentWardUninfectedCount();

        // Adjust button states
        bool canIncrement = vaccineCount < Mathf.Min(eligibleNPCCount, GetAffordableVaccineCount());
        bool canDecrement = vaccineCount > 0;

        vaccinePlusButton.color = canIncrement ? Color.gray : new Color(1, 1, 1, 0.5f);
        vaccinePlusButton.raycastTarget = canIncrement;

        vaccineMinusButton.color = canDecrement ? Color.gray : new Color(1, 1, 1, 0.5f);
        vaccineMinusButton.raycastTarget = canDecrement;
    }

    // 현재 병동 내 감염되지 않은 인원 수 계산 메서드
    private int GetCurrentWardUninfectedCount()
    {
        if (vaccineWard == null) return 0;

        int unInfectedCount = vaccineWard.doctors.Count(d => !d.infectionController.isInfected && d.personComponent.vaccineResist < 40 && d.isInCurrentWard && d.currentWard == vaccineWard.WardName) +
                              vaccineWard.nurses.Count(n => !n.infectionController.isInfected && n.personComponent.vaccineResist < 40 && n.isInCurrentWard && n.currentWard == vaccineWard.WardName) +
                              vaccineWard.outpatients.Count(o => !o.infectionController.isInfected && o.personComponent.vaccineResist < 40 && o.isInCurrentWard && o.currentWard == vaccineWard.WardName) +
                              vaccineWard.inpatients.Count(i => !i.infectionController.isInfected && i.personComponent.vaccineResist < 40 && i.isInCurrentWard && i.currentWard == vaccineWard.WardName) +
                              vaccineWard.emergencyPatients.Count(e => !e.infectionController.isInfected && e.personComponent.vaccineResist < 40 && e.isInCurrentWard && e.currentWard == vaccineWard.WardName)
                              + vaccineWard.icuPatients.Count(c => !c.infectionController.isInfected && c.personComponent.vaccineResist < 40 && c.isInCurrentWard && c.currentWard == vaccineWard.WardName) +
                              vaccineWard.quarantinedPatients.Count(c => !c.infectionController.isInfected && c.personComponent.vaccineResist < 40 && c.isInCurrentWard && c.currentWard == vaccineWard.WardName)
                              ;


        return unInfectedCount;
    }


    // 백신 개수 설정 메서드
    private void UpdateVaccineCount(int change)
    {
        int uninfectedNPCCount = GetCurrentWardUninfectedCount();
        vaccineCount += change;
        vaccineCount = Mathf.Clamp(vaccineCount, 0, uninfectedNPCCount);
        UpdateVaccineCountUI();
    }


    // 백신 사용 시 로직
    private void ApplyVaccine()
    {
        if (vaccineWard == null || vaccineCount == 0) return;

        int resistanceIncrease = 40; // 저항 증가값

        // 재화 확인 및 차감
        //if (!MoneyManager.Instance.DeductMoney(vaccineCount * MoneyManager.VaccinePrice))
        //{
            //Debug.LogWarning("Not enough money for vaccines.");
            //return;
        //}

        //int remainingCount = vaccineCount;

        foreach (var npc in vaccineWard.doctors.Concat<NPCController>(vaccineWard.inpatients)
                                               .Concat(vaccineWard.nurses)
                                               .Concat(vaccineWard.outpatients)
                                               .Concat(vaccineWard.emergencyPatients)
                                               .Concat(vaccineWard.icuPatients)
                                               .Concat(vaccineWard.quarantinedPatients)
                                               )
        {
            //if (remainingCount <= 0) break;

            // 감염되지 않은 NPC에게만 백신 적용
            if (!npc.infectionController.isInfected && npc.personComponent.vaccineResist < 40)
            {
                npc.personComponent.vaccineResist = resistanceIncrease;
                npc.personComponent.UpdateInfectionResistance();
                //remainingCount--;
            }
        }

        // 연구 버튼에 대한 데이터 저장
        //Debug.Log($"researchDB TEST: {vaccineWard.num+1}, {vaccineCount}");
        ResearchDBManager.Instance.AddResearchData(ResearchDBManager.ResearchMode.research, 2, vaccineWard.num + 1, vaccineCount);

        vaccineCount = 0;
        UpdateVaccineCountUI();
        UpdateTabUI();
        vaccineUsePanel.SetActive(false);
        PopulateVaccineWard();
    }




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


    // 정책 창 오픈 함수 (기본적으로 1번 탭으로 초기화)
    public void OpenResearchPanel()
    {
        currentTabIndex = 1; // 기본 탭 인덱스 설정
        UpdateTabUI();       // 1번 탭을 활성화하고 UI 업데이트
    }

    private void ChangeResearch(string direction)
    {
        if (direction == "Left" && currentTabIndex > 1)
        {
            currentTabIndex--;
        }
        else if (direction == "Right" && currentTabIndex < 3)
        {
            currentTabIndex++;
        }

        UpdateTabUI();
    }

    private void PopulateMedicineWard()
    {
        // 스크롤뷰 초기화
        foreach (Transform child in medicineScrollViewContent)
        {
            Destroy(child.gameObject);
        }

        var sortedWards = Ward.wards.Where(w => w.num >= 4 && w.num <= 7)
            .OrderByDescending(w => w.inpatients.Count(p => p.infectionController.isInfected && p.isInCurrentWard && p.currentWard == w.WardName)
            + w.quarantinedPatients.Count(q => q.infectionController.isInfected && q.isInCurrentWard && q.currentWard == w.WardName)).ToList(); ;

        foreach (var ward in sortedWards) // 입원병동 1 ~ 입원병동 4에 해당
        {
            GameObject wardItem = Instantiate(medicinePrefab, medicineScrollViewContent);

            // 각 텍스트 요소에 값을 할당
            TextMeshProUGUI wardInPatientCount = wardItem.transform.Find("WardInPatientCount").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI wardInfectedPatientCount = wardItem.transform.Find("WardInfectedPatientCount").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI researchWardName = wardItem.transform.Find("ResearchWardName").GetComponent<TextMeshProUGUI>();

            int inpatientCount = ward.inpatientCount + ward.outpatientCount +ward.doctorCount + ward.nurseCount;
            int infectedInpatientCount = ward.inpatients.Count(i => i.infectionController.isInfected && i.isInCurrentWard && i.currentWard == ward.WardName)
                + ward.doctors.Count(i => i.infectionController.isInfected && i.isInCurrentWard && i.currentWard == ward.WardName)
                + ward.nurses.Count(i => i.infectionController.isInfected && i.isInCurrentWard && i.currentWard == ward.WardName)
                + ward.quarantinedPatients.Count(q => q.infectionController && q.isInCurrentWard && q.currentWard == ward.WardName);
            int uninfectedInpatientCount = ward.inpatientCount - ward.inpatients.Count(p => p.infectionController.isInfected && p.isInCurrentWard && p.currentWard == ward.WardName)
                - ward.doctors.Count(i => i.infectionController.isInfected && i.isInCurrentWard && i.currentWard == ward.WardName)
                - ward.nurses.Count(i => i.infectionController.isInfected && i.isInCurrentWard && i.currentWard == ward.WardName)
                - ward.quarantinedPatients.Count(q => q.infectionController && q.isInCurrentWard && q.currentWard == ward.WardName);

            wardInPatientCount.text = inpatientCount.ToString();
            wardInfectedPatientCount.text = infectedInpatientCount.ToString();
            researchWardName.text = ward.WardName;

            // WardButton 클릭 시 MedicineUsePanel 열기
            Button wardButton = wardItem.transform.Find("WardButton").GetComponent<Button>();
            wardButton.onClick.AddListener(() => { OpenMedicineUsePanel(ward); BtnSoundManager.Instance.PlayButtonSound(); });
        }
    }

    private void PopulateVaccineWard()
    {
        foreach (Transform child in vaccineScrollViewContent)
        {
            Destroy(child.gameObject);
        }
        var sortedWards = Ward.wards.Where(w => w.num >= 0 && w.num <= 9)
            .OrderByDescending(w => w.doctors.Count(d => d.infectionController.isInfected && d.isInCurrentWard && d.currentWard == w.WardName) +
                w.nurses.Count(n => n.infectionController.isInfected && n.isInCurrentWard && n.currentWard == w.WardName) +
                w.outpatients.Count(o => o.infectionController.isInfected && o.isInCurrentWard && o.currentWard == w.WardName) +
                w.inpatients.Count(i => i.infectionController.isInfected && i.isInCurrentWard && i.currentWard == w.WardName) +
                w.emergencyPatients.Count(e => e.infectionController.isInfected && e.isInCurrentWard && e.currentWard == w.WardName) +
                w.icuPatients.Count(c => c.infectionController.isInfected && c.isInCurrentWard && c.currentWard == w.WardName)+
                w.quarantinedPatients.Count(c => c.infectionController.isInfected && c.isInCurrentWard && c.currentWard == w.WardName))
                    .ToList(); ;

        foreach (var ward in sortedWards)
        {
            GameObject wardItem = Instantiate(vaccinePrefab, vaccineScrollViewContent);

            TextMeshProUGUI wardInPatientCount = wardItem.transform.Find("VaccineWardPatient").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI wardInfectedPatientCount = wardItem.transform.Find("VaccineWardInfectedPatient").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI researchWardName = wardItem.transform.Find("VaccineWardName").GetComponent<TextMeshProUGUI>();
            int allCount = ward.doctorCount + ward.nurseCount + ward.emergencypatientCount + ward.outpatientCount + ward.inpatientCount + ward.icupatientCount;
            int infectedCount = ward.doctors.Count(d => d.infectionController.isInfected && d.isInCurrentWard && d.currentWard == ward.WardName) +
                ward.nurses.Count(n => n.infectionController.isInfected && n.isInCurrentWard && n.currentWard == ward.WardName) +
                ward.outpatients.Count(o => o.infectionController.isInfected && o.isInCurrentWard && o.currentWard == ward.WardName) +
                ward.inpatients.Count(i => i.infectionController.isInfected && i.isInCurrentWard && i.currentWard == ward.WardName) +
                ward.emergencyPatients.Count(e => e.infectionController.isInfected && e.isInCurrentWard && e.currentWard == ward.WardName) +
                ward.icuPatients.Count(c => c.infectionController.isInfected && c.isInCurrentWard && c.currentWard == ward.WardName) +
                ward.quarantinedPatients.Count(c => c.infectionController.isInfected && c.isInCurrentWard && c.currentWard == ward.WardName);
            int unInfectedCount = ward.doctors.Count(d => !d.infectionController.isInfected && d.isInCurrentWard && d.currentWard == ward.WardName) +
                ward.nurses.Count(n => !n.infectionController.isInfected && n.isInCurrentWard && n.currentWard == ward.WardName) +
                ward.outpatients.Count(o => !o.infectionController.isInfected && o.isInCurrentWard && o.currentWard == ward.WardName) +
                ward.inpatients.Count(i => !i.infectionController.isInfected && i.isInCurrentWard && i.currentWard == ward.WardName) +
                ward.emergencyPatients.Count(e => !e.infectionController.isInfected && e.isInCurrentWard && e.currentWard == ward.WardName) +
                ward.icuPatients.Count(c => !c.infectionController.isInfected && c.isInCurrentWard && c.currentWard == ward.WardName) +
                ward.quarantinedPatients.Count(c => !c.infectionController.isInfected && c.isInCurrentWard && c.currentWard == ward.WardName);
            wardInPatientCount.text = allCount.ToString();
            wardInfectedPatientCount.text = infectedCount.ToString();
            researchWardName.text = ward.WardName;
            Debug.Log($"{ward.WardName} 에서 병동 인원 수 : {infectedCount}, 감염 인원 수 : {unInfectedCount}(비감염 : {unInfectedCount}) 로 측정되었습니다.");

            Button wardButton = wardItem.transform.Find("VaccineWardButton").GetComponent<Button>();
            wardButton.onClick.AddListener(() => OpenVaccineUsePanel(ward));
        }
    }

    private void UpdateTabUI()
    {
        // 현재 선택된 탭만 활성화하고, 나머지 탭 비활성화
        researchTab_1.SetActive(currentTabIndex == 1);
        researchTab_2.SetActive(currentTabIndex == 2);
        researchTab_3.SetActive(currentTabIndex == 3);

        // 탭 버튼 색상 변경
        Color activeColor = new Color32(187, 198, 212, 255); // 선택된 탭 색상 (#BBC6D4)
        Color inactiveColor = new Color32(235, 239, 243, 255); // 선택되지 않은 탭 색상 (#EBEFF3)

        researchTabButton_1.GetComponent<Image>().color = currentTabIndex == 1 ? activeColor : inactiveColor;
        researchTabButton_2.GetComponent<Image>().color = currentTabIndex == 2 ? activeColor : inactiveColor;
        researchTabButton_3.GetComponent<Image>().color = currentTabIndex == 3 ? activeColor : inactiveColor;

        // 왼쪽 및 오른쪽 버튼 활성화/비활성화
        researchLeftButton.gameObject.SetActive(currentTabIndex > 1);
        researchRightButton.gameObject.SetActive(currentTabIndex < 3);

        // MedicineWard 스크롤뷰 업데이트 - 탭이 2번일 때만 호출
        if (currentTabIndex == 2)
        {
            PopulateMedicineWard();
        }
        if (currentTabIndex == 3)
        {
            PopulateVaccineWard();
        }
    }


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
