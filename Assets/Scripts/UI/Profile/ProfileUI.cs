using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

// ProfileWindowManager
public class ProfileUI : MonoBehaviour
{

    public GameObject showIndividualPanel;      // 해당 NPC 정보 패널
    public TextMeshProUGUI npcNameText;         // NPC 이름 Text
    public Image npcAvatarImage;
    public TextMeshProUGUI npcProtectionRateText; // 개인 감염방지 확률 (합)

    private ProfileWindow profileWindow; // ProfileWindow 인스턴스

    private Person currentNPC;
    private bool isActive;

    private float allStress;
    private int count = 0;


    void Start()
    {
        showIndividualPanel = Assign(showIndividualPanel, "ShowIndividualPanel");
        npcNameText = Assign(npcNameText, "NPCName");
        npcAvatarImage = Assign(npcAvatarImage, "NPCAvatarImage");
        npcProtectionRateText = Assign(npcProtectionRateText, "NPCProtectionRateText");
        profileWindow = FindObjectOfType<ProfileWindow>(); // ProfileWindow 인스턴스 찾기

    }
    private void Update()
    {
        if (currentNPC != null)
        {
            UpdateIndividual();
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

    public void ShowProfileUI(int personID)
    {
        // 현재 NPC 설정
        currentNPC = PersonManager.Instance.GetPerson(personID);

        if (currentNPC != null)
        {
            showIndividualPanel.SetActive(true);
            isActive = true;



            // NPC 보호율 텍스트 업데이트
            npcProtectionRateText.text = $"+ {currentNPC.GetTotalProtectionRate() + currentNPC.infectionResistance}%";
        }
        else
        {
            Debug.LogError("currentNPC is null.");
            isActive = false;
            showIndividualPanel.SetActive(false);
        }
    }



    public void UpdateIndividual()
    {
        // NPC 정보 텍스트 업데이트
        npcNameText.text = $"{currentNPC.Job} {currentNPC.Name}";
        npcAvatarImage.sprite = currentNPC.AvatarSprite;

        // NPC 보호율 텍스트 업데이트
        npcProtectionRateText.text = $"+ {currentNPC.GetTotalProtectionRate() + currentNPC.infectionResistance}%";
    }
}