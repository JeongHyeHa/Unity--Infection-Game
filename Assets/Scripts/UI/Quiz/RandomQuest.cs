using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyApp.DataAccess;

public class RandomQuest : MonoBehaviour
{
    // UI references
    public Button[] answerButton;
    public Button[] levelButton;
    public GameObject clickbtn;
    public TextMeshProUGUI quest;              // UI_문제
    public GameObject wrongPanel;
    public GameObject rightPanel;
    public TMP_Text rightText;                 //rightPanel의 텍스트
    public Canvas questCanvas;                 //퀴즈창
    public GameObject coolTimePanel;

    // Reference to MySQLConnector
    public MySQLConnector mySQLConnector;

    // Other references
    public CoolTime coolTimeScript;             //coolTime 스크립트
    public MonthlyReportUI monthlyReportUI;     //MonthlyReportUI 스크립트
    public CurrentMoney currentMoneyManager;    //CurrentMoney 스크립트
    public CountdownTimer countdownTimer;       // 카운트다운 타이머 
    public TMP_Text moneyText;                  // 돈 표시 UI
    public TMP_Text timerText;                  // 쿨타임 카운트다운 텍스트

    // DB data structures
    List<int> availableIndices = null;                         //문제 인덱스 리스트
    private List<User> usersDB = new List<User>();             // DB_퀴즈 및 선택지 데이터
    private List<User> answersDB = new List<User>();           // DB_정답 데이터
    private Dictionary<string, List<int>> availableIndicesByLevel = new Dictionary<string, List<int>>();
    private Dictionary<string, float> cooldownTimers = new Dictionary<string, float>();                 // 각 레벨의 쿨타임 타이머
    private Dictionary<string, int> questCount = new Dictionary<string, int>();                         //문제 개수 제어
    private Dictionary<string, int> lastIndexPerLevel = new Dictionary<string, int>();                  //레벨별 마지막 인덱스
    private Dictionary<string, bool> firstAttemptPerLevel = new Dictionary<string, bool>();             //레벨별 첫 번째 오답 여부

    private int currentIndex = -1, lastIndex = -1;
    private float duration = 0f;
    private bool firstAttempt = true;                          // 오답 첫 번째 재도전인지 확인
    private float maxCooldown = 60f;                         // 쿨타임 최대 시간

    void Start()
    {
        InitializeUIElements();                  //오브젝트 자동 할당
        InitializeIndices();                     //레벨별 문제 개수 초기화

        foreach (var button in levelButton)
        {
            string levelName = button.name;
            cooldownTimers[levelName] = 0f;     //쿨타임 타이머 초기화
            questCount[levelName] = 0;          //퀘스트 개수 초기화
        }

        StartCoroutine(WaitForData());          //데이터 로드 완료되면 시작
    }

    //초기 오브젝트 자동 할당 관리
    private void InitializeUIElements()
    {
        answerButton = Enumerable.Range(1, 4)
            .Select(i => Assign<Button>(null, $"AnswerButton{i}"))
            .ToArray();

        levelButton = Enumerable.Range(1, 3)
            .Select(i => Assign<Button>(null, $"LevelButton{i}"))
            .ToArray();

        quest = Assign(quest, "Quest");
        rightPanel = Assign(rightPanel, "RightPanel");
        rightText = Assign(rightText, "RightText");
        wrongPanel = Assign(wrongPanel, "WrongPanel");
        coolTimePanel = Assign(coolTimePanel, "CoolTimePanel");
        coolTimeScript = Assign(coolTimeScript, "CoolTimeManager");
        mySQLConnector = Assign(mySQLConnector, "MariaDBConnector");
        questCanvas = Assign(questCanvas, "QuestCanvas");
        monthlyReportUI = FindObjectOfType<MonthlyReportUI>();
        currentMoneyManager = Assign(currentMoneyManager, "CurrentMoneyManager");
        countdownTimer = Assign(countdownTimer, "Timer");
        moneyText = Assign(moneyText, "MoneyText");
        timerText = Assign(timerText, "TimerText");

 

        if (mySQLConnector != null) 
        {
            usersDB = mySQLConnector.GetUsers();
            answersDB = mySQLConnector.GetAnswers();
        }
        else 
        {
            Debug.LogError("MySQLConnector is not assigned.");
        }

        UpdateMoneyUI();
    }

