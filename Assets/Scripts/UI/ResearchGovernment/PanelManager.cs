using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour
{
    //public GameObject newsPanel;         // 패널
    //public TextMeshProUGUI newsText;    // 패널의 제목
    //public Image newsIcon;

    //void Start()
    //{
    //    newsPanel = Assign(newsPanel, "MessagePanel");
    //    newsText = Assign(newsText, "MessageText");
    //    newsIcon = Assign(newsIcon, "MessageIcon");

    //    newsPanel.SetActive(false); // 경고 패널 초기에는 비활성화
    //}

    ////패널 생성
    //public void ShowWarning(string status, string message)
    //{
    //    newsText.text = message;        // 경고 문구 변경
    //    newsIcon.sprite = TryLoadIcon(status);  //이미지 변경
    //    StartCoroutine(StartShowPanel());
    //}

    //private IEnumerator StartShowPanel()
    //{
    //    newsPanel.SetActive(true);
    //    yield return new WaitForSeconds(1.0f);
    //    newsPanel.SetActive(false);
    //}

    //// 이미지 변경
    //public Sprite TryLoadIcon(string status)
    //{
    //    Sprite icon = Resources.Load<Sprite>(status);
    //    if (icon == null)
    //    {
    //        Debug.LogError($"이미지 '{status}'를 Resources 폴더에서 찾을 수 없습니다.");
    //    }
    //    return icon;
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
