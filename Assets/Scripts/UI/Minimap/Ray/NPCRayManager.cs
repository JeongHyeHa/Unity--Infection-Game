//using UnityEngine;
//using UnityEngine.EventSystems;

//public class NPCRaymanager : MonoBehaviour
//{
//    public static NPCRaymanager Instance { get; private set; }

//    private LayerMask cullingMask; // Main Camera의 Culling Mask

//    private GameObject lastHighlightedNPC = null; // 마지막으로 하이라이트된 NPC

//    private void Awake()
//    {
//        // 싱글톤 패턴 구현
//        if (Instance == null)
//        {
//            Instance = this;
//            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않도록 설정
//        }
//        else
//        {
//            Destroy(gameObject); // 이미 인스턴스가 존재하면 새로운 인스턴스를 파괴
//        }
//    }

//    private void Start()
//    {
//        // 초기 카메라의 Culling Mask 설정
//        UpdateCullingMask();
//    }

//    private void Update()
//    {
//        // 마우스가 UI 위에 있을 때 모든 하이라이트 해제
//        if (EventSystem.current.IsPointerOverGameObject())
//        {
//            UnhighlightAllNPCs();
//            return; // UI가 마우스 이벤트를 차단하지 않도록 처리
//        }

//        // 카메라의 Culling Mask 업데이트
//        UpdateCullingMask();

//        // 마우스 위치에서 Ray를 생성
//        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//        RaycastHit hit;

//        // Raycast를 사용하여 NPC 레이어와 충돌 검사
//        if (Physics.Raycast(ray, out hit, 100f, cullingMask))
//        {
//            Transform currentTransform = hit.collider.transform;
//            Person person = null;

//            // 충돌한 오브젝트와 그 부모 오브젝트들을 탐색하여 Person 컴포넌트를 찾음
//            while (currentTransform != null)
//            {
//                person = currentTransform.GetComponent<Person>();
//                if (person != null) break;
//                currentTransform = currentTransform.parent;
//            }

//            if (person != null)
//            {
//                // 마우스 오버 처리
//                HandleMouseOver(hit.collider.gameObject);

//                // 마우스 클릭 처리
//                if (Input.GetMouseButtonDown(0))
//                {
//                    HandleMouseClick(person);
//                }
//            }
//            else
//            {
//                // Raycast가 NPC에 닿지 않으면 모든 하이라이트 해제
//                UnhighlightAllNPCs();
//            }
//        }
//        else
//        {
//            // Raycast가 NPC에 닿지 않으면 모든 하이라이트 해제
//            UnhighlightAllNPCs();
//        }
//    }

//    // NPC 오브젝트를 하이라이트하는 메서드
//    private void HandleMouseOver(GameObject npcObject)
//    {
//        // NPC를 하이라이트
//        NPCManager.Instance.HighlightNPC(npcObject);

//        // 이전에 하이라이트된 NPC가 있고 그것이 현재 NPC와 다르다면 하이라이트 해제
//        if (lastHighlightedNPC != null && lastHighlightedNPC != npcObject)
//        {
//            NPCManager.Instance.UnhighlightNPC(lastHighlightedNPC);
//        }

//        // 마지막 하이라이트된 NPC 업데이트
//        lastHighlightedNPC = npcObject;
//    }

//    // NPC 클릭 시 처리하는 메서드
//    private void HandleMouseClick(Person person)
//    {
//        PatientController patientController = person.gameObject.GetComponent<PatientController>();
//        Debug.Log("NPC clicked: " + person.gameObject.name);
        
//    }

//    // 모든 NPC의 하이라이트를 해제하는 메서드
//    private void UnhighlightAllNPCs()
//    {
//        if (lastHighlightedNPC != null)
//        {
//            NPCManager.Instance.UnhighlightAllNPCs();
//            lastHighlightedNPC = null;
//        }
//    }

//    // 카메라의 Culling Mask를 업데이트하는 메서드
//    private void UpdateCullingMask()
//    {
//        if (Camera.main != null)
//        {
//            cullingMask = Camera.main.cullingMask;
//        }
//    }
//}
