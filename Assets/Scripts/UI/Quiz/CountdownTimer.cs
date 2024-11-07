using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CountdownTimer : MonoBehaviour
{
    //public TMP_Text timerText;        // 타이머 UI
    public TextMeshProUGUI timerText;
    public Image shadowImage;         // Shadow 이미지
    private float timeRemaining;      // 남은 시간
    private bool isTiming;            // 타이머 진행 중인지 확인
    private System.Action onTimerEnd; // 타이머 종료 시 호출될 콜백
    private float maxDuration;        // 최대 타이머 시간

    void Start()
    {
        //오브젝트 자동할당
        timerText = Assign(timerText, "TimerText");
        shadowImage = Assign(shadowImage, "TimerShadow");

        if (timerText == null)
            Debug.LogError("Timertext is null");
        if (shadowImage == null)
            Debug.LogError("shadowImage is null");
    }

    void Update()
    {
        if (isTiming)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerUI();

            if (timeRemaining <= 0)
            {
                isTiming = false;
                timeRemaining = 0;
                OnTimerEnd();
            }
        }
    }

    // 자동 할당 코드  //추가
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

    // 타이머 시작 메서드
    public void StartTimer(float duration, System.Action timerEndCallback)
    {
        maxDuration = duration; // 최대 시간 설정
        timeRemaining = duration;
        onTimerEnd = timerEndCallback;
        isTiming = true;
        UpdateTimerUI();
    }

    // 타이머 멈추기 메서드 추가
    public void StopTimer()
    {
        isTiming = false;
    }

    public void ResetTimer()
    {
        timeRemaining = maxDuration;
        UpdateTimerUI();
    }

    public void SetTimerText(string text)
    {
        if (timerText != null)
        {
            timerText.text = text;
        }
    }

    // 타이머 UI 업데이트 메서드
    private void UpdateTimerUI()
    {
        timerText = GameObject.Find("TimerText").GetComponent<TextMeshProUGUI>();
        timerText.text = Mathf.Ceil(timeRemaining).ToString();

        // Shadow 영역 업데이트
        float fillAmount = timeRemaining / maxDuration;
        shadowImage = GameObject.Find("TimerShadow").GetComponent<Image>();
        shadowImage.fillAmount = fillAmount;
    }

    // 타이머 종료 시 호출될 메서드
    private void OnTimerEnd()
    {
        onTimerEnd?.Invoke();
    }
}
