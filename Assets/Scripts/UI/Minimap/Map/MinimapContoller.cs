using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MinimapController : MonoBehaviour
{
    public RawImage miniMap; // 미니맵 이미지
    public GameObject largeMapPanel; // 큰 맵 패널 오브젝트
    public RawImage largeMap; // 큰 맵 이미지
    private bool isLargeMapOpen = false; // 큰 맵 패널 활성화 상태를 추적하는 변수

    // 큰 맵 패널의 활성화 상태를 외부에서 접근할 수 있도록 하는 프로퍼티
    public bool IsLargeMapOpen
    {
        get { return isLargeMapOpen; }
    }

    void Start()
    {
        // 미니맵 클릭 시 큰 맵 패널 열기
        miniMap.GetComponent<Button>().onClick.AddListener(OpenLargeMap);
        // 큰 맵 패널 초기에는 비활성화
        largeMapPanel.SetActive(false);
    }

    void Update()
    {

        // 큰 맵 패널이 열려있고 마우스 클릭 시
        if (isLargeMapOpen && Input.GetMouseButtonDown(0))
        {
            // 마우스 클릭 위치가 큰 맵 패널 내부가 아닌 경우
            if (!RectTransformUtility.RectangleContainsScreenPoint(
                largeMapPanel.GetComponent<RectTransform>(),
                Input.mousePosition,
                Camera.main))
            {
                StartCoroutine(CloseLargeMapWithDelay()); // 일정 시간 후 큰 맵 패널 닫기
            }
        }
    }

    // 큰 맵 패널을 여는 메서드
    void OpenLargeMap()
    {
        if (IsAbleManager.Instance.CanOpenNewWindow())
        {
            IsAbleManager.Instance.OpenWindow(largeMapPanel);
            largeMap.texture = miniMap.texture; // 큰 맵 이미지에 미니맵 텍스처 복사
            largeMapPanel.SetActive(true); // 큰 맵 패널 활성화
            isLargeMapOpen = true; // 큰 맵 패널 열림 상태로 설정
            Debug.Log("Large map opened");
        }


    }

    // 일정 시간 후 큰 맵 패널을 닫는 메서드
    private IEnumerator CloseLargeMapWithDelay()
    {
        yield return new WaitForSeconds(0.1f); // 0.1초 기다린 후
        IsAbleManager.Instance.CloseWindow(largeMapPanel);
        largeMapPanel.SetActive(false); // 큰 맵 패널 비활성화
        isLargeMapOpen = false; // 큰 맵 패널 닫힘 상태로 설정
    }
}
