public class TutorialNews : TutorialBase
{
    public override void Enter()
    {
        NewsController newsController = FindObjectOfType<NewsController>();
        if (newsController != null)
        {
            // 뉴스 발생
            newsController.TriggerVirusOutbreakNews();
        }
    }

    public override void Execute(TutorialController controller)
    {
        // 뉴스가 끝나면 다음 튜토리얼로 넘어감
        controller.SetNextTutorial();
    }

    public override void Exit()
    {
        // 종료 로직 필요 시 작성
    }
}
