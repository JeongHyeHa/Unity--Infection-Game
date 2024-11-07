using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
public enum NurseRole
{
    ER,
    ICU,
    Ward,
    InpateintWard
}
public class NurseController : NPCController
{
    public NurseRole role;
    public bool isWorking = false; // 간호사가 일하는 중인지 여부
    public bool isRest = false;
    public bool isWaitingAtDoctorOffice = false;
    public bool isReturning = false;
    public DoctorController doctor;

    //public GameObject targetPatient; // 타겟 환자
    public GameObject chair;

    // Start는 첫 프레임 업데이트 전에 호출됩니다.
    void Start()
    {
        
    }

    // Update는 매 프레임 호출됩니다.
    void FixedUpdate()
    {

        // 애니메이션 업데이트
        Managers.NPCManager.UpdateAnimation(agent, animator);
        if(isWorking)
        {
            Managers.NPCManager.PlayWakeUpAnimation(this);
            return;
        }
        if (isWaiting || isRest)
        {
            return;
        }
        if (Managers.NPCManager.isArrived(agent))
        {
            if (role == NurseRole.Ward)
            {
                StartCoroutine(WardNurseMove());
            }
            else if (role == NurseRole.ER)
            {
                StartCoroutine(ERNurseMove());
            }
            else if (role == NurseRole.InpateintWard)
            {
                StartCoroutine(InpatientWardNurseMove());
            }
            else if (role == NurseRole.ICU)
            {
                StartCoroutine(ICUNurseMove());
            }
        }


    }

