using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsResearch : MonoBehaviour
{
    //public Person person;

    // 추후에 딕셔너리로 관리 해보기
    public bool isMedicalResearch_0 = false;
    public bool isMedicalResearch_1 = false;
    public bool isMedicalResearch_2 = false;
    public bool isMedicalResearch_3 = false;
    public bool isMedicalResearch_4 = false;
    public bool isMedicalResearch_5 = false;
    public bool isMedicalResearch_6 = false;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // 특정 연구 완료 시 장비 착용 설정
    public void EveryoneChangeEquipState(string itemName)
    {
        foreach (Item item in Managers.Item.items)
        {
            if (item.itemName == itemName)
            {
                item.isEquipped = true;
            }
        }
        List<Person> persons = PersonManager.Instance.GetAllPersons();
        foreach (Person person in persons)
        {
            if (person.Inventory.ContainsKey(itemName))
            {
                person.Inventory[itemName].isEquipped = true;
            }
        }
    }

    // 특정 연구 완료 시 감염방지율 수정 -> 아직 생성되는 사람들은 수정 안된 상태.
    public void EveryoneChangeInfectionResistance(int rate)
    {
        List<Person> persons = PersonManager.Instance.GetAllPersons();
        foreach (Person person in persons)
        {
            if (person.role == Role.Doctor || person.role == Role.Nurse)
            {
                person.infectionResistance = rate;
            }
        }
    }

    public void IsOnMedicalResearch_0()
    {
        EveryoneChangeEquipState("Dental 마스크");
        EveryoneChangeEquipState("일회용 장갑");
        Debug.Log("의료진 연구 0번 실행");
    }
    public void IsOnMedicalResearch_1()
    {
        EveryoneChangeEquipState("N95 마스크");
        EveryoneChangeEquipState("라텍스 장갑");
        Debug.Log("의료진 연구 1번 실행");
    }
    public void IsOnMedicalResearch_2()
    {
        EveryoneChangeEquipState("의료용 헤어캡");
        EveryoneChangeEquipState("의료용 고글");
        EveryoneChangeEquipState("AP 가운");
        Debug.Log("의료진 연구 2번 실행");
    }
    public void IsOnMedicalResearch_3()
    {
        EveryoneChangeEquipState("Level C");
        Debug.Log("의료진 연구 3번 실행");
    }
    public void IsOnMedicalResearch_4()
    {
        EveryoneChangeInfectionResistance(10);
        Debug.Log("의료진 연구 4번 실행");
    }
    public void IsOnMedicalResearch_5()
    {
        EveryoneChangeInfectionResistance(20);
        Debug.Log("의료진 연구 5번 실행");
    }
    public void IsOnMedicalResearch_6()
    {
        EveryoneChangeInfectionResistance(30);
        Debug.Log("의료진 연구 6번 실행");
    }

}
