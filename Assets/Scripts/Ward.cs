using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ward : MonoBehaviour
{
    public enum WardStatus
    {
        Normal,
        Closed,
        Quarantined
    }
    public int num;
    public string WardName;
    public WardStatus status = WardStatus.Normal;
    public static List<Ward> wards = new List<Ward>();
    public List<DoctorController> doctors = new List<DoctorController>();
    public List<PatientController> inpatients = new List<PatientController>();
    public List<NurseController> nurses = new List<NurseController>();
    public List<PatientController> outpatients = new List<PatientController>();
    public List<PatientController> emergencyPatients = new List<PatientController>();
    public List<PatientController> icuPatients = new List<PatientController>();
    public List<PatientController> quarantinedPatients = new List<PatientController>();
    public List<BedWaypoint> beds = new List<BedWaypoint>();
    public float totalOfNPC = 0;
    public float infectedNPC = 0;

    public bool isWaiting = false;

    public int doctorCount = 0;
    public int nurseCount = 0;
    public int outpatientCount = 0;
    public int inpatientCount = 0;
    public int emergencypatientCount = 0;
    public int icupatientCount = 0;

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
                WardName = "입원병동 1";
                break;
            case 5:
                WardName = "입원병동 2";
                break;
            case 6:
                WardName = "입원병동 3";
                break;
            case 7:
                WardName = "입원병동 4";
                break;
            case 8:
                WardName = "응급실";
                break;
            case 9:
                WardName = "중환자실/격리실";
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
    }

    // Update is called once per frame
    void Update()
    {
        totalOfNPC = doctors.Count + inpatients.Count + nurses.Count + outpatients.Count + emergencyPatients.Count + icuPatients.Count + quarantinedPatients.Count;
    }

    public void RemoveFromPatientList(PatientController patient)
    {
        if (patient.personComponent.role == Role.Outpatient && patient.wardComponent.outpatients.Contains(patient))
        {
            patient.wardComponent.outpatients.Remove(patient);
        }
        else if (patient.personComponent.role == Role.Inpatient && patient.wardComponent.inpatients.Contains(patient))
        {
            patient.wardComponent.inpatients.Remove(patient);
        }
        else if (patient.personComponent.role == Role.EmergencyPatient && patient.wardComponent.emergencyPatients.Contains(patient))
        {
            patient.wardComponent.emergencyPatients.Remove(patient);
        }
        else if (patient.personComponent.role == Role.ICUPatient && patient.wardComponent.icuPatients.Contains(patient))
        {
            patient.wardComponent.icuPatients.Remove(patient);
        }
        else if (patient.personComponent.role == Role.QuarantinedPatient && patient.wardComponent.quarantinedPatients.Contains(patient))
        {
            patient.wardComponent.quarantinedPatients.Remove(patient);
        }
    }
    public void CloseWard()
    {
        status = WardStatus.Closed;

        MoveOutpatients(num);
        MoveInpatientsToAvailableBeds();
        ClearEmergencyPatients();
        DeactivateNurses();
        DeactivateDoctors();
    }

    private void MoveOutpatients(int index)
    {
        for (int i = outpatients.Count - 1; i >= 0; i--)
        {
            PatientController patient = outpatients[i];
            if (patient == null) continue;
            if (patient.waypointIndex == 4)
            {
                continue;
            }
            patient.StopAllCoroutines();
            patient.agent.ResetPath();
            patient.agent.isStopped = false;
            bool moved = TryMovePatientToAdjacentWard(patient, index);
            if (!moved)
            {
                patient.StartCoroutine(patient.ExitHospital());
            }
        }
        outpatients.Clear();
    }

    private void MoveInpatientsToAvailableBeds()
    {
        for (int i = inpatients.Count - 1; i >= 0; i--)
        {
            PatientController inpatient = inpatients[i];
            if (inpatient == null || inpatient.isExiting) continue;

            //격리 병동으로 전환 시 감염된 환자는 병동을 옮기지 않고 격리 환자로 전환
            if(inpatient.personComponent.infectionStatus != InfectionStatus.Normal && status == WardStatus.Quarantined)
            {
                if(inpatient.hospitalizationCoroutine != null)
                {
                    inpatient.StopCoroutine(inpatient.hospitalizationCoroutine);
                }
                inpatient.isQuarantined = true;
                inpatient.StartCoroutine(inpatient.QuarantineTimeCounter());
                inpatient.personComponent.role = Role.QuarantinedPatient;
                inpatient.tag = "QuarantinedPatient";
                continue;
            }

            BedWaypoint nextBed = wards
                .Where(ward => ward.num >= 4 && ward.num <= 7 && ward.status == WardStatus.Normal)
                .SelectMany(ward => ward.beds)
                .FirstOrDefault(bed => bed.patient == null);

            if (nextBed != null)
            {
                nextBed.patient = inpatient.gameObject;
                if (inpatient.prevCoroutine != null)
                {
                    inpatient.StopCoroutine(inpatient.prevCoroutine);
                }
                inpatient.prevCoroutine = inpatient.StartCoroutine(inpatient.TransferToAvailableWard(nextBed));
            }
            else
            {
                inpatient.StopAllCoroutines();
                inpatient.agent.isStopped = false;
                inpatient.StartCoroutine(inpatient.ExitHospital());
            }
        }
        inpatients.Clear();
    }

    private void ClearEmergencyPatients()
    {
        for (int i = emergencyPatients.Count - 1; i >= 0; i--)
        {
            PatientController patient = emergencyPatients[i];
            if (patient != null)
            {
                patient.StartCoroutine(patient.ExitHospital());
            }
        }
        emergencyPatients.Clear();
    }

    private void DeactivateNurses()
    {
        for (int i = nurses.Count - 1; i >= 0; i--)
        {
            if (nurses[i] != null && !nurses[i].isWorking)
            {
                Managers.ObjectPooling.DeactivateNurse(nurses[i].gameObject);
            }
        }
        nurses.Clear();
    }

    private void DeactivateDoctors()
    {
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

    private bool TryMovePatientToAdjacentWard(PatientController patient, int index)
    {
        int targetIndex = -1;
        if ((index == 0 || index == 2) && wards[index + 1].status == WardStatus.Normal)
        {
            targetIndex = index + 1;
        }
        else if ((index == 1 || index == 3) && wards[index - 1].status == WardStatus.Normal)
        {
            targetIndex = index - 1;
        }

        if (targetIndex != -1)
        {
            patient.ward = targetIndex;
            patient.waypointsTransform = Managers.NPCManager.waypointDictionary[(targetIndex, "OutpatientWaypoints")];
            patient.wardComponent.outpatients.Remove(patient);
            patient.wardComponent = wards[targetIndex];
            patient.wardComponent.outpatients.Add(patient);
            patient.waypoints.Clear();
            patient.isWaitingForDoctor = false;
            patient.waypointIndex = 0;
            patient.isWaiting = false;
            return true;
        }
        return false;
    }

    public void OpenWard()
    {
        if(status == WardStatus.Closed)
        {
            status = WardStatus.Normal;
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
                for (int i = (num - 4) * 12; i < ((num - 4) * 12) + 12; i++)
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
        if(status == WardStatus.Quarantined)
        {
            for (int i = nurses.Count - 1; i >= 0; i--)
            {
                if (nurses[i] != null)
                {
                    nurses[i].protectedGear.meshRenderer.enabled = false;
                    nurses[i].meshRenderer.enabled = true;
                }
            }

            for (int i = doctors.Count - 1; i >= 0; i--)
            {
                if (doctors[i] != null)
                {
                    doctors[i].protectedGear.meshRenderer.enabled = false;
                    doctors[i].meshRenderer.enabled = true;
                }
            }
        }
        
    }

    public void QuarantineWard()
    {
        status = WardStatus.Quarantined;
        Debug.Log($"{WardName} 을 격리병동으로 전환함!");

        MoveOutpatients(num);
        MoveInpatientsToAvailableBeds();
        ClearEmergencyPatients();
        NurseWearProtectedGear();
    }
    public void NurseWearProtectedGear()
    {
        for (int i = nurses.Count - 1; i >= 0; i--)
        {
            if (nurses[i] != null)
            {
                nurses[i].protectedGear.meshRenderer.enabled = true;
                nurses[i].meshRenderer.enabled = false;
            }
        }
    }
}
