using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 게임 시작 시 UI 및 카메라 설정을 담당하는 스크립트
public class GoToGame : MonoBehaviour
{
    public static GoToGame Instance { get; private set; }

    public GameObject startItems;
    public GameObject profileWindowUI;
    public GameObject infoWindowUI;
    public GameObject settingButtonIcon;
    public GameObject minimapCanvas;
    public CalendarManager calendarManager;
    public GameObject disinfectionCanvas;
    public GameObject newsTickerCanvas;
    public GameObject policyCanvas;
    public GameObject policyPanel;
    public TextMeshProUGUI selectedLevel;

    public ProfileWindow profileWindow;
    public InfectionController infectionController;
    public PolicyResearch policyResearch;

    public bool isStart = false;

    void Awake()
    {
        GameObject hospital = GameObject.Find("Hospital");
        Vector3 hospitalPosition = hospital.transform.position;
        hospitalPosition = new Vector3(-hospitalPosition.x, hospitalPosition.y, hospitalPosition.z);
        hospital.transform.position = hospitalPosition;

        // 코드 간소화를 위한 객체 할당
        startItems = Assign(startItems, "StartUI");
        profileWindowUI = Assign(profileWindowUI, "ProfileCanvas");
        infoWindowUI = Assign(infoWindowUI, "InfoCanvas");
        settingButtonIcon = Assign(settingButtonIcon, "SettingButton");
        minimapCanvas = Assign(minimapCanvas, "MinimapCanvas");
        disinfectionCanvas = Assign(disinfectionCanvas, "DisinfectionCanvas");
        newsTickerCanvas = Assign(newsTickerCanvas, "NewsTickerCanvas");
        policyCanvas = Assign(policyCanvas, "PolicyCanvas");
        policyPanel = Assign(policyPanel, "PolicyPanel");
        selectedLevel = Assign(selectedLevel, "SelectedLevel");
        calendarManager = FindObjectOfType<CalendarManager>();
        profileWindow = FindObjectOfType<ProfileWindow>();
        infectionController = FindObjectOfType<InfectionController>();
        policyResearch = FindObjectOfType<PolicyResearch>();

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

    // 게임 시작 시 설정
    public void StartGame()
    {
        startItems.SetActive(false);
        profileWindowUI.SetActive(true);
        infoWindowUI.SetActive(true);
        settingButtonIcon.SetActive(true);
        minimapCanvas.SetActive(true);
        disinfectionCanvas.SetActive(true);
        newsTickerCanvas.SetActive(true);
        policyCanvas.SetActive(true);
        policyPanel.SetActive(false);
        profileWindow.UpdateButtonTexts("응급실");
        SetDifficultyAndStage();
        policyResearch.ResearchWaiting();

        isStart = true;
        Debug.Log("Start!!!");
    }

    // 난이도에 따라 감염 상태와 스테이지 설정
    private void SetDifficultyAndStage()
    {
        string levelText = selectedLevel.text;

        if (levelText == "Easy")
        {
            Managers.Instance.ChangeGameStage(1); // 스테이지를 1로 설정
        }
        else if (levelText == "Normal")
        {
            Managers.Instance.ChangeGameStage(2); // 스테이지를 2로 설정
        }
        else
        {
            Debug.LogWarning("알 수 없는 난이도: " + levelText);
        }
    }

    //// selectedLevel 텍스트에 따라 Person의 난이도 설정
    //private void SetPersonDifficultyBasedOnLevel()
    //{
    //    string levelText = selectedLevel.text;
    //    InfectionState infectionState;

    //    // 난이도에 따른 감염 상태 및 저항성 설정
    //    switch (levelText)
    //    {
    //        case "Easy":
    //            infectionState = InfectionState.CRE;
    //            break;
    //        case "Normal":
    //            infectionState = InfectionState.Covid;
    //            break;
    //        case "Hard":
    //            infectionState = InfectionState.Normal;
    //            break;
    //        default:
    //            infectionState = InfectionState.Normal;
    //            Debug.LogWarning("알 수 없는 난이도: " + levelText);
    //            break;
    //    }

    //    // 모든 Person 객체의 감염 상태 및 저항성 설정
    //    Person[] people = FindObjectsOfType<Person>();
    //    foreach (var person in people)
    //    {
    //        person.status = infectionState;
    //        Debug.Log($"Person {person.Name}의 감염 상태가 {infectionState}로 설정되었습니다.");
    //    }
    //}

}
