using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class DictionaryManager : MonoBehaviour
{
    //public GameObject dictionaryPanel;
    //public Button[] dicMenus;          // 메뉴 버튼 배열
    //public Button dicCloseButton;
    //public ProgressBar progressBar;         // ProgressBar 스크립트 참조
    //public GovernmentManager govManager;
    //public PanelManager panelManager;
    //public ResearchManager researchManager;
    //public TextMeshProUGUI dicTitle;    //TextMeshProUGUI는 UI요소, TextMeshPro는 3D 요소..
    //public TextMeshProUGUI dicText;
    //public Image dicVirusImage;
    //public string[] virusTypes = { "CRE", "COVID-19" };

    //private int currentLevel;       //현재 바이러스(초기값 0(=CRE))
    //private Button currentlySelectedButton;     // 현재 선택된 버튼 추적
    //private string[][] virusDescriptions = new string[][]
    //{
    //    new string[]
    //    {
    //        "- 항생제 내성\n카바페넴이라는 항생제에 내성을 가진 장내세균.\n일반적인 항생제 치료가 효과적이지 않다.\n\n- 높은 치사율\n치료가 어려워 사망률이 높다. 감염의 심각성과 치료의 어려움으로 인해 공공 보건에서 중요한 문제로 대두되고 있다.",
    //        "- 고위험 감염\n특히 면역력이 약한 환자, 장기 입원 환자, 중환자실 환자 등에서 심각한 감염을 유발할 수 있다.\n\n- 전파 경로\n주로 의료 환경에서 손을 통한 접촉으로 전파되며, 의료기기, 카테터, 수술 상처 등을 통해 감염될 수 있다.",
    //        "주로 요로감염, 폐렴, 혈류감염, 상처감염 등으로 나타날 수 있다.\n증상은 감염된 부위에 따라 다르며, 일반적인 감염 증상으로 발열, 통증 등이 있다.",
    //        "손 씻기, 환경 소독, 감염된 환자의 격리 등 기본적인 감염 관리 조치가 매우 중요하다."
    //    },
    //    new string[]
    //    {
    //        "- 바이러스 원인\nSARS-CoV-2라는 바이러스에 의해 발생하며, 주로 호흡기 증상을 유발한다.\n\n- 변이 바이러스\n여러 변이 바이러스가 등장하여 전파력과 증증도에 영향을 미치고 있다. 델타, 오미크론 변이가 대표적이다.",
    //        "비말(기침, 재채기)과 가까운 접촉을 통해 전파되며, 공기 중에 떠 있는 미세한 에어로졸을 통해서도 전파될 수 있다.",
    //        "발열, 기침, 피로, 호흡 곤란, 근육통, 두통, 인후통 등이 있으며, 심한 경우 폐렴, 다발성 장기 부전, 사망에 이를 수 있다.",
    //        "백신 접종이 주요 예방 수단이며, 마스크 착용, 사회적 거리두기, 손 씻기 등이 전파를 줄이는 데 효과적이다. 항바이러스제, 산소 치료 등이 중증 환자에게 사용된다."
    //    }
    //};

    //void Start()
    //{
    //    InitializeObject();
    //    dictionaryPanel.SetActive(false);
    //    if (govManager.currentStage == 1)
    //    {
    //        currentLevel = 0;
    //    }
    //    else
    //    {
    //        currentLevel = 1;
    //    }
    //}

    //private void InitializeObject()
    //{
    //    //오브젝트 자동 할당
    //    dicMenus = Enumerable.Range(1, 4)
    //        .Select(i => Assign<Button>(null, $"DicMenu{i}"))
    //        .ToArray();

    //    dictionaryPanel = Assign(dictionaryPanel, "DictionaryPanel");
    //    progressBar = Assign(progressBar, "ChargeProgress");
    //    researchManager = Assign(researchManager, "Research");
    //    panelManager = Assign(panelManager, "PanelManager");
    //    govManager = Assign(govManager, "Government");
    //    dicCloseButton = Assign(dicCloseButton, "DicCloseButton");
    //    dicTitle = Assign(dicTitle, "DicTitle");
    //    dicText = Assign(dicText, "DicText");
    //    dicVirusImage = Assign(dicVirusImage, "DicVirusImage");

    //    //클릭 이벤트 할당
    //    for (int i = 0; i < dicMenus.Length; i++)
    //    {
    //        int currentIndex = i;
    //        dicMenus[i].onClick.AddListener(() =>
    //        {
    //            OnChangeDicText(currentIndex + 1);
    //            HighlightButton(dicMenus[currentIndex]);
    //        });
    //    }

    //    dicCloseButton.onClick.AddListener(() => OnClosePanel());
    //}

    ////바이러스 사전 열람 관리(디폴트 값 관리)
    //public void OpenDictionaryPanel()
    //{
    //    UpdateVirusDetail();            //현재 바이러스에 맞게 UI 변경
    //    OnChangeDicText(1);             //메뉴 버튼 상태 초기화
    //    HighlightButton(dicMenus[0]);   //1번 메뉴 선택된 상태
    //    dictionaryPanel.SetActive(true);
    //}

    ////바이러스 정보 업데이트
    //private void UpdateVirusDetail()
    //{
    //    dicTitle.text = currentLevel == 0 ?
    //        "CRE(Carbapenem - Resistant Enterobacteriaceae)" :
    //        "COVID-19 (Coronavirus Disease 2019)";

    //    //이미지 변경
    //    dicVirusImage.sprite = panelManager.TryLoadIcon(virusTypes[currentLevel]);
    //}

    ////메뉴바를 선택할 때마다 UI 변경
    //private void OnChangeDicText(int menuIndex)
    //{
    //    dicText.text = virusDescriptions[currentLevel][menuIndex - 1];
    //}

    ////사전 닫기
    //private void OnClosePanel()
    //{
    //    dictionaryPanel.SetActive(false);
    //}

    ////메뉴 버튼 상태 관리
    //private void HighlightButton(Button selectedButton)
    //{
    //    if (currentlySelectedButton != null)
    //    {
    //        SetButtonColor(currentlySelectedButton, Color.white);
    //    }
    //    SetButtonColor(selectedButton, new Color32(200, 200, 200, 255));    // #C8C8C8
    //    currentlySelectedButton = selectedButton;   //버튼 상태 업데이트
    //}

    //// 버튼 색상 설정
    //private void SetButtonColor(Button button, Color color)
    //{
    //    ColorBlock colors = button.colors;
    //    colors.normalColor = color;
    //    button.colors = colors;
    //}

    //// 자동 할당 코드 
    //private T Assign<T>(T obj, string objectName) where T : Object
    //{
    //    if (obj == null)
    //    {
    //        GameObject foundObject = GameObject.Find(objectName);
    //        if (foundObject != null)
    //        {
    //            if (typeof(Component).IsAssignableFrom(typeof(T))) obj = foundObject.GetComponent(typeof(T)) as T;
    //            else if (typeof(GameObject).IsAssignableFrom(typeof(T))) obj = foundObject as T;
    //        }
    //        if (obj == null) Debug.LogError($"{objectName} 를 찾을 수 없습니다.");
    //    }
    //    return obj;
    //}
}
