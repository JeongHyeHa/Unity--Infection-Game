using System.Collections;
using System.Collections.Generic;
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
    public bool isQuarantineNurse = false;
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
        Managers.NPCManager.UpdateAnimation(agent, animator);

        if (isWorking)
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

    // 대기 후 랜덤 웨이포인트로 이동 코루틴
    public IEnumerator WardNurseMove()
    {
        isWaiting = true; // 기다리는 중으로 설정
        yield return YieldInstructionCache.WaitForSeconds(1.5f);
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
                                yield return YieldInstructionCache.WaitForSeconds(3);
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
        yield return YieldInstructionCache.WaitForSeconds(2.0f);
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
                agent.SetDestination(bed.gameObject.transform.position);
                yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));
                //Managers.NPCManager.FaceEachOther(bed.patient, gameObject);
                if (bed.patient == null)
                {
                    isWaiting = false;
                    yield break;
                }
                transform.LookAt(bed.transform);
                if (Random.Range(0, 100) <= 50)
                {
                    //if (targetInpatientController.standingState == StandingState.LayingDown)
                    //{
                    //    Managers.NPCManager.WakeUpAndSittingAndTalking(targetInpatientController);
                    //    yield return YieldInstructionCache.WaitForSeconds(4.0f);
                    //    Managers.NPCManager.PlayLayDownAnimation(targetInpatientController);
                    //}

                }
                else
                {
                    yield return YieldInstructionCache.WaitForSeconds(2.0f);
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
        yield return YieldInstructionCache.WaitForSeconds(2.0f);
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
                yield return YieldInstructionCache.WaitForSeconds(2.0f);

            }
            else if (6 <= num && num <= 8) //병원 입구 쪽 카운터 간호사들
            {
                agent.SetDestination(new Vector3(chair.transform.position.x, chair.transform.position.y, chair.transform.position.z - 0.5f));
                yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));
                transform.eulerAngles = new Vector3(0, 180, 0);
                Managers.NPCManager.PlaySittingAnimation(this);
                yield return YieldInstructionCache.WaitForSeconds(2.0f);
            }
            else //환자 보러다니는 간호사들
            {
                while (doctor.isWorking)
                {
                    isWorking = true;
                    agent.speed = doctor.agent.speed;
                    agent.SetDestination(doctor.transform.position - doctor.transform.forward * 0.5f);
                    yield return YieldInstructionCache.WaitForSeconds(0.1f);
                }
                int random = Random.Range(0, waypoints.Count);
                if (!waypoints[random].isEmpty && waypoints[random] is BedWaypoint bed)
                {
                    agent.SetDestination(bed.GetMiddlePointInRange());
                    yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));
                    transform.LookAt(bed.gameObject.transform);
                    isWaiting = false;
                    yield break;
                }
                else
                {
                    agent.SetDestination(waypoints[0].GetRandomPointInRange());
                }
                if (doctor.isWorking)
                {
                    isWaiting = false;
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
        yield return YieldInstructionCache.WaitForSeconds(2.0f);
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
                yield return YieldInstructionCache.WaitForSeconds(2.0f);
                isWaiting = false;
            }
            else //환자 보러다니는 간호사들
            {
                isWaitingAtDoctorOffice = true;
                while (doctor.isWorking)
                {
                    agent.SetDestination(doctor.transform.position - doctor.transform.forward * 0.5f);
                    yield return YieldInstructionCache.WaitForSeconds(0.1f);
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

    // 격리 시 간호사 움직임
    public IEnumerator QuarantineMove(PatientController patient)
    {
        isWorking = true; // 일하는 중으로 설정
        patient.isWaiting = true;
        if (DoctorController.ERWaitingList.Contains(patient))
        {
            DoctorController.ERWaitingList.Remove(patient);
        }
        if (patient.personComponent.role == Role.EmergencyPatient)
        {
            DoctorController.ERWaitingList.Remove(patient);
        }

        //4종 보호구 입기
        //personComponent.Inventory["Level C"].isEquipped = true;
        meshRenderer.enabled = false;
        protectedGear.meshRenderer.enabled = true;

        Managers.NPCManager.PlayWakeUpAnimation(this);
        yield return YieldInstructionCache.WaitForSeconds(1.0f);
        agent.avoidancePriority = patient.agent.avoidancePriority++ - 1;

        //Vector3 targetPatientPosition = Managers.NPCManager.GetPositionInFront(transform, patient.transform, 0.5f); // 환자 앞의 임의 위치 계산
        agent.SetDestination(patient.transform.position);
        //targetPatient = patient; // 타겟 환자 설정
        yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));

        if (patient.standingState == StandingState.LayingDown)
        {
            patient.standingState = StandingState.Standing;
            Managers.NPCManager.PlayWakeUpAnimation(patient);
            yield return YieldInstructionCache.WaitForSeconds(5.0f);
        }

        Managers.NPCManager.FaceEachOther(gameObject, patient.gameObject); // 간호사와 환자가 서로를 바라보게 설정
        patient.agent.ResetPath();
        yield return YieldInstructionCache.WaitForSeconds(2.0f);

        patient.StopCoroutine(patient.moveCoroutine);
        patient.waypoints.Clear();
        patient.wardComponent.RemoveFromPatientList(patient);
        patient.gameObject.tag = "QuarantinedPatient";

        patient.nurseSignal = true; // 환자에게 간호사가 도착했음을 알림
        //targetPatientController.nurse = gameObject; // 간호사 설정
        agent.speed = patient.agent.speed - 1.0f;
        patient.agent.stoppingDistance = 1.0f;
        patient.StartCoroutine(patient.FollowNurse(gameObject));

        if (patient.prevBed != null)
        {
            patient.prevBed.isEmpty = true;
            if (patient.prevBed.patient == patient.gameObject)
            {
                patient.prevBed.patient = null;
            }
        }
        AutoDoorWaypoint[] inFrontOfAutoDoors = patient.bedWaypoint.transform.GetComponentsInChildren<AutoDoorWaypoint>();

        if (patient.hospitalizationCoroutine != null)
        {
            patient.StopCoroutine(patient.hospitalizationCoroutine);
        }

        // 격리 프로세스 시작
        if (inFrontOfAutoDoors.Length > 0)
        {
            yield return MoveToQuarantineRoom(inFrontOfAutoDoors, patient);
        }
        else
        {
            yield return MoveToQuarantinedWard(patient);
        }

        // 환자 격리 상태 설정
        InitializeQuarantineState(patient);
        transform.LookAt(patient.transform.position);

        // 격리 후 이동 처리
        if (inFrontOfAutoDoors.Length > 0)
        {
            yield return CompleteQuarantineProcess(inFrontOfAutoDoors);
        }
        else
        {
            yield return ExitQuarantinedWard();
        }
        if (wardComponent.status == Ward.WardStatus.Quarantined)
        {
            meshRenderer.enabled = false;
            protectedGear.meshRenderer.enabled = true;
        }
        // 복귀 처리
        StartCoroutine(FinalizeReturn(agent));
    }

    private IEnumerator MoveToQuarantineRoom(AutoDoorWaypoint[] autoDoors, PatientController patient)
    {
        agent.SetDestination(autoDoors[0].GetMiddlePointInRange()); // 자동문 앞으로 이동

        while (!Managers.NPCManager.isArrived(agent))
        {
            yield return YieldInstructionCache.WaitForSeconds(0.5f);
            float distance = Vector3.Distance(gameObject.transform.position, patient.transform.position);
            if (distance > 3.0f)
            {
                agent.isStopped = true;
            }
            else
            {
                agent.isStopped = false;
            }
        }
        agent.isStopped = false;
        Animator autoDoorAnimator = autoDoors[0].quarantineRoom.GetComponent<Animator>();
        autoDoorAnimator.SetBool("IsExternalDoorOpened", true);
        yield return YieldInstructionCache.WaitForSeconds(1.0f);

        agent.SetDestination(autoDoors[1].GetMiddlePointInRange());
        yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));
        yield return YieldInstructionCache.WaitForSeconds(1.0f);
        autoDoorAnimator.SetBool("IsExternalDoorOpened", false);
        yield return YieldInstructionCache.WaitForSeconds(1.0f);
        autoDoorAnimator.SetBool("IsInternalDoorOpened", true);
        yield return YieldInstructionCache.WaitForSeconds(1.0f);

        if (patient.bedWaypoint == null)
        {
            Debug.Log("격리실 null");
            autoDoorAnimator.SetBool("IsInternalDoorOpened", false);
            yield break;
        }
        agent.SetDestination(patient.bedWaypoint.GetRandomPointInRange());
        yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));
        autoDoorAnimator.SetBool("IsInternalDoorOpened", false);
    }
    private IEnumerator MoveToQuarantinedWard(PatientController patient)
    {
        agent.SetDestination(patient.bedWaypoint.GetRandomPointInRange());
        while (!Managers.NPCManager.isArrived(agent))
        {
            yield return YieldInstructionCache.WaitForSeconds(0.5f);
            float distance = Vector3.Distance(gameObject.transform.position, patient.transform.position);
            if (distance > 3.0f)
            {
                agent.isStopped = true;
            }
            else
            {
                agent.isStopped = false;
            }
        }
        agent.isStopped = false;
    }
    private void InitializeQuarantineState(PatientController patient)
    {
        patient.StopAllCoroutines();
        patient.StartCoroutine(patient.QuarantineTimeCounter());
        Managers.NPCManager.FaceEachOther(gameObject, patient.gameObject);
        // 환자의 역할에 따라 환자 수 차감
        if (patient.personComponent.role == Role.Outpatient)
        {
            Managers.PatientCreator.numberOfOutpatient--;
        }
        else if (patient.personComponent.role == Role.Inpatient)
        {
            Managers.PatientCreator.numberOfInpatient--;
        }
        else if (patient.personComponent.role == Role.EmergencyPatient)
        {
            Managers.PatientCreator.numberOfEmergencyPatient--;
        }
        else if (patient.personComponent.role == Role.ICUPatient)
        {
            Managers.PatientCreator.numberOfICUPatient--;
        }
        patient.personComponent.role = Role.QuarantinedPatient;
        patient.agent.stoppingDistance = 0f;
        patient.isFollowingNurse = false;
        patient.isQuarantined = true;
        patient.isWaiting = false;
        patient.ward = patient.bedWaypoint.ward;
        patient.wardComponent = patient.bedWaypoint.wardComponent;
        patient.wardComponent.quarantinedPatients.Add(patient);
    }
    private IEnumerator CompleteQuarantineProcess(AutoDoorWaypoint[] autoDoors)
    {
        Animator autoDoorAnimator = autoDoors[0].quarantineRoom.GetComponent<Animator>();
        yield return YieldInstructionCache.WaitForSeconds(2.0f);
        agent.speed += 1.0f;

        // 자동문 나가기
        agent.SetDestination(autoDoors[2].GetMiddlePointInRange());
        yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));

        autoDoorAnimator.SetBool("IsInternalDoorOpened", true);
        yield return YieldInstructionCache.WaitForSeconds(1.0f);

        agent.SetDestination(autoDoors[1].GetMiddlePointInRange());
        yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));
        
        autoDoorAnimator.SetBool("IsInternalDoorOpened", false);
        yield return YieldInstructionCache.WaitForSeconds(1.0f);

        //4종 보호구 벗기
        MoneyManager.Instance.DeductQuarantineNurseExpense();
        meshRenderer.enabled = true;
        protectedGear.meshRenderer.enabled = false;

        autoDoorAnimator.SetBool("IsExternalDoorOpened", true);
        yield return YieldInstructionCache.WaitForSeconds(1.0f);

        agent.SetDestination(autoDoors[0].GetMiddlePointInRange());
        yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));
        autoDoorAnimator.SetBool("IsExternalDoorOpened", false);
    }

    private IEnumerator ExitQuarantinedWard()
    {
        agent.SetDestination(waypoints[0].GetRandomPointInRange());
        yield return YieldInstructionCache.WaitForSeconds(10.0f);
        //4종 보호구 벗기
        MoneyManager.Instance.DeductQuarantineNurseExpense();
        meshRenderer.enabled = true;
        protectedGear.meshRenderer.enabled = false;
    }

    private IEnumerator FinalizeReturn(NavMeshAgent agent)
    {
        agent.SetDestination(waypoints[0].GetSampledPosition());
        agent.stoppingDistance = 0f;
        if (wardComponent.status == Ward.WardStatus.Closed)
        {
            meshRenderer.enabled = false;
            protectedGear.meshRenderer.enabled = false;
        }
        yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));

        agent.avoidancePriority = 50;
        isWorking = false;
        if (wardComponent.status == Ward.WardStatus.Closed)
        {
            Managers.ObjectPooling.DeactivateNurse(gameObject);
        }
    }

}
