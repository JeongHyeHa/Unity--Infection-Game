using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class StageManager
{
    public int stage = 1;

    public void ChangeStage(int newStage)
    {
        stage = newStage;
        Debug.Log($"스테이지가 {newStage}로 변경되었습니다.");
        // 추가적으로 스테이지에 따라 초기화 작업이나 설정을 할 수 있습니다.
    }

    public int GetCurrentStage()
    {
        return stage;
    }

    public void Init()
    {
        // 스테이지 초기화 관련 작업
    }
}
