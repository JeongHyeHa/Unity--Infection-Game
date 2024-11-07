using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//플레이어의 상태(감염병)를 나타내는 enum
//Stage1은 접촉성 감염병
//Stage2은 비접촉성(범위형) 감염병
public enum InfectionState
{
    Normal,
    CRE,
    Covid
}
public enum Role
{
    Doctor,
    Nurse,
    Outpatient,
    Inpatient,
    EmergencyPatient,
    ICUPatient
}
public class Person : MonoBehaviour
{
    public InfectionState status = InfectionState.Normal;
    public int infectionResistance = 0;
    public Role role;

    public bool isImmune;
    private CapsuleCollider coll;
    private bool isWaiting;

    public PatientController patientController;
    public delegate void InfectionStateChanged(InfectionState newStatus);
    public event InfectionStateChanged OnInfectionStateChanged;


    public int ID { get; private set; }
    public string Name { get; private set; }
    public string Job { get; private set; }
    public bool IsResting { get; private set; }
    public Dictionary<string, Item> Inventory => RoleInventoryManager.GetInventoryByRole(role); // Role 기반 Inventory 참조
    public Sprite AvatarSprite { get; private set; } // 추가된 필드
    public bool IsMale { get; private set; } // 성별 필드 추가


    public void Initialize(int id, string name, string job, bool isResting, Role role)
    {
        ID = id;
        Name = name;
        Job = job;
        IsResting = isResting;
        this.role = role;

        // 성별 랜덤 설정
        IsMale = Random.Range(0, 2) == 0;
        AssignName();

        // AvatarSprite가 null인 경우에만 로드
        if (AvatarSprite == null)
        {
            string genderFolder = IsMale ? "Man" : "Woman";
            Sprite[] avatars = Resources.LoadAll<Sprite>($"Sprites/Avatars/{genderFolder}");
            if (avatars != null && avatars.Length > 0)
            {
                AvatarSprite = avatars[Random.Range(0, avatars.Length)];
            }
        }
    }
    void Start()
    {
        Transform ballTransform = transform.Find("IsInfection");
        coll = GetComponent<CapsuleCollider>();

        patientController = GetComponent<PatientController>();

    }
    void FixedUpdate()
    {

        //감염병 종류에 따라 감염 범위 설정
        if (status == InfectionState.CRE)
        {
            if (patientController != null)
            {
                if (patientController.standingState == StandingState.LayingDown && !patientController.isQuarantined)
                {
                    coll.radius = 2.5f;
                }
                else
                {
                    coll.radius = 0.3f;

                }
            }
            else
            {
                coll.radius = 0.3f;

            }

        }
        else if (status == InfectionState.Covid)
        {

            if (patientController != null)
            {
                if (patientController.standingState == StandingState.LayingDown && !patientController.isQuarantined)
                {
                    coll.radius = 3.0f;
                }
                else
                {
                    coll.radius = 1.0f;

                }
            }
            else
            {
                coll.radius = 1.0f;
            }
        }
        else if (status == InfectionState.Normal)
        {
            if (patientController != null)
            {
                if (patientController.standingState == StandingState.LayingDown && !patientController.isQuarantined)
                {

                    coll.radius = 2.3f;
                }
                else
                {
                    coll.radius = 0.2f;

                }
            }
            else
            {
                coll.radius = 0.2f;

            }
        }
        if (isWaiting)
        {
            return;
        }
        if (status != InfectionState.Normal)
        {
            //ballRenderer.enabled = true;
        }
        else
        {
            //ballRenderer.enabled = false;
        }

        //착용하고 있는 보호 장비에 따라 감염 저항성 설정

    }
    public void AssignName()
    {
        Name = NameList.GetUniqueName(role, IsMale);
    }

    public void ResetInventory()
    {
        List<string> keys = new List<string>(Inventory.Keys);
        foreach (var key in keys)
        {
            Inventory[key] = new Item(Inventory[key].itemName, false, Inventory[key].protectionRate);
        }
    }
    public void ToggleRestingState()
    {
        IsResting = !IsResting;
        if (IsResting)
        {
            ResetInventory();
        }
    }

    public void ResetPerson()
    {
        role = Role.Outpatient;
        status = InfectionState.Normal;
        // 필요한 다른 속성들도 초기화
    }


    public void ChangeStatus(InfectionState infection)
    {
        gameObject.GetComponent<NPCController>().wardComponent.infectedNPC++;
        NPCManager.Instance.HighlightNPC(gameObject);
        //Debug.Log("감염자 색상 변경" + gameObject.name);
        StartCoroutine(IncubationPeriod(infection));
        if (Random.Range(0, 100) <= 30)
        {
            StartCoroutine(SelfRecovery());
        }
    }

    public IEnumerator SelfRecovery()
    {
        yield return new WaitForSeconds(Random.Range(7, 15));
        //Debug.Log("자가 면역을 가져서 더 이상 감염되지 않음");
        NPCManager.Instance.UnhighlightNPC(gameObject);
        //Debug.Log("감염자 색상 풀림" + gameObject.name);
        status = InfectionState.Normal;
        isImmune = true;
    }
    public void Recover()
    {
        status = InfectionState.Normal;
    }
    private IEnumerator IncubationPeriod(InfectionState infection)
    {
        status = infection;
        isWaiting = true;
        yield return new WaitForSeconds(5);
        isWaiting = false;
        OnInfectionStateChanged?.Invoke(infection); // 이벤트 호출
    }

    public float GetTotalProtectionRate()
    {
        float totalProtectionRate = 0f;
        foreach (var item in Inventory.Values)
        {
            if (item.isEquipped)
            {
                totalProtectionRate += item.protectionRate;
            }
        }
        return totalProtectionRate;
    }
}
