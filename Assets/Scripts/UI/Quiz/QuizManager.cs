using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyApp.DataAccess;
using TMPro;
using System.Linq;
using System;
using System.Reflection;
using MySql.Data.MySqlClient;
using Unity.VisualScripting;

public class QuizManager : MonoBehaviour
{
    public static QuizManager Instance { get; private set; }
    // UI
    public Button[] levelButtons;    // 메뉴 버튼
    public Button[] answerButtons;
    public Canvas questCanvas;
    public TextMeshProUGUI quest;   // 퀴즈 문제   
    public GameObject wrongPanel;
    public GameObject rightPanel;
    public GameObject coolTimePanel;
    public TextMeshProUGUI rightText;
    //public TextMeshProUGUI moneyText;
    public TextMeshProUGUI timerText;
    CurrentMoney currentMoney;
    MonthlyReportUI monthlyReportUI;

    // 데이터 관리
    List<User> usersDB = new List<User>();      //DB 문제             
    List<User> answersDB = new List<User>();      //DB 문제             
    public Dictionary<string, float> cooldownTimers = new Dictionary<string, float>();  // 레벨별 남은 쿨타임 시간
    Dictionary<string, List<int>> availableIndicesByLevel = new Dictionary<string, List<int>>();    //레벨별 총 문제 개수
    Dictionary<string, bool> firstTry = new Dictionary<string, bool>();     // 레벨별 오답 재도전 여부
    public Dictionary<string, List<int>> questCount = new Dictionary<string, List<int>>();     // 레벨별 푼 문제

