using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewsTicker : MonoBehaviour
{
    public RectTransform ins_traTitle;  // 텍스트가 표시될 RectTransform (뉴스 텍스트)
    public float speed = 50f;  // 텍스트 이동 속도
    public Image iconImage;    // 아이콘 이미지

    private Queue<string> newsQueue = new Queue<string>();  // 뉴스 큐
    private bool isNewsDisplaying = false;  // 현재 뉴스가 표시 중인지 여부
    private Vector2 _vStartPos;  // 텍스트의 시작 위치
    private Vector2 _vEndPos;  // 텍스트의 끝 위치
    private Coroutine iconColorCoroutine; // 아이콘 색상 변경 코루틴

    private void Start()
    {
        RectTransform parentRect = ins_traTitle.parent.GetComponent<RectTransform>();
        float maskWidth = parentRect.rect.width;

        // 텍스트의 끝 위치를 텍스트 크기만큼 왼쪽으로 설정
        _vStartPos = new Vector2(maskWidth / 2 + ins_traTitle.rect.width / 2, ins_traTitle.anchoredPosition.y);
        _vEndPos = new Vector2(-maskWidth / 2 - ins_traTitle.rect.width, ins_traTitle.anchoredPosition.y);

        ins_traTitle.anchoredPosition = _vStartPos;

        ins_traTitle.pivot = new Vector2(0, 0.5f);
        ins_traTitle.anchorMin = new Vector2(0, 0.5f);
        ins_traTitle.anchorMax = new Vector2(0, 0.5f);
    }

    public void EnqueueNews(string newsText)
    {
        newsQueue.Enqueue(newsText);
        if (!isNewsDisplaying)
        {
            DisplayNextNews();
        }
    }

    private void DisplayNextNews()
    {
        if (newsQueue.Count > 0)
        {
            string nextNews = newsQueue.Dequeue();
            ins_traTitle.GetComponent<TextMeshProUGUI>().text = nextNews;

            // 텍스트의 크기를 다시 계산해서 시작 위치와 끝 위치를 조정
            float titleWidth = ins_traTitle.rect.width;
            RectTransform parentRect = ins_traTitle.parent.GetComponent<RectTransform>();
            float maskWidth = parentRect.rect.width;

            _vStartPos = new Vector2(maskWidth / 2 + titleWidth / 2, ins_traTitle.anchoredPosition.y);
            _vEndPos = new Vector2(-maskWidth / 2 - titleWidth / 2 - 200, ins_traTitle.anchoredPosition.y);

            ins_traTitle.anchoredPosition = _vStartPos;

            isNewsDisplaying = true;

            // 아이콘 색상 변경 시작
            if (iconColorCoroutine != null)
            {
                StopCoroutine(iconColorCoroutine);
            }
            iconColorCoroutine = StartCoroutine(IconColorChange());

            StartCoroutine(CorMoveText());
        }
        else
        {
            isNewsDisplaying = false;

            // 아이콘 색상 변경 중지
            if (iconColorCoroutine != null)
            {
                StopCoroutine(iconColorCoroutine);
                iconColorCoroutine = null;

                // 아이콘 색상을 기본 색상(예: 흰색)으로 설정
                iconImage.color = Color.white;
            }
        }
    }

    private IEnumerator CorMoveText()
    {
        while (true)
        {
            if (Time.timeScale == 0)
            {
                yield return null;
                continue;
            }

            ins_traTitle.Translate(Vector2.left * speed * Time.deltaTime);

            if (ins_traTitle.anchoredPosition.x <= _vEndPos.x)
            {
                break;
            }
            yield return null;
        }

        // 텍스트가 사라지면 아이콘 깜빡임을 중지하고 다음 뉴스로 넘어감
        if (iconColorCoroutine != null)
        {
            StopCoroutine(iconColorCoroutine);
            iconColorCoroutine = null;
            iconImage.color = Color.white;
        }

        isNewsDisplaying = false;
        DisplayNextNews();
    }

    private IEnumerator IconColorChange()
    {
        Color[] colors = { Color.white, Color.red };
        int colorIndex = 0;

        while (true)
        {
            iconImage.color = colors[colorIndex];
            colorIndex = (colorIndex + 1) % colors.Length;
            yield return new WaitForSeconds(0.75f);
        }
    }
}
