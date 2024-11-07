using UnityEngine;

public class MinimapRaycaster : MonoBehaviour
{
    public Camera minimapCamera; // 미니맵 카메라
    public MinimapController minimapController; // 미니맵 컨트롤러
    public RectTransform minimapRectTransform; // 미니맵 UI의 RectTransform
    private ClickableObject lastHighlighted = null; // 마지막으로 하이라이트된 오브젝트
    private Ray currentRay; // 현재 레이
    private bool isRayActive = false; // 레이 활성화 여부
    public float rayLength = 100f; // 레이 길이
    public LayerMask clickableLayer; // 클릭 가능한 오브젝트의 레이어

    void Start()
    {
        // 초기화 로직
        // Debug.Log("MinimapController found: " + (minimapController != null));
    }

    void Update()
    {
        if (minimapController != null)
        {
            if (minimapController.IsLargeMapOpen) // 미니맵 패널이 활성화된 상태에서만
            {
                HandleMinimapInteraction(); // 미니맵 상호작용 처리
            }
            else
            {
                ResetHighlight(); // 하이라이트 초기화
            }
        }
    }

    // 미니맵 상호작용을 처리하는 메서드
    void HandleMinimapInteraction()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector2 localPoint;

        // 마우스 포지션을 미니맵 UI의 로컬 포인트로 변환
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(minimapRectTransform, mousePosition, null, out localPoint))
        {
            // 로컬 포인트를 미니맵 카메라의 뷰포트 포인트로 변환
            Vector2 viewportPoint = new Vector2(
                (localPoint.x / minimapRectTransform.rect.width) + 0.5f,
                (localPoint.y / minimapRectTransform.rect.height) + 0.5f
            );

            // 뷰포트 포인트를 사용하여 레이캐스트
            Ray ray = minimapCamera.ViewportPointToRay(viewportPoint);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, rayLength, clickableLayer))
            {
                ClickableObject clickable = hit.collider.GetComponent<ClickableObject>();
                if (clickable != null)
                {
                    HandleHighlight(clickable); // 하이라이트 처리
                    HandleClick(clickable); // 클릭 처리
                }
                else
                {
                    Debug.Log("Hit object is not clickable: " + hit.collider.name);
                }
            }
            else
            {
                //Debug.Log("Ray did not hit anything");
                ResetHighlight(); // 하이라이트 초기화
            }
        }
    }

    // 층에 마우스 올리면 색상 변환
    void HandleHighlight(ClickableObject clickable)
    {
        if (lastHighlighted != clickable)
        {
            if (lastHighlighted != null)
            {
                lastHighlighted.OnMouseExit(); // 이전 하이라이트된 오브젝트의 색상 초기화
            }
            clickable.OnMouseEnter(); // 새로운 오브젝트의 색상 변경
            lastHighlighted = clickable; // 마지막 하이라이트된 오브젝트 업데이트
        }
    }

    // 층 클릭 시 작동
    void HandleClick(ClickableObject clickable)
    {
        if (Input.GetMouseButtonDown(0) && minimapController.IsLargeMapOpen)
        {
            Debug.Log("Mouse button down detected on: " + clickable.name);
            clickable.OnMouseDown(); // 오브젝트 클릭 처리
        }
    }

    // 층 색상 초기화
    void ResetHighlight()
    {
        if (lastHighlighted != null)
        {
            lastHighlighted.OnMouseExit(); // 마지막 하이라이트된 오브젝트의 색상 초기화
            lastHighlighted = null;
        }
        isRayActive = false; // 레이 비활성화
    }

    // 미니맵에 Ray를 잘 쏘고 있나 확인용
    void OnDrawGizmos()
    {
        if (isRayActive)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(currentRay.origin, currentRay.direction * rayLength); // 현재 레이의 길이만큼 빨간색으로 그림
        }
    }
}
