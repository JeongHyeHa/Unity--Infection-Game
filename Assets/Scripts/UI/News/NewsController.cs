using System.Collections.Generic;
using UnityEngine;

public class NewsController : MonoBehaviour
{
    public int GameLevel = 0;
    private int count = 0;

    private bool startNewsTriggered = false;

    private Dictionary<int, bool> wardInfectionNewsTriggered = new Dictionary<int, bool>();
    private Dictionary<int, bool> wardDoctorStressNewsTriggered = new Dictionary<int, bool>();
    private Dictionary<int, bool> wardNurseStressNewsTriggered = new Dictionary<int, bool>();
    private Dictionary<int, bool> wardClosedNewsTriggered = new Dictionary<int, bool>();

    private Dictionary<int, HashSet<int>> wardInfectionLevelsTriggered = new Dictionary<int, HashSet<int>>(); // 감염률 뉴스 단계 트리거

    public NewsTicker moveTextController;
    public PolicyResearch policyResearch;

    private bool virusOutbreakNewsTriggered = false;    // 감염병 발생 뉴스
    private bool worldFirstInfectionTriggered = false;

    private void Awake()
    {
        if (moveTextController == null)
        {
            moveTextController = FindObjectOfType<NewsTicker>();
            policyResearch = FindObjectOfType<PolicyResearch>();
        }
        InitializeNewsTriggers();
    }

    private void Start()
    {
        InvokeRepeating(nameof(CheckNewsRequirements), 1f, 1f);
    }

    private void InitializeNewsTriggers()
    {
        foreach (Ward ward in Ward.wards)
        {
            wardInfectionNewsTriggered[ward.num] = false;
            wardDoctorStressNewsTriggered[ward.num] = false;
            wardNurseStressNewsTriggered[ward.num] = false;
            wardClosedNewsTriggered[ward.num] = false;
            wardInfectionLevelsTriggered[ward.num] = new HashSet<int>();
        }
    }

    // 병원체 발생 뉴스 (튜토리얼 전용)
    public void TriggerVirusOutbreakNews()
    {
        if (!virusOutbreakNewsTriggered)
        {
            EnqueueNews("원인불명의 병원체가 발생했습니다.");
            virusOutbreakNewsTriggered = true;
        }
    }

    // 병원체 연구가 완료되었을 때 뉴스 발생
    public void TriggerPathogenResearchCompleteNews(string pathogenName)
    {
        EnqueueNews($"연구가 완료되었습니다! 원인불명의 병원체는 '{pathogenName}'으로 밝혀졌습니다.", true);
    }

    // 치료제 연구가 완료되었을 때 뉴스 발생
    public void TriggerCureResearchCompleteNews(string cureName)
    {
        EnqueueNews($"좋은 소식입니다! '{cureName}' 치료제가 성공적으로 개발되었습니다.", true);
    }

    // 백신 연구가 완료되었을 때 뉴스 발생
    public void TriggerVaccineResearchCompleteNews(string vaccineName)
    {
        EnqueueNews($"기쁜 소식입니다! '{vaccineName}' 백신이 성공적으로 개발되었습니다.", true);
    }

    private void CheckNewsRequirements()
    {
        List<Ward> allWards = Ward.wards;

        CheckFirstInfectionNews(allWards);
        CheckInfectionNews(allWards);
        CheckWardClosedNews(allWards);
    }

    public void CheckWorldFirstInfectionNews()
    {
        if (!worldFirstInfectionTriggered)
        {
            PolicyWard.Instance.qtStartButton_1.gameObject.SetActive(true);
            EnqueueNews("국내 최초 감염자 발생! 각 병원은 감염병을 주의하시기 바랍니다!");
            worldFirstInfectionTriggered = true;
        }
    }

    private void CheckFirstInfectionNews(List<Ward> wards)
    {
        if (!startNewsTriggered && InfectionManager.Instance.GetOverallInfectionRate(wards) > 0)
        {
            EnqueueNews("병원 내 감염자가 발생하였습니다!");
            policyResearch.FirstInfectedAppear();
            PolicyWard.Instance.qtStartButton_2.gameObject.SetActive(true);
            startNewsTriggered = true;
        }
    }

    private void CheckInfectionNews(List<Ward> wards)
    {
        foreach (Ward ward in wards)
        {
            int infectionRate = Mathf.RoundToInt(InfectionManager.Instance.GetInfectionRate(ward));

            CheckAndTriggerInfectionLevelNews(ward.num, infectionRate, 20, "경고!! {ward.WardName} 내 감염률이 20%에 도달했습니다!");
            CheckAndTriggerInfectionLevelNews(ward.num, infectionRate, 50, "경고!! {ward.WardName} 내 감염률이 50%에 도달했습니다!");
            CheckAndTriggerInfectionLevelNews(ward.num, infectionRate, 80, "경고!! {ward.WardName} 내 감염률이 80%에 도달했습니다!");
        }
    }

    private void CheckWardClosedNews(List<Ward> wards)
    {
        foreach (Ward ward in wards)
        {
            UpdateNewsTrigger(ward.num, wardClosedNewsTriggered, ward.status,
                           $"<color=#FF0000>경고!!</color> {ward.WardName} 병동이 폐쇄되었습니다!");
        }
    }

    private void CheckAndTriggerInfectionLevelNews(int wardNum, int infectionRate, int threshold, string newsMessage)
    {
        if (!wardInfectionLevelsTriggered[wardNum].Contains(threshold) && infectionRate >= threshold)
        {
            EnqueueNews(newsMessage.Replace("{ward.WardName}", Ward.wards.Find(w => w.num == wardNum).WardName));
            wardInfectionLevelsTriggered[wardNum].Add(threshold);
        }
    }

    private void UpdateNewsTrigger(int wardNum, Dictionary<int, bool> triggerDictionary, Ward.WardStatus condition, string mainNews)
    {
        if (!triggerDictionary[wardNum] && condition == Ward.WardStatus.Closed)
        {
            EnqueueNews(mainNews);
            triggerDictionary[wardNum] = true;
        }
        else if (triggerDictionary[wardNum] && condition != Ward.WardStatus.Closed)
        {
            triggerDictionary[wardNum] = false;
        }
    }

    private float GetInfectionThreshold()
    {
        return GameLevel switch
        {
            0 => 20,
            1 => 15,
            2 => 10,
            _ => 20
        };
    }

    private void EnqueueNews(string mainNews, bool isPositive = false)
    {
        if (isPositive)
        {
            moveTextController.EnqueuePositiveNews(mainNews);
        }
        else
        {
            moveTextController.EnqueueNews(mainNews);
        }
    }
}
