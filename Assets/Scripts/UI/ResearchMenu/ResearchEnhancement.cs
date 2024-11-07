using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

class ResearchInfomation
{
    public string medicalResearch_0_1 = "Dental 마스크 착용|의료진이 기본적인 Dental 마스크를 착용하여 \n초기 단계 감염 예방 조치를 시행합니다.\n이 마스크는 비말 차단을 통해 감염병의\n확산을 줄이는 데 중요한 역할을 합니다.";
    public string medicalResearch_0_2 = "일회용 장갑 착용|의료진이 환자와의 접촉 시 감염을 예방하기 위해\n일회용 장갑을 착용합니다.\n장갑은 환자 사이의 교차 감염을 줄이기 위한\n필수 보호 장비입니다.";

    public string medicalResearch_1_1 = "N95 마스크 착용|의료진이 N95 마스크를 착용하여 공기 중에\n떠다니는 미세 입자로부터 보호를 받습니다.\n이 마스크는 고위험 상황에서 감염을 \n방지하는 것에 중요한 역할을 합니다.";
    public string medicalResearch_1_2 = "라텍스 장갑 착용|의료진이 환자의 체액 및 혈액으로부터 손을\n보호하기 위해 고품질의 라텍스 장갑을 착용합니다.\n이 장갑은 민감한 의료 절차 시 안전을 보장합니다.";

    public string medicalResearch_2_1 = "의료용 고글 착용|의료진이 눈을 통해 바이러스가 \n침투하는 것을 막기 위해 \n의료용 고글을 착용합니다.";
    public string medicalResearch_2_2 = "의료용 헤어캡 착용|의료진이 머리카락을 통한 \n오염을 막기 위해 \n의료용 헤어캡을 착용합니다.";
    public string medicalResearch_2_3 = "AP 가운 착용|다양한 유해물질로부터 \n보호받기 위해 의료진이\nAP 가운을 착용하여\n전신을 보호합니다.";

    public string medicalResearch_3_1 = "Level C 착용|의료진이 완전한 Level C 보호 장비를 착용하여, PAPR, Level C 보호복, 팔토시, 덧신, 장갑,\n마스크 등을 갖추고 고위험 감염 상황에서 철저한 방호를 시행합니다.\n이 장비는 매우 높은 보호 수준을 제공합니다.";

    public string medicalResearch_4_1 = "의료진 훈련 I|의료진에게 감염병의 기초 지식과 기본적인 대응 방법을 교육하여, \n감염 상황에서 신속하고 효율적인 조치를 취할 수 있도록 훈련합니다.\n이를 통해 의료진의 감염 방지율을 10 향상시킵니다.";
    public string medicalResearch_5_1 = "의료진 훈련 II|의료진이 고위험 상황에서의 대응 능력을 강화하기 위한 심화 훈련을 진행합니다.\n이를 통해 예기치 못한 위기 상황에서도 의료진의 감염 예방 능력이 대폭 향상됩니다.\n의료진의 감염방지율을 20 향상시킵니다.";
    public string medicalResearch_6_1 = "의료진 훈련 III|의료진이 실제 감염 상황에서 실시간으로 적절한 조치를 취할 수 있는 고급 대응 능력을 배양합니다.\n이 훈련을 통해 의료진은 감염병 확산을 효과적으로 방지할 수 있습니다.\n의료진의 감염방지율을 30 향상시킵니다.";

}

public class ResearchEnhancement : MonoBehaviour
{
    public IsResearch isResearch;

    public Image medicalResearch_0;
    public Image medicalResearch_1;
    public Image medicalResearch_2;
    public Image medicalResearch_3;
    public Image medicalResearch_4;
    public Image medicalResearch_5;
    public Image medicalResearch_6;

    public GameObject medicalLock_0;
    public GameObject medicalLock_1;
    public GameObject medicalLock_2;
    public GameObject medicalLock_3;
    public GameObject medicalLock_4;
    public GameObject medicalLock_5;
    public GameObject medicalLock_6;

