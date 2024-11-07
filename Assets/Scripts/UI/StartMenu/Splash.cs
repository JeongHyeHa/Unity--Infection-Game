using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class Splash : MonoBehaviour
{
    public CanvasGroup coverImage;      // 검은배경용 이미지
    public TextMeshProUGUI splashText;  // CGLab 로고
    public CanvasGroup mainMenuCanvas;  // 스플래시 이후 나타나는 메인메뉴 캔버스
    public GameObject startGameOverlay;
    public float fadeDuration = 0.5f;   // 페이드 속도
    public float initialDelay = 1.0f;   // 로고 등장까지 대기시간

    private void Start()
    {
        coverImage = Assign(coverImage, "SplashCoverImage");
        splashText = Assign(splashText, "CGLab");
        mainMenuCanvas = Assign(mainMenuCanvas, "MainMenuCanvas");
        startGameOverlay = Assign(startGameOverlay, "StartGameOverlay");

        // 초기 값 설정
        coverImage.gameObject.SetActive(true);  // 처음에 꺼두었기에 시작할 때 다시 On
        coverImage.alpha = 1f;
        SetAlpha(splashText, 0f);
        mainMenuCanvas.alpha = 0f;

        // 메인 메뉴 텍스트에 테두리 추가
        foreach (TextMeshProUGUI text in mainMenuCanvas.GetComponentsInChildren<TextMeshProUGUI>())
        {
            SetOutline(text, Color.black, 0.3f);
        }

        // 스플래시 시퀀스 시작
        StartCoroutine(SplashSequence());
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

    private IEnumerator SplashSequence()
    {
        // 초기 대기 시간
        yield return new WaitForSeconds(initialDelay);

        // 스플래시 텍스트 페이드 인
        yield return StartCoroutine(FadeText(splashText, 0f, 1f, fadeDuration));

        // 사용자가 클릭할 때까지 대기
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        startGameOverlay.SetActive(false);

        // 스플래시 텍스트 페이드 아웃
        yield return StartCoroutine(FadeText(splashText, 1f, 0f, fadeDuration));

        // 커버 이미지 페이드 아웃
        yield return StartCoroutine(FadeCanvasGroup(coverImage, 1f, 0f, fadeDuration));

        // 메인 메뉴 활성화 및 페이드 인
        mainMenuCanvas.gameObject.SetActive(true);
        yield return StartCoroutine(FadeCanvasGroup(mainMenuCanvas, 0f, 1f, fadeDuration));

        gameObject.SetActive(false);
    }

    // 텍스트 페이드 시간 구현
    private IEnumerator FadeText(TextMeshProUGUI text, float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        Color color = text.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            text.color = color;
            yield return null;
        }

        color.a = endAlpha;
        text.color = color;
    }

    // 캔버스 그룹 페이드 시간 구현
    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = endAlpha;
    }

    // 투명도 조정
    private void SetAlpha(TextMeshProUGUI text, float alpha)
    {
        Color color = text.color;
        color.a = alpha;
        text.color = color;
    }

    // 테두리 색 조정
    private void SetOutline(TextMeshProUGUI text, Color color, float width)
    {
        text.outlineColor = color;
        text.outlineWidth = width;
    }
}
