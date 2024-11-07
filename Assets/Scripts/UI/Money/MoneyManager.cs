using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static MoneyManager Instance { get; private set; }
    public CurrentMoney currentMoneyManager;        // CurrentMoney 스크립트
    public MonthlyReportUI monthlyReportUI;         // 월정산 UI 인스턴스 추가

    //public TextMeshProUGUI moneyText; // 게임 재화 Text (Inspector에서 할당)
    // 병원 의료비 설정 (일단 임의값)
    public static int MedicalFee = 1;            // 진료비
    public static int HospitalizationFee = 30;    // 입원비 한달 비용
    public static int SurgeryFee = 20;            // 수술비

    // 치료제 및 백신 가격 설정
    public static int MedicinePrice = 2;         // 치료제 가격
    public static int VaccinePrice = 1;          // 백신 가격


    private void Awake()
    {
        // 싱글톤 패턴 구현: Instance 설정 및 중복 방지
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 이 오브젝트는 씬 전환 시에도 파괴되지 않음
        }
        else
        {
            Destroy(gameObject); // 중복된 인스턴스가 생기면 파괴
        }
    }

    private void Start()
    {
        currentMoneyManager = Assign(currentMoneyManager, "CurrentMoneyManager");
        monthlyReportUI = FindObjectOfType<MonthlyReportUI>();
    }

    public void IncreaseMoney(int amount)
    {
        currentMoneyManager.CurrentMoneyGetter += amount;
        /*string moneyTextValue = moneyText.text;

        if (int.TryParse(moneyTextValue.Trim().Replace(",", ""), out int currentMoney))
        {
            currentMoney += amount;
            moneyText.text = $"{currentMoney:N0}";
            Debug.Log($"Increased money by {amount}. New total: {currentMoney}");
        }
        else
        {
            Debug.LogError("Failed to parse money amount.");
        }*/
    }

    // 치료제 사용에 따른 재화 차감
    public bool DeductForMedicine(int quantity)
    {
        int totalCost = MedicinePrice * quantity;
        if (currentMoneyManager.CurrentMoneyGetter >= totalCost)
        {
            currentMoneyManager.CurrentMoneyGetter -= totalCost;
            monthlyReportUI.AddExpenseDetail("백신", totalCost);

            Debug.Log($"Deducted {totalCost} for {quantity} medicine(s).");
            return true;
        }
        else
        {
            Debug.LogWarning("Not enough money for medicine.");
            return false;
        }
    }

    // 백신 사용에 따른 재화 차감
    public bool DeductMoney(int amount)
    {
        if (currentMoneyManager.CurrentMoneyGetter < amount)
        {
            Debug.LogWarning("Not enough money.");
            return false;
        }

        monthlyReportUI.AddExpenseDetail("백신", amount);
        currentMoneyManager.CurrentMoneyGetter -= amount;
        
        return true;
    }

    public void DeductDailyExpense()
    {
        int totalDailyExpense = 0;

        // 아이템별, 직업별 착용자 수를 저장하는 딕셔너리
        Dictionary<string, Dictionary<Role, int>> itemUsageByRole = new Dictionary<string, Dictionary<Role, int>>();

        foreach (Person person in PersonManager.Instance.GetAllPersons())
        {
            foreach (var item in person.Inventory.Values)
            {
                if (item.isEquipped)
                {
                    if (!itemUsageByRole.ContainsKey(item.itemName))
                    {
                        itemUsageByRole[item.itemName] = new Dictionary<Role, int>();
                        foreach (Role role in System.Enum.GetValues(typeof(Role)))
                        {
                            itemUsageByRole[item.itemName][role] = 0;
                        }
                    }
                    itemUsageByRole[item.itemName][person.role]++;
                }
            }
        }

        // 비용 계산 및 직업별 착용자 수 출력
        foreach (var itemEntry in itemUsageByRole)
        {
            string itemName = itemEntry.Key;
            int itemPrice = Mathf.RoundToInt(RoleInventoryManager.GetInventoryByRole(Role.Doctor)[itemName].protectionRate);

            string wearerInfo = $"{itemName} 착용 중: ";
            int totalWearerCount = 0;

            foreach (var roleEntry in itemEntry.Value)
            {
                wearerInfo += $"{roleEntry.Key}({roleEntry.Value}) ";
                totalWearerCount += roleEntry.Value;
            }

            Debug.Log(wearerInfo.Trim());
            int itemTotalCost = itemPrice * totalWearerCount;
            totalDailyExpense += itemPrice * totalWearerCount;

            // 각 아이템별 지출 내역을 월정산 UI에 추가
            monthlyReportUI.AddExpenseDetail(itemName, itemTotalCost);
        }

        // 총 일일 비용 차감
        currentMoneyManager.CurrentMoneyGetter -= totalDailyExpense;
        Debug.Log($"일일 총 비용: {totalDailyExpense}가 차감되었습니다.");
    }



    // 특정 아이템을 착용 중인 사람 수 계산
    private int GetEquippedPersonCount(string itemName)
    {
        int count = 0;
        foreach (Person person in PersonManager.Instance.GetAllPersons())
        {
            if (person.Inventory.TryGetValue(itemName, out Item item) && item.isEquipped)
            {
                count++;
            }
        }
        return count;
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
