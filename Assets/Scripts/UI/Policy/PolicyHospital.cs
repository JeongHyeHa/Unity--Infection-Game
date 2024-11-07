using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PolicyHospital : MonoBehaviour
{
    public Toggle[] closingtoggle = new Toggle[8];
    public Toggle[] disinfectiontoggle = new Toggle[8];

    public TextMeshProUGUI[] disinfectionText = new TextMeshProUGUI[8];
    public TextMeshProUGUI[] closingText = new TextMeshProUGUI[8];

    Ward ward;
    string[] wards = new string[] { "내과 1", "내과 2", "외과 1", "외과 2", "입원병동1", "입원병동2", "입원병동3", "입원병동4" };


    void Start()
    {
        ward = FindObjectOfType<Ward>();

        for (int i = 0; i < closingtoggle.Length; i++)
        {
            int currentIndex = i;

            //자동할당
            closingtoggle[currentIndex] = GameObject.Find($"Closingtoggle{currentIndex}").GetComponent<Toggle>();
            disinfectiontoggle[currentIndex] = GameObject.Find($"Disinfectiontoggle{currentIndex}").GetComponent<Toggle>();
            disinfectionText[currentIndex] = GameObject.Find($"DisinfectionText{currentIndex}").GetComponent<TextMeshProUGUI>();
            closingText[currentIndex] = GameObject.Find($"ClosingText{currentIndex}").GetComponent<TextMeshProUGUI>();

            //Toggle 초기화
            closingtoggle[currentIndex].isOn = false;

            // 각 병동별 소독 및 폐쇄 토글 설정
            TogglePairControl(closingtoggle[currentIndex], disinfectiontoggle[currentIndex], disinfectionText[currentIndex]);

            //Toggle 변경 시 UpdateWardCounts 호출
            closingtoggle[currentIndex].onValueChanged.AddListener(delegate {
                TogglePairControl(closingtoggle[currentIndex], disinfectiontoggle[currentIndex], disinfectionText[currentIndex]);
                if(closingtoggle[currentIndex].isOn)
                {
                    Ward.wards[currentIndex].CloseWard();
                }
                else
                {
                    Ward.wards[currentIndex].OpenWard();
                }
                UpdateWardCounts();
            });
        }

        // 일정 주기로 병동 데이터 업데이트
        StartCoroutine(UpdateWardCountsPeriodically());
    }

    // 병동별 의사, 간호사, 외래환자 수 1초마다 업데이트
    IEnumerator UpdateWardCountsPeriodically()
    {
        while (true)
        {
            for (int i = 0; i < 8; i++)
            {
                UpdateWardCounts();
            }
            yield return new WaitForSeconds(1f);
        }
    }

    // 폐쇄 토글 상태에 따른 소독 토글 활성화 관리
    void TogglePairControl(Toggle topToggle, Toggle bottomToggle, TextMeshProUGUI bottomText)
    {
        bottomToggle.interactable = topToggle.isOn;

        bottomText.text = topToggle.isOn ? "소독 가능" : "";
        if (!topToggle.isOn)
        {
            bottomToggle.isOn = false;
        }
    }

    // 토글 상태별 데이터 출력
    void UpdateWardCounts()
    {
        var wardCounts = GetStaffAndOutpatientCounts();

        for (int i = 0; i < closingtoggle.Length; i++)
        {
            if (closingtoggle[i].isOn)
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

        // wardCounts에 실제로 추가된 병동 확인
        /*foreach (var key in wardCounts.Keys)
        {
            Debug.Log($"wardCounts contains key: {key}");
        }*/

        return wardCounts;
    }
}
