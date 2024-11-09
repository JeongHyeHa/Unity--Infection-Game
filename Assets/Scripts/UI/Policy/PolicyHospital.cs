﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PolicyHospital : MonoBehaviour
{
    public Button[] closingButton = new Button[8];
    public Button[] disinfectionButton = new Button[8];

    public TextMeshProUGUI[] disinfectionText = new TextMeshProUGUI[8];
    public TextMeshProUGUI[] closingText = new TextMeshProUGUI[8];

    Ward ward;
    ResearchDBManager researchDBManager;
    string[] wards = new string[] { "내과 1", "내과 2", "외과 1", "외과 2", "입원병동1", "입원병동2", "입원병동3", "입원병동4" };
    bool[] isClosed = new bool[8];
    bool[] isDisinfected = new bool[8];

    void Start()
    {
        ward = FindObjectOfType<Ward>();
        researchDBManager = FindObjectOfType<ResearchDBManager>();

        for (int i = 0; i < closingButton.Length; i++)
        {
            int currentIndex = i;

            //자동할당
            closingButton[currentIndex] = GameObject.Find($"ClosingButton{currentIndex}").GetComponent<Button>();
            disinfectionButton[currentIndex] = GameObject.Find($"DisinfectionButton{currentIndex}").GetComponent<Button>();
            closingText[currentIndex] = GameObject.Find($"ClosingText{currentIndex}").GetComponent<TextMeshProUGUI>();
            disinfectionText[currentIndex] = GameObject.Find($"DisinfectionText{currentIndex}").GetComponent<TextMeshProUGUI>();

            //소독 버튼을 비활성화 상태로 초기화
            disinfectionButton[currentIndex].interactable = false;
            isClosed[currentIndex] = false;       // 모든 병동을 열림 상태로 저장
            isDisinfected[currentIndex] = false;  // 모든 병동을 소독 안 한 상태로 저장

            // 폐쇄 버튼 클릭 시 처리
            closingButton[currentIndex].onClick.AddListener(() =>
            {
                //Debug.Log($"PolicyHospital: {currentIndex}");
                ToggleColsing(currentIndex);
                if (isClosed[currentIndex])
                {
                    Ward.wards[currentIndex].CloseWard();
                }
                else
                {
                    Ward.wards[currentIndex].OpenWard();
                }
                UpdateWardCounts();
                PrintButtonState(1, currentIndex, isClosed[currentIndex]);             // DB에 폐쇄 상태 저장
            });

            // 소독 버튼 클릭 시 처리
            disinfectionButton[currentIndex].onClick.AddListener(() =>
            {
                ToggleDisinfection(currentIndex);
            });
        }

        // 일정 주기로 병동 데이터 업데이트
        InvokeRepeating("UpdateWardCounts", 1, 1);
    }

    // 폐쇄 버튼 클릭 시 처리
    void ToggleColsing(int index)
    {
        //Debug.Log($"PolicyHospital: {index}");
        isClosed[index] = !isClosed[index]; // 선택 상태를 토글

        disinfectionButton[index].interactable = isClosed[index]; // 소독 버튼 활성화 관리
        disinfectionText[index].text = isClosed[index] ? "소독 가능" : "";
        // 폐쇄 시 빨간 테두리 이미지 
        isDisinfected[index] = isClosed[index];
    }

    // 소독 버튼 클릭 시 소독 상태 업데이트
    void ToggleDisinfection(int index)
    {
        if (isClosed[index] && !isDisinfected[index])
        {
            // 소독 중일 때 비활성화하여 추가 클릭을 방지
            disinfectionButton[index].interactable = false;
            disinfectionText[index].text = "소독 중...";
            // 소독 시 초록색 테두리

            // 소독 시간 대기 후 완료 텍스트로 전환
            StartCoroutine(DisinfectionTimer(index));
        }
    }

    IEnumerator DisinfectionTimer(int index)
    {
        float elapsedTime = 0f;
        float disinfectionTime = 30f; // 소독 시간 30초

        // TimeScale이 0이어도 진행되도록 unscaledDeltaTime을 사용하여 시간 증가
        while (elapsedTime < disinfectionTime)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float remainingTime = Mathf.Ceil(disinfectionTime - elapsedTime);
            disinfectionText[index].text = $"소독 중\n{remainingTime}초";
            yield return null;
        }

        // 소독 완료 처리
        isDisinfected[index] = true;
        disinfectionText[index].text = "소독 완료";
        PrintButtonState(2, index, true); // 소독 완료 상태를 DB에 저장

        // 소독 버튼 비활성화
        disinfectionButton[index].interactable = false;
    }

    //DB 데이터 만들기
    void PrintButtonState(int toggleType, int wardIndex, bool isOn)
    {
        int toggleState = isOn ? 1 : 0;
        int wardNumber = wardIndex + 1; // 병동 번호 1부터 시작
        //Debug.Log($"PolicyHospital: {toggleType}.{wardNumber}.{toggleState}");

        researchDBManager.AddResearchData(ResearchDBManager.ResearchMode.patient, toggleType, wardNumber, toggleState);
    }

    // 병동별 의사, 간호사, 외래환자 수 1초마다 업데이트
    void UpdateWardCountsPeriodically()
    {
        while (true)
        {
            for (int i = 0; i < 8; i++)
            {
                UpdateWardCounts();
            }
        }
    }

    // 토글 상태별 데이터 출력
    void UpdateWardCounts()
    {
        var wardCounts = GetStaffAndOutpatientCounts();

        for (int i = 0; i < closingButton.Length; i++)
        {
            if (closingButton[i])
                closingText[i].text = "의사 x0\n간호사 x0\n외래환자 x0";
            else if (wardCounts.ContainsKey(wards[i]))
            {
                // 병동이 열려있을 때 병동 정보를 출력
                var wardInfo = wardCounts[wards[i]];
                closingText[i].text = $"의사 x{wardInfo.doctorCount}\n간호사 x{wardInfo.nurseCount}\n외래환자 x{wardInfo.outpatientCount}";
            }
        }
    }

    // 병동별 의사, 간호사, 외래환자 데이터 수집
    public Dictionary<string, (int doctorCount, int nurseCount, int outpatientCount)> GetStaffAndOutpatientCounts()
    {
        Dictionary<string, (int doctorCount, int nurseCount, int outpatientCount)> wardCounts = new Dictionary<string, (int, int, int)>();

        foreach (Ward ward in Ward.wards)
        {
            if (ward.num >= 0 && ward.num <= 7)
            {
                int doctorCount = ward.doctors.Count;
                int nurseCount = ward.nurses.Count;
                int outpatientCount = ward.outpatients.Count;

                //Debug.Log($"Ward: {ward.WardName}, Doctors: {doctorCount}, Nurses: {nurseCount}, Outpatients: {outpatientCount}");
                wardCounts.Add(ward.WardName, (doctorCount, nurseCount, outpatientCount));
            }
        }
        return wardCounts;
    }
}