    public int clickbtn;     // 현재 선택한 레벨별 메뉴 버튼 인덱스
    string levelname;
    int currentIndex = -1;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    void Start()
    {
        levelButtons = new Button[3];
        answerButtons = new Button[4];
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int index = i;
            levelButtons[index] = GameObject.Find($"LevelButton{index + 1}").GetComponent<Button>();
            levelButtons[index].onClick.AddListener(() => { OnLevelButtonClicked(index); BtnSoundManager.Instance.PlayButtonSound(); });

            string levelName = levelButtons[index].name;
            cooldownTimers[levelName] = 0;      // 레벨별 초기화
            firstTry[levelName] = true;
            questCount[levelName] = new List<int>();
        }
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;
            answerButtons[index] = GameObject.Find($"AnswerButton{index + 1}").GetComponent<Button>();
            answerButtons[index].onClick.AddListener(() => CheckAnswer(index + 1));
        }

        questCanvas = gameObject.GetComponent<Canvas>();
        quest = GameObject.Find("Quest").GetComponent<TextMeshProUGUI>();
        wrongPanel = GameObject.Find("WrongPanel");
        rightPanel = GameObject.Find("RightPanel");
        coolTimePanel = GameObject.Find("CoolTimePanel");
        rightText = GameObject.Find("RightText").GetComponent<TextMeshProUGUI>();
        //moneyText = GameObject.Find("MoneyText").GetComponent<TextMeshProUGUI>();
        timerText = GameObject.Find("TimerText").GetComponent<TextMeshProUGUI>();
        currentMoney = FindObjectOfType<CurrentMoney>();
        monthlyReportUI = FindObjectOfType<MonthlyReportUI>();

        StartCoroutine(WaitForData());
    }

    // DB 데이터 로드가 끝날 때까지 대기
    IEnumerator WaitForData()
    {
        yield return new WaitUntil(() => MySQLConnector.Instance.GetUsers() != null && MySQLConnector.Instance.GetUsers().Count > 0);
        usersDB = MySQLConnector.Instance.GetUsers();
        answersDB = MySQLConnector.Instance.GetAnswers();

        InitializeIndices();
        //UpdateMoneyUI();
        OnLevelButtonClicked(0);
    }

    // 레벨별 문제 개수 초기화
    void InitializeIndices(int num = -1)
    {
        if (num == -1)
        {
            availableIndicesByLevel.Clear();
            availableIndicesByLevel["LevelButton1"] = Enumerable.Range(0, 31).ToList();     //0~30
            availableIndicesByLevel["LevelButton2"] = Enumerable.Range(31, 30).ToList();    //31~61
            availableIndicesByLevel["LevelButton3"] = Enumerable.Range(61, 20).ToList();    //61~81
        }
        else
        {
            int value = num * 20;
            availableIndicesByLevel[levelButtons[num].name] = Enumerable.Range(value - 20, value).ToList();
        }
    }

    // 현재 금액 업데이트
    /*void UpdateMoneyUI()
    {
        moneyText.text = $"{currentMoney.CurrentMoneyGetter:N0}Sch";  // 천단위 콤마 추가
    }*/

    // 메뉴 버튼을 누를 때마다 호출되는 함수 (0~2)
    public void OnLevelButtonClicked(int menuIndex)
    {
        clickbtn = menuIndex;    // 선택한 버튼 저장 
        levelname = levelButtons[clickbtn].name;

        // 안 풀고 넘어갔다면 다시 도전
        if (questCount[levelname].Count > 0 && questCount[levelname][questCount[levelname].Count - 1] != -1)
            currentIndex = questCount[levelname][questCount[levelname].Count - 1];
        else
            currentIndex = -1;

        // 선택된 레벨 메뉴 버튼은 비활성화
        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (i == menuIndex)
                levelButtons[i].interactable = false;
            else
                levelButtons[i].interactable = true;
        }

        // 쿨타임 패널 or 문제 출력 결정
        //Debug.Log($"Quiz, {levelname}, 쿨타임 남은 시간: {cooldownTimers[levelname]}");
        if (cooldownTimers[levelname] > 0)
            ManagerCooltimer();
        else
        {
            coolTimePanel.SetActive(false);
            SetRandomQuest();
        }
    }

    // 쿨타임 패널이 뜰 때 호출되는 함수
    void ManagerCooltimer()
    {
        QuizTimer.Instance.ResetTimerText(levelname);  // 문제 카운트다운 시간 초기화
        coolTimePanel.SetActive(true);
        QuizCooltime.Instance.currentName = levelname;
        QuizCooltime.Instance.ShowCooltimePanel(levelname);
    }

    // 퀴즈 출력
    public void SetRandomQuest()
    {
        if (!questCanvas.enabled) return;   // 퀘스트 창이 안 보일 경우

        string levelname = levelButtons[clickbtn].name;

        // 해당 레벨의 문제 리스트가 비었을 경우 초기화
        if (availableIndicesByLevel[levelname].Count <= 0)
            InitializeIndices(clickbtn);

        // 5문제를 다 풀면 쿨타임 대기
        if (questCount[levelname].Count % 5 == 0 && questCount[levelname].Count != 0)
        {
            cooldownTimers[levelname] = QuizCooltime.maxCooltime;
            ManagerCooltimer();
            return;
        }

        // 새로운 문제 설정
        if (currentIndex == -1)
        {
            currentIndex = availableIndicesByLevel[levelname][UnityEngine.Random.Range(0, availableIndicesByLevel[levelname].Count)];
            questCount[levelname].Add(currentIndex);
            //Debug.Log($"Quiz, {levelname} 새로운 문제: {currentIndex}");
            Debug.Log($"Quiz, {currentIndex} 정답: {answersDB[currentIndex].right}");

            if (currentIndex >= usersDB.Count)
            {
                Debug.LogError($"currentIndex({currentIndex})가 users 리스트의 범위 {usersDB.Count}를 벗어났습니다.");
                return;
            }
        }

        // 해당 레벨의 문제 카운트다운 시작 (1레벨-10초, 2레벨-20초, 3레벨-30초)
        QuizTimer.Instance.StartQuizTimer(levelname);

        // 문제 출력
        quest.text = usersDB[currentIndex].quest;
        string[] answerArray = new string[] { usersDB[currentIndex].answer1, usersDB[currentIndex].answer2, usersDB[currentIndex].answer3, usersDB[currentIndex].answer4 };
        for (int i = 0; i < answerButtons.Length; i++)
            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = answerArray[i];
    }

    // 정답 체크하는 함수
    void CheckAnswer(int selectedAnswer)
    {
        if (selectedAnswer == answersDB[currentIndex].right)
        {
            StartCoroutine(ShowRightPanel());
            AddMoney();
        }
        else
            StartCoroutine(ShowWrongPanel());
    }

    // 정답 패널 생성하는 함수
    IEnumerator ShowRightPanel()
    {
        rightPanel.SetActive(true);
        yield return YieldInstructionCache.WaitForSecondsRealtime(1.5f);
        rightPanel.SetActive(false);
        NextQuestionDelay();
    }

    // 오답 패널 생성하는 함수
    IEnumerator ShowWrongPanel()
    {
        wrongPanel.SetActive(true);
        yield return YieldInstructionCache.WaitForSecondsRealtime(1.5f);
        wrongPanel.SetActive(false);

        if (firstTry[levelname])
        {
            QuizTimer.Instance.StartQuizTimer(levelname);
            firstTry[levelname] = false;
        }
        else
            NextQuestionDelay();    // 두 번째 시도
    }

    // 돈 획득
    void AddMoney()
    {
        int moneyToAdd = GetMoneyByLevelButton();
        rightText.text = $"{moneyToAdd}Sch 획득하였습니다.";   // 레벨별 정답 패널 텍스트 설정
        currentMoney.CurrentMoneyGetter += moneyToAdd;   // 월말정산표에 반영
        monthlyReportUI.AddIncomeDetail("퀴즈", moneyToAdd);
        //UpdateMoneyUI();
    }

    // 레벨별 정산 금액 반환 
    int GetMoneyByLevelButton()
    {
        if (clickbtn == 0)
            return 500;
        else if (clickbtn == 1)
            return 1000;
        else
            return 1500;
    }

    //다음 문제로 전환될 때 호출되는 함수
    public void NextQuestionDelay()
    {
        firstTry[levelname] = true;
        questCount[levelname][questCount[levelname].Count - 1] = -1;
        availableIndicesByLevel[levelname].Remove(currentIndex);  // 사용한 문제는 삭제
        currentIndex = -1;
        SetRandomQuest();
    }
}