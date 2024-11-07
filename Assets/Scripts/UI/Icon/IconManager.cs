using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class IconManager : MonoBehaviour
{
    public GameObject iconCanvas;
    public Sprite smileSprite;
    public Sprite[] symptomSprites;

    private PatientController patientController;
    private Coroutine symptomCoroutine;
    private Person person;
    private Image infectionIcon;

    private void Awake()
    {
        if (iconCanvas != null)
        {
            iconCanvas.SetActive(false);

            Transform infectionIconTransform = iconCanvas.transform.Find("InfectionIcon");
            if (infectionIconTransform != null)
            {
                infectionIcon = infectionIconTransform.GetComponent<Image>();
                if (infectionIcon == null)
                {
                    Debug.LogError("InfectionIcon 오브젝트에서 Image 컴포넌트를 찾을 수 없습니다.");
                }
            }
        }

        person = GetComponent<Person>();
        if (person == null)
        {
            Debug.LogError("Person component not found on the GameObject.");
        }
        else
        {
            person.OnInfectionStateChanged += HandleInfectionStateChanged; // 이벤트 구독
        }

        infectionIcon.sprite = smileSprite;

    }

    private void OnDestroy()
    {
        if (person != null)
        {
            person.OnInfectionStateChanged -= HandleInfectionStateChanged; // 이벤트 구독 해제
        }
    }

    private void HandleInfectionStateChanged(InfectionState newStatus)
    {
        if ((newStatus == InfectionState.CRE || newStatus == InfectionState.Covid) && (person.role == Role.Outpatient || person.role == Role.Inpatient))
        {
            if (symptomCoroutine != null)
            {
                StopCoroutine(symptomCoroutine);
            }
            symptomCoroutine = StartCoroutine(ShowRandomSymptomIcon());
            QuarantineManager targetQuarantineManager = person.gameObject.GetComponent<QuarantineManager>();
            targetQuarantineManager.StartCoroutine(targetQuarantineManager.Quarantine());
            //Debug.Log("증상 발현!");
        }
        else
        {
            if (symptomCoroutine != null)
            {
                StopCoroutine(symptomCoroutine);
                symptomCoroutine = null;
                iconCanvas.gameObject.SetActive(false); // Canvas 비활성화
            }
        }
    }

    private void Start()
    {
        patientController = GetComponent<PatientController>();
        if (patientController == null)
        {
            Debug.LogError("OutpatientController를 찾을 수 없습니다.");
        }
    }

    public void IsIcon()
    {
        StartCoroutine(ShowIcon());
    }

    private IEnumerator ShowIcon()
    {
        if (iconCanvas != null)
        {
            iconCanvas.SetActive(true);
            yield return new WaitForSeconds(2f);
            iconCanvas.SetActive(false);
        }
    }

    private IEnumerator ShowRandomSymptomIcon()
    {
        while ((person.status == InfectionState.CRE || person.status == InfectionState.Covid) && person.role == Role.Outpatient)
        {
            int randomIndex = Random.Range(0, symptomSprites.Length);

            infectionIcon.sprite = symptomSprites[randomIndex];
            iconCanvas.gameObject.SetActive(true); // Canvas 활성화

            yield return new WaitForSeconds(2); // 아이콘을 2초 동안 표시

            iconCanvas.gameObject.SetActive(false); // Canvas 비활성화

            yield return new WaitForSeconds(5); // 2초 대기
        }
    }

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

}