    //돈 금액 출력
    private void UpdateMoneyUI()
    {
        //현재 금액 가져오기 
        moneyText.text = $"{currentMoneyManager.CurrentMoneyGetter:N0}Sch";  // 천단위 콤마 추가
    }

    // DB 데이터 로드가 끝나면 
    IEnumerator WaitForData()
    {
        yield return new WaitUntil(() => mySQLConnector.GetUsers() != null && mySQLConnector.GetUsers().Count > 0);
        usersDB = mySQLConnector.GetUsers();
        answersDB = mySQLConnector.GetAnswers();
        //Debug.Log($"Loaded {usersDB.Count} users and {answersDB.Count} answers from the database.");

        OnLevelButtonClicked(levelButton[0]);                                       // 처음에 level1btn이 선택된 상태로 시작

        foreach (var button in levelButton)
        {
            button.onClick.AddListener(() => OnLevelButtonClicked(button));         // 버튼 클릭 이벤트 등록  
        }
    }

    //초기 레벨별 문제 인덱스 초기화
    private void InitializeIndices()
    {
        availableIndicesByLevel.Clear();
        availableIndicesByLevel["LevelButton1"] = Enumerable.Range(0, 20).ToList();     //0~19
        availableIndicesByLevel["LevelButton2"] = Enumerable.Range(20, 20).ToList();    //20~39
        availableIndicesByLevel["LevelButton3"] = Enumerable.Range(40, 60).ToList();    //40~99
    }

    // 버튼 클릭 시 호출되는 메서드
    public void OnLevelButtonClicked(Button clickedButton)
    {
        SaveCurrentState();

        clickbtn = clickedButton.gameObject;
        //Debug.Log($"{clickbtn.name}을 선택함");
        if (clickbtn == null)
        {
            Debug.LogError("현재 선택된 오브젝트가 없습니다");
            return;
        }

        //레벨별 문제 인덱스 리스트 선택
        int selectedLevel = System.Array.IndexOf(levelButton, clickedButton);
        availableIndices = availableIndicesByLevel[clickbtn.name];

        //선택된 버튼만 비활성화
        for (int i = 0; i < levelButton.Length; i++)
        {
            levelButton[i].interactable = true;
        }
        levelButton[selectedLevel].interactable = false;

        RestorePreviousState();
        ManageCooltimePanels(clickedButton.name);
        SetRandomQuest();
    }

    // 현재 상태 저장
    private void SaveCurrentState()
    {
        if (clickbtn != null)
        {
            lastIndexPerLevel[clickbtn.name] = currentIndex;    //현재 문제의 마지막 인덱스 저장
            firstAttemptPerLevel[clickbtn.name] = firstAttempt; //레벨별 오답 재도전 여부 저장
        }
    }

    // 이전 상태 복원
    private void RestorePreviousState()
    {
        if (lastIndexPerLevel.ContainsKey(clickbtn.name))
        {
            currentIndex = lastIndexPerLevel[clickbtn.name];
            firstAttempt = firstAttemptPerLevel[clickbtn.name];
        }
        else
        {
            currentIndex = -1;
            firstAttempt = true;
        }
    }

