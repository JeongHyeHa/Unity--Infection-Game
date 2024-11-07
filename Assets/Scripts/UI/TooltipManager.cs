using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance;

    public GameObject tooltipPrefab; // 툴팁 프리팹
    private GameObject tooltipInstance;
    private TextMeshProUGUI tooltipText;
    private RectTransform tooltipRectTransform;

    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // 툴팁 인스턴스 생성
        tooltipInstance = Instantiate(tooltipPrefab, transform);
        tooltipText = tooltipInstance.GetComponentInChildren<TextMeshProUGUI>();
        tooltipRectTransform = tooltipInstance.GetComponent<RectTransform>();

        // 툴팁을 항상 최상위에 표시하도록 설정
        Canvas tooltipCanvas = tooltipInstance.GetComponent<Canvas>();
        if (tooltipCanvas == null)
        {
            tooltipCanvas = tooltipInstance.AddComponent<Canvas>();
        }
        tooltipCanvas.overrideSorting = true;
        tooltipCanvas.sortingOrder = 1000; // 모든 UI보다 위에 표시되도록 높은 값 설정

        // GraphicRaycaster 추가 (툴팁에서 클릭 이벤트를 받지 않도록 할 경우 필요하지 않을 수 있음)
        if (tooltipInstance.GetComponent<GraphicRaycaster>() == null)
        {
            tooltipInstance.AddComponent<GraphicRaycaster>();
        }
        tooltipInstance.SetActive(false); // 초기에는 비활성화
    }

    private void Update()
    {
        if (tooltipInstance.activeSelf)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, Input.mousePosition, null, out localPoint);
            // 마우스 위치에 오프셋을 추가하여 툴팁이 깜빡거리지 않도록 함
            tooltipRectTransform.localPosition = localPoint + new Vector2(0f, 36f);
        }
    }


    public void ShowTooltip(string text)
    {
        tooltipText.text = text;
        // 텍스트의 크기에 따라 툴팁의 배경 크기 조정
        LayoutRebuilder.ForceRebuildLayoutImmediate(tooltipRectTransform);
        tooltipInstance.SetActive(true);
    }

    public void HideTooltip()
    {
        tooltipInstance.SetActive(false);
    }
}
