using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class AllClearManager : MonoBehaviour
{
    public Button allClearButton;           // 전체 소독 버튼
    public Image allClearCooltimeImage;     // 쿨타임을 보여줄 이미지
    public GameObject questDisfectCanvas;   // 소독 퀴즈 캔버스
    public TextMeshProUGUI disinfectQuest;  // 퀴즈 문제
    public Button[] disinfectAnswers;       // 퀴즈 선택지
    public GameObject disWrongPanel;
    public GameObject disCorrectPanel;
    public Button disinfectXButton;
    public float cooltimeDuration = 15f;    // 쿨타임 시간 (초)
    private bool isCooldown = false;        // 쿨타임 진행 여부

    // 퀴즈 질문
    string[] questions = {
        "바이러스와 세균을 효과적으로 제거하기 위해 가장 많이 사용되는 소독제는 무엇인가요?",
        "손 소독에 적합한 알코올의 농도는 얼마인가요?",
        "세균 소독 시 가장 효과적인 소독 시간과 방법은 무엇인가요?",
        "소독제를 사용할 때 가장 주의해야 할 사항은 무엇인가요?",
        "세균성 감염을 예방하기 위해 자주 소독해야 하는 의료기구는 무엇인가요?",
        "바이러스를 사멸시키기 위해 손 소독제의 주요 활성 성분으로 가장 적합한 것은 무엇인가요?",
        "의료 시설에서 사용하는 소독제로 가장 흔히 사용되는 화학 물질은 무엇인가요?",
        "알코올 소독제가 바이러스와 세균을 죽이는 원리는 무엇인가요?",
        "의료기구 소독 시 가장 흔히 사용하는 열처리 방법은 무엇인가요?",
        "의료 환경에서 표면 소독 시 효과적인 방법은 무엇인가요?"
    };

    // 각 질문에 대한 선택지
    string[,] choices = {
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
    int[] correctAnswers = { 0, 2, 1, 1, 0, 2, 1, 3, 1, 2 };

    void Start()
    {
        SetObject();
        allClearCooltimeImage.fillAmount = 0;
        allClearCooltimeImage.gameObject.SetActive(false);  //쿨타임 이미지 비활성화
        disWrongPanel.SetActive(false);                     //정답 패널 비활성화
        disCorrectPanel.SetActive(false);                     //오답 패널 비활성화
    }

    // 오브젝트 자동할당 및 클릭 이벤트 설정
    private void SetObject()
    {
        allClearButton = Assign(allClearButton, "AllClearButton");
        allClearCooltimeImage = Assign(allClearCooltimeImage, "AllClearCooltimeImage");
        questDisfectCanvas = Assign(questDisfectCanvas, "QuestDisinfectCanvas");
        disinfectQuest = Assign(disinfectQuest, "DisinfectQuest");
        disWrongPanel = Assign(disWrongPanel, "DisWrongPanel");
        disCorrectPanel = Assign(disCorrectPanel, "DisCorrectPanel");
        disinfectXButton = Assign(disinfectXButton, "DisinfectXButton");

        //선택지 버튼(DisinfectAnswerButton1~ DisinfectAnswerButton4)
        disinfectAnswers = Enumerable.Range(1, 4)
            .Select(i => Assign<Button>(null, $"DisinfectAnswerButton{i}"))
            .ToArray();

        allClearButton.onClick.AddListener(OpenDisinfectQuiz);      //전체 소독 버튼 이벤트
        disinfectXButton.onClick.AddListener(CloseDisinfectQuiz);   //닫기 버튼 이벤트
    }

    //소독 퀴즈 시작
    private void OpenDisinfectQuiz()
    {
        if (isCooldown)
        {
            Debug.Log("소독 쿨타임 중");
            return;
        }
        questDisfectCanvas.SetActive(true);

        int randomIndex = Random.Range(0, questions.Length);

        //질문 텍스트 설정
        disinfectQuest.text = questions[randomIndex];

        //선택지 텍스트 설정 & 정답 체크
        for (int i = 0; i < disinfectAnswers.Length; i++)
        {
            disinfectAnswers[i].GetComponentInChildren<TextMeshProUGUI>().text = choices[randomIndex, i];
            int answerIndex = i;
            disinfectAnswers[i].onClick.RemoveAllListeners();      //이전에 설정된 이벤트가 있다면 초기화
            disinfectAnswers[i].onClick.AddListener(() => OnAnswerSelected(answerIndex, randomIndex));
        }
    }

    //정답 체크
    void OnAnswerSelected(int selectedAnswerIndex, int questionIndex)
    {
        if (selectedAnswerIndex == correctAnswers[questionIndex])
        {
            StartCoroutine(ShowCorrectPanel());
        }
        else
        {
            StartCoroutine(ShowDisWrongPanel());
        }
    }

    //정답 패널 생성
    IEnumerator ShowCorrectPanel()
    {
        disCorrectPanel.SetActive(true);
        yield return new WaitForSeconds(1.3f);
        disCorrectPanel.SetActive(false);
        questDisfectCanvas.SetActive(false);
        DisinfectAllViruses();
        StartCoroutine(CooltimeCoroutine());
    }

    //오답 패널 생성
    IEnumerator ShowDisWrongPanel()
    {
        disWrongPanel.SetActive(true);
        yield return new WaitForSeconds(1.3f);
        disWrongPanel.SetActive(false);
        questDisfectCanvas.SetActive(false);
        StartCoroutine(CooltimeCoroutine());
    }

    //모든 바이러스 소독
    public void DisinfectAllViruses()
    {
        Virus[] viruses = Object.FindObjectsOfType<Virus>();
        foreach (Virus virus in viruses)
        {
            virus.Disinfect();
        }
        Debug.Log("모든 바이러스 소독 완료");
    }

    // 쿨타임 코루틴
    IEnumerator CooltimeCoroutine()
    {
        isCooldown = true;
        allClearButton.interactable = false;
        allClearCooltimeImage.gameObject.SetActive(true);

        float elapsedTime = 0;

        while (elapsedTime < cooltimeDuration)
        {
            elapsedTime += Time.deltaTime;
            allClearCooltimeImage.fillAmount = 1 - (elapsedTime / cooltimeDuration);
            yield return null;
        }

        allClearCooltimeImage.fillAmount = 0;
        allClearCooltimeImage.gameObject.SetActive(false);
        allClearButton.interactable = true;
        isCooldown = false;
    }

    private void CloseDisinfectQuiz()
    {
        questDisfectCanvas.SetActive(false);
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
