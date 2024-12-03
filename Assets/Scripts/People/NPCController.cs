using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public enum StandingState
{
    Standing,
    Sitting,
    LayingDown
}

public class NPCController : MonoBehaviour
{
    public bool isInCurrentWard = false; // 현재 병동에 있는지 확인하는 변수
    public string currentWard;

    public SkinnedMeshRenderer meshRenderer;
    public Animator animator;
    public StandingState standingState = StandingState.Standing;
    public NavMeshAgent agent;
    public List<Waypoint> waypoints = new List<Waypoint>();
    public Ward wardComponent;
    public int ward;

    public bool isWaiting = false;

    public Person personComponent;
    public InfectionController infectionController;

    public int num;

    public ProtectedGearController protectedGear;

    public bool hasChangedLayer = false;

    private void Awake()
    {
        // 컴포넌트 초기화
        animator = GetComponent<Animator>();
        infectionController = GetComponent<InfectionController>();
        personComponent = GetComponent<Person>();
        agent = GetComponent<NavMeshAgent>();
        agent.avoidancePriority = Random.Range(0, 100);
        agent.speed = Random.Range(3.0f, 5.0f);
        agent.stoppingDistance = 0.5f;
        
        if(gameObject.CompareTag("Doctor") || gameObject.CompareTag("Nurse"))
        {
            protectedGear = gameObject.transform.Find("ProtectedGear").GetComponent<ProtectedGearController>();
            protectedGear.agent = agent;
            protectedGear.animator = protectedGear.GetComponent<Animator>();
            protectedGear.parentObject = gameObject;
            protectedGear.meshRenderer = protectedGear.GetComponent<SkinnedMeshRenderer>();
            protectedGear.meshRenderer.enabled = false;
        }
        meshRenderer = GetComponent<SkinnedMeshRenderer>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Floor"))
        {
            gameObject.layer = other.gameObject.layer;
            // 자식 오브젝트도 레이어 변경
            foreach (Transform child in gameObject.GetComponentsInChildren<Transform>(true))
            {
                child.gameObject.layer = other.gameObject.layer;
            }
        }
        if (other.GetComponent<WardRender>() != null && !isInCurrentWard)
        {
            string layerName = LayerMask.LayerToName(gameObject.layer);
            switch (layerName)
            {
                case "Floor 1 L":
                    currentWard = Ward.wards[8].WardName;
                    UpdateWardCount(Ward.wards[8], 1);
                    break;
                case "Floor 1 R":
                    currentWard = Ward.wards[9].WardName;
                    UpdateWardCount(Ward.wards[9], 1);
                    break;
                case "Floor 2 L":
                    currentWard = Ward.wards[0].WardName;
                    UpdateWardCount(Ward.wards[0], 1);
                    break;
                case "Floor 2 R":
                    currentWard = Ward.wards[1].WardName;
                    UpdateWardCount(Ward.wards[1], 1);
                    break;
                case "Floor 3 L":
                    currentWard = Ward.wards[2].WardName;
                    UpdateWardCount(Ward.wards[2], 1);
                    break;
                case "Floor 3 R":
                    currentWard = Ward.wards[3].WardName;
                    UpdateWardCount(Ward.wards[3], 1);
                    break;
                case "Floor 4 L":
                    currentWard = Ward.wards[4].WardName;
                    UpdateWardCount(Ward.wards[4], 1);
                    break;
                case "Floor 4 R":
                    currentWard = Ward.wards[5].WardName;
                    UpdateWardCount(Ward.wards[5], 1);
                    break;
                case "Floor 5 L":
                    currentWard = Ward.wards[6].WardName;
                    UpdateWardCount(Ward.wards[6], 1);
                    break;
                case "Floor 5 R":
                    currentWard = Ward.wards[7].WardName;
                    UpdateWardCount(Ward.wards[7], 1);
                    break;
            }
            isInCurrentWard = true; // 병동에 진입하면 상태를 true로 설정
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<WardRender>() != null && isInCurrentWard)
        {
            string layerName = LayerMask.LayerToName(other.gameObject.layer);
            switch (layerName)
            {
                case "Floor 1 L":
                    UpdateWardCount(Ward.wards[8], -1);
                    break;
                case "Floor 1 R":
                    UpdateWardCount(Ward.wards[9], -1);
                    break;
                case "Floor 2 L":
                    UpdateWardCount(Ward.wards[0], -1);
                    break;
                case "Floor 2 R":
                    UpdateWardCount(Ward.wards[1], -1);
                    break;
                case "Floor 3 L":
                    UpdateWardCount(Ward.wards[2], -1);
                    break;
                case "Floor 3 R":
                    UpdateWardCount(Ward.wards[3], -1);
                    break;
                case "Floor 4 L":
                    UpdateWardCount(Ward.wards[4], -1);
                    break;
                case "Floor 4 R":
                    UpdateWardCount(Ward.wards[5], -1);
                    break;
                case "Floor 5 L":
                    UpdateWardCount(Ward.wards[6], -1);
                    break;
                case "Floor 5 R":
                    UpdateWardCount(Ward.wards[7], -1);
                    break;
            }
            // 인원 수 감소 후 태그를 변경하여 역할에 맞는 상태를 유지
            if (personComponent.role == Role.Outpatient)
            {
                gameObject.tag = "Outpatient";
            }
            else if (personComponent.role == Role.Inpatient)
            {
                gameObject.tag = "Inpatient";
            }
            else if (personComponent.role == Role.EmergencyPatient)
            {
                gameObject.tag = "EmergencyPatient";
            }
            else if (personComponent.role == Role.ICUPatient)
            {
                gameObject.tag = "ICUPatient";
            }
            else if (personComponent.role == Role.Doctor)
            {
                gameObject.tag = "Doctor";
            }
            else if (personComponent.role == Role.Nurse)
            {
                gameObject.tag = "Nurse";
            }
            isInCurrentWard = false; // 병동에서 나가면 상태를 false로 설정
            currentWard = null;
        }
    }

    private void UpdateWardCount(Ward ward, int countChange)
    {
        // Tag에 따라 각 역할별 카운트 업데이트
        switch (gameObject.tag)
        {
            case "Doctor":
                ward.doctorCount += countChange;
                break;
            case "Nurse":
                ward.nurseCount += countChange;
                break;
            case "Outpatient":
                ward.outpatientCount += countChange;
                break;
            case "Inpatient":
                ward.inpatientCount += countChange;
                break;
            case "EmergencyPatient":
                ward.emergencypatientCount += countChange;
                break;
            case "ICUPatient":
                ward.icupatientCount += countChange;
                break;
        }
    }
}