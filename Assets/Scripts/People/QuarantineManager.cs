using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// QuarantineManager 클래스는 NPC(여기서는 간호사)를 검색하고, 격리하는 등의 작업을 수행합니다.
public class QuarantineManager : MonoBehaviour
{
    public static int quarantineStep = 0;
    // 박스캐스트의 거리 설정
    public float boxCastDistance = 100f;  // 박스캐스트 거리
    // 박스캐스트의 크기 설정
    public Vector3 boxCastSize = new Vector3(100f, 1f, 100f); // 박스캐스트 크기

    // 간호사를 검색하는 메서드, origin 위치에서 가장 가까운 간호사를 찾습니다.
    public GameObject SearchNurse(Vector3 origin)
    {
        Transform closestNurse = null;
        float closestDistance = Mathf.Infinity;
        // 'Nurse' 태그를 가진 모든 게임 오브젝트를 찾습니다.
        GameObject[] nurses = GameObject.FindGameObjectsWithTag("Nurse");

        foreach (GameObject nurse in nurses)
        {
            // 각 간호사의 NurseController 컴포넌트를 가져옵니다.
            NurseController nurseController = nurse.GetComponent<NurseController>();
            // 간호사가 근무 중이고 의사 사무실에서 대기 중이면 건너뜁니다.
            if (nurseController.isWorking || nurseController.isWaitingAtDoctorOffice || nurseController.isRest || !nurseController.isQuarantineNurse)
            {
                continue;
            }
            // 간호사가 같은 층에 있는지 확인합니다.
            if (Mathf.Abs(origin.y - nurse.transform.position.y) <= 1.0f)
            {
                // origin과 간호사 사이의 거리를 계산합니다.
                float distance = Vector3.Distance(origin, nurse.transform.position);
                // 가장 가까운 간호사를 업데이트합니다.
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestNurse = nurse.transform;
                }
            }
        }
        // 가장 가까운 간호사가 있으면 로그에 이름을 출력합니다.
        if (closestNurse != null)
        {
            Person person = closestNurse.GetComponent<Person>();
            if (person != null)
            {
                //Debug.Log("Closest Nurse found: " + person.gameObject.name);
            }
        }
        else
        {
            //Debug.Log("No Nurse found.");
            return null;
        }
        // 가장 가까운 간호사의 게임 오브젝트를 반환합니다.
        return closestNurse.gameObject;
    }

    public IEnumerator Quarantine()
    {
        if (Random.Range(0, 100) <= 30)
        {
            //Debug.Log("증상 발견!");
            if (gameObject.CompareTag("Outpatient") || gameObject.CompareTag("Inpatient") || gameObject.CompareTag("EmergencyPatient"))
            {
                PatientController patientController = gameObject.GetComponent<PatientController>();
                if (patientController.isFollowingNurse || patientController.isQuarantined || patientController.isWaitingForNurse || patientController.isExiting || patientController.isWaitingForDoctor || (patientController.personComponent.role == Role.Outpatient && patientController.waypointIndex == 3))
                {
                    yield break;
                }

                Transform parentTransform = Managers.NPCManager.waypointDictionary[(9, "NurseWaypoints")];

                // 빈 격리실 찾기
                BedWaypoint quarantineRoom = parentTransform
                    .GetComponentsInChildren<BedWaypoint>()
                    .FirstOrDefault(room => room.isEmpty && room.patient == null);

                if (quarantineRoom != null)
                {
                    AssignQuarantineRoom(patientController, quarantineRoom);
                }
                else
                {
                    PolicyWard.Instance.qtStartButton_3.gameObject.SetActive(true);
                    // 격리 병동에서 빈 병상 찾기
                    BedWaypoint nextBed = Ward.wards
                        .Where(ward => ward.status == Ward.WardStatus.Quarantined && ward.num >= 4 && ward.num <= 7)
                        .SelectMany(ward => ward.beds)
                        .FirstOrDefault(bed => bed.patient == null);

                    if (nextBed == null)
                    {
                        patientController.StartCoroutine(patientController.ExitHospital());
                    }
                    else
                    {
                        AssignQuarantineRoom(patientController, nextBed);
                    }
                }
            }
        }
        else
        {
            //Debug.Log("증상 미발견...");
        }
    }

    private void AssignQuarantineRoom(PatientController patientController, BedWaypoint quarantineRoom)
    {
        // 현재 입원 중인 병상이 있을 때
        if (patientController.bedWaypoint != null)
        {
            patientController.prevBed = patientController.bedWaypoint;
        }
        patientController.bedWaypoint = quarantineRoom;
        patientController.bedWaypoint.isEmpty = false;
        patientController.bedWaypoint.patient = patientController.gameObject;

        GameObject closestNurse = SearchNurse(patientController.transform.position);

        if (closestNurse == null || !IsValidForQuarantine(patientController, closestNurse))
        {
            CancelQuarantine(patientController);
            return;
        }
        
        AssignNurseToQuarantine(patientController, closestNurse);
    }

    private bool IsValidForQuarantine(PatientController patientController, GameObject nurse)
    {
        return nurse != null &&
               !patientController.isFollowingNurse &&
               !patientController.isQuarantined &&
               !patientController.isWaitingForNurse &&
               !patientController.isExiting &&
               !patientController.isWaitingForDoctor &&
               !(patientController.personComponent.role == Role.Outpatient && patientController.waypointIndex == 3) &&
               PolicyItem.Instance.isAllItemsEquipped;
    }
    private void CancelQuarantine(PatientController patientController)
    {
        if(patientController.bedWaypoint.patient == patientController.gameObject)
        {
            patientController.bedWaypoint.isEmpty = true;
            patientController.bedWaypoint.patient = null;
        }
        if (patientController.prevBed != null)
        {
            patientController.bedWaypoint = patientController.prevBed;
        }
    }

    private void AssignNurseToQuarantine(PatientController patientController, GameObject nurse)
    {
        patientController.StopAllCoroutines();

        NurseController nurseController = nurse.GetComponent<NurseController>();
        if (nurseController == null)
        {
            Debug.LogError("nurseController를 찾을 수 없습니다.");
            return;
        }

        patientController.nurseSignal = false;
        patientController.StartCoroutine(patientController.WaitForNurse());
        nurseController.StartCoroutine(nurseController.QuarantineMove(patientController));
    }
}
