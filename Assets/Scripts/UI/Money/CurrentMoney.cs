using UnityEngine;
using TMPro;

public class CurrentMoney : MonoBehaviour
{
    public TextMeshProUGUI moneyInfo;       // 현재 금액
    public MonthlyReportUI monthlyReport;

    void Start()
    {
        moneyInfo = Assign(moneyInfo, "MoneyInfo");
        monthlyReport = Assign(monthlyReport, "InGameUI Manager");
    }

    //외부에서 현재 금액을 변수처럼 사용하기
    public int CurrentMoneyGetter
    {
        get
        {
            if (moneyInfo == null)
            {
                Debug.LogError("moneyInfo가 null입니다. CurrentMoney를 가져올 수 없습니다.");
                return 0;
            }
            if (string.IsNullOrEmpty(moneyInfo.text))
            {
                Debug.LogError("moneyInfo.text가 비어있습니다. CurrentMoney를 가져올 수 없습니다.");
                return 0;
            }

            if (int.TryParse(moneyInfo.text.Trim().Replace(",", "").Trim(), out int currentMoney))
            {
                return currentMoney;
            }
            else
            {
                Debug.LogError("돈 가져오기 실패");
                return 0;
            }
        }
        set
        {
            moneyInfo.text = $"{value:N0}";
            monthlyReport.UpdateNowMoney();   //금액이 변경되면 잔여 금액도 업데이트
        }
    }

    // 오브젝트 자동 할당
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
        }
        return obj;
    }
}
