using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ArrowDirection
{
    Up,
    Down,
    Left,
    Right
}

[System.Serializable]
public struct MaskPosition
{
    public Vector4 maskRect;  // 강조할 사각형 위치 및 크기
    public bool isHighlighted;  // 강조 여부 (true이면 활성화, false이면 비활성화)
    public ArrowDirection arrowDirection; // 화살표 방향 (위, 아래, 왼쪽, 오른쪽)
}

public class MaskController : MonoBehaviour
{
    public Material maskMaterial;
    public List<MaskPosition> maskPositions;

    [SerializeField] private Image highlightArrowImage; // 강조할 화살표 이미지
    [SerializeField] private float arrowAnimationDistance = 10f; // 화살표 움직임 거리
    [SerializeField] private float arrowAnimationDuration = 0.5f; // 화살표 움직임 주기

    private Canvas canvas;
    private Coroutine arrowAnimationCoroutine;

    private void Start()
    {
        canvas = highlightArrowImage.GetComponentInParent<Canvas>();

        // 초기 마스크 설정
        if (maskPositions != null && maskPositions.Count > 0)
        {
            UpdateMask(0);
        }
    }

    // 현재 index에 해당하는 마스크 업데이트
    public void UpdateMask(int index)
    {
        if (index >= 0 && index < maskPositions.Count)
        {
            MaskPosition maskPos = maskPositions[index];
            maskMaterial.SetVector("_MaskRect", maskPos.maskRect);

            if (maskPos.isHighlighted)
            {
                UpdateHighlightArrow(maskPos.maskRect, maskPos.arrowDirection);
            }
            else
            {
                DisableHighlightArrow();
            }
        }
    }

    // 강조할 부분에 화살표 배치 및 활성화
    private void UpdateHighlightArrow(Vector4 maskValue, ArrowDirection direction)
    {
        if (highlightArrowImage != null && canvas != null)
        {
            highlightArrowImage.gameObject.SetActive(true);

            RectTransform arrowRectTransform = highlightArrowImage.GetComponent<RectTransform>();

            // 좌표 변환
            RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();
            Vector2 canvasSize = canvasRectTransform.sizeDelta;
            Vector2 maskCenter = new Vector2(maskValue.x * canvasSize.x, maskValue.y * canvasSize.y);
            Vector2 maskSize = new Vector2(maskValue.z * canvasSize.x, maskValue.w * canvasSize.y);

            // 방향에 따라 위치 및 회전 설정
            Vector3 offsetPosition = Vector3.zero;
            float rotationZ = 0f;

            switch (direction)
            {
                case ArrowDirection.Up:
                    offsetPosition = new Vector3(0, maskSize.y / 2 + 20f, 0);
                    rotationZ = 0f;
                    break;
                case ArrowDirection.Down:
                    offsetPosition = new Vector3(0, -maskSize.y / 2 - 20f, 0);
                    rotationZ = 180f;
                    break;
                case ArrowDirection.Left:
                    offsetPosition = new Vector3(-maskSize.x / 2 - 20f, 0, 0);
                    rotationZ = 90f;
                    break;
                case ArrowDirection.Right:
                    offsetPosition = new Vector3(maskSize.x / 2 + 20f, 0, 0);
                    rotationZ = 270f;
                    break;
            }

            arrowRectTransform.anchoredPosition = maskCenter - (canvasSize * 0.5f) + (Vector2)offsetPosition;
            arrowRectTransform.rotation = Quaternion.Euler(0, 0, rotationZ);

            // 중복된 코루틴 방지
            if (arrowAnimationCoroutine != null)
            {
                StopCoroutine(arrowAnimationCoroutine);
            }

            arrowAnimationCoroutine = StartCoroutine(AnimateHighlightArrow(direction));
        }
    }

    // 화살표에 애니메이션 효과 적용
    private IEnumerator AnimateHighlightArrow(ArrowDirection direction)
    {
        float elapsedTime = 0f;
        Vector3 originalPosition = highlightArrowImage.rectTransform.anchoredPosition;

        while (true)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float offset = Mathf.PingPong(elapsedTime, arrowAnimationDuration) / arrowAnimationDuration;
            float move = Mathf.Lerp(0f, arrowAnimationDistance, offset);

            // 방향에 따라 움직이는 방향을 설정
            if (direction == ArrowDirection.Up || direction == ArrowDirection.Down)
            {
                // 위, 아래는 상하로 움직임
                highlightArrowImage.rectTransform.anchoredPosition = originalPosition + new Vector3(0, move, 0);
            }
            else if (direction == ArrowDirection.Left || direction == ArrowDirection.Right)
            {
                // 왼쪽, 오른쪽은 좌우로 움직임
                highlightArrowImage.rectTransform.anchoredPosition = originalPosition + new Vector3(move, 0, 0);
            }

            yield return null;
        }
    }

    // 강조 화살표 비활성화
    public void DisableHighlightArrow()
    {
        if (highlightArrowImage != null)
        {
            highlightArrowImage.gameObject.SetActive(false);
            if (arrowAnimationCoroutine != null)
            {
                StopCoroutine(arrowAnimationCoroutine);
                arrowAnimationCoroutine = null;
            }
        }
    }
}
