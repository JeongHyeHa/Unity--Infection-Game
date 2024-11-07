using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TextMeshProUGUI CurrentFloorText; // 현재 층의 이름을 표시할 텍스트
    public TextMeshProUGUI FloorNameText; // 오브젝트 이름을 표시할 텍스트


    private Person currentNPC;
    private bool ignoreNextClick = false;  // 다음 클릭 무시 플래그
    private Coroutine updateCoroutine;

    private void Awake()
    {
        // 싱글톤
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // 씬 전환에도 파괴 x
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 초기 UI 업데이트
        UpdateCurrentFloorText("응급실");  // 맨 처음 시작 위치
        HideObjectName();       // 시작 시 확대된 미니맵 패널의 텍스트 감추기

        // CurrentFloorText와 FloorNameText에 아웃라인 추가
        AddOutline(CurrentFloorText, Color.black, 0.4f);
        AddOutline(FloorNameText, Color.black, 0.4f);
    }


    private IEnumerator ResetIgnoreNextClick()
    {
        ignoreNextClick = true;
        yield return new WaitForSeconds(0.2f); // 0.2초 동안 클릭 무시
        ignoreNextClick = false;
    }

    public bool IsIgnoreNextClick()
    {
        return ignoreNextClick;
    }

    /// <summary>
    /// ///////////////////////////////////////////////////////////////////////// 현 위치 보여주는 UI
    /// </summary>
    /// <param name="floorName"></param>

    public void UpdateCurrentFloorText(string floorName)    // 현재 위치 정보 텍스트
    {
        CurrentFloorText.text = "[" + floorName + "]";
    }

    public void ShowObjectName(string objectName)           // 미니맵 건물 마우스 올리면 어느 위치인지 텍스트
    {
        FloorNameText.text = "[" + objectName + "]";
        FloorNameText.gameObject.SetActive(true);
    }

    public void HideObjectName()                            // 오브젝트 이름 감추기
    {
        FloorNameText.gameObject.SetActive(false);
    }

    private void AddOutline(TextMeshProUGUI textComponent, Color color, float thickness)
    {
        textComponent.outlineColor = color;
        textComponent.outlineWidth = thickness;
    }
}