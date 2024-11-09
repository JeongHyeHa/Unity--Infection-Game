using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ward : MonoBehaviour
{
    public int num;
    public string WardName;
    public static List<Ward> wards = new List<Ward>();
    public List<DoctorController> doctors = new List<DoctorController>();
    public List<PatientController> inpatients = new List<PatientController>();
    public List<NurseController> nurses = new List<NurseController>();
    public List<PatientController> outpatients = new List<PatientController>();
    public List<PatientController> emergencyPatients = new List<PatientController>();
    public List<PatientController> icuPatients = new List<PatientController>();
    public List<BedWaypoint> beds = new List<BedWaypoint>();
    public float totalOfNPC = 0;
    public float infectedNPC = 0;
    public bool isClosed = false;

    public bool isWaiting = false;

    // Start is called before the first frame update
    void Awake()
    {
        wards.Add(this);
        num = gameObject.name[gameObject.name.Length - 2] - '0';
        Waypoint[] waypoints = transform.GetComponentsInChildren<Waypoint>();
        foreach (Waypoint waypoint in waypoints)
        {
            if (waypoint != null)
            {
                waypoint.ward = num;
            }
        }
        switch (num)
        {
            case 0:
                WardName = "내과 1";
                break;
            case 1:
                WardName = "내과 2";
                break;
            case 2:
                WardName = "외과 1";
                break;
            case 3:
                WardName = "외과 2";
                break;
            case 4:
                WardName = "입원병동1";
                break;
            case 5:
                WardName = "입원병동2";
                break;
            case 6:
                WardName = "입원병동3";
                break;
            case 7:
                WardName = "입원병동4";
                break;
            case 8:
                WardName = "응급실";
                break;
            case 9:
                WardName = "중환자실";
                break;
        }
        wards.Sort((w1, w2) => w1.num.CompareTo(w2.num));
        Transform waypointsTransform;
        if (num == 8)
        {
            waypointsTransform = transform.Find("EmergencyPatientWaypoints");
        }
        else if (num == 9)
        {
            waypointsTransform = transform.Find("DoctorWaypoints");
        }
        else
        {
            waypointsTransform = transform.Find("InpatientWaypoints");
        }
        if (waypointsTransform != null)
        {
            beds = waypointsTransform.GetComponentsInChildren<BedWaypoint>().ToList();
        }
        StartCoroutine(WaitOneSecond());
    }

    // Update is called once per frame
    void Update()
    {
        totalOfNPC = doctors.Count + inpatients.Count + nurses.Count + outpatients.Count;
        if (isWaiting)
        {
            return;
        }

        //병동 폐쇄
        //if ((infectedNPC / totalOfNPC >= 0.5) || doctors.Count == 0)
        //{
        //    isClosed = true;
        //}
    }

    public void CloseWard()
    {
        isClosed = true;
        foreach (PatientController patient in outpatients)
        {
            if (patient != null)
            {
                int index = wards.IndexOf(this);
                if (patient.waypointIndex == 3)
                {
                    patient.StopAllCoroutines();
                    patient.agent.isStopped = false;
                }
                else if (patient.waypointIndex == 4)
                {
                    continue;
                }
                if ((index == 0 || index == 2) && !wards[index + 1].isClosed)
                {
                    patient.ward = index + 1;
                    patient.waypointsTransform = Managers.NPCManager.waypointDictionary[(index + 1, "InpatientWaypoints")];
                    patient.wardComponent = wards[index + 1];
                    patient.wardComponent.outpatients.Add(patient);
                    patient.waypoints.Clear();
                    patient.isWaitingForDoctor = false;
                    patient.waypointIndex = 0;
                }
                else if ((index == 1 || index == 3) && !wards[index - 1].isClosed)
                {
                    patient.ward = index - 1;
                    patient.waypointsTransform = Managers.NPCManager.waypointDictionary[(index - 1, "InpatientWaypoints")];
                    patient.wardComponent = wards[index - 1];
                    patient.wardComponent.outpatients.Add(patient);
                    patient.waypoints.Clear();
                    patient.isWaitingForDoctor = false;
                    patient.waypointIndex = 0;
                }
                else
                {
                    patient.StartCoroutine(patient.ExitHospital());
                }

            }
        }
        outpatients.Clear();

        for (int i = inpatients.Count - 1; i >= 0; i--)
        {
            PatientController inpatient = inpatients[i];
            if (inpatient != null)
            {
                bool isFound = false;
                BedWaypoint nextBed = null;
                for (int j = 4; j <= 7; j++)
                {
                    if (j != num && !wards[j].isClosed) // num을 제외하고 닫히지 않은 병동을 찾음
                    {
                        foreach (BedWaypoint bed in wards[j].beds)
                        {
                            if (bed.patient == null)
                            {
                                nextBed = bed;
                                nextBed.patient = inpatient.gameObject;
                                isFound = true;
                                break;
                            }
                        }
                    }
                    if (isFound)
                    {
                        break;
                    }
                }
                if (isFound)
                {
                    StartCoroutine(TransferToAvailableWard(inpatient, nextBed));
                }
                else
                {
                    inpatients[i].StopAllCoroutines();
                    inpatients[i].agent.isStopped = false;
                    inpatients[i].StartCoroutine(inpatients[i].ExitHospital());
                }
            }
        }

        for (int i = nurses.Count - 1; i >= 0; i--)
        {
            if (nurses[i] != null)
            {
                Managers.ObjectPooling.DeactivateNurse(nurses[i].gameObject);
            }
        }
        nurses.Clear();

        for (int i = doctors.Count - 1; i >= 0; i--)
        {
            if (doctors[i] != null)
            {
                if (doctors[i].waypoints[0] is DoctorOffice doctorOffice)
                {
                    doctorOffice.waitingQueue.Clear();
                }
                Managers.ObjectPooling.DeactivateDoctor(doctors[i].gameObject);
            }
        }
        doctors.Clear();
    }
    public void OpenWard()
    {
        isClosed = false;

        if (0 <= num && num <= 3)
        {
            for (int i = num * 16; i < (num * 16) + 16; i++)
            {
                GameObject nurse = GameObject.Find($"WardNurse {i}");
                Managers.ObjectPooling.ActivateNurse(nurse);
            }
            for (int i = num * 6; i < (num * 6) + 6; i++)
            {
                GameObject doctor = GameObject.Find($"WardDoctor {i}");
                Managers.ObjectPooling.ActivateDoctor(doctor);
            }
        }
        else if (4 <= num && num <= 7)
        {
            for (int i = num * 12; i < (num * 12) + 12; i++)
            {
                GameObject nurse = GameObject.Find($"InpatientWardNurse {i}");
                Managers.ObjectPooling.ActivateNurse(nurse);
            }
        }
        else if (num == 8)
        {
            for (int i = 0; i < 10; i++)
            {
                GameObject nurse = GameObject.Find($"ERNurse {i}");
                Managers.ObjectPooling.ActivateNurse(nurse);
            }
            GameObject doctor = GameObject.Find($"ERDoctor 0");
            Managers.ObjectPooling.ActivateDoctor(doctor);
        }
    }

    IEnumerator TransferToAvailableWard(PatientController inpatient, BedWaypoint nextBed)
    {
        Managers.NPCManager.PlayWakeUpAnimation(inpatient);
        yield return new WaitForSeconds(5.0f);
        inpatient.bedWaypoint = nextBed;
        //inpatient.agent.SetDestination(inpatient.bedWaypoint.GetBedPoint());
        inpatient.standingState = StandingState.Standing;

        inpatient.ward = inpatient.bedWaypoint.ward;
        inpatient.wardComponent = Managers.NPCManager.waypointDictionary[(inpatient.ward, "InpatientWaypoints")].GetComponentInParent<Ward>();
        inpatient.waypointsTransform = Managers.NPCManager.waypointDictionary[(inpatient.ward, "InpatientWaypoints")];
        inpatient.doctorSignal = false;
        inpatient.nurseSignal = false;
        inpatient.waypoints.Clear();
        inpatient.AddInpatientWaypoints();

        inpatient.wardComponent.inpatients.Add(inpatient);

        inpatient.StopAllCoroutines();
        inpatient.agent.isStopped = false;
        inpatient.agent.SetDestination(inpatient.bedWaypoint.GetMiddlePointInRange());
        yield return new WaitUntil(() => Managers.NPCManager.isArrived(inpatient.agent));
        inpatient.isWaiting = false;
        //yield return new WaitForSeconds(2.0f);
    }

    private IEnumerator WaitOneSecond()
    {
        isWaiting = true;
        yield return new WaitForSeconds(1.0f);
        isWaiting = false;
    }

    // 의사, 간호사, 외래환자의 수를 반환하는 메서드 추가
    public (int doctorCount, int nurseCount, int outpatientCount) GetCounts()
    {
        int doctorCount = doctors.Count;
        int nurseCount = nurses.Count;
        int outpatientCount = outpatients.Count;

        return (doctorCount, nurseCount, outpatientCount);
    }
}
