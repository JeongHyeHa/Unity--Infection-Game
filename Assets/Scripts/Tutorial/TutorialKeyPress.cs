using UnityEngine;
using UnityEngine.UI;

public class TutorialKeyPress : TutorialBase
{
    [SerializeField] private KeyCode keyToPress = KeyCode.Space; // 사용자에게 입력받을 키
    [SerializeField] private GameObject KeyImage;        // 키 이미지 UI
    [SerializeField] private Toggle keyCheckToggle; // 체크박스

    private bool isKeyPressed = false;

    public override void Enter()
    {
        isKeyPressed = false;

        // UI 초기 상태 설정
        KeyImage.SetActive(true);
        keyCheckToggle.isOn = false;
    }

    public override void Execute(TutorialController controller)
    {
        // 사용자가 지정된 키를 누르면
        if (Input.GetKeyDown(keyToPress))
        {
            isKeyPressed = true;
            keyCheckToggle.isOn = true;
            Debug.Log("Key pressed: " + keyToPress.ToString());

            // 다음 튜토리얼로 이동
            StartCoroutine(controller.Delay());
        }
    }

    public override void Exit()
    {
        // UI 요소 비활성화
        KeyImage.SetActive(false);
        keyCheckToggle.isOn = false;
    }
}