    public GameObject researchItem_1;
    public GameObject researchItem_2;
    public GameObject researchItem_3;
    public TextMeshProUGUI item_1_Headline;
    public TextMeshProUGUI item_2_Headline;
    public TextMeshProUGUI item_3_Headline;
    public TextMeshProUGUI item_1_Explain;
    public TextMeshProUGUI item_2_Explain;
    public TextMeshProUGUI item_3_Explain;

    public Image researchButton;
    public TextMeshProUGUI mainResearchText;
    public TextMeshProUGUI isResearchText;
    public Sprite unlockIcon;

    private int selectedResearchIndex = 0;

    // Start is called before the first frame update
    void Awake()
    {
        medicalResearch_0 = Assign(medicalResearch_0, "MedicalResearch_0");
        medicalResearch_1 = Assign(medicalResearch_1, "MedicalResearch_1");
        medicalResearch_2 = Assign(medicalResearch_2, "MedicalResearch_2");
        medicalResearch_3 = Assign(medicalResearch_3, "MedicalResearch_3");
        medicalResearch_4 = Assign(medicalResearch_4, "MedicalResearch_4");
        medicalResearch_5 = Assign(medicalResearch_5, "MedicalResearch_5");
        medicalResearch_6 = Assign(medicalResearch_6, "MedicalResearch_6");

        medicalLock_0 = Assign(medicalLock_0, "MedicalLock_0");
        medicalLock_1 = Assign(medicalLock_1, "MedicalLock_1");
        medicalLock_2 = Assign(medicalLock_2, "MedicalLock_2");
        medicalLock_3 = Assign(medicalLock_3, "MedicalLock_3");
        medicalLock_4 = Assign(medicalLock_4, "MedicalLock_4");
        medicalLock_5 = Assign(medicalLock_5, "MedicalLock_5");
        medicalLock_6 = Assign(medicalLock_6, "MedicalLock_6");

        researchItem_1 = Assign(researchItem_1, "ResearchItem_1");
        researchItem_2 = Assign(researchItem_2, "ResearchItem_2");
        researchItem_3 = Assign(researchItem_3, "ResearchItem_3");
        item_1_Headline = Assign(item_1_Headline, "Item_1_Headline");
        item_2_Headline = Assign(item_2_Headline, "Item_2_Headline");
        item_3_Headline = Assign(item_3_Headline, "Item_3_Headline");
        item_1_Explain = Assign(item_1_Explain, "Item_1_Explain");
        item_2_Explain = Assign(item_2_Explain, "Item_2_Explain");
        item_3_Explain = Assign(item_3_Explain, "Item_3_Explain");

        researchButton = Assign(researchButton, "ResearchButton");
        mainResearchText = Assign(mainResearchText, "MainResearchText");
        isResearchText = Assign(isResearchText, "IsResearchText");

        // 각 버튼에 클릭 이벤트 등록
        SetupButton(medicalResearch_0, () => OnMedicalResearchClick(0));
        SetupButton(medicalResearch_1, () => OnMedicalResearchClick(1));
        SetupButton(medicalResearch_2, () => OnMedicalResearchClick(2));
        SetupButton(medicalResearch_3, () => OnMedicalResearchClick(3));
        SetupButton(medicalResearch_4, () => OnMedicalResearchClick(4));
        SetupButton(medicalResearch_5, () => OnMedicalResearchClick(5));
        SetupButton(medicalResearch_6, () => OnMedicalResearchClick(6));

        SetupResearchButton(researchButton, OnResearchButtonClick);
        EnableUnlockableResearch(medicalLock_0);
    }

