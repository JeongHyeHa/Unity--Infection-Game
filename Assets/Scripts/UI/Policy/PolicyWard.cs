﻿using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Linq;
using Unity.VisualScripting;

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

    public GameObject infoPanel; // 병동 관리 탭 위 설명표
    public Button quarantineWardButton; // 격리 병동 전환 버튼
    public Button closeWardButton;
    public Button normalWardButton;

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
        wardDropdown.value = 0;
        wardDropdown.onValueChanged.AddListener(UpdateWardName);
        quarantineWardButton.onClick.AddListener(ChangeWardToQuarantine);
        closeWardButton.onClick.AddListener(ChangeWardToClose);
        normalWardButton.onClick.AddListener(ChangeWardToOpen);
        qtStartButton_1.onClick.AddListener(GoLevel1);
        qtStartButton_2.onClick.AddListener(GoLevel2);
        qtStartButton_3.onClick.AddListener(GoLevel3);
    }

    private void Update()
    {
        CheckIsolationStatus(); // 격리 상태 확인
    }



    // 드롭다운 초기화
    private void InitializeDropdown()
    {
        wardDropdown.ClearOptions();
        wardDropdown.AddOptions(new List<string>(wardNames));
    }

    public void UpdateWardName(int index)
    {
        UpdateWardInfomation(index);
    }


    // 병동 정보 업데이트
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

        // 격리 병동 전환 버튼 활성화 상태 설정
        UpdateQuarantineButtonState(currentWard);

        MinimapRaycaster.Instance.SetExternalHighlightActive(true, currentWard.WardName);

        // 인원 수 및 감염 수 업데이트
        UpdateCountText(currentWard.doctors, doctorCountText, doctorInfCountText, doctorBack, currentWard.WardName);
        UpdateCountText(currentWard.nurses, nurseCountText, nurseInfCountText, nurseBack, currentWard.WardName);
        UpdateCountText(currentWard.outpatients, outpatientCountText, outpatientInfCountText, outpatientBack, currentWard.WardName);
        UpdateCountText(currentWard.inpatients, inpatientCountText, inpatientInfCountText, inpatientBack, currentWard.WardName);
        UpdateCountText(currentWard.emergencyPatients, emergencypatientCountText, emergencypatientInfCountText, emergencypatientBack, currentWard.WardName);
        UpdateCountText(currentWard.icuPatients, icupatientCountText, icupatientInfCountText, icupatientBack, currentWard.WardName);

        CheckIsolationStatus(); // 격리 상태 확인
    }

    private void UpdateQuarantineButtonState(Ward currentWard)
    {
        // 격리 병동 전환 버튼은 입원 병동에서만 활성화
        if (!currentWard.WardName.StartsWith("입원병동"))
        {
            quarantineWardButton.gameObject.SetActive(false);
            closeWardButton.gameObject.SetActive(false);
            normalWardButton.gameObject.SetActive(false);
        }
        else
        {
            quarantineWardButton.gameObject.SetActive(true);
            closeWardButton.gameObject.SetActive(true);
        }
    }

    public void CheckIsolationStatus()
    {
        // 격리 1단계 발령 조건
        if (isQuarantineLevel_1)
        {
            qtOutline_1.SetActive(true);
            qtStartButton_1.gameObject.SetActive(false);
            QuarantineManager.quarantineStep = 1;
        }
        // 격리 1단계 발령 조건
        if (isQuarantineLevel_2)
        {
            qtOutline_1.SetActive(false);
            qtOutline_2.SetActive(true);
            qtStartButton_2.gameObject.SetActive(false);
        }

        // 격리 2단계 발령 조건
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

    public void ChangeWardToQuarantine()
    {
        closeWardButton.gameObject.SetActive(false);
        quarantineWardButton.gameObject.SetActive(false);
        normalWardButton.gameObject.SetActive(true);
        selectWard.QuarantineWard();

        //격리 병동으로 전환된 병동 정보 업데이트
        int index = 1;
        foreach (string ward in wardNames)
        {
            if (ward == selectWard.WardName)
                ResearchDBManager.Instance.AddResearchData(ResearchDBManager.ResearchMode.patient, 2, index, 1);
            index++;
        }
    }

    public void ChangeWardToOpen()
    {
        quarantineWardButton.gameObject.SetActive(true);
        closeWardButton.gameObject.SetActive(true);
        normalWardButton.gameObject.SetActive(false);
        selectWard.OpenWard();
        //일반 병동으로 전환된 병동 정보 업데이트
        int index = 1;
        foreach (string ward in wardNames)
        {
            if (ward == selectWard.WardName)
                ResearchDBManager.Instance.AddResearchData(ResearchDBManager.ResearchMode.patient, 2, index, 3);
            index++;
        }
    }

    public void ChangeWardToClose()
    {
        quarantineWardButton.gameObject.SetActive(false);
        closeWardButton.gameObject.SetActive(false);
        normalWardButton.gameObject.SetActive(true);
        selectWard.CloseWard();
        //폐쇄 병동으로 전환된 병동 정보 업데이트
        int index = 1;
        foreach (string ward in wardNames)
        {
            if (ward == selectWard.WardName)
                ResearchDBManager.Instance.AddResearchData(ResearchDBManager.ResearchMode.patient, 2, index, 2);
            index++;
        }
    }

    // 제네릭 메서드로 리스트 데이터 처리
    private void UpdateCountText<T>(List<T> list, TextMeshProUGUI countText, TextMeshProUGUI infCountText, Image backImage, string wardName) where T : NPCController
    {
        // 현재 병동에 있는 사람들의 수 계산
        int totalCount = list.Count(p => p.isInCurrentWard && p.currentWard == wardName);
        int infectedCount = list.Count(p => p.isInCurrentWard && p.currentWard == wardName && p.infectionController.isInfected);

        // UI 업데이트
        countText.text = $"{totalCount}";
        infCountText.text = $"{infectedCount}";

        // Back 활성화/비활성화
        backImage.gameObject.SetActive(totalCount == 0);
    }
}
