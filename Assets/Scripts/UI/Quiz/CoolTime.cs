using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CoolTime : MonoBehaviour
{
    public TMP_Text cooltimeText;
    public Image fillImage;
    private float maxCooldown;
    private float currentCooldown;  //현재 남은 쿨타임 시간
    private System.Action onCooldownComplete;  //쿨타임이 끝나면 호출
    Coroutine _co;

    private void Start()
    {
        cooltimeText = Assign(cooltimeText, "TextCoolTime");
        fillImage = Assign(fillImage, "Shadow");
    }

    // 자동 할당 코드 
    private T Assign<T>(T obj, string objectName) where T : Object
    {
        if (obj == null)
        {
            GameObject foundObject = GameObject.Find(objectName);
            if (foundObject != null)
            {
                if (typeof(Component).IsAssignableFrom(typeof(T))) obj = foundObject.GetComponent(typeof(T)) as T;
                else if (typeof(GameObject).IsAssignableFrom(typeof(T))) obj = foundObject as T;
            }
            if (obj == null) Debug.LogError($"{objectName} 를 찾을 수 없습니다.");
        }
        return obj;

    }
    private float startTime; // 쿨타임 시작 시간

    public void StartCooldown(float remainingCooldown, float maxCooldown, System.Action onCooldownComplete)
    {
        this.maxCooldown = maxCooldown;  //쿨타임의 최대 시간 설정
        this.onCooldownComplete = onCooldownComplete;  //쿨타임이 끝날 때 실행할 함수 지정
        currentCooldown = remainingCooldown;
        startTime = Time.realtimeSinceStartup; // 시작 시간 기록
        UpdateCooltimeUI();
        gameObject.SetActive(true);  // 오브젝트를 활성화
        if (_co != null)
        {
            StopCoroutine(_co);
        }
        _co = StartCoroutine(CooldownCoroutine());
    }

    //시간이 남아있으면 매 프레임마다 currentCooldown을 감소시킴
    //쿨타임 끝나면 onCooldownCompete 호출
    private IEnumerator CooldownCoroutine()
    {
        float endTime = startTime + currentCooldown; // 종료 시간 계산

        while (Time.realtimeSinceStartup < endTime)
        {
            currentCooldown = endTime - Time.realtimeSinceStartup; // 남은 시간 계산
            UpdateCooltimeUI();
            yield return null;
        }

        currentCooldown = 0;
        UpdateCooltimeUI();
        onCooldownComplete?.Invoke();  //null이 아니면 호출해서 등록된 메서드 실행
        gameObject.SetActive(false);  // 오브젝트를 비활성화
    }

    //쿨타임 UI 업데이트
    private void UpdateCooltimeUI()
    {
        if (cooltimeText != null && fillImage != null)
        {
            // 남은 시간 계산
            int hours = Mathf.FloorToInt(currentCooldown / 3600F);
            int minutes = Mathf.FloorToInt((currentCooldown - hours * 3600) / 60F);
            int seconds = Mathf.FloorToInt(currentCooldown - hours * 3600 - minutes * 60);

            // 텍스트 업데이트
            cooltimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            // 이미지 업데이트
            fillImage.fillAmount = currentCooldown / maxCooldown;
        }
    }

}