    // 난이도에 따른 문제 및 객관식 선택지 텍스트 변환
    public void SetRandomQuest()
    {
        if (!questCanvas.enabled)
        {
            //Debug.Log("Quiz Canvas is not active. Skipping question setup.");
            return;
        }

        if (availableIndices.Count == 0)
        {
            Debug.LogError("No available indices to select a question.");
            return;
        }

        //5문제 다 풀었으면 쿨타임 패널
        if (questCount[clickbtn.name] % 5 == 0 && questCount[clickbtn.name] != 0)
        {
            ShowCooltimePanel(clickbtn.name);
            return;
        }

        // 첫 번째 시도에서 틀린 경우 문제가 고정되도록 설정
        if (currentIndex == -1 && availableIndices.Count > 0)
        {
            currentIndex = availableIndices[UnityEngine.Random.Range(0, availableIndices.Count)];
        }

        //currentIndex < 0 || 
        if (currentIndex >= usersDB.Count)
        {
            Debug.LogError($"{usersDB.Count}currentIndex({currentIndex})가 users 리스트의 범위를 벗어났습니다.");
            return;
        }

        countdownTimer.StartTimer(TimerDuration(), OnTimeOut);

        //문제와 선택지를 UI에 표시
        quest.text = usersDB[currentIndex].quest;
        string[] arr_users = { usersDB[currentIndex].answer1, usersDB[currentIndex].answer2, usersDB[currentIndex].answer3, usersDB[currentIndex].answer4 };
        for (int i = 0; i < answerButton.Length; i++)
        {
            int index = i;
            answerButton[i].GetComponentInChildren<TextMeshProUGUI>().text = arr_users[i];
            answerButton[i].onClick.RemoveAllListeners();
            answerButton[i].onClick.AddListener(() => CheckAnswer(index + 1));
        }
    }

    // 정답 채점
    public void CheckAnswer(int selectedAnswer)
    {
        if (selectedAnswer == answersDB[currentIndex].right)  //정답일 경우
        {
            AddMoney();
            StartCoroutine(ShowRightPanel());
            availableIndices.Remove(currentIndex);          //문제를 풀었으면 삭제
            currentIndex = -1;                              // 문제 초기화
        }
        else
        {
            if (firstAttempt)
            {
                firstAttempt = false;
                StartCoroutine(ShowWrongPanel(false));      // 첫 번째 시도에서 틀림
            }
            else
            {
                StartCoroutine(ShowWrongPanel(true));       // 두 번째 시도에서도 틀림
                availableIndices.Remove(currentIndex);
                currentIndex = -1;
            }
        }

        foreach (var button in answerButton)                // 버튼 클릭 이벤트 초기화
        {
            button.onClick.RemoveAllListeners();
        }
    }

    //레벨별 돈 계산
    private void AddMoney()
    {
        int moneyToAdd = clickbtn.name switch
        {
            "LevelButton1" => 100,
            "LevelButton2" => 200,
            "LevelButton3" => 300,
            _ => 0
        };

        rightText.text = $"{moneyToAdd}Sch 획득하였습니다.";

        if (monthlyReportUI != null)
        {
            currentMoneyManager.CurrentMoneyGetter += moneyToAdd;
            monthlyReportUI.AddIncomeDetail("퀴즈", moneyToAdd);
        }
        else
        {
            Debug.LogError("monthlyReportUI가 null. 돈을 추가할 수 없음");
        }
        UpdateMoneyUI();
    }

    //정답 시 정답 패널 생성
    IEnumerator ShowRightPanel()
    {
        if (rightPanel != null)
        {
            rightPanel.SetActive(true);
            yield return new WaitForSecondsRealtime(1.5f);
            rightPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("rightPanel is null");
        }

        firstAttempt = true;        // 시도 횟수 초기화
        StartCoroutine(NextQuestionDelay());
    }

