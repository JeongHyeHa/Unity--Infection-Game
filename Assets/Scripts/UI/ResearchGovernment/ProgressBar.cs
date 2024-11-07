using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ProgressBar : MonoBehaviour
{
    //public Image chargeProgress;  // 진행바 이미지
    //public float baseInspectionTime = 3f; // 기본 검사 시간 (초)
    //public PanelManager panelManager;       // PanelManager 스크립트 참조
    ////private Coroutine fillCoroutine;   // 진행 코루틴  
    //private bool isFilling = false;  // 현재 진행바가 채워지고 있는지 여부

    //void Start()
    //{
    //    // 오브젝트 자동 할당
    //    chargeProgress = Assign(chargeProgress, "ChargeProgress");
    //    panelManager = Assign(panelManager, "PanelManager");

    //    chargeProgress.fillAmount = 0;
    //}

    //// 연구 레벨에 따른 진행바 관리(현재 연구 레벨, 진행바 끝나면 실행할 함수)
    //public void StartFilling(int currentResearchLevel, System.Action onComplete = null)
    //{
    //    if (isFilling)
    //    {
    //        panelManager?.ShowWarning("WARNING", "이미 진행 중인 검사가 있습니다.");
    //        return;
    //    }

    //    float inspectionTime = CalculateSpeed(currentResearchLevel);
    //    Debug.Log($"검사 시간이 {inspectionTime}초로 설정되었습니다.");

    //    StartCoroutine(FillOverTime(inspectionTime, onComplete)); // 조정된 시간으로 진행바 시작
    //}

    //// 단계별 검사시간
    //private float CalculateSpeed(int currentResearchLevel)
    //{
    //    switch (currentResearchLevel)
    //    {
    //        case 1:
    //            return 120f; //1단계 - 120초
    //        case 2:
    //            return 180f; // 2단계 - 180초
    //        case 3:
    //            return 60f;  // 3단계 - 60초
    //        default:
    //            return 10f; // 기본 속도 (1단계 - 120초)
    //    }
    //    /*switch (currentResearchLevel)
    //    {
    //        case 2:
    //            return 1.5f; // 1.5배 속도 증가
    //        case 3:
    //            return 2.0f; // 2배 속도 증가
    //        default:
    //            return 1.0f; // 기본 속도
    //    }*/
    //}


    //// 지정된 시간 동안 진행바를 채우는 코루틴
    //private IEnumerator FillOverTime(float duration, System.Action onComplete)
    //{
    //    isFilling = true;  //검사 진행 중
    //    float elapsed = 0f;
    //    chargeProgress.fillAmount = 0f;

    //    while (elapsed < duration)
    //    {
    //        elapsed += Time.deltaTime;
    //        chargeProgress.fillAmount = Mathf.Clamp01(elapsed / duration);
    //        yield return null;
    //    }

    //    chargeProgress.fillAmount = 1f;
    //    onComplete?.Invoke(); // 콜백 함수 호출
    //    isFilling = false;
    //}

    //public void ResetProgress()
    //{
    //    chargeProgress.fillAmount = 0;
    //}

    //// 자동 할당 코드 
    //private T Assign<T>(T obj, string objectName) where T : Object
    //{
    //    if (obj == null)
    //    {
    //        GameObject foundObject = GameObject.Find(objectName);
    //        if (foundObject != null)
    //        {
    //            if (typeof(Component).IsAssignableFrom(typeof(T))) obj = foundObject.GetComponent(typeof(T)) as T;
    //            else if (typeof(GameObject).IsAssignableFrom(typeof(T))) obj = foundObject as T;
    //        }
    //        if (obj == null) Debug.LogError($"{objectName} 를 찾을 수 없습니다.");
    //    }
    //    return obj;
    //}
}