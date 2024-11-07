using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 자동 할당 위해 1프레임 느리게 OFF 시킴
public class Close : MonoBehaviour
{
    public GameObject loginCanvas;
    public GameObject signUpCanvas;
    public GameObject mainMenuCanvas;
    public GameObject exitGameCanvas;
    public GameObject profileCanvas;
    public GameObject profileOverlay;
    public Canvas questCanvas;
    public GameObject questDisfectCanvas;
    public GameObject scoreGraphCanvas;
    public GameObject windowPanel;
    public GameObject infoCanvas;
    public GameObject settingWindow;
    public GameObject overLayUI;
    public GameObject wrongPanel;
    public GameObject rightPanel;
    public GameObject coolTimePanel;
    public GameObject minimapCanvas;
    public GameObject settingButton;
    public GameObject monthlyReportCanvas;
    public GameObject disinfectionCanvas;
    public GameObject newsTickerCanvas;
    public GameObject policyCanvas;
    public GameObject policyOpenButton;
    public GameObject researchingItem;

    void Start()
    {
        StartCoroutine(LateStart());
    }

    IEnumerator LateStart()
    {
        yield return null;  // 한 프레임 대기

        loginCanvas = Assign(loginCanvas, "LoginCanvas");
        signUpCanvas = Assign(signUpCanvas, "SignUpCanvas");
        mainMenuCanvas = Assign(mainMenuCanvas, "MainMenuCanvas");
        exitGameCanvas = Assign(exitGameCanvas, "ExitGameCanvas");
        profileCanvas = Assign(profileCanvas, "ProfileCanvas");
        profileOverlay = Assign(profileOverlay, "ProfileOverlay");
        infoCanvas = Assign(infoCanvas, "InfoCanvas");
        questCanvas = Assign(questCanvas, "QuestCanvas");
        questDisfectCanvas = Assign(questDisfectCanvas, "QuestDisinfectCanvas");
        scoreGraphCanvas = Assign(scoreGraphCanvas, "ScoreGraphCanvas");
        settingWindow = Assign(settingWindow, "SettingWindow");
        overLayUI = Assign(overLayUI, "OverLayUI");
        windowPanel = Assign(windowPanel, "WindowPanel");
        wrongPanel = Assign(wrongPanel, "WrongPanel");
        rightPanel = Assign(rightPanel, "RightPanel");
        coolTimePanel = Assign(coolTimePanel, "CoolTimePanel");
        minimapCanvas = Assign(minimapCanvas, "MinimapCanvas");
        settingButton = Assign(settingButton, "SettingButton");
        monthlyReportCanvas = Assign(monthlyReportCanvas, "MonthlyReportCanvas");
        disinfectionCanvas = Assign(disinfectionCanvas, "DisinfectionCanvas");
        newsTickerCanvas = Assign(newsTickerCanvas, "NewsTickerCanvas");
        policyCanvas = Assign(policyCanvas, "PolicyCanvas");
        researchingItem = Assign(researchingItem, "ResearchComplete");

        loginCanvas?.SetActive(false);
        signUpCanvas?.SetActive(false);
        mainMenuCanvas?.SetActive(false);
        exitGameCanvas?.SetActive(false);
        profileCanvas?.SetActive(false);
        profileOverlay?.SetActive(false);
        infoCanvas?.SetActive(false);
        questCanvas.enabled = false;
        questDisfectCanvas.SetActive(false);
        scoreGraphCanvas.SetActive(false);
        settingWindow?.SetActive(false);
        overLayUI?.SetActive(false);
        windowPanel?.SetActive(false);
        wrongPanel?.SetActive(false);
        rightPanel?.SetActive(false);
        coolTimePanel?.SetActive(false);
        minimapCanvas?.SetActive(false);
        settingButton?.SetActive(false);
        monthlyReportCanvas?.SetActive(false);
        disinfectionCanvas?.SetActive(false);
        newsTickerCanvas?.SetActive(false);
        policyCanvas?.SetActive(false);
        researchingItem?.SetActive(false);
    }

    // 오브젝트 자동 할당
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