    // 환자에게 이동
    public IEnumerator GoToPatient(GameObject patientGameObject)
    {
        PatientController targetPatientController = patientGameObject.GetComponent<PatientController>();
        isWorking = true; // 일하는 중으로 설정
        Managers.NPCManager.PlayWakeUpAnimation(this);
        yield return new WaitForSeconds(1.0f);
        Vector3 targetPatientPosition = Managers.NPCManager.GetPositionInFront(transform, patientGameObject.transform, 0.5f); // 환자 앞의 임의 위치 계산
        agent.SetDestination(targetPatientPosition); // 에이전트 목적지 설정
        //targetPatient = patientGameObject; // 타겟 환자 설정

        yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));

        if (targetPatientController.animator.GetBool("Sleeping"))
        {
            Managers.NPCManager.PlayWakeUpAnimation(targetPatientController);
            yield return new WaitForSeconds(5.0f);
        }

        Managers.NPCManager.FaceEachOther(gameObject, patientGameObject); // 간호사와 환자가 서로를 바라보게 설정

        targetPatientController.nurseSignal = true; // 환자에게 간호사가 도착했음을 알림
                                                    //targetPatientController.nurse = gameObject; // 간호사 설정

    }

    // 음압실로 이동
    public IEnumerator GoToQuarantineRoom(GameObject patientGameObject)
    {
        PatientController targetPatientController = patientGameObject.GetComponent<PatientController>();
        isWorking = true; // 일하는 중으로 설정
        Managers.NPCManager.PlayWakeUpAnimation(this);
        yield return new WaitForSeconds(1.0f);
        agent.avoidancePriority = targetPatientController.agent.avoidancePriority++ - 1;

        //Vector3 targetPatientPosition = Managers.NPCManager.GetPositionInFront(transform, patientGameObject.transform, 0.5f); // 환자 앞의 임의 위치 계산
        agent.SetDestination(patientGameObject.transform.position);
        //targetPatient = patientGameObject; // 타겟 환자 설정
        yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));


        if (targetPatientController.standingState == StandingState.LayingDown)
        {
            targetPatientController.standingState = StandingState.Standing;
            Managers.NPCManager.PlayWakeUpAnimation(targetPatientController);
            yield return new WaitForSeconds(5.0f);
        }

        Managers.NPCManager.FaceEachOther(gameObject, patientGameObject); // 간호사와 환자가 서로를 바라보게 설정
        if (targetPatientController.isWaiting)
        {
            targetPatientController.StopCoroutine(targetPatientController.OutpatientMove());
            targetPatientController.StopCoroutine(targetPatientController.InpatientMove());
            targetPatientController.StopCoroutine(targetPatientController.EmergencyPatientMove());
        }


        targetPatientController.nurseSignal = true; // 환자에게 간호사가 도착했음을 알림
        //targetPatientController.nurse = gameObject; // 간호사 설정
        agent.speed = targetPatientController.agent.speed - 1.0f;
        targetPatientController.StartCoroutine(targetPatientController.FollowNurse(gameObject));
        AutoDoorWaypoint[] inFrontOfAutoDoor = targetPatientController.quarantineRoom.transform.GetComponentsInChildren<AutoDoorWaypoint>();
        agent.SetDestination(inFrontOfAutoDoor[0].GetMiddlePointInRange());  //격리실 자동문 앞으로 이동

        targetPatientController.StopCoroutine(targetPatientController.HospitalizationTimeCounter());

        while (!Managers.NPCManager.isArrived(agent))
        {
            yield return new WaitForSeconds(0.5f);
            float distance = Vector3.Distance(gameObject.transform.position, patientGameObject.transform.position);
            if (distance > 3.0f)
            {
                agent.isStopped = true;
            }
            else
            {
                agent.isStopped = false;
            }
        }

        inFrontOfAutoDoor[0].quarantineRoom.GetComponent<Animator>().SetBool("IsOpened", true);
        yield return new WaitForSeconds(2.0f);
        if(targetPatientController.quarantineRoom == null)
        {
            Debug.Log("격리실 null");
        }
        agent.SetDestination(targetPatientController.quarantineRoom.GetRandomPointInRange()); // 음압실로 이동
        yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));
        inFrontOfAutoDoor[0].quarantineRoom.GetComponent<Animator>().SetBool("IsOpened", false);
        targetPatientController.StopAllCoroutines();
        targetPatientController.StartCoroutine(targetPatientController.QuarantineTimeCounter());
        Managers.NPCManager.FaceEachOther(gameObject, targetPatientController.gameObject);
        yield return new WaitForSeconds(2.0f);

        targetPatientController.isFollowingNurse = false;
        targetPatientController.isQuarantined = true;
        targetPatientController.isWaiting = false;
        targetPatientController.ward = 9;
        targetPatientController.wardComponent = Managers.NPCManager.waypointDictionary[(9, "NurseWaypoints")].GetComponentInParent<Ward>();

        agent.speed += 1.0f;
        agent.SetDestination(inFrontOfAutoDoor[1].GetMiddlePointInRange());
        yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));
        inFrontOfAutoDoor[0].quarantineRoom.GetComponent<Animator>().SetBool("IsOpened", true);
        yield return new WaitForSeconds(1.0f);

        agent.SetDestination(inFrontOfAutoDoor[0].GetMiddlePointInRange());
        yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));
        inFrontOfAutoDoor[0].quarantineRoom.GetComponent<Animator>().SetBool("IsOpened", false);
        agent.SetDestination(waypoints[0].GetMiddlePointInRange());

        isReturning = true;
        yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));
        agent.avoidancePriority = 50;
        isReturning = false;
        isWorking = false;
    }


    // 대기 후 랜덤 웨이포인트로 이동 코루틴
    public IEnumerator WardNurseMove()
    {
        isWaiting = true; // 기다리는 중으로 설정
        yield return new WaitForSeconds(1.5f);
        if (isWorking)
        {
            isWaiting = false;
            yield break;
        }
        if (waypoints.Count > 0)
        {
            int roleNum = num % 16;
            if (8 <= roleNum && roleNum <= 13)  //진료실 앞 대기 간호사들
            {
                for (int i = 4; i <= 13; i++)
                {
                    if (waypoints[i] is NurseWaitingPoint nurseWaitingPoint && nurseWaitingPoint.doctorOffice.doctor != null)
                    {
                        DoctorController doctorController = nurseWaitingPoint.doctorOffice.doctor.GetComponent<DoctorController>();
                        if (nurseWaitingPoint.isEmpty && !doctorController.isResting && isWaitingAtDoctorOffice == false)
                        {
                            doctorController.nurse = gameObject;
                            nurseWaitingPoint.isEmpty = false;
                            isWaitingAtDoctorOffice = true;
                            agent.SetDestination(waypoints[i].GetMiddlePointInRange());
                            yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));
                            transform.eulerAngles = nurseWaitingPoint.doctorOffice.doctor.transform.eulerAngles;
                        }
                        if (doctorController.nurse != null)
                        {
                            NurseController nurseController = doctorController.nurse.GetComponent<NurseController>();
                            if (!doctorController.waypoints[1].isEmpty)
                            {
                                yield return new WaitForSeconds(3);
                                nurseController.agent.SetDestination(doctorController.nurse.GetComponent<NurseController>().waypoints[i + 10].GetMiddlePointInRange());
                                yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));
                                nurseController.gameObject.transform.LookAt(doctorController.gameObject.transform);
                            }
                            else
                            {
                                nurseController.agent.SetDestination(doctorController.nurse.GetComponent<NurseController>().waypoints[i].GetMiddlePointInRange());
                                yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));

                                nurseController.gameObject.transform.eulerAngles = nurseWaitingPoint.doctorOffice.doctor.GetComponent<DoctorController>().chair.transform.eulerAngles;

                            }
                        }

                    }
                }
            }
            else if (0 <= roleNum && roleNum <= 7)  //카운터에 있는 간호사들
            {
                if (animator.GetBool("Sitting"))
                {
                    isWaiting = false;
                    yield break;
                }
                if (chair.transform.parent.parent.parent.parent.eulerAngles == new Vector3(0, 0, 0))
                {
                    agent.SetDestination(new Vector3(chair.transform.position.x, chair.transform.position.y, chair.transform.position.z - 0.5f));
                }
                else
                {
                    agent.SetDestination(new Vector3(chair.transform.position.x, chair.transform.position.y, chair.transform.position.z + 0.5f));
                }
                yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));
                if (chair.transform.parent.parent.parent.parent.eulerAngles == new Vector3(0, 0, 0))
                {
                    transform.eulerAngles = new Vector3(0, 180, 0);
                }
                else
                {
                    transform.eulerAngles = new Vector3(0, 0, 0);
                }
                Managers.NPCManager.PlaySittingAnimation(this);
            }

            else
            {
                agent.SetDestination(waypoints[Random.Range(24, 27)].GetRandomPointInRange()); // 랜덤 웨이포인트로 이동
            }
        }
        isWaiting = false; // 기다리는 중 해제
    }

    public IEnumerator InpatientWardNurseMove()
    {
        isWaiting = true;
        yield return new WaitForSeconds(2.0f);
        int roleNum = num % 12;
        if (0 <= roleNum && roleNum <= 7)
        {
            if (animator.GetBool("Sitting"))
            {
                if (chair.transform.parent.parent.parent.eulerAngles == new Vector3(0, 0, 0))
                {
                    transform.eulerAngles = new Vector3(0, 180, 0);
                }
                else
                {
                    transform.eulerAngles = new Vector3(0, 0, 0);
                }
                isWaiting = false;
                yield break;
            }
            if (chair.transform.parent.parent.parent.eulerAngles == new Vector3(0, 0, 0))
            {
                agent.SetDestination(new Vector3(chair.transform.position.x, chair.transform.position.y, chair.transform.position.z - 0.5f));
            }
            else
            {
                agent.SetDestination(new Vector3(chair.transform.position.x, chair.transform.position.y, chair.transform.position.z + 0.5f));
            }
            yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));
            if (chair.transform.parent.parent.parent.eulerAngles == new Vector3(0, 0, 0))
            {
                transform.eulerAngles = new Vector3(0, 180, 0);
            }
            else
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
            }
            Managers.NPCManager.PlaySittingAnimation(this);
        }
        else if (8 <= roleNum && roleNum <= 11)
        {
            int random = Random.Range(4, waypoints.Count);
            if (!waypoints[random].isEmpty && waypoints[random] is BedWaypoint bed && bed.patient != null)
            {
                PatientController targetInpatientController = bed.patient.GetComponent<PatientController>();
                //targetInpatientController.StartCoroutine(targetInpatientController.WaitForNurse());
                agent.SetDestination(bed.patient.transform.position);
                yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));
                //Managers.NPCManager.FaceEachOther(bed.patient, gameObject);
                if (bed.patient == null)
                {
                    isWaiting = false;
                    yield break;
                }
                transform.LookAt(bed.patient.transform);
                if (Random.Range(0, 100) <= 50)
                {
                    //if (targetInpatientController.standingState == StandingState.LayingDown)
                    //{
                    //    Managers.NPCManager.WakeUpAndSittingAndTalking(targetInpatientController);
                    //    yield return new WaitForSeconds(4.0f);
                    //    Managers.NPCManager.PlayLayDownAnimation(targetInpatientController);
                    //}

                }
                else
                {
                    yield return new WaitForSeconds(2.0f);
                }
                //targetInpatientController.nurseSignal = true;
            }
            else
            {
                agent.SetDestination(waypoints[Random.Range(4, 8)].GetRandomPointInRange());
            }
        }

        isWaiting = false;
    }


    public IEnumerator ERNurseMove()
    {
        isWaiting = true;
        yield return new WaitForSeconds(2.0f);
        if (isWorking)
        {
            isWaiting = false;
            yield break;
        }
        if (waypoints.Count > 0)
        {
            if (0 <= num && num <= 5) //중앙 카운터 간호사들
            {
                if (chair.transform.parent.parent.eulerAngles == new Vector3(0, 0, 0))
                {
                    agent.SetDestination(new Vector3(chair.transform.position.x, chair.transform.position.y, chair.transform.position.z - 0.5f));
                }
                else
                {
                    agent.SetDestination(new Vector3(chair.transform.position.x, chair.transform.position.y, chair.transform.position.z + 0.5f));
                }
                yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));

                if (chair.transform.parent.parent.eulerAngles == new Vector3(0, 0, 0))
                {
                    transform.eulerAngles = new Vector3(0, 180, 0);
                }
                else
                {
                    transform.eulerAngles = new Vector3(0, 0, 0);
                }
                Managers.NPCManager.PlaySittingAnimation(this);
                yield return new WaitForSeconds(2.0f);

            }
            else if (6 <= num && num <= 8) //병원 입구 쪽 카운터 간호사들
            {
                agent.SetDestination(new Vector3(chair.transform.position.x, chair.transform.position.y, chair.transform.position.z - 0.5f));
                yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));
                transform.eulerAngles = new Vector3(0, 180, 0);
                Managers.NPCManager.PlaySittingAnimation(this);
                yield return new WaitForSeconds(2.0f);
            }
            else //환자 보러다니는 간호사들
            {
                while (doctor.isWorking)
                {
                    agent.SetDestination(doctor.transform.position - doctor.transform.forward * 0.5f);
                    yield return new WaitForSeconds(0.1f);
                }
                int random = Random.Range(0, waypoints.Count);
                if (!waypoints[random].isEmpty && waypoints[random] is BedWaypoint bed)
                {
                    agent.SetDestination(waypoints[random].GetMiddlePointInRange());
                }
                else
                {
                    agent.SetDestination(waypoints[0].GetRandomPointInRange());
                }
                if (doctor.isWorking)
                {
                    yield break;
                }
                yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));
            }
        }
        isWaiting = false;
    }
    public IEnumerator ICUNurseMove()
    {
        isWaiting = true;
        yield return new WaitForSeconds(2.0f);
        if (isWorking)
        {
            isWaiting = false;
            yield break;
        }
        if (waypoints.Count > 0)
        {
            if (0 <= num && num <= 10) //중앙 카운터 간호사들
            {
                if (chair.transform.parent.parent.eulerAngles == new Vector3(0, 0, 0))
                {
                    agent.SetDestination(new Vector3(chair.transform.position.x, chair.transform.position.y, chair.transform.position.z - 0.5f));
                }
                else
                {
                    agent.SetDestination(new Vector3(chair.transform.position.x, chair.transform.position.y, chair.transform.position.z + 0.5f));
                }
                yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));

                if (chair.transform.parent.parent.eulerAngles == new Vector3(0, 0, 0))
                {
                    transform.eulerAngles = new Vector3(0, 180, 0);
                }
                else
                {
                    transform.eulerAngles = new Vector3(0, 0, 0);
                }
                Managers.NPCManager.PlaySittingAnimation(this);
                yield return new WaitForSeconds(2.0f);
                isWaiting = false;
            }
            else //환자 보러다니는 간호사들
            {
                while (doctor.isWorking)
                {
                    agent.SetDestination(doctor.transform.position - doctor.transform.forward * 0.5f);
                    yield return new WaitForSeconds(0.1f);
                }
                int random = Random.Range(0, waypoints.Count);
                if (!waypoints[random].isEmpty && waypoints[random] is BedWaypoint bed)
                {
                    agent.SetDestination(waypoints[random].GetMiddlePointInRange());
                }
                else
                {
                    agent.SetDestination(waypoints[0].GetRandomPointInRange());
                }
                if (doctor.isWorking)
                {
                    yield break;
                }
                yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));
            }
        }
        isWaiting = false;
    }
}
