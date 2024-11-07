using System.Collections;
using System.Collections.Generic;
using TMPro; // TextMeshProUGUI를 사용하기 위해 필요
using UnityEngine;
using UnityEngine.UI;

public class NewsManager : MonoBehaviour
{
    public static NewsManager Instance { get; private set; }

    public GameObject newsCanvas;
    public Image newsBackground;
    public Image newsPanel;
    public TextMeshProUGUI mainNews;
    public TextMeshProUGUI goodInfo;
    public TextMeshProUGUI badInfo;
    public Button closeButton;

    private Vector2 originalPanelPosition;
    private Queue<NewsData> newsQueue = new Queue<NewsData>(); // 큐로 동시 뉴스 발생 제거
    private bool isNewsShowing = false; // 현재 뉴스가 표시되고 있는지 확인

    // 뉴스 데이터를 저장하기 위한 구조체
    private struct NewsData
    {
        public string MainNews;
        public string GoodInfo;
        public string BadInfo;

        public NewsData(string mainNews, string goodInfo, string badInfo)
        {
            MainNews = mainNews;
            GoodInfo = goodInfo;
            BadInfo = badInfo;
        }
    }

    private void Awake()
    {
        // 싱글톤 인스턴스 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시에도 유지하고 싶으면 사용
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        newsCanvas = Assign(newsCanvas, "NewsCanvas");
        newsBackground = Assign(newsBackground, "NewsBackground");
        newsPanel = Assign(newsPanel, "NewsPanel");
        mainNews = Assign(mainNews, "MainNews");
        goodInfo = Assign(goodInfo, "GoodInfo");
        badInfo = Assign(badInfo, "BadInfo");
        closeButton = Assign(closeButton, "CloseButton");

        // 뉴스패널의 원래 위치 저장
        originalPanelPosition = newsPanel.rectTransform.anchoredPosition;
        // closeButton 클릭 이벤트 등록
        closeButton.onClick.AddListener(CloseNewsCanvas);
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

    // 뉴스 보여주기
    public void ShowNewsCanvas(string mainNewsText, string goodInfoText, string badInfoText)
    {
        // 뉴스를 큐에 추가
        newsQueue.Enqueue(new NewsData(mainNewsText, goodInfoText, badInfoText));

        // 만약 뉴스가 표시되고 있지 않다면, 큐에서 뉴스 데이터를 꺼내 표시
        if (!isNewsShowing)
        {
            StartCoroutine(DisplayNextNews());
        }
    }

    // 큐에 남은 뉴스가 있다면 다음 뉴스 보여주기
    private IEnumerator DisplayNextNews()
    {
        if (newsQueue.Count > 0)
        {
            isNewsShowing = true;

            // 큐에서 뉴스 데이터를 가져와 화면에 표시
            NewsData nextNews = newsQueue.Dequeue();
            UpdateNewsTexts(nextNews.MainNews, nextNews.GoodInfo, nextNews.BadInfo);
            newsCanvas.SetActive(true);
            StartCoroutine(AnimateNewsBackground(true));
            StartCoroutine(AnimateNewsPanel(true));
            yield return StartCoroutine(PauseGame());
        }
    }

    public void CloseNewsCanvas()
    {
        StartCoroutine(CloseNewsAnimation());
    }

    private IEnumerator PauseGame()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        Time.timeScale = 0f; // 게임 멈춤
    }

    private IEnumerator CloseNewsAnimation()
    {
        Time.timeScale = 1f; // 게임 재개
        yield return StartCoroutine(AnimateNewsBackground(false));
        yield return StartCoroutine(AnimateNewsPanel(false));
        newsCanvas.SetActive(false);

        // 뉴스 표시가 끝났음을 알림
        isNewsShowing = false;
    }

    private IEnumerator AnimateNewsBackground(bool isShowing)
    {
        // 배경 투명도 애니메이션
        Color bgColor = newsBackground.color;
        float elapsedTime = 0f;
        float duration = 0.5f;
        float startAlpha = isShowing ? 0f : 155f / 255f;
        float endAlpha = isShowing ? 155f / 255f : 0f;

        while (elapsedTime < duration)
        {
            bgColor.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            newsBackground.color = bgColor;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        bgColor.a = endAlpha;
        newsBackground.color = bgColor;
    }

    private IEnumerator AnimateNewsPanel(bool isShowing)
    {
        // 패널 이동 애니메이션
        RectTransform panelRectTransform = newsPanel.GetComponent<RectTransform>();
        Vector2 startPosition = isShowing ? new Vector2(Screen.width, originalPanelPosition.y) : originalPanelPosition;
        Vector2 endPosition = isShowing ? originalPanelPosition : new Vector2(Screen.width, originalPanelPosition.y);
        float elapsedTime = 0f;
        float duration = 0.5f;

        while (elapsedTime < duration)
        {
            panelRectTransform.anchoredPosition = Vector2.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        panelRectTransform.anchoredPosition = endPosition;
    }

    public void UpdateNewsTexts(string mainNewsText, string goodInfoText, string badInfoText)
    {
        mainNews.text = $"- {mainNewsText}";
        goodInfo.text = $"- {goodInfoText}";
        badInfo.text = $"- {badInfoText}";
    }
}
