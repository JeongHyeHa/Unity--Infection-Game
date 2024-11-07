using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;



class PolicyItemInfo
{
    public string[] itemInfos = new string[]
    {
        "Dental 마스크|1|비말 차단 + 미세 입자 방어|방수 외부, 고성능 필터, 부드러운 내부 레이어의\n3중 구조로 이루어진 일회용 의료 마스크입니다.\n주로 의료 환경에서 감염 예방을 위해 사용되며,\n착용감이 뛰어나고 장시간 사용에도 불편함이 적습니다.",
        "일회용 장갑|2|비말 및 오염 방지|의료 환경에서 감염 예방을 위해 사용되는 일회용 장갑으로\n피부 오염을 방지하며, 사용이 간편하고 다양한 크기로\n제공되어 의료 종사자들이 효율적으로 사용할 수 있습니다.",
        "N95 마스크|3|미세 입자 및 비말 차단|고효율 필터로 미세 입자와 비말을 효과적으로\n차단하는 의료용 마스크로, 높은 밀착성과 편안한\n착용감을 제공하여 감염 예방에 필수적입니다.",
        "라텍스 장갑|4|세균 및 바이러스 차단|밀착성이 뛰어난 라텍스 소재로 세균 및 바이러스로부터\n손을 보호하며, 내구성이 우수하여 의료 환경에서\n사용하기 적합한 장갑입니다.",
        "의료용 고글|5|눈 보호 및 비말 차단|의료 현장에서 눈을 보호하고 비말로부터 안전하게\n유지하는 의료용 보호 고글로, 김 서림 방지 처리가 되어\n시야 확보에 용이합니다.",
        "의료용 헤어캡|6|머리카락 오염 방지|의료 환경에서 머리카락을 감싸 비말과 오염으로부터\n보호하는 헤어캡으로, 신축성이 좋아 다양한 머리 크기에\n맞게 착용 가능합니다.",
        "AP 가운|7|전신 감염 방어|비말과 체액으로부터 전신을 보호하는 방수 의료용 가운으로,\n내구성이 뛰어나며 의료 종사자의 안전을 보장합니다.",
        "Level C|8|고위험 감염 보호|Level C 보호 장비로 고위험 환경에서 의료 종사자를\n전신 감염으로부터 보호하며, 편리한 착용과\n빠른 탈의가 가능하여 긴급 상황에 적합합니다."
    };
}

public class PolicyItem : MonoBehaviour
{
    public static PolicyItem Instance { get; private set; } // 싱글톤 인스턴스

    public GameObject itemInfoPrefab;
    public Transform itemScrollViewContent;
    private PolicyItemInfo policyItemInfo = new PolicyItemInfo();

    // 각 아이템별 직업별 착용 상태 관리
    private Dictionary<string, Dictionary<string, bool>> equippedStatesByJob = new Dictionary<string, Dictionary<string, bool>>();

