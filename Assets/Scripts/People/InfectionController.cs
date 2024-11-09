using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InfectionController : MonoBehaviour
{
    public CreateVirus createVirusManager;
    public TextMeshProUGUI selectedLevel; // 난이도 UI 텍스트
    private Person person;       //추가
    private List<Person> delayList = new List<Person>();
    //private InfectionState activeInfectionState = InfectionState.Normal; // 현재 게임에서 활성화된 감염 상태


    Coroutine _co;

    // 감염 상태 확인 프로퍼티
    public bool isInfected => person != null && person.status != InfectionState.Normal;

    // 감염 저항성 프로퍼티
    public int infectionResistance => person != null ? person.infectionResistance : 0;

    // 감염 저항성 설정 메서드
    public void SetInfectionResistance(int infectionResistance_)
    {
        if (person != null)
        {
            person.infectionResistance += infectionResistance_;
        }
    }

    private void Awake()
    {
        selectedLevel = Assign(selectedLevel, "SelectedLevel");
    }

    // 감염이면 바이러스 떨어뜨리기 반복
    void Start()
    {
        person = GetComponent<Person>();
        createVirusManager = Assign(createVirusManager, "DisinfectionManager");
        StartCoroutine(CheckInfectionStatus());
    }

    private InfectionState ChangeNPCState()
    {
        if (selectedLevel.text == "Easy")
            return InfectionState.CRE;
        else if (selectedLevel.text == "Normal")
            return InfectionState.Covid;
        return InfectionState.Normal;

    }

    IEnumerator CheckInfectionStatus()
    {
        while (true)
        {
            if (person.status != InfectionState.Normal && _co == null)
            {
                _co = StartCoroutine(TryDropVirus());
            }
            yield return new WaitForSeconds(Virus.checkInterval);
        }
    }


    //감염이면 바이러스 떨어뜨리기 시도
    IEnumerator TryDropVirus()
    {
        while (person.status != InfectionState.Normal)
        {
            if (Random.value < Virus.virusDropProbability)
            {
                DropVirus();
            }
            yield return new WaitForSeconds(Virus.checkInterval);
        }
    }

    //바이러스 떨어뜨리기
    //현재 위치에서 무작위로 약간 이동된 위치에 바이러스 생성
    void DropVirus()
    {
        Vector3 dropPosition = transform.position + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        createVirusManager.CreateVirusObject(dropPosition, person);
        //Debug.Log("바이러스 떨어뜨리기 작동"); //수정
    }

    void OnTriggerEnter(Collider other)
    {
        InfectionState thisPersonStatus = GetComponent<Person>().status;
        // 충돌한 오브젝트의 레이어가 지정된 레이어 마스크에 포함되어 있는지 확인
        if (other.gameObject == gameObject || thisPersonStatus == InfectionState.Normal)
        {
            return;
        }

        Person otherPerson = other.GetComponent<Person>();
        if (otherPerson == null)
        {
            return;
        }
        if (delayList.Contains(otherPerson))
        {
            //Debug.Log("이미 접촉된 사람");
            return;
        }
        if (otherPerson.status != InfectionState.Normal || otherPerson.isImmune)
        {
            return;
        }
        int random = Random.Range(0, Managers.Infection.infectionProbability);
        //감염되는 사람의 감염 저항성을 고려하여 감염 확률 계산
        int totalRandom = Random.Range(0, 101);
        if (random - otherPerson.infectionResistance >= totalRandom)
        {
            //Debug.Log(random - otherPerson.infectionResistance + " 값이 나왔기 때문에 감염됨");
            //other.GetComponent<NPCController>().wardComponent.infectedNPC++;
            otherPerson.ChangeStatus(thisPersonStatus);
        }
        else
        {
            //Debug.Log(random - otherPerson.infectionResistance + "값이 나왔기 때문에 감염되지 않음");
        }
        delayList.Add(otherPerson);
        StartCoroutine(CoRemoveDelay(otherPerson));
    }

    IEnumerator CoRemoveDelay(Person person)
    {
        yield return new WaitForSeconds(0.5f);
        delayList.Remove(person);
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
}