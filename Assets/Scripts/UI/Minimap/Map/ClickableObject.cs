using UnityEngine;

public class ClickableObject : MonoBehaviour
{
    private Color[] originalColors; // 원래 색상을 배열로 저장
    private Color highlightColor = new Color(0.5f, 0.5f, 1.0f, 1.0f); // 연한 파랑색

    public int floorIndex;
    public string floorName; // 각 오브젝트에 원하는 층 이름을 할당

    private CameraController cameraController;
    private ProfileWindow profileWindow;

    void Start()
    {
        // 원래 색상을 저장합니다.
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            originalColors = new Color[renderer.materials.Length];
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                originalColors[i] = renderer.materials[i].color;
            }
        }
        profileWindow = FindObjectOfType<ProfileWindow>();
        cameraController = FindObjectOfType<CameraController>();
    }

    public void OnMouseDown()
    {
        Debug.Log("OnMouseDown called on " + gameObject.name);

        // 오브젝트의 이름을 가져옵니다
        string FloorName = gameObject.name;

        // 오브젝트 이름에 따라 다른 메시지를 출력하고 카메라 위치를 전환
        switch (FloorName)
        {
            case "Floor 1 L":
                Debug.Log("1층 왼쪽이 클릭되었습니다!");
                ChangeFloor("응급실");
                profileWindow.UpdateButtonTexts("응급실");
                break;
            case "Floor 1 R":
                Debug.Log("1층 오른쪽이 클릭되었습니다!");
                ChangeFloor("중환자실");
                profileWindow.UpdateButtonTexts("중환자실");
                break;
            case "Floor 2 L":
                Debug.Log("2층 왼쪽이 클릭되었습니다!");
                ChangeFloor("내과 1");
                profileWindow.UpdateButtonTexts("내과 1");
                break;
            case "Floor 2 R":
                Debug.Log("2층 오른쪽이 클릭되었습니다!");
                ChangeFloor("내과 2");
                profileWindow.UpdateButtonTexts("내과 2");
                break;
            case "Floor 3 L":
                Debug.Log("3층 왼쪽이 클릭되었습니다!");
                ChangeFloor("외과 1");
                profileWindow.UpdateButtonTexts("외과 1");
                break;
            case "Floor 3 R":
                Debug.Log("3층 오른쪽이 클릭되었습니다!");
                ChangeFloor("외과 2");
                profileWindow.UpdateButtonTexts("외과 2");
                break;
            case "Floor 4 L":
                Debug.Log("4층 왼쪽이 클릭되었습니다!");
                ChangeFloor("입원병동1");
                profileWindow.UpdateButtonTexts("입원병동1");
                break;
            case "Floor 4 R":
                Debug.Log("4층 오른쪽이 클릭되었습니다!");
                ChangeFloor("입원병동2");
                profileWindow.UpdateButtonTexts("입원병동2");
                break;
            case "Floor 5 L":
                Debug.Log("5층 왼쪽이 클릭되었습니다!");
                ChangeFloor("입원병동3");
                profileWindow.UpdateButtonTexts("입원병동3");
                break;
            case "Floor 5 R":
                Debug.Log("5층 오른쪽이 클릭되었습니다!");
                ChangeFloor("입원병동4");
                profileWindow.UpdateButtonTexts("입원병동4");
                break;
            case "Roof":
                Debug.Log("옥상이 클릭되었습니다!");
                ChangeFloor("옥상");
                profileWindow.UpdateButtonTexts("옥상");
                break;
            default:
                Debug.Log(FloorName + "가 클릭되었습니다!");
                break;
        }
    }

    public void ChangeFloor(string floorName)
    {
        if (cameraController != null)
        {
            cameraController.MoveToFloor(floorIndex);   // 카메라를 특정 층으로 이동시키고 레이어를 설정
        }
        UIManager.Instance.UpdateCurrentFloorText(floorName);
    }

    public void OnMouseEnter()
    {
        // 마우스가 오브젝트 위에 올라갔을 때 색상을 변경합니다
        ChangeColor(highlightColor);
        UIManager.Instance.ShowObjectName(floorName); // 지정된 층 이름 표시
    }

    public void OnMouseExit()
    {
        // 마우스가 오브젝트에서 벗어났을 때 원래 색상으로 복원합니다
        RestoreOriginalColor();
        UIManager.Instance.HideObjectName(); // 이름 숨기기
    }

    void ChangeColor(Color color)
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            foreach (Material mat in renderer.materials)
            {
                mat.color = color;
            }
        }
    }

    void RestoreOriginalColor()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null && originalColors != null)
        {
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                renderer.materials[i].color = originalColors[i];
            }
        }
    }
}