    private string[] jobNames = { "Doctor", "Nurse", "Outpatient", "Inpatient", "EmergencyPatient" }; // 직업명

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
        ConfigureGridLayout();
        AdjustScrollSpeed();
        InitializeDefaultEquippedStates();
        CreateItemEntries();
    }

    void ConfigureGridLayout()
    {
        GridLayoutGroup gridLayoutGroup = itemScrollViewContent.GetComponent<GridLayoutGroup>();
        if (gridLayoutGroup != null)
        {
            gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayoutGroup.constraintCount = 1;
            gridLayoutGroup.cellSize = new Vector2(1850, 400);
            gridLayoutGroup.spacing = new Vector2(30, 30);
            gridLayoutGroup.padding = new RectOffset(60, 60, 30, 30);
        }
    }

    void AdjustScrollSpeed()
    {
        ScrollRect scrollRect = itemScrollViewContent.GetComponentInParent<ScrollRect>();
        if (scrollRect != null)
        {
            scrollRect.scrollSensitivity = 80.0f;
        }
    }

    void InitializeDefaultEquippedStates()
    {
        foreach (var itemInfo in policyItemInfo.itemInfos)
        {
            string[] itemDetails = itemInfo.Split('|');
            if (itemDetails.Length > 0)
            {
                string itemName = itemDetails[0];
                equippedStatesByJob[itemName] = new Dictionary<string, bool>();

                // 각 직업별 초기 착용 상태 설정
                foreach (var jobName in jobNames)
                {
                    equippedStatesByJob[itemName][jobName] = false; // 기본값은 착용 해제
                }
            }
        }
    }

    void CreateItemEntries()
    {
        // i: 각 아이템 번호(0~7)
        for (int i = 0; i < policyItemInfo.itemInfos.Length; i++)
        {
            string itemInfo = policyItemInfo.itemInfos[i];
            string[] itemDetails = itemInfo.Split('|');

            if (itemDetails.Length == 4)
            {
                GameObject itemInstance = Instantiate(itemInfoPrefab, itemScrollViewContent);

                string itemName = itemDetails[0];
                string itemPrice = itemDetails[1];
                string itemEffect = itemDetails[2];
                string itemInformation = itemDetails[3];

                Image itemIcon = itemInstance.transform.Find("ItemImageSlot/ItemIcon").GetComponent<Image>();
                TextMeshProUGUI itemNameText = itemInstance.transform.Find("ItemNameSlot/ItemName").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI itemPriceText = itemInstance.transform.Find("ItemInfoSlot/ItemPrice").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI itemEffectText = itemInstance.transform.Find("ItemInfoSlot/ItemEffect").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI itemInformationText = itemInstance.transform.Find("ItemInfoSlot/ItemInfomation").GetComponent<TextMeshProUGUI>();
                Sprite itemSprite = Resources.Load<Sprite>($"Sprites/ItemSprites/{itemName}");

                itemNameText.text = itemName;
                itemPriceText.text = itemPrice + " SCH(인당)";
                itemEffectText.text = itemEffect;
                itemInformationText.text = itemInformation;
                itemIcon.sprite = itemSprite;

                Slider[] itemSwitches = new Slider[5];

                // j: 각 아이템의 토글 번호(0~4) 
                for (int j = 0; j < itemSwitches.Length; j++)
                {
                    int switchIndex = j;
                    //string toggleNumber = $"{i + 1}.{switchIndex + 1}"; // 현재 선택한 토글 번호(1.1 ~ 1.5)
                    int toggleNumber = i + 1; // 현재 선택한 토글 번호(1~8s)

                    string switchPath = $"ItemWearToggle/ItemToggle{j + 1}/Outline/ItemSwitch";  
                    Slider itemSwitch = itemInstance.transform.Find(switchPath).GetComponent<Slider>();

                    // 토글이 변경될 때 번호를 포함하여 호출
                    itemSwitch.onValueChanged.AddListener(delegate (float value){
                        int toggleState = value == 1 ? 1 : 0;
                        ResearchDBManager.Instance.AddResearchData(ResearchDBManager.ResearchMode.gear, toggleNumber, switchIndex + 1, toggleState);
                        //Debug.Log($"toggleNum {toggleNumber}, changed for item {itemName}");    
                        OnSwitchValueChanged(itemName, jobNames[switchIndex], itemInstance); 
                    });

                    itemSwitches[j] = itemSwitch;
                }

                // 예외 처리: 특정 아이템은 일부 토글을 비활성화
                if (itemName != "Dental 마스크" && itemName != "N95 마스크")
                {
                    for (int k = 2; k <= 4; k++)
                    {
                        Image switchBackground = itemInstance.transform.Find($"ItemWearToggle/ItemToggle{k + 1}/Outline/ItemSwitch/Background").GetComponent<Image>();
                        itemSwitches[k].interactable = false;
                        switchBackground.color = Color.gray;
                    }
                }
            }
            else
            {
                Debug.LogError($"Item details format is incorrect for item {i + 1}.");
            }
        }
    }

    void OnSwitchValueChanged(string itemName, string jobName, GameObject itemInstance)
    {
        Slider itemSwitch = itemInstance.transform.Find($"ItemWearToggle/ItemToggle{System.Array.IndexOf(jobNames, jobName) + 1}/Outline/ItemSwitch").GetComponent<Slider>();
        bool isEquipping = itemSwitch.value == 1;

        // 직업별 착용 상태 업데이트
        if (equippedStatesByJob.ContainsKey(itemName))
        {
            equippedStatesByJob[itemName][jobName] = isEquipping;
        }

        // 특정 직업군의 Person 객체의 Inventory 상태를 갱신
        List<Person> persons = PersonManager.Instance.GetAllPersons();
        foreach (Person person in persons)
        {
            if (person.role == GetRoleFromJobName(jobName))
            {
                if (person.Inventory.TryGetValue(itemName, out Item item))
                {
                    item.isEquipped = isEquipping;
                }
            }
        }

        // 10초 동안 상호작용 비활성화 처리
        itemSwitch.interactable = false;
        StartCoroutine(ReenableSwitchAfterCooldown(itemSwitch, 10f)); // 10초 후 다시 활성화
    }


    private IEnumerator ReenableSwitchAfterCooldown(Slider itemSwitch, float cooldownTime)
    {
        yield return new WaitForSeconds(cooldownTime);
        itemSwitch.interactable = true;
    }

    private Role GetRoleFromJobName(string jobName)
    {
        return jobName switch
        {
            "Doctor" => Role.Doctor,
            "Nurse" => Role.Nurse,
            "Outpatient" => Role.Outpatient,
            "Inpatient" => Role.Inpatient,
            "EmergencyPatient" => Role.EmergencyPatient,
            _ => throw new System.ArgumentException("Invalid job name")
        };
    }
}