    //오답 시 오답 패널 생성
    IEnumerator ShowWrongPanel(bool moveToNext)
    {
        if (wrongPanel != null)
        {
            wrongPanel.SetActive(true);
            yield return new WaitForSecondsRealtime(1.5f);
            wrongPanel.SetActive(false);

            if (moveToNext)
            {
                firstAttempt = true;
                StartCoroutine(NextQuestionDelay());
            }
            else
            {
                // 같은 문제를 다시 풀기 위해 이벤트 리스너를 다시 등록
                foreach (var button in answerButton)
                {
                    button.onClick.AddListener(() => CheckAnswer(System.Array.IndexOf(answerButton, button) + 1));
                }
                countdownTimer.StartTimer(TimerDuration(), OnTimeOut);
            }
        }
        else
        {
            Debug.LogError("WrongPanel is null");
        }
    }

    //5문제를 풀 때마다 쿨타임 패널 생성
    private void ShowCooltimePanel(string levelName)
    {
        if (!cooldownTimers.ContainsKey(levelName) || cooldownTimers[levelName] <= 0)
        {
            cooldownTimers[levelName] = maxCooldown;
        }

        coolTimePanel.SetActive(true);
        coolTimeScript.StartCooldown(cooldownTimers[levelName], maxCooldown, () => OnCooldownComplete(levelName));
        countdownTimer.StopTimer();
        countdownTimer.SetTimerText(TimerDuration().ToString());
    }

    //쿨타임 패널이 끝나면 할 일
    private void OnCooldownComplete(string levelName)
    {
        if (availableIndicesByLevel[levelName].Count == 0)
        {
            InitializeIndicesForLevel(levelName);
        }

        questCount[levelName] = 0;
        cooldownTimers[levelName] = 0;
        coolTimePanel.SetActive(false);

        if (clickbtn.name == levelName)
        {
            SetRandomQuest();
        }

        UpdateMoneyUI();
    }

    //문제 난이도별 문제 개수 초기화
    private void InitializeIndicesForLevel(string levelName)
    {
        int startIdx = levelName == "LevelButton1" ? 0 : levelName == "LevelButton2" ? 20 : 40;
        int endIdx = levelName == "LevelButton1" ? 20 : levelName == "LevelButton2" ? 40 : 100;

        availableIndicesByLevel[levelName] = Enumerable.Range(startIdx, endIdx - startIdx).ToList();
    }

    //다음 문제로 전환
    IEnumerator NextQuestionDelay()
    {
        questCount[clickbtn.name]++;            //푼 문제 개수 누적
        yield return new WaitForSecondsRealtime(0.2f);
        SetRandomQuest();                       // 새로운 퀘스트와 버튼 텍스트 설정
        UpdateMoneyUI();
    }

    //쿨타임 패널 관리
    private void ManageCooltimePanels(string currentLevel)
    {
        //클릭된 버튼의 쿨타임 패널만 활성화
        if (cooldownTimers[currentLevel] > 0)
        {
            ShowCooltimePanel(currentLevel);
        }
        else
        {
            coolTimePanel.SetActive(false);
            SetRandomQuest();
        }

        UpdateMoneyUI();
    }

    void Update()
    {   //계속 쿨타임 시간 측정(유니티 기준이라 1초마다 x)
        foreach (var level in new[] { "LevelButton1", "LevelButton2", "LevelButton3" })
        {
            UpdateCooldown(level);
        }
    }

    //레벨별 쿨타임 시간 저장
    private void UpdateCooldown(string levelName)
    {
        if (cooldownTimers[levelName] > 0)
        {
            cooldownTimers[levelName] -= Time.deltaTime;
            if (cooldownTimers[levelName] <= 0)
            {
                OnCooldownComplete(levelName);
            }
        }
    }

    //레벨별 문제 카운트다운 시간 관리
    public float TimerDuration()
    {
        duration = clickbtn.name switch
        {
            "LevelButton1" => 10f,
            "LevelButton2" => 20f,
            "LevelButton3" => 30f,
            _ => 10f
        };
        return duration;
    }

    // 타이머가 끝났을 때 다음 문제로 넘어감
    private void OnTimeOut()
    {
        availableIndices.Remove(currentIndex);
        currentIndex = -1;
        StartCoroutine(NextQuestionDelay());
    }

    // 자동 할당 코드 
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