    private void SetupButton(Image image, System.Action onClick)
    {
        if (image == null)
        {
            Debug.LogError("Button image is not assigned.");
            return;
        }

        Button button = image.GetComponent<Button>();
        if (button == null)
        {
            button = image.gameObject.AddComponent<Button>();
        }
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick());
    }

    private void SetupResearchButton(Image image, System.Action onClick)
    {
        if (image == null)
        {
            Debug.LogError("Button image is not assigned.");
            return;
        }

        Button button = image.GetComponent<Button>();
        if (button == null)
        {
            button = image.gameObject.AddComponent<Button>();
        }
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick());
    }

    public void OnResearchButtonClick()
    {
        if (selectedResearchIndex == -1)
        {
            Debug.LogError("No research selected!");
            return;
        }

        UnlockResearch(selectedResearchIndex);
    }


    private void OnMedicalResearchClick(int researchIndex)
    {
        // RectTransform을 사용하여 UI 위치 조정
        RectTransform rectTransform = researchItem_2.GetComponent<RectTransform>();

        // research DB에 버튼 누른 기록 저장
        //ResearchDBManager.Instance.AddResearchData(ResearchDBManager.ResearchMode.medical);

        // 현재 위치를 가져옴
        Vector2 currentPosition = rectTransform.anchoredPosition;

        // x좌표와 y좌표를 설정
        currentPosition.x = -312;
        currentPosition.y = -358;
        rectTransform.anchoredPosition = currentPosition;

        // 연구 인덱스에 따라 위치를 조정
        selectedResearchIndex = researchIndex;

        if (selectedResearchIndex == 0 || selectedResearchIndex == 1)
        {
            currentPosition.x = -48;
            currentPosition.y = -358;
            rectTransform.anchoredPosition = currentPosition;
        }


        // 선택된 연구의 하위 오브젝트에서 ResearchText를 찾아 MainResearchText에 반영
        Image selectedResearch = null;
        bool item1Exists = false, item2Exists = false, item3Exists = false;
        GameObject item1 = null, item2 = null, item3 = null;

        switch (researchIndex)
        {
            case 0:
                selectedResearch = medicalResearch_0;
                item1 = GameObject.Find("Item_0_1");
                item2 = GameObject.Find("Item_0_2");
                item3 = GameObject.Find("Item_0_3");
                item1Exists = item1 != null;
                item2Exists = item2 != null;
                item3Exists = item3 != null;
                break;
            case 1:
                selectedResearch = medicalResearch_1;
                item1 = GameObject.Find("Item_1_1");
                item2 = GameObject.Find("Item_1_2");
                item3 = GameObject.Find("Item_1_3");
                item1Exists = item1 != null;
                item2Exists = item2 != null;
                item3Exists = item3 != null;
                break;
            case 2:
                selectedResearch = medicalResearch_2;
                item1 = GameObject.Find("Item_2_1");
                item2 = GameObject.Find("Item_2_2");
                item3 = GameObject.Find("Item_2_3");
                item1Exists = item1 != null;
                item2Exists = item2 != null;
                item3Exists = item3 != null;
                break;
            case 3:
                selectedResearch = medicalResearch_3;
                item1 = GameObject.Find("Item_3_1");
                item2 = GameObject.Find("Item_3_2");
                item3 = GameObject.Find("Item_3_3");
                item1Exists = item1 != null;
                item2Exists = item2 != null;
                item3Exists = item3 != null;
                break;
            case 4:
                selectedResearch = medicalResearch_4;
                item1 = GameObject.Find("Item_4_1");
                item2 = GameObject.Find("Item_4_2");
                item3 = GameObject.Find("Item_4_3");
                item1Exists = item1 != null;
                item2Exists = item2 != null;
                item3Exists = item3 != null;
                break;
            case 5:
                selectedResearch = medicalResearch_5;
                item1 = GameObject.Find("Item_5_1");
                item2 = GameObject.Find("Item_5_2");
                item3 = GameObject.Find("Item_5_3");
                item1Exists = item1 != null;
                item2Exists = item2 != null;
                item3Exists = item3 != null;
                break;
            case 6:
                selectedResearch = medicalResearch_6;
                item1 = GameObject.Find("Item_6_1");
                item2 = GameObject.Find("Item_6_2");
                item3 = GameObject.Find("Item_6_3");
                item1Exists = item1 != null;
                item2Exists = item2 != null;
                item3Exists = item3 != null;
                break;
            default:
                Debug.LogError("Invalid research index");
                return;
        }

        // 하위 오브젝트 중 ResearchText를 찾음
        if (selectedResearch != null)
        {
            Transform researchTextTransform = selectedResearch.transform.Find("ResearchText");
            if (researchTextTransform != null)
            {
                TextMeshProUGUI researchText = researchTextTransform.GetComponent<TextMeshProUGUI>();
                if (researchText != null)
                {
                    // MainResearchText 업데이트
                    mainResearchText.text = researchText.text;
                }
                else
                {
                    Debug.LogError("ResearchText component is missing.");
                }
            }
            else
            {
                Debug.LogError("ResearchText object not found.");
            }
        }

        // 선택된 연구가 해금 가능한지 여부에 따라 isResearchText 업데이트
        bool canUnlock = false;
        switch (researchIndex)
        {
            case 0: canUnlock = true; break; // 0번은 항상 가능으로 설정해놓음. 나중에 수정할 것.
            case 1: canUnlock = isResearch.isMedicalResearch_0; break;
            case 2: canUnlock = isResearch.isMedicalResearch_1; break;
            case 3: canUnlock = isResearch.isMedicalResearch_2; break;
            case 4: canUnlock = isResearch.isMedicalResearch_0; break;
            case 5: canUnlock = isResearch.isMedicalResearch_4; break;
            case 6: canUnlock = isResearch.isMedicalResearch_5; break;
            default: canUnlock = false; break;
        }

        isResearchText.text = canUnlock ? "연구 가능!" : "연구 불가능";

        // 연구의 Item 존재 여부에 따라 researchItem_1~3 활성화/비활성화 설정
        researchItem_1.SetActive(item1Exists);  // Item_1이 존재하면 활성화
        researchItem_2.SetActive(item2Exists);  // Item_2가 존재하면 활성화
        researchItem_3.SetActive(item3Exists);  // Item_3이 존재하면 활성화

        // 연구 아이템이 활성화되었을 때 Icon 스프라이트 복사
        if (item1Exists)
        {
            CopyIcon(item1, researchItem_1);
        }
        if (item2Exists)
        {
            CopyIcon(item2, researchItem_2);
        }
        if (item3Exists)
        {
            CopyIcon(item3, researchItem_3);
        }

        LoadSubResearch(researchIndex);
    }

    private void LoadSubResearch(int researchIndex)
    {
        ResearchInfomation researchInfo = new ResearchInfomation();

        string[] subResearchData = null;

        // 현재 선택된 연구에 따른 하위 연구 설정
        switch (researchIndex)
        {
            case 0:
                subResearchData = new string[] { researchInfo.medicalResearch_0_1, researchInfo.medicalResearch_0_2 };
                break;
            case 1:
                subResearchData = new string[] { researchInfo.medicalResearch_1_1, researchInfo.medicalResearch_1_2 };
                break;
            case 2:
                subResearchData = new string[] { researchInfo.medicalResearch_2_1, researchInfo.medicalResearch_2_2, researchInfo.medicalResearch_2_3 };
                break;
            case 3:
                subResearchData = new string[] { researchInfo.medicalResearch_3_1};
                break;
            case 4:
                subResearchData = new string[] { researchInfo.medicalResearch_4_1 };
                break;
            case 5:
                subResearchData = new string[] { researchInfo.medicalResearch_5_1 };
                break;
            case 6:
                subResearchData = new string[] { researchInfo.medicalResearch_6_1 };
                break;
            default:
                Debug.LogError("Invalid research index.");
                return;
        }

        // 하위 연구 데이터가 존재하는지 확인 후 할당
        if (subResearchData.Length > 0 && researchItem_1 != null)
        {
            SetResearchItem(subResearchData[0], item_1_Headline, item_1_Explain);
        }
        if (subResearchData.Length > 1 && researchItem_2 != null)
        {
            SetResearchItem(subResearchData[1], item_2_Headline, item_2_Explain);
        }
        if (subResearchData.Length > 2 && researchItem_3 != null)
        {
            SetResearchItem(subResearchData[2], item_3_Headline, item_3_Explain);
        }
    }

    // 하위 연구 데이터를 분리하여 각각 헤드라인과 설명에 적용하는 메서드
    private void SetResearchItem(string researchData, TextMeshProUGUI headlineText, TextMeshProUGUI explainText)
    {
        string[] splitData = researchData.Split('|');
        if (splitData.Length == 2)
        {
            headlineText.text = splitData[0];
            explainText.text = splitData[1];
        }
        else
        {
            Debug.LogError("Invalid research data format.");
        }
    }


    // Icon 이미지를 복사하는 함수
    private void CopyIcon(GameObject item, GameObject researchItem)
    {
        Transform itemIconTransform = item.transform.Find("Icon");
        Transform researchItemIconTransform = researchItem.transform.Find("Icon");

        if (itemIconTransform != null && researchItemIconTransform != null)
        {
            Image itemIcon = itemIconTransform.GetComponent<Image>();
            Image researchItemIcon = researchItemIconTransform.GetComponent<Image>();

            if (itemIcon != null && researchItemIcon != null)
            {
                // 아이템의 Icon 스프라이트를 연구 아이템의 Icon 스프라이트로 설정
                researchItemIcon.sprite = itemIcon.sprite;
            }
        }
    }

    private void UnlockResearch(int researchIndex)
    {
        switch (researchIndex)
        {
            case 0:
                isResearch.isMedicalResearch_0 = true;
                isResearch.IsOnMedicalResearch_0();
                DisableLock(medicalLock_0);
                EnableUnlockableResearch(medicalLock_1, medicalLock_4);
                break;
            case 1:
                if (isResearch.isMedicalResearch_0)  // 0번 연구가 이미 해금된 상태라면 1번 해금 가능
                {
                    isResearch.isMedicalResearch_1 = true;
                    isResearch.IsOnMedicalResearch_1();
                    DisableLock(medicalLock_1);
                    EnableUnlockableResearch(medicalLock_2);
                }
                break;
            case 2:
                if (isResearch.isMedicalResearch_1)
                {
                    isResearch.isMedicalResearch_2 = true;
                    isResearch.IsOnMedicalResearch_2();
                    DisableLock(medicalLock_2);
                    EnableUnlockableResearch(medicalLock_3);
                }
                break;
            case 3:
                if (isResearch.isMedicalResearch_2)
                {
                    isResearch.isMedicalResearch_3 = true;
                    isResearch.IsOnMedicalResearch_3();
                    DisableLock(medicalLock_3);
                }
                break;
            case 4:
                if (isResearch.isMedicalResearch_0)
                {
                    isResearch.isMedicalResearch_4 = true;
                    isResearch.IsOnMedicalResearch_4();
                    DisableLock(medicalLock_4);
                    EnableUnlockableResearch(medicalLock_5);
                }
                break;
            case 5:
                if (isResearch.isMedicalResearch_4)
                {
                    isResearch.isMedicalResearch_5 = true;
                    isResearch.IsOnMedicalResearch_5();
                    DisableLock(medicalLock_5);
                    EnableUnlockableResearch(medicalLock_6);
                }
                break;
            case 6:
                if (isResearch.isMedicalResearch_5)
                {
                    isResearch.isMedicalResearch_6 = true;
                    isResearch.IsOnMedicalResearch_6();
                    DisableLock(medicalLock_6);
                }
                break;
            default:
                Debug.LogError("Invalid research index.");
                break;
        }

        isResearchText.text = "연구 완료!";
    }


    private void DisableLock(GameObject lockObject)
    {
        if (lockObject != null)
        {
            lockObject.SetActive(false);
            Transform imageTransform = lockObject.transform.Find("Image");
            if (imageTransform != null)
            {
                Image icon = imageTransform.GetComponent<Image>();
                if (icon != null)
                {
                    icon.sprite = unlockIcon;
                }
            }
        }
    }


    private void EnableUnlockableResearch(params GameObject[] lockObjects)
    {
        foreach (var lockObject in lockObjects)
        {
            Transform imageTransform = lockObject.transform.Find("Image");
            if (imageTransform != null)
            {
                Image icon = imageTransform.GetComponent<Image>();
                if (icon != null)
                {
                    icon.sprite = unlockIcon;
                }
            }
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
}
