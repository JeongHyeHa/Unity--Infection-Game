using UnityEngine;
using UnityEngine.UI;

public class SettingButton : MonoBehaviour
{
    public GameObject settingWindow;
    public GameObject overLayUI;

    void Start()
    {
        settingWindow = Assign(settingWindow, "SettingWindow");
        overLayUI = Assign(overLayUI, "OverLayUI");

        // 버튼 컴포넌트를 가져와 클릭 이벤트에 메서드 등록
        Button settingButtonIcon = GetComponent<Button>();
        settingButtonIcon.onClick.AddListener(OpenSettingWindow);
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

    void OpenSettingWindow()
    {
        if (IsAbleManager.Instance.CanOpenNewWindow())
        {
            IsAbleManager.Instance.OpenWindow(settingWindow);
            // 패널 활성화
            settingWindow.SetActive(true);
            overLayUI.SetActive(true);
        }
    }
}
