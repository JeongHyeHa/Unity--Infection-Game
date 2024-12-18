﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//플레이어의 상태(감염병)를 나타내는 enum
//Stage1은 접촉성 감염병
//Stage2은 비접촉성(범위형) 감염병
public enum InfectionStatus
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
    ICUPatient,
    QuarantinedPatient
}
public class Person : MonoBehaviour
{
    public InfectionStatus infectionStatus = InfectionStatus.Normal;
    public int infectionResistance = 0;
    public int vaccineResist = 0;
    public Role role;

    public bool isImmune;
    private CapsuleCollider coll;
    private bool isWaiting;

    public PatientController patientController;
    public delegate void InfectionStateChanged(InfectionStatus newStatus);
    public event InfectionStateChanged OnInfectionStateChanged;


    public int ID { get; private set; }
    public string Name { get; private set; }
    public string Job { get; private set; }
    public bool IsResting { get; private set; }
    public Dictionary<string, Item> Inventory { get; private set; } // Role 기반 Inventory 참조
    public Sprite AvatarSprite { get; private set; } // 추가된 필드
    public bool IsMale { get; private set; } // 성별 필드 추가


    public void Initialize(int id, string name, string job, bool isResting, Role role)
    {
        ID = id;
        Name = name;
        Job = job;
        IsResting = isResting;
        this.role = role;

        Inventory = new Dictionary<string, Item>();
        foreach (var item in RoleInventoryManager.GetInventoryByRole(role))
        {
            Inventory[item.Key] = item.Value.Clone();
        }

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
        if (infectionStatus == InfectionStatus.CRE)
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
        else if (infectionStatus == InfectionStatus.Covid)
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
        else if (infectionStatus == InfectionStatus.Normal)
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
        if (infectionStatus != InfectionStatus.Normal)
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


    public void ChangeStatus(InfectionStatus infection)
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
        yield return YieldInstructionCache.WaitForSeconds(Random.Range(7, 15));
        //Debug.Log("자가 면역을 가져서 더 이상 감염되지 않음");
        NPCManager.Instance.UnhighlightNPC(gameObject);
        //Debug.Log("감염자 색상 풀림" + gameObject.name);
        infectionStatus = InfectionStatus.Normal;
        gameObject.GetComponent<NPCController>().wardComponent.infectedNPC--;
        isImmune = true;
    }
    public void Recover()
    {
        NPCManager.Instance.UnhighlightNPC(gameObject);
        infectionStatus = InfectionStatus.Normal;
        isImmune = true;
        StartCoroutine(SetImmune());
    }
    private IEnumerator SetImmune()
    {
        yield return new WaitForSeconds(5);
        isImmune = false;
    }
    private IEnumerator IncubationPeriod(InfectionStatus infection)
    {
        infectionStatus = infection;
        isWaiting = true;
        yield return YieldInstructionCache.WaitForSeconds(5);
        isWaiting = false;
        OnInfectionStateChanged?.Invoke(infection); // 이벤트 호출
    }

    public int GetTotalProtectionRate()
    {
        int totalProtectionRate = 0;
        foreach (var item in Inventory.Values)
        {
            if (item.isEquipped)
            {
                totalProtectionRate += item.protectionRate;
            }
        }
        return totalProtectionRate;
    }

    // 아이템 방어율 업데이트
    public void UpdateInfectionResistance()
    {
        infectionResistance = vaccineResist + GetTotalProtectionRate(); // 아이템 방어율 합산
    }
}
