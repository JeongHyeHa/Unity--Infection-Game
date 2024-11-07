using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    public bool isPolicyMenuOpen = false; // 정책 창이 열려 있는지 여부

    public enum ViewMode { TopView, QuarterView }
    public ViewMode currentViewMode = ViewMode.TopView;

    public Camera mainCamera;

    private CameraController cameraController;

    public float rotationSpeed = 5.0f;
    public float zoomSpeed = 3.0f;
    public float minZoomDistance = 2.0f;
    public float maxZoomDistance = 20.0f;
    public float moveSpeed = 3.0f;
    public float edgeMoveSpeed = 5.0f;
    public float minYPosition = 15.0f;
    public float minFOV = 15.0f;
    public float maxFOV = 90.0f;
    public float edgeBoundary = 10.0f;

    public Vector3 centerPosition;
    public float maxXDistanceFromCenter = 50.0f;
    public float maxZDistanceFromCenter = 50.0f;

    public Vector3 movementDirection { get; private set; }

    void Start()
    {
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera is not assigned!");
            return;
        }

        // GetComponent를 사용하여 같은 GameObject에 있는 CameraController를 가져옴
        cameraController = GetComponent<CameraController>();
    }

    void Update()
    {
        movementDirection = Vector3.zero;
        HandleKeyboardMovement();
        if (movementDirection == Vector3.zero)
        {
            HandleEdgeMovement();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            cameraController.ToggleViewMode();
        }
        ApplyMovement();
        HandleMouseZoom();
        LimitCameraPosition();
    }



    void HandleKeyboardMovement()
    {
        if (currentViewMode == ViewMode.TopView)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                movementDirection += Vector3.back;
            }
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                movementDirection += Vector3.forward;
            }
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                movementDirection += Vector3.right;
            }
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                movementDirection += Vector3.left;
            }
        }
        else if (currentViewMode == ViewMode.QuarterView)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                movementDirection += (Vector3.back + Vector3.left).normalized;
            }
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                movementDirection += (Vector3.forward + Vector3.right).normalized;
            }
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                movementDirection += (Vector3.right + Vector3.back).normalized;
            }
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                movementDirection += (Vector3.left + Vector3.forward).normalized;
            }
        }
    }

    void HandleEdgeMovement()
    {
        if (currentViewMode == ViewMode.TopView)
        {
            if (Input.mousePosition.x >= Screen.width - edgeBoundary)
            {
                movementDirection += Vector3.left;
            }
            if (Input.mousePosition.x <= edgeBoundary)
            {
                movementDirection += Vector3.right;
            }
            if (Input.mousePosition.y >= Screen.height - edgeBoundary)
            {
                movementDirection += Vector3.back;
            }
            if (Input.mousePosition.y <= edgeBoundary)
            {
                movementDirection += Vector3.forward;
            }
        }
        else if (currentViewMode == ViewMode.QuarterView)
        {
            if (Input.mousePosition.x >= Screen.width - edgeBoundary)
            {
                movementDirection += (Vector3.left + Vector3.forward).normalized;
            }
            if (Input.mousePosition.x <= edgeBoundary)
            {
                movementDirection += (Vector3.right + Vector3.back).normalized;
            }
            if (Input.mousePosition.y >= Screen.height - edgeBoundary)
            {
                movementDirection += (Vector3.back + Vector3.left).normalized;
            }
            if (Input.mousePosition.y <= edgeBoundary)
            {
                movementDirection += (Vector3.forward + Vector3.right).normalized;
            }
        }
    }

    void HandleMouseZoom()
    {
        if (isPolicyMenuOpen) return; // 정책 창이 열려 있을 때 줌 기능 비활성화

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        mainCamera.fieldOfView -= scroll * zoomSpeed;
        mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView, minFOV, maxFOV);
    }


    void ApplyMovement()
    {
        if (movementDirection != Vector3.zero)
        {
            Vector3 newPosition = mainCamera.transform.position + movementDirection * moveSpeed * Time.deltaTime;
            newPosition.y = mainCamera.transform.position.y;
            mainCamera.transform.position = newPosition;
        }
    }

    void LimitCameraPosition()
    {
        Vector3 position = mainCamera.transform.position;

        if (currentViewMode == ViewMode.TopView)
        {
            float minX = centerPosition.x - maxXDistanceFromCenter;
            float maxX = centerPosition.x + maxXDistanceFromCenter;
            float minZ = centerPosition.z - maxZDistanceFromCenter;
            float maxZ = centerPosition.z + maxZDistanceFromCenter;

            position.x = Mathf.Clamp(position.x, minX, maxX);
            position.z = Mathf.Clamp(position.z, minZ, maxZ);
        }
        else if (currentViewMode == ViewMode.QuarterView)
        {
            Vector3 offsetFromCenter = position - centerPosition;
            Vector2 rotatedOffset = RotateVector2(new Vector2(offsetFromCenter.x, offsetFromCenter.z), -45f);

            rotatedOffset.x = Mathf.Clamp(rotatedOffset.x, -maxXDistanceFromCenter, maxXDistanceFromCenter);
            rotatedOffset.y = Mathf.Clamp(rotatedOffset.y, -maxZDistanceFromCenter, maxZDistanceFromCenter);

            rotatedOffset = RotateVector2(rotatedOffset, 45f);

            position.x = centerPosition.x + rotatedOffset.x;
            position.z = centerPosition.z + rotatedOffset.y;
        }

        mainCamera.transform.position = position;
    }

    Vector2 RotateVector2(Vector2 v, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);

        float tx = v.x;
        float ty = v.y;

        return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
    }

    public void SetViewMode(ViewMode mode)
    {
        currentViewMode = mode;
    }

    public void SetCenterPosition(Vector3 newCenterPosition)
    {
        centerPosition = newCenterPosition;
    }
}
