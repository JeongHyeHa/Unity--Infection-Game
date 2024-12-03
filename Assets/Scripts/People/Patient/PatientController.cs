using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;

public class PatientController : NPCController
{
    // 웨이포인트 관련 변수
    public int waypointIndex = 0;

    // 상태 플래그
    public bool isQuarantined = false;
    public bool isFollowingNurse = false;
    public bool isWaitingForDoctor = false;
    public bool isWaitingForNurse = false;
    public bool isExiting = false;

    public bool officeSignal = false;
    public bool nurseSignal = false;
    public bool doctorSignal = false;

    public GameObject nurse;
    public MonthlyReportUI monthlyReportUI;     //MonthlyReportUI 스크립트

    private IconManager iconManager;       // 진오 추가

    public BedWaypoint bedWaypoint;
    public int prevWaypointIndex = -1;

    public Transform waypointsTransform;

    public bool excutedHC = false;  //입원 시간 재는 코루틴 실행 여부
    public bool excutedQC = false;  //격리 시간 재는 코루틴 실행 여부

    public ProfileWindow profileWindow;

    public Coroutine prevCoroutine;
    public Coroutine moveCoroutine;
    public Coroutine hospitalizationCoroutine;
    public void Activate()
    {
        do
        {
            ward = Random.Range(0, 4);
            waypointsTransform = Managers.NPCManager.waypointDictionary[(ward, "OutpatientWaypoints")];
            wardComponent = waypointsTransform.gameObject.GetComponentInParent<Ward>();
        }
        while (wardComponent.status != Ward.WardStatus.Normal);
        // 첫 번째 웨이포인트 추가
        AddWaypoint(waypointsTransform, $"CounterWaypoint (0)");

    }

    // 진오 추가
    private void Start()
    {
        profileWindow = FindObjectOfType<ProfileWindow>();
        iconManager = GetComponent<IconManager>();
        personComponent = GetComponent<Person>();
        //입원 환자 웨이포인트 추가
        if (personComponent.role == Role.Inpatient)
        {
            AddInpatientWaypoints();
        }
        if (personComponent.role == Role.EmergencyPatient)
        {
            moveCoroutine = StartCoroutine(EmergencyPatientMove());
        }
        if (personComponent.role == Role.ICUPatient)
        {
            moveCoroutine = StartCoroutine(ICUPateintMove());
        }
    }

