using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PolicyQuizManager : MonoBehaviour
{
    public static PolicyQuizManager Instance { get; private set; }
    public GameObject questDisfectCanvas;
    public TextMeshProUGUI disinfectQuest;
    public Button[] disinfectAnswers;
    public GameObject disWrongPanel;
    public GameObject disCorrectPanel;
    public Button disinfectXButton;
    public MonthlyReportUI monthlyReportUI;

    int randomIndex;
    string currentWard;
    string[] wardNames;
    string[] layerNames;
    Dictionary<string, List<string>> wardLayerMapping = new Dictionary<string, List<string>>();
    public static Dictionary<int, bool> isCorrect = new Dictionary<int, bool>(); // 병동별 맞춘 상태 저장
    WardState wardState;
    int wardIndex;

    // 퀴즈 질문
    public static string[] questions = {
        "병원체와 세균을 효과적으로 제거하기 위해 가장 많이 사용되는 소독제는 무엇인가요?",
        "손 소독에 적합한 알코올의 농도는 얼마인가요?",
        "세균 소독 시 가장 효과적인 소독 시간과 방법은 무엇인가요?",
        "소독제를 사용할 때 가장 주의해야 할 사항은 무엇인가요?",
        "세균성 감염을 예방하기 위해 자주 소독해야 하는 의료기구는 무엇인가요?",
        "병원체를 사멸시키기 위해 손 소독제의 주요 활성 성분으로 가장 적합한 것은 무엇인가요?",
        "의료 시설에서 사용하는 소독제로 가장 흔히 사용되는 화학 물질은 무엇인가요?",
        "알코올 소독제가 병원체와 세균을 죽이는 원리는 무엇인가요?",
        "의료기구 소독 시 가장 흔히 사용하는 열처리 방법은 무엇인가요?",
        "의료 환경에서 표면 소독 시 효과적인 방법은 무엇인가요?"
    };

    // 각 질문에 대한 선택지
    public static string[,] choices = {
        { "알코올 70%", "식염수", "증류수", "글리세린" },
        { "30%", "50%", "70%", "100%" },
        { "10초 동안 소독제 뿌리기", "30초 동안 문지르기", "1분 동안 자연 건조", "5분 동안 젖은 상태 유지" },
        { "희석하지 않고 사용하기", "환기가 잘 되는 곳에서 사용하기", "다른 소독제와 혼합하여 사용하기", "소독제를 마시지 않도록 주의하기" },
        { "청진기", "주사기", "혈압계", "전자 체온계" },
        { "벤잘코늄 클로라이드", "차아염소산 나트륨", "에탄올", "과산화수소" },
        { "염산", "차아염소산 나트륨", "설탕", "바세린" },
        { "세포벽을 파괴한다", "세포 내부로 침투하여 DNA를 변형시킨다", "세포의 수분을 증발시킨다", "세포막의 단백질을 변성시킨다" },
        { "자외선 소독", "고압 증기 멸균 (오토클레이브)", "알코올 침지", "냉동 처리" },
        { "물로만 닦기", "마른 천으로 닦기", "소독제 뿌린 후 문지르기", "자연건조 후 소독제 닦아내기" }
    };

    // 정답 배열 
    public static int[] correctAnswers = { 0, 2, 1, 1, 0, 2, 1, 3, 1, 2 };

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
        questDisfectCanvas = GameObject.Find("QuestDisinfectCanvas");
        disinfectQuest = GameObject.Find("DisinfectQuest").GetComponent<TextMeshProUGUI>();
        disWrongPanel = GameObject.Find("DisWrongPanel");
        disCorrectPanel = GameObject.Find("DisCorrectPanel");
        disinfectXButton = GameObject.Find("DisinfectXButton").GetComponent<Button>();

        //4지선다형 버튼 (DisinfectAnswerButton1~ DisinfectAnswerButton4)
        disinfectAnswers = new Button[4];
        for (int i = 0; i < disinfectAnswers.Length; i++)
        {
            int index = i;
            disinfectAnswers[index] = GameObject.Find($"DisinfectAnswerButton{index + 1}").GetComponent<Button>();
            disinfectAnswers[index].onClick.AddListener(() => OnAnswerSelected(index + 1));
        }

        disWrongPanel.SetActive(false);
        disCorrectPanel.SetActive(false);

        disinfectXButton.onClick.AddListener(() => { questDisfectCanvas.SetActive(false); BtnSoundManager.Instance.PlayButtonSound(); });

        wardNames = PolicyWard.Instance.wardNames;
        layerNames = Managers.LayerChanger.layers;

        InitializeWardLayerMapping();
    }

    // 병동별 레이어 매핑 초기화
    void InitializeWardLayerMapping()
    {
        wardLayerMapping = new Dictionary<string, List<string>>()
        {
            { wardNames[2], new List<string> { layerNames[2]} }, // 외과1
            { wardNames[3], new List<string> { layerNames[3]} }, // 외과2
            { wardNames[0], new List<string> { layerNames[0]} }, // 내과1
            { wardNames[1], new List<string> { layerNames[1] } }, // 내과2
            {wardNames[4], new List<string> {layerNames[4] } },    //입원병동 1
            {wardNames[5], new List<string> {layerNames[5]} },    //입원병동 2
            {wardNames[6], new List<string> {layerNames[6]} },    //입원병동 3
            {wardNames[7], new List<string> {layerNames[7]} },    //입원병동 4
            { wardNames[8], new List<string> { layerNames[8] } },   // 응급실
            { wardNames[9], new List<string> { layerNames[9] } }    // 중환자실/격리실
        };
    }

    // 특정 병동을 소독할 때 호출하는 함수
    public void ClearVirusesInWard(string ward, int wardId)
    {
        if (!wardLayerMapping.ContainsKey(currentWard))
        {
            Debug.LogError($"PolicyQuiz, {currentWard} 레이어에 해당하는 병동 이름이 없음");
            return;
        }

        questDisfectCanvas.SetActive(true);

        // 랜덤 문제 생성
        randomIndex = UnityEngine.Random.Range(0, questions.Length);
        disinfectQuest.text = questions[randomIndex];
        for (int i = 0; i < disinfectAnswers.Length; i++)
            disinfectAnswers[i].GetComponentInChildren<TextMeshProUGUI>().text = choices[randomIndex, i];
        Debug.Log($"PolicyQuiz, {randomIndex}의 정답은 {correctAnswers[randomIndex] + 1}");
    }

    //정답 체크
    void OnAnswerSelected(int selectedAnswerIndex)
    {
        if (selectedAnswerIndex == correctAnswers[randomIndex] + 1)
        {
            isCorrect[wardIndex] = true;
            monthlyReportUI.AddExpenseDetail("병동 소독", 500);
            StartCoroutine(ShowCorrectPanel());
        }
        else
        {
            isCorrect[wardIndex] = false;
            StartCoroutine(ShowDisWrongPanel());
        }
    }

    IEnumerator ShowCorrectPanel()
    {
        disCorrectPanel.SetActive(true);
        yield return YieldInstructionCache.WaitForSecondsRealtime(1.3f);
        disCorrectPanel.SetActive(false);
        questDisfectCanvas.SetActive(false);

        Virus[] viruses = FindObjectsOfType<Virus>();
        foreach (Virus virus in viruses)
        {
            string[] targetLayers = wardLayerMapping[currentWard].ToArray();
            string layerName = LayerMask.LayerToName(virus.gameObject.layer);
            if (targetLayers.Contains(layerName))
            {
                Destroy(virus.gameObject);
                Debug.Log($"PolicyQuiz, {virus.gameObject.name} 바이러스 제거됨. {layerName}");
            }
        }

        WardState wardState = PolicyWard.Instance.wardStates[wardIndex];
        wardState.IsDisinfecting = true;
        wardState.DisinfectEndTime = Time.time + PolicyWard.disinfectCooldownTime;
        PolicyWard.Instance.disInfectButtonText.text = $"소독 중: {Mathf.CeilToInt(PolicyWard.disinfectCooldownTime)}초 남음";
        PolicyWard.Instance.UpdateWardInfomation(wardIndex);
    }

    //오답 패널 생성
    IEnumerator ShowDisWrongPanel()
    {
        disWrongPanel.SetActive(true);
        yield return YieldInstructionCache.WaitForSecondsRealtime(1.3f);
        disWrongPanel.SetActive(false);
        questDisfectCanvas.SetActive(false);

        WardState wardState = PolicyWard.Instance.wardStates[wardIndex];
        wardState.IsDisinfecting = true;
        wardState.DisinfectEndTime = Time.time + PolicyWard.disinfectFalseTime;
        PolicyWard.Instance.disInfectButtonText.text = $"대기 중: {Mathf.CeilToInt(PolicyWard.disinfectFalseTime)}초 남음";
        PolicyWard.Instance.UpdateWardInfomation(wardIndex);
    }
}
