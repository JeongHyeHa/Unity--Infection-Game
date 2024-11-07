using UnityEngine;

public class NPCSelector : MonoBehaviour
{
    private LayerMask mainCameraCullingMask; // Main 카메라의 CullingMask

    void Start()
    {
        // Main 카메라의 CullingMask 설정
        mainCameraCullingMask = Camera.main.cullingMask;
    }

    void FixedUpdate()
    {
        // 왼쪽 마우스 버튼이 눌렸을 때 처리
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // 마우스 포인터의 화면 좌표를 기준으로 Ray 생성
            RaycastHit hit; // RaycastHit 정보 저장 변수

            // 충돌을 감지하도록 설정, Main 카메라의 CullingMask를 사용하여 검사
            if (Physics.Raycast(ray, out hit, 100f, mainCameraCullingMask))
            {
                // Debug.Log("Raycast hit: " + hit.collider.name); // Raycast가 hit한 콜라이더의 이름 로그 출력

                // 충돌 지점에 작은 구를 그려서 시각적으로 확인 (디버그용)
                // Debug.DrawRay(hit.point, Vector3.up * 1f, Color.green, 2f);

                // 충돌한 Collider를 가진 오브젝트와 그 부모 오브젝트들을 탐색하여 Person 컴포넌트를 찾음
                Transform currentTransform = hit.collider.transform;
                while (currentTransform != null)
                {
                    Person person = currentTransform.GetComponent<Person>();
                    if (person != null)
                    {
                        Debug.Log("NPC clicked: " + person.gameObject.name); // NPC 클릭 로그 출력
                        break; // Person 컴포넌트를 찾으면 반복 종료
                    }
                    currentTransform = currentTransform.parent; // 부모 오브젝트로 이동
                }

                if (currentTransform == null)
                {
                    // Debug.Log("No Person component found on the hit object or its parents."); // Person 컴포넌트를 찾지 못한 경우 로그 출력
                }
            }
            else
            {
                // Debug.Log("Raycast did not hit any object."); // Raycast가 아무 오브젝트도 hit하지 못한 경우 로그 출력
            }
        }
    }
}
