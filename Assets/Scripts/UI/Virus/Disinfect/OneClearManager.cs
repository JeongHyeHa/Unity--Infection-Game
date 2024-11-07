using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OneClearManager : MonoBehaviour
{
    public Button oneClearButton;
    public GameObject guidGreenImage;            //소독중 표지판
    public Texture2D customCursor;                  //소독 커서 이미지
    private bool isDisinfectionOn = false;       //소독 중인지 여부
    private bool isCustomCursorActive = false;   //현재 커서 상태 추적

    void Start()
    {
        oneClearButton = Assign(oneClearButton, "OneClearButton");
        guidGreenImage = Assign(guidGreenImage, "GuidGreenImage");

        //사용자 지정 커스텀 이미지 불러오기
        customCursor = Resources.Load<Texture2D>("mopCursorIcon");
        if (customCursor == null)
        {
            Debug.LogError("mopCursorIcon 이미지가 Resource 폴더에 없습니다.");
        }

        guidGreenImage.SetActive(false);
        oneClearButton.onClick.AddListener(ToggleDisinfection);  //버튼 상태 전환
    }

    void ToggleDisinfection()
    {
        isDisinfectionOn = !isDisinfectionOn;
        Debug.Log("Disinfection 상태: " + (isDisinfectionOn ? "On" : "Off"));  //수정
        if (isDisinfectionOn)
        {
            guidGreenImage.SetActive(true);

            //소독 모드 ON -> 사용자 지정 커서로 변경
            //Cursor.SetCursor(customCursor, Vector2.zero, CursorMode.Auto);        //커서 좌표는 왼쪽 상단
            Vector2 hotspot = new Vector2(customCursor.width/2, customCursor.height/2); // 커서 중심을 이미지 크기와 동일하게
            //Cursor.SetCursor(customCursor, hotspot, CursorMode.Auto);
            Cursor.SetCursor(customCursor, hotspot, CursorMode.ForceSoftware);
            isCustomCursorActive = true;
        }
        else
        {
            guidGreenImage.SetActive(false);

            //소독 모드 OFF -> 원래 커서로 복원
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            isCustomCursorActive = false;
        }
    }

    // 소독 기능 활성화 상태 확인
    // Virus 스크립트 -> ture면 마우스 클릭으로 하나씩 소독 시작
    public bool IsDisinfectionOn()
    {
        return isDisinfectionOn;
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