    private void FixedUpdate()
    {
        Managers.NPCManager.UpdateAnimation(agent, animator);
        
        if (standingState == StandingState.Standing)
        {
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            Managers.NPCManager.PlayWakeUpAnimation(this);
            agent.radius = 0.175f;
        }
        else
        {
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        }

        if (isExiting)
        {
            standingState = StandingState.Standing;
            return;
        }
        if (isQuarantined)
        {
            if (standingState != StandingState.Standing)
            {
                if(ward == 9)
                {
                    transform.eulerAngles = new Vector3(0, -90, 0);
                }
                else
                {
                    agent.radius = 0.000001f;
                    if (bedWaypoint.bedGameObject.transform.parent.eulerAngles == new Vector3(0, 0, 0))
                    {
                        transform.eulerAngles = new Vector3(0, -180, 0);
                    }
                    else
                    {
                        transform.eulerAngles = Vector3.zero;
                    }
                }
            }
            else
            {
                moveCoroutine = StartCoroutine(QuarantineMove());
            }
        }
        if (isWaiting)
        {
            return;
        }
        if (isQuarantined && excutedQC)
        {
            excutedQC = false;
            StartCoroutine(QuarantineTimeCounter());
        }
        if (personComponent.role == Role.Inpatient)
        {
            if (!excutedHC && Managers.PatientCreator.startSignal)
            {
                excutedHC = true;
                hospitalizationCoroutine = StartCoroutine(HospitalizationTimeCounter());
            }
            if (isWaitingForNurse || isQuarantined)
            {
                return;
            }
            if (standingState != StandingState.Standing)
            {
                agent.radius = 0.000001f;
                if (bedWaypoint.bedGameObject.transform.parent.eulerAngles == new Vector3(0, 0, 0))
                {
                    transform.eulerAngles = new Vector3(0, -180, 0);
                }
                else
                {
                    transform.eulerAngles = Vector3.zero;
                }
            }
            if (!Managers.PatientCreator.startSignal)
            {
                return;
            }
            if (isWaitingForNurse || isQuarantined)
            {
                return;
            }
            // 목적지에 도착했는지 확인
            if (Managers.NPCManager.isArrived(agent))
                moveCoroutine = StartCoroutine(InpatientMove());
        }

        if (personComponent.role == Role.Outpatient)
        {
            // 애니메이션 업데이트
            //Managers.NPCManager.UpdateAnimation(agent, animator);

            if (isWaitingForNurse || isQuarantined)
            {
                return;
            }
            // 목적지에 도착했는지 확인
            if (Managers.NPCManager.isArrived(agent))
            {
                if (waypointIndex == 4 && !isWaitingForNurse && !isFollowingNurse && !isQuarantined)
                {
                    // 모든 웨이포인트를 방문했으면 비활성화
                    Managers.ObjectPooling.DeactivatePatient(gameObject);
                    Managers.PatientCreator.numberOfOutpatient--;
                    return;
                }
                else
                {
                    // 다음 웨이포인트로 이동
                    moveCoroutine = StartCoroutine(OutpatientMove());
                }
            }
        }
        if (personComponent.role == Role.EmergencyPatient)
        {
            if(moveCoroutine == null)
            {
                moveCoroutine = StartCoroutine(EmergencyPatientMove());
            }
            if (standingState != StandingState.Standing)
            {
                agent.radius = 0;
                if (bedWaypoint.bedGameObject.transform.parent.eulerAngles == new Vector3(0, 270, 0))
                {
                    transform.eulerAngles = new Vector3(0, 180, 0);
                }
                else
                {
                    transform.eulerAngles = Vector3.zero;
                }
                //Managers.NPCManager.PlayLayDownAnimation(this);
            }
        }
        if (personComponent.role == Role.ICUPatient)
        {
            if (standingState != StandingState.Standing)
            {
                agent.radius = 0;
                transform.eulerAngles = new Vector3(bedWaypoint.bedGameObject.transform.parent.eulerAngles.x, bedWaypoint.bedGameObject.transform.parent.eulerAngles.y + 90, bedWaypoint.bedGameObject.transform.parent.eulerAngles.z);
            }
        }
    }

