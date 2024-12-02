using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class PolicyWard : MonoBehaviour
{
    public static PolicyWard Instance { get; private set; }
    public TMP_Dropdown wardDropdown;
    public TextMeshProUGUI wardNameText;

    public TextMeshProUGUI doctorCountText, doctorInfCountText;
    public TextMeshProUGUI nurseCountText, nurseInfCountText;
    public TextMeshProUGUI outpatientCountText, outpatientInfCountText;
    public TextMeshProUGUI inpatientCountText, inpatientInfCountText;
    public TextMeshProUGUI emergencypatientCountText, emergencypatientInfCountText;
    public TextMeshProUGUI icupatientCountText, icupatientInfCountText;

    public Image doctorBack, nurseBack, outpatientBack, inpatientBack, emergencypatientBack, icupatientBack;

    public GameObject infoPanel;        // 병동 관리 탭 위 설명표
    public Button quarantineWardButton; // 격리 병동 전환 버튼
    public Button closeWardButton;      // 폐쇄 병동 전환 버튼
    public Button normalWardButton;     // 일반 병동 전환 버튼
    public Button disInfectWardButton;  // 해당 병동 소독 버튼
    public TextMeshProUGUI disInfectButtonText; // 소독 버튼의 카운트다운 텍스트

    public Button qtStartButton_1;
    public Button qtStartButton_2;
    public Button qtStartButton_3;

    public GameObject qtOutline_1;
    public GameObject qtOutline_2;
    public GameObject qtOutline_3;

    public Ward selectWard;

    public bool isQuarantineLevel_1 = false; // 격리 1단계 활성화 여부
    public bool isQuarantineLevel_2 = false; // 격리 2단계 활성화 여부
    public bool isQuarantineLevel_3 = false; // 격리 3단계 활성화 여부

    private Dictionary<int, WardState> wardStates = new Dictionary<int, WardState>(); // 병동별 상태 저장
    public const float disinfectCooldownTime = 30f;

    public string[] wardNames = {
        "내과 1", "내과 2",
        "외과 1", "외과 2",
        "입원병동 1", "입원병동 2",
        "입원병동 3", "입원병동 4",
        "응급실", "중환자실/격리실"
    };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        InitializeDropdown();
        InitializeWardStates();
        wardDropdown.value = 0;
        wardDropdown.onValueChanged.AddListener(UpdateWardInfomation);

        quarantineWardButton.onClick.AddListener(ChangeWardToQuarantine);
        closeWardButton.onClick.AddListener(ChangeWardToClose);
        normalWardButton.onClick.AddListener(ChangeWardToOpen);
        disInfectWardButton.onClick.AddListener(DisinfectWard);
        qtStartButton_1.onClick.AddListener(GoLevel1);
        qtStartButton_2.onClick.AddListener(GoLevel2);
        qtStartButton_3.onClick.AddListener(GoLevel3);
    }

    private void Update()
    {
        CheckIsolationStatus();
        UpdateDisinfectCooldowns();
    }

    private void InitializeDropdown()
    {
        wardDropdown.ClearOptions();
        wardDropdown.AddOptions(new List<string>(wardNames));
    }

    private void InitializeWardStates()
    {
        for (int i = 0; i < wardNames.Length; i++)
        {
            wardStates[i] = new WardState
            {
                IsQuarantined = false,
                IsClosed = false,
                IsDisinfecting = false,
                DisinfectEndTime = 0f
            };
        }
    }

    public void UpdateWardInfomation(int index)
    {
        Ward currentWard = Ward.wards.Find(w => w.num == index);
        selectWard = currentWard;

        if (currentWard == null)
        {
            Debug.LogError($"병동 데이터를 찾을 수 없습니다. Index: {index}");
            return;
        }

        wardNameText.text = currentWard.WardName;

        WardState state = wardStates[index];

        // 병동 이름이 "입원병동"으로 시작하지 않는 경우 버튼 비활성화
        bool isInpatientWard = currentWard.WardName.StartsWith("입원병동");

        quarantineWardButton.gameObject.SetActive(isInpatientWard);
        closeWardButton.gameObject.SetActive(isInpatientWard);
        normalWardButton.gameObject.SetActive(isInpatientWard);
        disInfectWardButton.gameObject.SetActive(isInpatientWard);

        if (isInpatientWard)
        {
            // 병동 상태에 따라 버튼 활성화/비활성화
            if (state.IsQuarantined)
            {
                quarantineWardButton.gameObject.SetActive(false);
                closeWardButton.gameObject.SetActive(false);
                normalWardButton.gameObject.SetActive(true);
                disInfectWardButton.gameObject.SetActive(false);
            }
            else if (state.IsClosed)
            {
                quarantineWardButton.gameObject.SetActive(false);
                closeWardButton.gameObject.SetActive(false);
                normalWardButton.gameObject.SetActive(true);
                disInfectWardButton.interactable = !state.IsDisinfecting;
                normalWardButton.interactable = !state.IsDisinfecting;
            }
            else if (state.IsDisinfecting)
            {
                quarantineWardButton.gameObject.SetActive(true);
                closeWardButton.gameObject.SetActive(true);
                normalWardButton.gameObject.SetActive(false);
                disInfectWardButton.interactable = false;
                float remainingTime = state.DisinfectEndTime - Time.time;
                if (PolicyQuizManager.isCorrect)
                {
                    disInfectButtonText.text = $"소독 중: {Mathf.CeilToInt(remainingTime)}초 남음";
                }
                else
                {
                    disInfectButtonText.text = $"소독 재시도까지 {Mathf.CeilToInt(remainingTime)}초 남음";
                }
                
                
            }
            else
            {
                quarantineWardButton.gameObject.SetActive(true);
                closeWardButton.gameObject.SetActive(true);
                normalWardButton.gameObject.SetActive(false);
                disInfectWardButton.gameObject.SetActive(false);
                disInfectWardButton.interactable = true;
                normalWardButton.interactable = true;
                disInfectButtonText.text = "병동 소독 실시";
            }
        }
        else
        {
            quarantineWardButton.gameObject.SetActive(false);
            closeWardButton.gameObject.SetActive(false);
            normalWardButton.gameObject.SetActive(false);
            disInfectWardButton.gameObject.SetActive(false);

            disInfectButtonText.text = "";
        }

        // 인원 수 및 감염 수 업데이트
        UpdateCountText(currentWard.doctors, doctorCountText, doctorInfCountText, doctorBack, currentWard.WardName);
        UpdateCountText(currentWard.nurses, nurseCountText, nurseInfCountText, nurseBack, currentWard.WardName);
        UpdateCountText(currentWard.outpatients, outpatientCountText, outpatientInfCountText, outpatientBack, currentWard.WardName);
        UpdateCountText(currentWard.inpatients, inpatientCountText, inpatientInfCountText, inpatientBack, currentWard.WardName);
        UpdateCountText(currentWard.emergencyPatients, emergencypatientCountText, emergencypatientInfCountText, emergencypatientBack, currentWard.WardName);
        UpdateCountText(currentWard.icuPatients, icupatientCountText, icupatientInfCountText, icupatientBack, currentWard.WardName);
    }



    public void ChangeWardToQuarantine()
    {
        int wardId = selectWard.num;
        WardState state = wardStates[wardId];

        // 병동 상태 업데이트
        state.IsQuarantined = true;
        state.IsClosed = false;
        state.IsDisinfecting = false;

        // 버튼 상태 업데이트
        closeWardButton.gameObject.SetActive(false);
        quarantineWardButton.gameObject.SetActive(false);
        normalWardButton.gameObject.SetActive(true);
        disInfectWardButton.gameObject.SetActive(false);

        // 병동 격리 로직 호출
        selectWard.QuarantineWard();

        // 격리 병동으로 전환된 병동 정보 업데이트
        ResearchDBManager.Instance.AddResearchData(ResearchDBManager.ResearchMode.patient, 2, wardId, 1);

        // UI 업데이트
        UpdateWardInfomation(wardId);
    }

    public void ChangeWardToClose()
    {
        int wardId = selectWard.num;
        WardState state = wardStates[wardId];

        state.IsClosed = true;
        state.IsQuarantined = false;
        state.IsDisinfecting = false;

        closeWardButton.gameObject.SetActive(false);
        quarantineWardButton.gameObject.SetActive(false);
        normalWardButton.gameObject.SetActive(true);
        disInfectWardButton.gameObject.SetActive(true);

        selectWard.CloseWard();

        // 폐쇄 병동 정보 업데이트
        ResearchDBManager.Instance.AddResearchData(ResearchDBManager.ResearchMode.patient, 2, wardId, 2);


        UpdateWardInfomation(wardId);
    }

    public void ChangeWardToOpen()
    {
        int wardId = selectWard.num;
        WardState state = wardStates[wardId];

        state.IsQuarantined = false;
        state.IsClosed = false;
        state.IsDisinfecting = false;

        quarantineWardButton.gameObject.SetActive(true);
        closeWardButton.gameObject.SetActive(true);
        normalWardButton.gameObject.SetActive(false);
        disInfectWardButton.gameObject.SetActive(false);

        selectWard.OpenWard();

        // 일반 병동 정보 업데이트
        ResearchDBManager.Instance.AddResearchData(ResearchDBManager.ResearchMode.patient, 2, wardId, 3);

        UpdateWardInfomation(wardId);
    }

    public void DisinfectWard()
    {
        int wardId = selectWard.num;
        WardState state = wardStates[wardId];

        if (!state.IsDisinfecting)
        {
            state.IsDisinfecting = true;
            PolicyQuizManager.Instance.ClearVirusesInWard(selectWard.WardName);

            ResearchDBManager.Instance.AddResearchData(ResearchDBManager.ResearchMode.patient, 3, wardId, 1);
            state.DisinfectEndTime = Time.time + disinfectCooldownTime;

            //if (PolicyQuizManager.isCorrect)
            //{
            //    Debug.Log($"병동 소독 {selectWard.WardName} 시작...");
            //    disInfectButtonText.text = $"소독 중: {Mathf.CeilToInt(disinfectCooldownTime)}초 남음";
            //}
            //else
            //{
            //    Debug.Log($"병동 소독 {selectWard.WardName} 실패");
            //    disInfectButtonText.text = $"소독 재시도: {Mathf.CeilToInt(disinfectCooldownTime)}초 남음";
            //}

            //Debug.Log($"병동 소독 텍스트: {disInfectButtonText.text}");

            // UI 업데이트
            UpdateWardInfomation(wardId);
        }
    }

    private void UpdateDisinfectCooldowns()
    {
        foreach (var kvp in wardStates)
        {
            int wardId = kvp.Key;
            WardState state = kvp.Value;

            if (state.IsDisinfecting)
            {
                float remainingTime = state.DisinfectEndTime - Time.time;

                // 소독 완료 시 상태 초기화
                if (remainingTime <= 0)
                {
                    state.IsDisinfecting = false;

                    // 현재 병동 UI 업데이트
                    if (selectWard != null && selectWard.num == wardId)
                    {
                        disInfectWardButton.interactable = true;
                        normalWardButton.interactable = true;
                        disInfectButtonText.text = "소독";
                    }
                }
                else if (selectWard != null && selectWard.num == wardId)
                {
                    // 남은 시간 텍스트 업데이트
                    disInfectButtonText.text = $"소독 중: {Mathf.CeilToInt(remainingTime)}초 남음";
                }
            }
        }
    }


    private void UpdateCountText<T>(List<T> list, TextMeshProUGUI countText, TextMeshProUGUI infCountText, Image backImage, string wardName) where T : NPCController
    {
        int totalCount = list.Count(p => p.isInCurrentWard && p.currentWard == wardName);
        int infectedCount = list.Count(p => p.isInCurrentWard && p.currentWard == wardName && p.infectionController.isInfected);

        countText.text = $"{totalCount}";
        infCountText.text = $"{infectedCount}";

        backImage.gameObject.SetActive(totalCount == 0);
    }

    public void CheckIsolationStatus()
    {
        if (isQuarantineLevel_1)
        {
            qtOutline_1.SetActive(true);
            qtStartButton_1.gameObject.SetActive(false);
            QuarantineManager.quarantineStep = 1;
        }

        if (isQuarantineLevel_2)
        {
            qtOutline_2.SetActive(true);
            qtStartButton_2.gameObject.SetActive(false);
        }

        if (isQuarantineLevel_3)
        {
            infoPanel.SetActive(false);
        }
    }

    public void GoLevel1()
    {
        isQuarantineLevel_1 = true;
        ResearchDBManager.Instance.AddResearchData(ResearchDBManager.ResearchMode.patient, 1, 1, 1);
    }

    public void GoLevel2()
    {
        isQuarantineLevel_2 = true;
        ResearchDBManager.Instance.AddResearchData(ResearchDBManager.ResearchMode.patient, 1, 2, 1);
    }

    public void GoLevel3()
    {
        isQuarantineLevel_3 = true;
        UpdateWardInfomation(0);
        ResearchDBManager.Instance.AddResearchData(ResearchDBManager.ResearchMode.patient, 1, 3, 1);
    }
}

public class WardState
{
    public bool IsQuarantined;
    public bool IsClosed;
    public bool IsDisinfecting;
    public float DisinfectEndTime;
}
