using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ProfileInventoryManager : MonoBehaviour
{
    public Transform inventoryContainer;    // 인벤토리 아이템을 추가할 스크롤 뷰의 콘텐츠 영역
    public GameObject itemPrefab;           // 아이템 프리팹
    public TextMeshProUGUI moneyText;       // 게임 재화 Text
    public GameObject decreaseTextPrefab;   // 감소 텍스트 프리팹

    void Start()
    {
        inventoryContainer = Assign(inventoryContainer, "InventoryContent");
        itemPrefab = Assign(itemPrefab, "ItemPrefab");
        moneyText = Assign(moneyText, "MoneyInfo");
        decreaseTextPrefab = Assign(decreaseTextPrefab, "DecreasePrefab");

        // GridLayoutGroup 설정
        GridLayoutGroup gridLayoutGroup = inventoryContainer.GetComponent<GridLayoutGroup>();
        if (gridLayoutGroup != null)
        {
            gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayoutGroup.constraintCount = 1; // 한 줄에 들어가는 프로필 개수
            gridLayoutGroup.cellSize = new Vector2(200, 125); // 프로필의 크기 (필요에 따라 조정)
            gridLayoutGroup.spacing = new Vector2(10, 10); // 프로필 간의 간격
            gridLayoutGroup.padding = new RectOffset(10, 10, 10, 10); // 스크롤 뷰와의 간격
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

    // 인벤토리 활성화
    public void ShowInventory(int personID)
    {
        ClearInventory();

        Person person = PersonManager.Instance.GetPerson(personID);
        if (person == null)
        {
            Debug.LogError($"Person with ID {personID} not found.");
            return;
        }

        if (person.IsResting)
        {
            Debug.Log($"Person {person.Name} is resting. Inventory will not be shown.");
            return;
        }
        foreach (var item in person.Inventory)
        {
            GameObject itemObj = Instantiate(itemPrefab, inventoryContainer);
            TextMeshProUGUI itemNameText = itemObj.transform.Find("ItemNameText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI itemStatusText = itemObj.transform.Find("ItemStatusText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI protectionRateText = itemObj.transform.Find("ProtectionRateText").GetComponent<TextMeshProUGUI>();
            Image itemImage = itemObj.transform.Find("ItemImage").GetComponent<Image>(); // ItemImage 오브젝트에 접근

            if (itemNameText == null || itemStatusText == null)
            {
                Debug.LogError("Item prefab is missing required TextMeshProUGUI components.");
                continue;
            }

            itemNameText.text = item.Key;
            if (item.Value.isEquipped)
            {
                itemStatusText.text = "O"; // 착용 중은 "O"
                itemStatusText.color = new Color32(0, 128, 255, 255); // 밝은 파란색
            }
            else
            {
                itemStatusText.text = "X"; // 착용 안함은 "X"
                itemStatusText.color = new Color32(255, 69, 0, 255); // 빨간색
            }
            protectionRateText.text = $"+ {item.Value.protectionRate}";

            Sprite itemSprite = Resources.Load<Sprite>($"Sprites/ItemSprites/{item.Key}");
            if (itemSprite != null)
            {
                itemImage.sprite = itemSprite;
            }
            else
            {
                Debug.LogWarning($"Sprite for item '{item.Key}' not found in Resources/Sprites/ItemSprites.");
            }

        }
    }


    public void ClearInventory()
    {
        foreach (Transform child in inventoryContainer)
        {
            Destroy(child.gameObject);
        }
    }

}