    public IEnumerator QuarantineMove()
    {
        isWaiting = true;
        yield return YieldInstructionCache.WaitForSeconds(2.0f);
        if (isQuarantined)
        {
            if (standingState == StandingState.Standing)
            {
                agent.SetDestination(bedWaypoint.GetBedPoint());
                yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));
                if (standingState == StandingState.LayingDown)
                {
                    if (bedWaypoint.bedNum >= 8)
                    {
                        transform.eulerAngles = new Vector3(0, 90, 0);
                    }
                    else
                    {
                        transform.eulerAngles = new Vector3(0, -90, 0);

                    }
                }
                standingState = StandingState.LayingDown;
                Managers.NPCManager.PlaySittingAnimation(this);
                yield return YieldInstructionCache.WaitForSeconds(0.5f);
                Managers.NPCManager.PlayLayDownAnimation(this);
                yield return YieldInstructionCache.WaitForSeconds(2.0f);
            }
            yield break;
        }
        isWaiting = false;
    }

    // 다음 웨이포인트로 이동하는 코루틴
    public IEnumerator OutpatientMove()
    {
        isWaiting = true;
        yield return YieldInstructionCache.WaitForSeconds(1.5f);
        if (isExiting)
        {
            yield break;
        }
        if (waypoints.Count > 0 && waypointIndex > 0 && waypoints[waypointIndex - 1] is DoctorOffice docOffice)
        {
            Managers.NPCManager.PlaySittingAnimation(this);
            DoctorController targetDoctor = docOffice.doctor.GetComponent<DoctorController>();
            targetDoctor.patient = gameObject;
            targetDoctor.outpatientSignal = true;
            yield return YieldInstructionCache.WaitForSeconds(1.0f);
            yield return new WaitUntil(() => doctorSignal);
            Managers.NPCManager.FaceEachOther(docOffice.doctor, gameObject);
            yield return YieldInstructionCache.WaitForSeconds(1.5f);
            Managers.NPCManager.PlayWakeUpAnimation(this);
            yield return YieldInstructionCache.WaitForSeconds(1.0f);
        }

        // 현재 웨이포인트 인덱스에 따라 다음 웨이포인트 추가
        AddOutpatientWaypoint();

        if (waypointIndex < waypoints.Count)
        {

            // 의사 사무실인 경우 대기
            if (waypoints[waypointIndex] is DoctorOffice doctorOffice)
            {
                StartCoroutine(WaitForDoctorOffice(doctorOffice));
            }
            else if (waypointIndex > 0 && waypoints[waypointIndex - 1] is DoctorOffice doc)
            {
                DoctorController doctorController = doc.doctor.GetComponent<DoctorController>();
                doc.isEmpty = true;

                // 진료비 수입 증가 임시 위치
                // 진료비 : 10000
                MoneyManager.Instance.IncreaseMoney(MoneyManager.MedicalFee);
                monthlyReportUI = FindObjectOfType<MonthlyReportUI>();
                monthlyReportUI.AddIncomeDetail("진료비", MoneyManager.MedicalFee);

                doctorController.outpatientSignal = false;
            }

            // 다음 웨이포인트로 이동
            if (!isWaitingForDoctor)
            {
                // 현재 웨이포인트가 DoctorOffice일 경우
                if (waypointIndex > 0 && waypoints[waypointIndex - 1] is DoctorOffice)
                {
                    BedWaypoint nextBed = Ward.wards
                        .Where(ward => ward.status == Ward.WardStatus.Normal && ward.num >= 4 && ward.num <= 7)
                        .SelectMany(ward => ward.beds)
                        .FirstOrDefault(bed => bed.patient == null);

                    if (nextBed != null)
                    {
                        nextBed.isEmpty = false;
                        nextBed.patient = gameObject;
                    }

                    // 랜덤으로 입원 혹은 퇴원
                    int random = Random.Range(0, 101);
                    //입원 환자로 전환
                    if (random >= 30 && nextBed != null)
                    {
                        //Debug.Log("외래 환자 -> 입원 환자");
                        bedWaypoint = nextBed;
                        bedWaypoint.patient = gameObject;
                        standingState = StandingState.Standing;
                        waypoints.Clear();

                        officeSignal = false;
                        doctorSignal = false;
                        wardComponent.outpatients.Remove(this);

                        profileWindow.RemoveProfile(personComponent.ID);
                        Managers.PatientCreator.numberOfOutpatient--;
                        ward = bedWaypoint.ward;
                        wardComponent = Managers.NPCManager.waypointDictionary[(ward, "InpatientWaypoints")].GetComponentInParent<Ward>();
                        waypointsTransform = Managers.NPCManager.waypointDictionary[(ward, "InpatientWaypoints")];
                        profileWindow.AddInpatientProfile(gameObject);
                        wardComponent.inpatients.Add(this);
                        Managers.PatientCreator.numberOfInpatient++;
                        doctorSignal = false;
                        AddInpatientWaypoints();
                        personComponent.role = Role.Inpatient;

                        Waypoint[] passPoints = Managers.NPCManager.passPointTransform.GetComponentsInChildren<Waypoint>();
                        agent.SetDestination(passPoints[1].GetSampledPosition());
                        yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));

                        if (6 <= ward && ward <= 7)
                        {
                            agent.SetDestination(passPoints[0].GetSampledPosition());
                            yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));
                        }

                        agent.SetDestination(bedWaypoint.GetRandomPointInRange());
                        isWaiting = true;
                        yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));
                        //gameObject.tag = "Inpatient";
                        waypointIndex = -1;
                        isWaiting = false;
                        yield break;
                    }
                    else
                    {
                        iconManager.IsIcon();
                    }
                }
                if (waypoints.Count > 0)
                {
                    if (waypoints[waypointIndex] is DoctorOffice DO)
                    {
                        agent.SetDestination(waypoints[waypointIndex++].GetMiddlePointInRange());
                    }
                    else
                    {
                        agent.SetDestination(waypoints[waypointIndex++].GetRandomPointInRange());
                    }
                }
            }

        }
        isWaiting = false;
    }

    public IEnumerator InpatientMove()
    {
        isWaiting = true;

        if (waypointIndex == 1)
        {
            waypoints[1].isEmpty = true;
        }

        if (isQuarantined)
        {
            yield return YieldInstructionCache.WaitForSeconds(2.0f);
            agent.SetDestination(bedWaypoint.GetBedPoint());
            isWaiting = false;
            yield break;
        }
        float random = Random.Range(0, 101);
        if (random <= 70)
        {
            if (waypointIndex == 0)
            {
                yield return YieldInstructionCache.WaitForSeconds(Random.Range(4.5f, 5.5f));
                isWaiting = false;
                yield break;
            }
            agent.stoppingDistance = 0f;
            agent.SetDestination(bedWaypoint.GetBedPoint());
            yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));
            if (bedWaypoint != null && bedWaypoint.bedGameObject.transform.parent.eulerAngles == new Vector3(0, 0, 0))
            {
                transform.eulerAngles = new Vector3(0, -180, 0);
            }
            else
            {
                transform.eulerAngles = Vector3.zero;
            }
            standingState = StandingState.LayingDown;
            Managers.NPCManager.PlaySittingAnimation(this);
            yield return YieldInstructionCache.WaitForSeconds(0.5f);
            Managers.NPCManager.PlayLayDownAnimation(this);
            yield return YieldInstructionCache.WaitForSeconds(2.0f);
            if (isExiting)
            {
                yield break;
            }
            waypoints[0].isEmpty = false;
            waypointIndex = 0;
            yield return YieldInstructionCache.WaitForSeconds(2.0f);
        }
        else if (random <= 80 && waypoints[1].isEmpty)
        {
            if (waypointIndex == 0)
            {
                waypoints[0].isEmpty = true;
                Managers.NPCManager.PlayWakeUpAnimation(this);
                yield return YieldInstructionCache.WaitForSeconds(5.0f);
                standingState = StandingState.Standing;
            }
            if (isExiting)
            {
                yield break;
            }
            waypoints[1].isEmpty = false;
            agent.stoppingDistance = 0.5f;
            agent.SetDestination(waypoints[1].GetRandomPointInRange());
            waypointIndex = 1;
            //yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));

        }
        else if (random <= 90)
        {
            if (waypointIndex == 0)
            {
                waypoints[0].isEmpty = true;
                Managers.NPCManager.PlayWakeUpAnimation(this);
                yield return YieldInstructionCache.WaitForSeconds(5.0f);
                standingState = StandingState.Standing;
            }
            if (isExiting)
            {
                yield break;
            }
            agent.stoppingDistance = 0.5f;
            agent.SetDestination(waypoints[2].GetRandomPointInRange());

            //yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));
            waypointIndex = 2;
        }
        else
        {
            if (waypointIndex == 0)
            {
                waypoints[0].isEmpty = true;
                Managers.NPCManager.PlayWakeUpAnimation(this);
                yield return YieldInstructionCache.WaitForSeconds(5.0f);
                standingState = StandingState.Standing;
            }
            if (isExiting)
            {
                yield break;
            }
            agent.stoppingDistance = 0.5f;
            agent.SetDestination(waypoints[Random.Range(3, 5)].GetRandomPointInRange());

            //yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));

            waypointIndex = 3;
        }
        yield return YieldInstructionCache.WaitForSeconds(Random.Range(6f, 10f));
        isWaiting = false;
    }

    public IEnumerator EmergencyPatientMove()
    {
        agent.stoppingDistance = 0f;
        agent.SetDestination(bedWaypoint.GetBedPoint());
        yield return YieldInstructionCache.WaitForSeconds(2.0f);
        yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));
        DoctorController.ERWaitingList.Add(this);
        bedWaypoint.isEmpty = false;
        standingState = StandingState.LayingDown;
        Managers.NPCManager.PlaySittingAnimation(this);
        yield return YieldInstructionCache.WaitForSeconds(0.5f);

        Managers.NPCManager.PlayLayDownAnimation(this);
        yield return YieldInstructionCache.WaitForSeconds(5.0f);

        yield return new WaitUntil(() => doctorSignal);

        BedWaypoint nextBed = Ward.wards
             .Where(ward => ward.status == Ward.WardStatus.Normal && ward.num >= 4 && ward.num <= 7)
             .SelectMany(ward => ward.beds)
             .FirstOrDefault(bed => bed.patient == null);
        if (nextBed != null)
        {
            nextBed.patient = gameObject;
        }


        // 랜덤으로 입원 혹은 퇴원
        int random = Random.Range(0, 101);
        // 퇴원
        if (random <= 30 || nextBed == null)
        {
            AddWaypoint(waypointsTransform, $"Counter");
            AddWaypoint(Managers.NPCManager.gatewayTransform, $"Gateway ({Random.Range(0, 2)})");
            int stayDuration = Random.Range(5, 10);
            yield return YieldInstructionCache.WaitForSeconds(stayDuration);

            Managers.NPCManager.PlayWakeUpAnimation(this);
            yield return YieldInstructionCache.WaitForSeconds(5.0f);
            standingState = StandingState.Standing;

            agent.SetDestination(waypoints[1].GetRandomPointInRange());

            yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));
            yield return YieldInstructionCache.WaitForSeconds(2.0f);
            
            if (bedWaypoint.patient == gameObject)
            {
                bedWaypoint.patient = null;
                bedWaypoint.isEmpty = true;
            }

            agent.SetDestination(waypoints[2].GetRandomPointInRange());

            yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));
            Managers.ObjectPooling.DeactivatePatient(gameObject);
            Managers.PatientCreator.numberOfEmergencyPatient--;
        }
        //입원 환자로 전환
        else
        {
            isWaiting = true;
            wardComponent.emergencyPatients.Remove(this);
            profileWindow.RemoveProfile(personComponent.ID);
            waypoints.Clear();

            bedWaypoint = nextBed;
            bedWaypoint.patient = gameObject;
            ward = bedWaypoint.ward;
            wardComponent = Managers.NPCManager.waypointDictionary[(ward, "InpatientWaypoints")].GetComponentInParent<Ward>();
            waypointsTransform = Managers.NPCManager.waypointDictionary[(ward, "InpatientWaypoints")];

            wardComponent.inpatients.Add(this);
            profileWindow.AddInpatientProfile(gameObject);
            Managers.PatientCreator.numberOfInpatient++;

            doctorSignal = false;
            AddInpatientWaypoints();

            Managers.NPCManager.PlayWakeUpAnimation(this);
            standingState = StandingState.Standing;
            yield return YieldInstructionCache.WaitForSeconds(5.0f);

            Waypoint[] passPoints = Managers.NPCManager.passPointTransform.GetComponentsInChildren<Waypoint>();
            agent.SetDestination(passPoints[2].GetSampledPosition());
            yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));

            Managers.PatientCreator.numberOfEmergencyPatient--;

            agent.SetDestination(passPoints[1].GetSampledPosition());
            yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));

            if (6 <= ward && ward <= 7)
            {
                agent.SetDestination(passPoints[0].GetSampledPosition());
                yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));
            }

            agent.SetDestination(bedWaypoint.GetRandomPointInRange());
            yield return YieldInstructionCache.WaitForSeconds(2.0f);

            yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));
            personComponent.role = Role.Inpatient;
            gameObject.tag = "Inpatient";
            yield return YieldInstructionCache.WaitForSeconds(2.0f);
            isWaiting = false;
        }
    }
    public IEnumerator ICUPateintMove()
    {
        agent.stoppingDistance = 0f;
        agent.SetDestination(bedWaypoint.GetBedPoint());
        yield return YieldInstructionCache.WaitForSeconds(2.0f);
        yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));
        bedWaypoint.isEmpty = false;
        standingState = StandingState.LayingDown;
        Managers.NPCManager.PlaySittingAnimation(this);
        yield return YieldInstructionCache.WaitForSeconds(0.5f);

        Managers.NPCManager.PlayLayDownAnimation(this);
        yield return YieldInstructionCache.WaitForSeconds(5.0f);
    }

    // 다음 웨이포인트 추가 메서드
    private void AddOutpatientWaypoint()
    {
        switch (waypointIndex)
        {
            case 0:
                AddWaypoint(waypointsTransform, $"CounterWaypoint (0)");
                break;
            case 1:
                AddWaypoint(waypointsTransform, $"SofaWaypoint (0)");
                break;
            case 2:
                if (waypoints.Count < 3)
                {
                    AddWaypoint(waypointsTransform, $"Doctor'sOffice (0)");
                }
                break;
            case 3:
                AddWaypoint(Managers.NPCManager.gatewayTransform, $"Gateway ({Random.Range(0, 2)})");
                break;
        }
    }

    // 의사 사무실 대기 코루틴
    private IEnumerator WaitForDoctorOffice(DoctorOffice doctorOffice)
    {
        isWaitingForDoctor = true;
        while (!officeSignal)
        {
            yield return YieldInstructionCache.WaitForSeconds(1.0f);
        }
        doctorOffice.isEmpty = false;
        isWaitingForDoctor = false;
    }

    //간호사가 올 때까지 대기 코루틴
    public IEnumerator WaitForNurse()
    {
        isWaitingForNurse = true;
        agent.isStopped = true;
        yield return new WaitUntil(() => nurseSignal);
        agent.ResetPath();
        nurseSignal = false;
        agent.isStopped = false;
        isWaitingForNurse = false;
    }

    // 웨이포인트 추가 메서드
    private void AddWaypoint(Transform parentTransform, string childName)
    {
        Transform waypointTransform = parentTransform.Find(childName);
        if (waypointTransform != null)
        {
            Waypoint comp = waypointTransform.gameObject.GetComponent<Waypoint>();
            if (comp is DoctorOffice)
            {
                // 의사 사무실 선택 로직
                SelectDoctorOffice(parentTransform);
            }
            else
            {
                if (!waypoints.Contains(comp))
                {
                    waypoints.Add(comp);
                }
            }
        }
        else
        {
            Debug.LogWarning($"Can't find waypoint: {childName}");
        }
    }

    // 의사 사무실 선택 메서드
    private void SelectDoctorOffice(Transform parentTransform)
    {
        // 가능한 의사 사무실 목록 생성
        Dictionary<DoctorOffice, int> countDic = new Dictionary<DoctorOffice, int>();
        for (int i = 0; i < 10; i++)
        {
            DoctorOffice doctorOffice = parentTransform.Find("Doctor'sOffice (" + i + ")").gameObject.GetComponent<DoctorOffice>();
            GameObject searchedDoctor = doctorOffice.doctor;
            if (doctorOffice.doctor == null)
            {
                continue;
            }
            DoctorController doctorController = searchedDoctor.GetComponent<DoctorController>();
            if (doctorController == null || doctorController.isResting)
            {
                continue;
            }
            int patientCount = doctorController.patientCount + doctorOffice.waitingQueue.Count;
            if (!countDic.ContainsKey(doctorOffice))
            {
                countDic.Add(doctorOffice, patientCount);
            }
        }

        // 최적의 의사 사무실 선택
        if (countDic.Count > 0)
        {
            DoctorOffice searchedOffice = countDic
                .OrderBy(kvp => kvp.Value)
                .FirstOrDefault().Key;
            searchedOffice.waitingQueue.Enqueue(this);
            waypoints.Add(searchedOffice);
            countDic.Clear();
        }
        else
        {
            Debug.LogError("의사를 찾을 수 없습니다.");
        }
    }
    public IEnumerator FollowNurse(GameObject nurse)
    {
        this.nurse = nurse;
        isFollowingNurse = true;
        while (isFollowingNurse)
        {
            standingState = StandingState.Standing;
            agent.SetDestination(nurse.transform.position);
            yield return YieldInstructionCache.WaitForSeconds(0.1f);
        }
        agent.ResetPath();
    }

    public IEnumerator ExitHospital()
    {
        isExiting = true;
        isWaiting = true;
        waypoints.Clear();
        StopCoroutine(moveCoroutine);
        agent.ResetPath();

        wardComponent.RemoveFromPatientList(this);
        profileWindow.RemoveProfile(personComponent.ID);
        yield return new WaitUntil(() => Time.timeScale > 0);
        //Debug.Log($"{gameObject.name} 퇴원 시작");
        Managers.NPCManager.PlayWakeUpAnimation(this);
        yield return YieldInstructionCache.WaitForSeconds(5.0f);
        Managers.NPCManager.PlayWakeUpAnimation(this);

        // 5층에 있는 NPC
        if (gameObject.layer >= 20 && gameObject.layer <= 22)
        {
            agent.SetDestination(Managers.NPCManager.passPointTransform.GetChild(0).GetComponent<Waypoint>().GetSampledPosition());
            yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));
        }

        // 4층에 있는 NPC
        else if (gameObject.layer == 13 || gameObject.layer == 14 || gameObject.layer == 18)
        {
            agent.SetDestination(Managers.NPCManager.passPointTransform.GetChild(1).GetComponent<Waypoint>().GetSampledPosition());
            yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));
        }

        // 3층에 있는 NPC
        else if (gameObject.layer == 11 || gameObject.layer == 12 || gameObject.layer == 17)
        {

            agent.SetDestination(Managers.NPCManager.passPointTransform.GetChild(2).GetComponent<Waypoint>().GetSampledPosition());
            yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));
        }

        if (bedWaypoint != null)
        {
            
            if(bedWaypoint.patient == gameObject)
            {
                bedWaypoint.isEmpty = true;
                bedWaypoint.patient = null;
            }
        }
        bedWaypoint = null;
        agent.SetDestination(Managers.NPCManager.gatewayTransform.Find("Gateway (" + Random.Range(0, 2) + ")").GetComponent<Waypoint>().GetSampledPosition());
        yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));

        Managers.ObjectPooling.DeactivatePatient(gameObject);
    }

    public void AddInpatientWaypoints()
    {
        waypoints.Add(bedWaypoint);
        waypoints[0].isEmpty = true;
        waypoints.Add(bedWaypoint.toilet);
        waypoints.Add(waypointsTransform.Find("SofaWaypoint").gameObject.GetComponent<Waypoint>());
        waypoints.Add(waypointsTransform.Find("CounterWaypoint (0)").gameObject.GetComponent<Waypoint>());
        waypoints.Add(waypointsTransform.Find("CounterWaypoint (1)").gameObject.GetComponent<Waypoint>());
    }
    public IEnumerator HospitalizationTimeCounter()
    {
        float randomTime = Random.Range(20f, 100f);
        int dividedTime = Mathf.FloorToInt(randomTime / 10f);

        yield return YieldInstructionCache.WaitForSeconds(randomTime);

        isWaiting = true;

        MoneyManager.Instance.IncreaseMoney(MoneyManager.HospitalizationFee * dividedTime);
        //Debug.Log($"입원 환자 {gameObject.name} 수입: {MoneyManager.HospitalizationFee * dividedTime} 며칠 : {dividedTime})");
        monthlyReportUI = FindObjectOfType<MonthlyReportUI>();
        monthlyReportUI.AddIncomeDetail("입원비", MoneyManager.HospitalizationFee * dividedTime);

        StopCoroutine(moveCoroutine);
        StartCoroutine(ExitHospital());
    }
    public IEnumerator QuarantineTimeCounter()
    {
        yield return YieldInstructionCache.WaitForSeconds(50f);
        StopCoroutine(moveCoroutine);
        isQuarantined = false;
        Managers.NPCManager.PlayWakeUpAnimation(this);
        yield return YieldInstructionCache.WaitForSeconds(5.0f);
        AutoDoorWaypoint[] autoDoors = bedWaypoint.transform.GetComponentsInChildren<AutoDoorWaypoint>();
        if(autoDoors.Length > 0)
        {
            // 자동문 나가기
            Animator autoDoorsAnimator = autoDoors[0].quarantineRoom.GetComponent<Animator>();

            agent.SetDestination(autoDoors[2].GetMiddlePointInRange());
            yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));

            autoDoorsAnimator.SetBool("IsInternalDoorOpened", true);
            yield return YieldInstructionCache.WaitForSeconds(1.0f);

            agent.SetDestination(autoDoors[1].GetMiddlePointInRange());
            yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));

            autoDoorsAnimator.SetBool("IsInternalDoorOpened", false);
            yield return YieldInstructionCache.WaitForSeconds(1.0f);

            autoDoorsAnimator.SetBool("IsExternalDoorOpened", true);
            yield return YieldInstructionCache.WaitForSeconds(1.0f);

            agent.SetDestination(autoDoors[0].GetMiddlePointInRange());
            yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));

            autoDoorsAnimator.SetBool("IsExternalDoorOpened", false);
        }
        if(bedWaypoint.patient == gameObject)
        {
            bedWaypoint.patient = null;
            bedWaypoint.isEmpty = true;
        }
        bedWaypoint = null;
        StartCoroutine(ExitHospital());
    }

    public IEnumerator TransferToAvailableWard(BedWaypoint nextBed)
    {
        isWaiting = true;
        StopCoroutine(moveCoroutine);
        bedWaypoint = nextBed;
        wardComponent.inpatients.Remove(this);
        ward = bedWaypoint.ward;
        wardComponent = Managers.NPCManager.waypointDictionary[(ward, "InpatientWaypoints")].GetComponentInParent<Ward>();
        waypointsTransform = Managers.NPCManager.waypointDictionary[(ward, "InpatientWaypoints")];
        wardComponent.inpatients.Add(this);
        if (standingState != StandingState.Standing)
        {
            Managers.NPCManager.PlayWakeUpAnimation(this);
            yield return YieldInstructionCache.WaitForSeconds(5.0f);
        }
        standingState = StandingState.Standing;

        prevWaypointIndex = -1;
        doctorSignal = false;
        nurseSignal = false;
        waypoints.Clear();
        AddInpatientWaypoints();

        agent.isStopped = false;
        agent.SetDestination(bedWaypoint.GetMiddlePointInRange());
        isWaiting = false;
        //yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));

    }
}