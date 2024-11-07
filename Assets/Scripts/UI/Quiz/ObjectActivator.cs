using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class ObjectActivator : MonoBehaviour
{
    public Button activateButton;         // 활성화할 버튼
    public Button quizCloseButton;        //퀴즈창 닫기 버튼
    public Canvas questCanvas;            //퀴즈창
    public CountdownTimer countdownTimer; // 카운트다운 타이머 
    public RandomQuest randomQuest;       // RandomQuest 스크립트

    void Start()
    {
        //자동할당
        activateButton = Assign(activateButton, "QuizStartButton");
        questCanvas = Assign(questCanvas, "QuestCanvas");
        quizCloseButton = Assign(quizCloseButton, "XButton");
        countdownTimer = FindObjectOfType<CountdownTimer>();
        //countdownTimer = Assign(countdownTimer, "Timer");
        randomQuest = Assign(randomQuest, "QuestCanvas");

        // 초기 상태 설정: 퀴즈창을 보이지 않게 설정
        SetQuestCanvasVisible(false);

        if (activateButton != null)
        {
            activateButton.onClick.AddListener(OnActivateButtonClick);
        }

        if (quizCloseButton != null)
        {
            quizCloseButton.onClick.AddListener(OnDisactivateButtonClick);
        }
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

    public void OnActivateButtonClick()
    {
        SetQuestCanvasVisible(true);
        Time.timeScale = 0f;

        // 레벨 1을 기본으로 설정
        if (randomQuest != null)
        {
            randomQuest.OnLevelButtonClicked(randomQuest.levelButton[0]);
        }

        //레벨1의 카운트다운 타이머 설정
        if (countdownTimer != null)
        {
            countdownTimer.StartTimer(10f, null);
        }
    }

    //QuestCanvas가 비활성화돼도 게임 로직이나 쿨타임 패널은 정상 유지
    public void OnDisactivateButtonClick()
    {
        SetQuestCanvasVisible(false);
        Time.timeScale = 1f;

        // 카운트다운 타이머 멈추기
        if (countdownTimer != null)
        {
            countdownTimer.StopTimer();
            countdownTimer.ResetTimer();
        }

        // 퀴즈 답변과의 상호작용을 방지하기 위해 선택적으로 버튼 리스너를 지움
        if (randomQuest != null)
        {
            foreach (var button in randomQuest.answerButton)
            {
                button.onClick.RemoveAllListeners();  // Clear all listeners from answer buttons
            }
        }
    }

    private void SetQuestCanvasVisible(bool isVisible)
    {
        if (questCanvas != null)
        {
            questCanvas.enabled = isVisible;

            // CanvasGroup 속성 조정
            CanvasGroup canvasGroup = questCanvas.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.interactable = isVisible;
                canvasGroup.blocksRaycasts = isVisible;
            }
            if (!isVisible && countdownTimer != null)
            {
                countdownTimer.StopTimer();
                countdownTimer.ResetTimer();
            }
        }
        else
        {
            Debug.LogError("QuestCanvas is not assigned or found.");
        }
    }
}
