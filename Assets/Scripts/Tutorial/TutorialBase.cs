using UnityEngine;

public abstract class TutorialBase : MonoBehaviour
{
    protected TutorialController tutorialController;
    // 해당 튜토리얼 과정을 시작할 때 1회 호출
    public virtual void Enter()
    {
        // TutorialController 인스턴스 미리 저장
        tutorialController = FindObjectOfType<TutorialController>();
        if (tutorialController == null)
        {
            Debug.LogError("TutorialController not found!");
        }
    }

    // 해당 튜토리얼 과정을 진행하는 동안 매 프레임 호출
    public abstract void Execute(TutorialController controller);

    // 해당 튜토리얼 과정을 종료할 때 1회 호출
    public abstract void Exit();
}
