using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class MonthlyReportUI : MonoBehaviour
{
    public GameObject monthlyReportCanvas;  // 월정산 UI 캔버스
    public TextMeshProUGUI nowMoney;        // 월정산 이후 잔여 금액
    public TextMeshProUGUI incomeMoney;     // 한달 동안의 총 수입 금액
    public TextMeshProUGUI expenseMoney;    // 한달 동안의 총 지출 금액
    //public TextMeshProUGUI netIncomeMoney;  // 순수익 금액
    public Image exitButton;                // 월정산 나가기 버튼
    public CurrentMoney currentMoneyManager; // CurrentMoney 스크립트

    [SerializeField] private Transform incomeDetailContainer; // 수입 내역 컨테이너
    [SerializeField] private Transform expenseDetailContainer; // 지출 내역 컨테이너
    [SerializeField] private GameObject detailPrefab; // 내역을 표시할 프리팹

    private Dictionary<string, int> incomeDetails = new Dictionary<string, int>();
    private Dictionary<string, int> expenseDetails = new Dictionary<string, int>();

    private int month = 1;
    private int totalIncome = 0;
    private int totalExpense = 0;

    private void Start()
    {
        monthlyReportCanvas = Assign(monthlyReportCanvas, "MonthlyReportCanvas");
        nowMoney = Assign(nowMoney, "NowMoney");
        incomeMoney = Assign(incomeMoney, "IncomeMoney");
        expenseMoney = Assign(expenseMoney, "ExpenseMoney");
        //netIncomeMoney = Assign(netIncomeMoney, "NetIncomeMoney");
        exitButton = Assign(exitButton, "ExitButton");
        currentMoneyManager = Assign(currentMoneyManager, "CurrentMoneyManager");

        if (exitButton != null)
        {
            EventTrigger trigger = exitButton.gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((eventData) => { CloseMonthlyReport(); });
            trigger.triggers.Add(entry);
        }
    }

    private T Assign<T>(T obj, string objectName) where T : Object
    {
        if (obj == null)
        {
            GameObject foundObject = GameObject.Find(objectName);
            if (foundObject != null)
            {
                if (typeof(Component).IsAssignableFrom(typeof(T)))
                    obj = foundObject.GetComponent(typeof(T)) as T;
                else if (typeof(GameObject).IsAssignableFrom(typeof(T)))
                    obj = foundObject as T;
            }
            if (obj == null)
                Debug.LogError($"{objectName} 를 찾을 수 없습니다.");
            else
                Debug.Log($"{objectName} 를 성공적으로 찾았습니다.");
        }
        return obj;
    }

    private void AddDetailToContainer(Transform container, string detailName, int amount)
    {
        if (detailPrefab == null) return;

        GameObject detailItem = Instantiate(detailPrefab, container);
        TextMeshProUGUI[] texts = detailItem.GetComponentsInChildren<TextMeshProUGUI>();

        if (texts.Length >= 2)
        {
            texts[0].text = detailName;         // 첫 번째 텍스트에 detailName 설정
            texts[1].text = $"{amount:N0} sch";  // 두 번째 텍스트에 amount 설정
        }
        else
        {
            Debug.LogError("Detail prefab does not have sufficient TextMeshProUGUI components.");
        }
    }

    private void UpdateIncomeDetailText(string detailName, int amount)
    {
        foreach (Transform item in incomeDetailContainer)
        {
            TextMeshProUGUI[] texts = item.GetComponentsInChildren<TextMeshProUGUI>();
            if (texts.Length >= 2 && texts[0].text == detailName)
            {
                texts[1].text = $"{amount:N0} sch"; // 수입 금액 갱신
                break;
            }
        }
    }

    private void UpdateExpenseDetailText(string detailName, int amount)
    {
        foreach (Transform item in expenseDetailContainer)
        {
            TextMeshProUGUI[] texts = item.GetComponentsInChildren<TextMeshProUGUI>();
            if (texts.Length >= 2 && texts[0].text == detailName)
            {
                texts[1].text = $"{amount:N0} sch"; // 지출 금액 갱신
                break;
            }
        }
    }

    public void AddIncomeDetail(string detailName, int amount)
    {
        if (incomeDetails.ContainsKey(detailName))
        {
            incomeDetails[detailName] += amount;
        }
        else
        {
            incomeDetails[detailName] = amount;
            AddDetailToContainer(incomeDetailContainer, detailName, incomeDetails[detailName]);
        }
        UpdateIncomeDetailText(detailName, incomeDetails[detailName]);
        AddIncome(amount);
    }

    public void AddExpenseDetail(string detailName, int amount)
    {
        if (expenseDetails.ContainsKey(detailName))
        {
            expenseDetails[detailName] += amount;
        }
        else
        {
            expenseDetails[detailName] = amount;
            AddDetailToContainer(expenseDetailContainer, detailName, expenseDetails[detailName]);
        }
        UpdateExpenseDetailText(detailName, expenseDetails[detailName]);
        AddExpense(amount);
    }

    public void AddExpense(int amount)
    {
        totalExpense += amount;
        UpdateExpenseText();
    }

    public void AddIncome(int amount)
    {
        totalIncome += amount;
        UpdateIncomeText();
    }

    private void UpdateExpenseText()
    {
        expenseMoney.text = $"{totalExpense:N0} sch";
        UpdateNetIncomeText();
    }

    private void UpdateIncomeText()
    {
        incomeMoney.text = $"{totalIncome:N0} sch";
        UpdateNetIncomeText();
    }

    private void UpdateNetIncomeText()
    {
        int netIncome = totalIncome - totalExpense;
        //netIncomeMoney.text = $"{netIncome:N0} 원";
    }

    public void UpdateNowMoney()
    {
        nowMoney.text = $"{currentMoneyManager.CurrentMoneyGetter:N0} sch";
    }

    public void ShowMonthlyReport()
    {
        if (monthlyReportCanvas != null)
        {
            UpdateNowMoney();
            monthlyReportCanvas.SetActive(true);
            Time.timeScale = 0;
        }
    }

    private void CloseMonthlyReport()
    {
        if (monthlyReportCanvas != null)
        {
            monthlyReportCanvas.SetActive(false);
            month++;
            Time.timeScale = 1;
        }

        ResetMonthlyReport();
    }

    private void ResetMonthlyReport()
    {
        totalIncome = 0;
        totalExpense = 0;
        incomeDetails.Clear();
        expenseDetails.Clear();
        foreach (Transform child in incomeDetailContainer)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in expenseDetailContainer)
        {
            Destroy(child.gameObject);
        }
        UpdateIncomeText();
        UpdateExpenseText();
        UpdateNetIncomeText();
    }
}
