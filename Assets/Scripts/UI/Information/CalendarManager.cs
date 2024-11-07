using System.Collections;
using UnityEngine;
using TMPro;

public class CalendarManager : MonoBehaviour
{
    public TextMeshProUGUI dateText;
    public TextMeshProUGUI monthText;
    public int currentDay = 1;
    private int currentMonth = 1;
    private Coroutine dateCoroutine;
    private const int maxDaysInMonth = 28; // 한 달 최대 일 수

    private MonthlyReportUI monthlyReportUI;
    private readonly string[] monthNames = { "Jan.", "Feb.", "Mar.", "Apr.", "May.", "Jun.", "Jul.", "Aug.", "Sep.", "Oct.", "Nov.", "Dec." };

    void Start()
    {
        Debug.Log("Start 메소드 호출됨");
        dateText = Assign(dateText, "DateText");
        monthText = Assign(monthText, "MonthText");
        monthlyReportUI = FindObjectOfType<MonthlyReportUI>();
        UpdateMonthText();
    }

    // 오브젝트 자동 할당
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
            else Debug.Log($"{objectName} 를 성공적으로 찾았습니다.");
        }
        return obj;
    }

    // 달력 시작 메서드
    public void StartCalendar()
    {
        if (dateCoroutine == null)
        {
            dateCoroutine = StartCoroutine(UpdateDate());
        }
    }

    void OnDisable()
    {
        if (dateCoroutine != null)
        {
            StopCoroutine(dateCoroutine); // 날짜 업데이트 코루틴 중지
            dateCoroutine = null;
        }
    }

    // 10초 당 1일 날짜 증가시키는 코루틴
    private IEnumerator UpdateDate()
    {
        while (true)
        {
            yield return new WaitForSeconds(10); // 10초 대기
            IncrementDay();
        }
    }

    // 날짜 증가 및 텍스트 업데이트
    void IncrementDay()
    {
        currentDay++;
        MoneyManager.Instance.DeductDailyExpense(); // 매일 비용 차감

        if (currentDay > maxDaysInMonth)
        {
            currentDay = 1; // 한 달 최대 일 수 초과 시 초기화
            IncrementMonth();
        }
        UpdateDateText();
    }

    // 월 증가 및 텍스트 업데이트
    void IncrementMonth()
    {
        currentMonth++;
        if (currentMonth > 12)
        {
            currentMonth = 1; // 12월 초과 시 1월로 초기화
        }
        UpdateMonthText();
        HandleEndOfMonth();
    }

    // 날짜 텍스트 업데이트
    void UpdateDateText()
    {
        dateText.text = $"{currentDay.ToString()}th";
    }

    // 월 텍스트 업데이트
    void UpdateMonthText()
    {
        monthText.text = monthNames[currentMonth - 1];
    }

    //게임 일시정지 및 월 정산 표시
    void HandleEndOfMonth()
    {
        Time.timeScale = 0;
        if (monthlyReportUI != null)
        {
            monthlyReportUI.ShowMonthlyReport();    // 월 정산 목록 표시
        }
        else Debug.LogError("월정산목록 표시 안댐");
    }
}
