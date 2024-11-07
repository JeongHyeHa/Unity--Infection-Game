using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum DoctorRole
{
    ER,
    Ward,
    InpatientWard,
    ICU
}
public class DoctorController : NPCController
{
    public DoctorRole role;
    public int patientCount = 0;
    public bool isResting = false;
    public bool changeSignal = false;
    public bool outpatientSignal = false;
    public GameObject chair;
    public GameObject patient;
    public GameObject nurse;
    public bool isWorking = false;

    public static List<PatientController> ERWaitingList = new List<PatientController>();
    //public static List<PatientController> ICUWaitingList = new List<PatientController>();

    private void Start()
    {
        if (waypoints[0] is DoctorOffice doctorOffice && role == DoctorRole.Ward)
        {
            chair = doctorOffice.chair;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isResting)
        {
            return;
        }
        // 애니메이션
        Managers.NPCManager.UpdateAnimation(agent, animator);



        //if (patientCount >= patientMaxCount && waypoints[1] is DoctorOffice doctorOffice)
        //{
        //    if (doctorOffice.waitingQueue.Count == 0 && doctorOffice.isEmpty)
        //    {
        //        StartCoroutine(Rest());
        //        DoctorCreator.Instance.ChangeDoctor(gameObject);
        //        return;
        //    }
        //}

        if (isWaiting)
        {
            return;
        }
        if (Managers.NPCManager.isArrived(agent))
        {
            if (role == DoctorRole.Ward)
            {
                StartCoroutine(WardDoctorMove());
            }
            else if (role == DoctorRole.ER)
            {
                StartCoroutine(ERDoctorMove());
            }
            else if(role==DoctorRole.ICU)
            {
                StartCoroutine(ICUDoctorMove());
            }
        }


    }
    private IEnumerator WardDoctorMove()
    {
        isWaiting = true;
        yield return new WaitForSeconds(1.5f);


        if (!agent.isOnNavMesh)
        {
            Debug.LogError("NavMeshAgent가 내비게이션 준비가 되지 않았습니다. 활성화 상태, 활성화 여부, NavMesh 위치 여부를 확인하세요.");
        }

        if (waypoints[1] is DoctorOffice doctorOffice)
        {
            if (!outpatientSignal)
            {
                if (animator.GetBool("Sitting"))
                {
                    transform.eulerAngles = chair.transform.rotation.eulerAngles;
                    isWaiting = false;
                    yield break;
                }
                if (chair.transform.rotation.eulerAngles == new Vector3(0, 0, 0))
                {
                    agent.SetDestination(new Vector3(chair.transform.position.x, chair.transform.position.y, chair.transform.position.z + 0.5f));
                }
                else
                {
                    agent.SetDestination(new Vector3(chair.transform.position.x, chair.transform.position.y, chair.transform.position.z - 0.5f));
                }
                yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));
                yield return new WaitForSeconds(1.0f);
                transform.eulerAngles = chair.transform.rotation.eulerAngles;
                Managers.NPCManager.PlaySittingAnimation(this);
            }
            else
            {
                Managers.NPCManager.PlayWakeUpAnimation(this);
                yield return new WaitForSeconds(1.0f);
                Vector3 outpatientLocation = Managers.NPCManager.GetPositionInFront(transform, patient.transform, 0.75f);
                agent.SetDestination(outpatientLocation);
                yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));
                patient.GetComponent<PatientController>().doctorSignal = true;
            }
        }
        isWaiting = false;
    }

    public IEnumerator ERDoctorMove()
    {
        isWaiting = true;
        if (ERWaitingList.Count > 0)
        {
            isWorking = true;
            int random = Random.Range(0, ERWaitingList.Count);
            patient = ERWaitingList[random].gameObject;
            agent.SetDestination(ERWaitingList[random].bedWaypoint.GetRandomPointInRange());
            yield return new WaitForSeconds(2.0f);
            yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));
            transform.LookAt(ERWaitingList[random].bedWaypoint.bedGameObject.transform);
            yield return new WaitForSeconds(2.0f);
            ERWaitingList[random].doctorSignal = true;
            ERWaitingList.RemoveAt(random);
            isWaiting = false;
            yield break;
        }
        else
        {
            isWorking = false;
            agent.SetDestination(waypoints[0].GetRandomPointInRange());
            yield return new WaitForSeconds(2.0f);
        }
        isWaiting = false;
    }

    public IEnumerator ICUDoctorMove()
    {
        isWaiting = true;
        yield return new WaitForSeconds(2.0f);
        int randomBed = Random.Range(2, waypoints.Count);
        if (waypoints[randomBed] is BedWaypoint bed)
        {
            if(bed.patient != null)
            {
                isWorking = true;
                agent.SetDestination(waypoints[randomBed].GetRandomPointInRange());
                yield return new WaitUntil(() => Managers.NPCManager.isArrived(agent));
                transform.LookAt(bed.patient.transform);
                yield return new WaitForSeconds(2.0f);
            }
            else
            {
                isWorking = false;
                agent.SetDestination(waypoints[Random.Range(0, 2)].GetRandomPointInRange());
            }
        }
        isWaiting = false;
    }
    public IEnumerator Rest()
    {
        isResting = true;
        if (!changeSignal)
        {
            yield return new WaitForSeconds(1);
        }
        isResting = false;
        changeSignal = false;
    }
}
