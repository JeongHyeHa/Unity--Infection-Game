using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class ResearchDBManager : MonoBehaviour
{
    public static ResearchDBManager Instance { get; private set; }

    public string userNum;
    public string userName;
    public string currentDate;

    // 연구 메뉴에 따른 모드에 대한 리스트
    public Dictionary<ResearchMode, List<string>> researchRecords = new Dictionary<ResearchMode, List<string>>()
    {
        {ResearchMode.gear, new List<string>() },
        {ResearchMode.patient, new List<string>() },
        {ResearchMode.research, new List<string>() }
    };

    private string urlUpdateResearch = "http://220.69.209.164:3333/update_research";

    public enum ResearchMode { gear, patient, research }
    private ResearchMode currentMode;

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
    }

    private void Start()
    {
        currentDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    // 리스트 생성 및 DB 저장
    public void AddResearchData(ResearchMode mode, int btnNum, int targetNum, int toggleIsOn)
    {
        string currentMoment = System.DateTime.Now.ToString("mm:ss");
        string formattedData = $"{btnNum}.{targetNum}.{toggleIsOn}.{currentMoment}";

        researchRecords[mode].Add((formattedData));  // 연구 메뉴별 버튼의 인덱스와 누른 시간을 튜플 저장
        SendResearchDataToServer();                  // DB 전송
    }

    // POST 요청으로 서버에 데이터 보내기
    public void SendResearchDataToServer()
    {
        WWWForm form = new WWWForm();
        form.AddField("userNum", userNum);
        form.AddField("userName", userName);
        form.AddField("gearResearch", FormatDataForDB(researchRecords[ResearchMode.gear]));
        form.AddField("patientResearch", FormatDataForDB(researchRecords[ResearchMode.patient]));
        form.AddField("advancedResearch", FormatDataForDB(researchRecords[ResearchMode.research]));
        form.AddField("playingDate", currentDate);

        StartCoroutine(PostRequest(urlUpdateResearch, form));
    }

    // DB에 보낼 수 있게 리스트->문자열 형변환
    string FormatDataForDB(List<string> records)
    {
        if (records != null && records.Count > 0)
        {
            return string.Join(", ", records);
        }
        return " ";
    }

    // Send a POST request to the Flask API
    IEnumerator PostRequest(string url, WWWForm form)
    {
        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error sending request: " + www.error);
            }
            else
            {
                //Debug.Log("Request successfully sent: " + www.downloadHandler.text);
            }
        }
    }
}
