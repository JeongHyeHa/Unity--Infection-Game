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
    public static int MedicalFee = 30;            // 진료비
    public static int HospitalizationFee = 75;    // 입원비 한달 비용
    public static int SurgeryFee = 20;            // 수술비

    // 치료제 및 백신 가격 설정
    public static int MedicinePrice = 200;         // 치료제 가격
    public static int VaccinePrice = 100;          // 백신 가격


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

        monthlyReportUI.AddExpenseDetail("치료제 사용", amount);
        currentMoneyManager.CurrentMoneyGetter -= amount;

        return true;
    }
    public void ResearchExpense()
    {
        int researchCost = 2500;
        if (currentMoneyManager.CurrentMoneyGetter >= researchCost)
        {
            currentMoneyManager.CurrentMoneyGetter -= researchCost;
            monthlyReportUI.AddExpenseDetail("연구 비용", researchCost);
        }
        else
        {
            Debug.LogWarning("NoMoney");
        }

    }

    public void DeductDailyExpense()
    {
        int totalDailyExpense = 0;

        // 아이템 이름 배열 (ItemManager와 동일한 순서로 설정)
        string[] itemNames = { "Dental 마스크", "N95 마스크", "의료용 장갑", "의료용 고글", "AP 가운" };

        // 아이템 금액 배열 (itemNames와 1:1 매칭)
        int[] itemPrices = { 10, 20, 15, 50, 25 };

        // 아이템별, 직업별 착용자 수를 저장하는 딕셔너리
        Dictionary<string, Dictionary<string, int>> itemUsageByRoleAndType = new Dictionary<string, Dictionary<string, int>>();

        foreach (Person person in PersonManager.Instance.GetAllPersons())
        {
            foreach (var item in person.Inventory.Values)
            {
                if (item.isEquipped)
                {
                    string roleKey = person.role.ToString();

                    // NurseController를 확인하여 일반 간호사와 격리 간호사를 구분
                    if (person.role == Role.Nurse)
                    {
                        NurseController nurseController = person.GetComponent<NurseController>();
                        if (nurseController != null)
                        {
                            roleKey = nurseController.isQuarantineNurse ? "QuarantineNurse" : "Nurse";
                        }
                    }

                    if (!itemUsageByRoleAndType.ContainsKey(item.itemName))
                    {
                        itemUsageByRoleAndType[item.itemName] = new Dictionary<string, int>();
                    }

                    if (!itemUsageByRoleAndType[item.itemName].ContainsKey(roleKey))
                    {
                        itemUsageByRoleAndType[item.itemName][roleKey] = 0;
                    }

                    itemUsageByRoleAndType[item.itemName][roleKey]++;
                }
            }
        }

        // 비용 계산 및 직업별 착용자 수 출력
        foreach (var itemEntry in itemUsageByRoleAndType)
        {
            string itemName = itemEntry.Key;

            // 아이템 금액을 배열에서 가져오기
            int itemIndex = System.Array.IndexOf(itemNames, itemName);
            if (itemIndex < 0)
            {
                Debug.LogWarning($"아이템 {itemName}에 대한 금액 정보를 찾을 수 없습니다.");
                continue;
            }
            int itemPrice = itemPrices[itemIndex];

            string wearerInfo = $"{itemName} 착용 중: ";
            int totalWearerCount = 0;

            foreach (var roleEntry in itemEntry.Value)
            {
                // 격리 간호사는 비용 계산에서 제외
                if (roleEntry.Key == "QuarantineNurse")
                {
                    wearerInfo += $"{roleEntry.Key}({roleEntry.Value}, 비용 제외) ";
                    continue;
                }

                wearerInfo += $"{roleEntry.Key}({roleEntry.Value}) ";
                totalWearerCount += roleEntry.Value;
            }

            //Debug.Log(wearerInfo.Trim());
            int itemTotalCost = itemPrice * totalWearerCount;
            totalDailyExpense += itemTotalCost;

            // 각 아이템별 지출 내역을 월정산 UI에 추가
            monthlyReportUI.AddExpenseDetail(itemName, itemTotalCost);
        }

        // 총 일일 비용 차감
        currentMoneyManager.CurrentMoneyGetter -= totalDailyExpense;

        Debug.Log($"총 일일 비용: {totalDailyExpense}");
    }

    public int CalculateQuarantineNurseExpense()
    {
        int totalQuarantineNurseExpense = 0;

        // 아이템 이름 배열 (ItemManager와 동일한 순서로 설정)
        string[] itemNames = { "Dental 마스크", "N95 마스크", "의료용 장갑", "의료용 고글", "AP 가운" };

        // 아이템 금액 배열 (itemNames와 1:1 매칭)
        int[] itemPrices = { 10, 20, 15, 50, 25 };

        // 아이템별, 직업별 착용자 수를 저장하는 딕셔너리
        Dictionary<string, int> quarantineNurseItemUsage = new Dictionary<string, int>();

        foreach (Person person in PersonManager.Instance.GetAllPersons())
        {
            // NurseController 확인하여 격리 간호사만 처리
            if (person.role == Role.Nurse)
            {
                NurseController nurseController = person.GetComponent<NurseController>();
                if (nurseController != null && nurseController.isQuarantineNurse)
                {
                    foreach (var item in person.Inventory.Values)
                    {
                        if (item.isEquipped)
                        {
                            if (!quarantineNurseItemUsage.ContainsKey(item.itemName))
                            {
                                quarantineNurseItemUsage[item.itemName] = 0;
                            }
                            quarantineNurseItemUsage[item.itemName]++;
                        }
                    }
                }
            }
        }

        // 비용 계산
        foreach (var itemEntry in quarantineNurseItemUsage)
        {
            string itemName = itemEntry.Key;

            // 아이템 금액을 배열에서 가져오기
            int itemIndex = System.Array.IndexOf(itemNames, itemName);
            if (itemIndex < 0)
            {
                Debug.LogWarning($"아이템 {itemName}에 대한 금액 정보를 찾을 수 없습니다.");
                continue;
            }
            int itemPrice = itemPrices[itemIndex];

            int itemTotalCost = itemPrice * itemEntry.Value;
            totalQuarantineNurseExpense += itemTotalCost;

            //Debug.Log($"격리 간호사 {itemName} 비용: {itemTotalCost} (착용 수: {itemEntry.Value})");
        }

        //Debug.Log($"격리 간호사 총 비용: {totalQuarantineNurseExpense}");
        return totalQuarantineNurseExpense;
    }


    public void DeductQuarantineNurseExpense()
    {
        // 격리 간호사 비용 계산
        int quarantineNurseCost = CalculateQuarantineNurseExpense();

        // 현재 재화 차감
        if (currentMoneyManager.CurrentMoneyGetter >= quarantineNurseCost)
        {
            currentMoneyManager.CurrentMoneyGetter -= quarantineNurseCost;
            //Debug.Log($"격리 간호사 비용 {quarantineNurseCost} 차감 완료.");

            // 월 정산 UI 업데이트
            monthlyReportUI.AddExpenseDetail("격리 간호사 비용", quarantineNurseCost);
        }
        else
        {
            Debug.LogWarning("재화가 부족하여 격리 간호사 비용을 차감할 수 없습니다.");
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
