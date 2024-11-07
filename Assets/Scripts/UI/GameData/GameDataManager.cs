using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; private set; }

    public string userName;
    public string userId;
    public string currentDate;

    private string doctorScore = "";    //감염률 데이터
    private string nurseScore = "";
    private string inpatientsScore = "";
    private string outpatientsScore = "";
    private string emergencyPatientsScore = "";
    private string icuPatientsScore = "";
    private string totalScores = "";

    private List<float> infectionRates = new List<float>();          // 10초 단위 감염률
    Dictionary<string, float> infectionRoleDictionary = new Dictionary<string, float>();
    private List<float> doctorInfectionRates = new List<float>();
    private List<float> nurseInfectionRates = new List<float>();
    private List<float> inpatientsRates = new List<float>();
    private List<float> outpatientsRates = new List<float>();
    private List<float> emergencyPatientsRates = new List<float>();
    private List<float> icuPatientsRates = new List<float>();

    private List<float> averagedInfectionRates = new List<float>();  // 하루 단위 감염률
    private List<float> avgDoctorRate = new List<float>();
    private List<float> avgNurseRate = new List<float>();
    private List<float> avgInpatientsRate = new List<float>();
    private List<float> avgOutpatientsRate = new List<float>();
    private List<float> avgEmergencyPatientsRate = new List<float>();
    private List<float> avgIcuPatientsRate = new List<float>();

    GameObject scoreGraphCanvas;
    public Button scoreMoreButton;
    private DateTime gameStartTime;

    private string urlUpdateData = "http://220.69.209.164:3333/update_game_score";
    private int pointsPerMinute = 6;        // 1분 동안의 감염률 평균 내기(10초*6)

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

    void Start()
    {
        scoreGraphCanvas = GameObject.Find("ScoreGraphCanvas");
        gameStartTime = DateTime.Now;  // 게임 시작 시간 저장

        //scoreMoreButton = GameObject.Find("ScoreMoreButton").GetComponent<Button>();
    }

    // 초기 데이터 삽입 (Flask API로 전송)
    public void InsertInitialData()
    {
        currentDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        WWWForm form = new WWWForm();
        form.AddField("user_id", userId);
        form.AddField("user_name", userName);
        form.AddField("doctorRate", " ");
        form.AddField("nurseRate", " ");
        form.AddField("inpatientsRate", " ");
        form.AddField("outpatientsRate", " ");
        form.AddField("emergencyPatientsRate", " ");
        form.AddField("icuPatientsRate", " ");
        form.AddField("totalRate", " ");
        form.AddField("playingDate", currentDate);

        StartCoroutine(PostRequest(urlUpdateData, form));
        StartCoroutine(SaveInfectionRateFor15Minutes());
    }

    // 15분 동안 10초 간격으로 감염률 리스트에 저장
    private IEnumerator SaveInfectionRateFor15Minutes()
    {
        float totalTime = 900f;      // 전체 실행 시간 (초)
        //float totalTime = 200f;
        float interval = 10f;        // 간격 (초)
        float elapsedTime = 0f;      // 경과 시간 (초)
        int dataCount = 0;           // 현재까지 모은 데이터 개수  //수정

        while (elapsedTime < totalTime)
        {
            yield return new WaitForSeconds(interval);  // 10초 대기

            float infectionRate = InfectionManager.Instance.GetOverallInfectionRate(Ward.wards);  // 현재 감염률 가져오기
            if (float.IsNaN(infectionRate))
            {
                Debug.LogError("InfectionManager에서 유효한 감염률을 가져오지 못했습니다.");
            }
            else
            {
                infectionRates.Add(infectionRate);
                Debug.Log($"감염률 수집: {infectionRate}, 총 감염률 수집된 데이터 수: {infectionRates.Count}");
            }

            // 역할별 감염률 리스트 저장
            infectionRoleDictionary = InfectionManager.Instance.GetInfectionRateByRole();
            if (infectionRoleDictionary == null || !infectionRoleDictionary.ContainsKey("doctor") || !infectionRoleDictionary.ContainsKey("nurse")
                || !infectionRoleDictionary.ContainsKey("inpatients") || !infectionRoleDictionary.ContainsKey("outpatients")
                || !infectionRoleDictionary.ContainsKey("emergencyPatients") || !infectionRoleDictionary.ContainsKey("icuPatients"))
            {
                Debug.LogError("InfectionManager에서 역할별 감염률 데이터를 제대로 가져오지 못했습니다.");
            }
            else
            {
                doctorInfectionRates.Add(infectionRoleDictionary["doctor"] * 100);
                nurseInfectionRates.Add(infectionRoleDictionary["nurse"] * 100);
                inpatientsRates.Add(infectionRoleDictionary["inpatients"] * 100);
                outpatientsRates.Add(infectionRoleDictionary["outpatients"] * 100);
                icuPatientsRates.Add(infectionRoleDictionary["icuPatients"] * 100);
                emergencyPatientsRates.Add(infectionRoleDictionary["emergencyPatients"] * 100);

                Debug.Log($"의사 감염률: {infectionRoleDictionary["doctor"] * 100}, 간호사 감염률: {infectionRoleDictionary["nurse"] * 100}, 내원환자 감염률: {infectionRoleDictionary["inpatients"] * 100}, 외래환자 감염률: {infectionRoleDictionary["outpatients"] * 100}, 응급환자 감염률: {infectionRoleDictionary["emergencyPatients"] * 100}, 중환자 감염률: {infectionRoleDictionary["icuPatients"] * 100}");
            }

            dataCount++;

            // 6개의 데이터를 모으면 평균 계산하고 DB에 업데이트
            if (dataCount == pointsPerMinute)
            {
                Debug.Log($"DB_ 리스트 데이터 6개 모임. 현재 데이터: 감염률={infectionRates.Count}, 의사 감염률={doctorInfectionRates.Count}, 간호사 감염률={nurseInfectionRates.Count}, 내원환자 감염률={inpatientsRates.Count}, 외래환자 감염률={outpatientsRates.Count}, 응급환자 감염률={emergencyPatientsRates.Count}, 중환자 감염률={icuPatientsRates.Count}");

                CalculateAndSaveAverage(infectionRates, "totalScores", averagedInfectionRates);
                CalculateAndSaveAverage(doctorInfectionRates, "doctorScore", avgDoctorRate);
                CalculateAndSaveAverage(nurseInfectionRates, "nurseScore", avgNurseRate);
                CalculateAndSaveAverage(inpatientsRates, "inpatientsScore", avgInpatientsRate);
                CalculateAndSaveAverage(outpatientsRates, "outpatientsScore", avgOutpatientsRate);
                CalculateAndSaveAverage(icuPatientsRates, "icuPatientsScore", avgIcuPatientsRate);
                CalculateAndSaveAverage(emergencyPatientsRates, "emergencyPatientsScore", avgEmergencyPatientsRate);

                dataCount = 0; // 데이터 수 초기화
            }

            elapsedTime += interval;    // 경과 시간 업데이트   
        }

        Time.timeScale = 0;                 // 게임 일시 정지
        scoreGraphCanvas.SetActive(true);
        GraphSourceChangeInt();             // 그래프 생성
    }

    // 평균 계산 및 DB 업데이트 함수
    private void CalculateAndSaveAverage(List<float> ratesList, string scoreType, List<float> avgList)
    {
        // 마지막 6개의 데이터를 가져와 평균을 계산
        List<float> lastSixData;
        if (ratesList.Count >= pointsPerMinute)
        {
            lastSixData = ratesList.GetRange(ratesList.Count - pointsPerMinute, pointsPerMinute);

            float average = CalculateAverage(lastSixData);

            // 평균 비교
            float previousAverage = avgList.Count > 0 ? avgList[avgList.Count - 1] : 0f;
            float difference = Mathf.Abs(average - previousAverage);

            // 차이가 20 이상인 경우
            if (difference >= 20)
            {
                //Debug.Log($"20이상계산하기) 차이가 20 이상인 평균들: 이전 평균 = {previousAverage}, 현재 평균 = {average}");
                DateTime startTime = gameStartTime.AddSeconds(avgList.Count * 60);
                DateTime endTime = startTime.AddSeconds(60);

                string formattedStartTime = startTime.ToString("mm:ss");
                string formattedEndTime = endTime.ToString("mm:ss");

                //Debug.Log($"20이상계산하기) 시간 범위: {formattedStartTime}~ {formattedEndTime}");
                FindTimeInResearchRecords(formattedStartTime, formattedEndTime);
            }

            // 평균을 리스트에 추가
            avgList.Add(average);

            // 실수형 감염률 리스트를 정수형 문자열로 변환하여 적절한 컬럼에 업데이트
            switch (scoreType)
            {
                case "totalScores":
                    totalScores += average.ToString("F0") + ",";
                    break;
                case "doctorScore":
                    doctorScore += average.ToString("F0") + ",";
                    break;
                case "nurseScore":
                    nurseScore += average.ToString("F0") + ",";
                    break;
                case "inpatientsScore":
                    inpatientsScore += average.ToString("F0") + ",";
                    break;
                case "outpatientsScore":
                    outpatientsScore += average.ToString("F0") + ",";
                    break;
                case "emergencyPatientsScore":
                    emergencyPatientsScore += average.ToString("F0") + ",";
                    break;
                case "icuPatientsScore":
                    icuPatientsScore += average.ToString("F0") + ",";
                    break;
            }
        }
        else
        {
            Debug.LogError($"데이터가 부족함. {ratesList.Count}");
        }

        UpdateDatabaseInfectionScore();     // DB 업데이트
    }

    // 평균 계산하는 함수
    private float CalculateAverage(List<float> chunk)
    {
        if (chunk.Count == 0) return 0f;

        float sum = 0f;
        foreach (float value in chunk)
        {
            sum += value;
        }
        return sum / chunk.Count;
    }

    // ResearchDBManager의 researchRecords 딕셔너리에서 시간 찾기
    private void FindTimeInResearchRecords(string startTime, string endTime)
    {
        ResearchDBManager researchManager = ResearchDBManager.Instance;

        // 모든 모드에 대해 시간 찾기
        foreach (ResearchDBManager.ResearchMode mode in Enum.GetValues(typeof(ResearchDBManager.ResearchMode)))
        {
            List<(int, string)> recordList = researchManager.researchRecords[mode];  // 현재 모드의 리스트 가져오기

            for (int i = 0; i < recordList.Count; i++)
            {
                string recordTime = recordList[i].Item2;

                // 시간 범위 내에 있는지 확인
                if (IsTimeInRange(recordTime, startTime, endTime))
                {
                    Debug.Log($"20이상계산하기) {mode} 리스트의 {i}번째 인덱스에서 시간 {recordTime}을 찾았습니다.");
                }
            }
        }
    }

    // 주어진 시간 범위에 포함되는지 확인하는 함수
    private bool IsTimeInRange(string recordTime, string startTime, string endTime)
    {
        try
        {
            // 시간을 TimeSpan으로 변환
            TimeSpan recordTimeSpan = TimeSpan.ParseExact(recordTime, "mm\\:ss", null);
            TimeSpan startSpan = TimeSpan.ParseExact(startTime, "mm\\:ss", null);
            TimeSpan endSpan = TimeSpan.ParseExact(endTime, "mm\\:ss", null);

            return (recordTimeSpan >= startSpan && recordTimeSpan <= endSpan);
        }
        catch (FormatException e)
        {
            Debug.LogError($"시간 파싱 오류: {e.Message}, recordTime: {recordTime}");
            return false;
        }
    }

    // DB에 최종 감염 데이터 정보 업그레이드(1일~ 15일 데이터)
    void UpdateDatabaseInfectionScore()
    {
        WWWForm form = new WWWForm();
        form.AddField("user_id", userId);
        form.AddField("user_name", userName);
        form.AddField("doctorRate", string.IsNullOrEmpty(doctorScore) ? "0" : doctorScore);
        form.AddField("nurseRate", string.IsNullOrEmpty(nurseScore) ? "0" : nurseScore);
        form.AddField("inpatientsRate", string.IsNullOrEmpty(inpatientsScore) ? "0" : inpatientsScore);
        form.AddField("outpatientsRate", string.IsNullOrEmpty(outpatientsScore) ? "0" : outpatientsScore);
        form.AddField("emergencyPatientsRate", string.IsNullOrEmpty(emergencyPatientsScore) ? "0" : emergencyPatientsScore);
        form.AddField("icuPatientsRate", string.IsNullOrEmpty(icuPatientsScore) ? "0" : icuPatientsScore);
        form.AddField("totalRate", string.IsNullOrEmpty(totalScores) ? "0" : totalScores);
        form.AddField("playingDate", currentDate);

        //Debug.Log($"DB: Sending Data: user_id={userId}, user_name={userName}, infectionScore={infectionScores}, playingDate ={currentDate}");
        StartCoroutine(PostRequest(urlUpdateData, form));
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
        }
    }

    // 문장열 형태의 감염 데이터 정수화
    private void GraphSourceChangeInt()
    {
        FindObjectOfType<GraphManager>().DrawGraph(infectionRates, "total");
        FindObjectOfType<GraphManager>().DrawGraph(doctorInfectionRates, "doctor");
        FindObjectOfType<GraphManager>().DrawGraph(nurseInfectionRates, "nurse");
        FindObjectOfType<GraphManager>().DrawGraph(inpatientsRates, "inpatients");
        FindObjectOfType<GraphManager>().DrawGraph(outpatientsRates, "outpatients");
        FindObjectOfType<GraphManager>().DrawGraph(emergencyPatientsRates, "emergencyPatients");
        FindObjectOfType<GraphManager>().DrawGraph(icuPatientsRates, "icuPatients");
    }
}