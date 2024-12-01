using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // UI 관련 네임스페이스 추가
using UnityEngine.SceneManagement;
using System.Collections;
using MyApp.UserManagement;

public class TutorialController : MonoBehaviour
{
    [SerializeField]
    private List<TutorialBase>  tutorials;              // 튜토리얼 목록들
    [SerializeField]
    private string              nextSceneNamge = "";    // 다음 씬 이름

    private TutorialBase        currentTutorial = null; // 현재 진행될 튜토리얼
    private int                 currentIndex = -1;      // 튜토리얼 Index

    private bool isPaused = false;                      // 게임 멈춰 있는지 여부

    // DB로부터 받은 튜토리얼 완료 여부를 저장할 변수
    public bool hasCompletedTutorial;       

    private MaskController maskController;              // MaskController 참조
    private NewsController newscontroller;

    public string id;
    public string username;

    // 튜토리얼 스킵 여부를 묻는 UI 창
    [SerializeField] private GameObject tutorialSkipPromptUI;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    public GameObject blockPanel; // 칸막이 역할 (버튼 막기용)

    // 로그인 시스템으로부터 튜토리얼 완료 여부를 받는 메서드 (가정)
    public void SetTutorialCompletionStatus(bool completed)
    {
        hasCompletedTutorial = completed;
    }

    public void TutorialCheck()
    {
        // 만약 사용자가 이미 튜토리얼을 완료했다면 스킵 여부 묻는 UI 창을 띄움
        if (hasCompletedTutorial)
        {
            ShowTutorialSkipPrompt();
        }
        else
        {
            // 튜토리얼 시작
            StartTutorial();
        }
    }

    private void ShowTutorialSkipPrompt()
    {
        // 튜토리얼 스킵 여부를 묻는 UI 창을 활성화
        tutorialSkipPromptUI.SetActive(true);

        // 버튼 클릭 리스너 등록
        yesButton.onClick.AddListener(SkipTutorials);
        noButton.onClick.AddListener(StartTutorial);
    }

    private void SkipTutorials()
    {
        // "네"를 선택했을 때 튜토리얼을 건너뜀
        CompletedAllTutorials();
        BtnSoundManager.Instance.PlayButtonSound();
        tutorialSkipPromptUI.SetActive(false);
    }

    private void StartTutorial()
    {
        // "아니요"를 선택했거나 튜토리얼을 처음 진행할 때 실행
        tutorialSkipPromptUI.SetActive(false);  // UI 창 비활성화
        BtnSoundManager.Instance.PlayButtonSound();
        maskController = GetComponent<MaskController>();

        // 칸막이 활성화
        if (blockPanel != null)
        {
            blockPanel.SetActive(true);
        }

        SetNextTutorial();
    }

    private void Update()
    {
        if(currentTutorial != null)
        {
            currentTutorial.Execute(this);
        }
    }

    public void SetNextTutorial()
    {
        // 현재 튜토리얼의 Exit() 메소드 호출
        if ( currentTutorial != null )
        {
            currentTutorial.Exit();
        }

        // 마지막 튜토리얼을 진행했다면 CompletedAllTutorials() 메소드 호출
        if(currentIndex >= tutorials.Count-1)
        {
            CompletedAllTutorials();
            return;
        }

        // 다음 튜토리얼 과정을 currentTutorial로 등록
        currentIndex++;
        currentTutorial = tutorials[currentIndex];

        // 새로 바뀐 튜토리얼의 Enter() 메소드 호출
        currentTutorial.Enter();

        // 마스크 업데이트
        maskController.UpdateMask(currentIndex);
    }

    public void CompletedAllTutorials()
    {
        currentTutorial = null;
        //Debug.Log("Compleye All");
        newscontroller = FindObjectOfType<NewsController>();
        Managers.PatientCreator.startSignal = true;         // 튜토리얼 끝나면 npc 생성 시작
        GoToGame.Instance.calendarManager.StartCalendar();  // 튜토리얼 끝나면 시간 흐름
        //newscontroller.TriggerVirusOutbreakNews();          // 뉴스 발생
        newscontroller.CheckWorldFirstInfectionNews();
        UserManager.Instance.AddUser(id, username, AuthManager.Instance.password, 1, " ", " ", " ");      // 튜토리얼 완료 여부 DB에 저장
        GameDataManager.Instance.InsertInitialData();       // 감염률 수집 시작
        // 칸막이 비활성화
        if (blockPanel != null)
        {
            blockPanel.SetActive(false);
        }
    }

    // 현재 튜토리얼에서 대화가 나오면 게임을 멈춥니다.
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // 게임 멈추기
    }

    // 대화가 끝나면 게임을 다시 시작합니다.
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // 게임 다시 시작
    }

    public IEnumerator Delay()
    {
        // 설정된 시간만큼 기다림
        yield return YieldInstructionCache.WaitForSecondsRealtime(0.3f);

        // 다음 튜토리얼로 이동
        SetNextTutorial();
    }

}
