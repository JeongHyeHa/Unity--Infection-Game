using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolingManager
{
    // 최대 외래 환자, 의사, 간호사, 입원 환자 수
    public int maxOfPatientObject = 300;
    public int maxOfOutpatient = 50;
    public int maxOfInpatient = 50;
    public int maxOfEmergencyPatient = 28;
    public int maxOfICUPateint = 18;


    public int maxOfWardDoctor = 24;
    public int maxOfERDoctor = 1;
    public int maxOfICUDoctor = 1;

    public int maxOfWardNurse = 64;
    public int maxOfERNurse = 10;
    public int maxOfInpatientWardNurse = 48;
    public int maxOfICUNurse = 12;

    public float newOutpatients = 0;
    public float infectedOutpatients = 0;

    public ProfileWindow profileWindow;

    // 비활성화된 외래 환자 오브젝트를 저장하는 큐
    public Queue<GameObject> patientQueue = new Queue<GameObject>();

    public void Init()
    {
        profileWindow = GameObject.FindObjectOfType<ProfileWindow>();

        // 의사, 외래 환자, 간호사 초기화
        DoctorInitialize();
        NurseInitialize();
        PatientInitialize();
    }

    private void PatientInitialize()
    {
        GameObject[] patientPrefabs = Resources.LoadAll<GameObject>("Prefabs/Patient");

        for (int i = 0; i < maxOfPatientObject; i++)
        {
            Waypoint spawnArea = Managers.NPCManager.gatewayTransform.Find("Gateway (" + Random.Range(0, 2) + ")").GetComponent<Waypoint>();
            // 프리팹 리스트에서 랜덤으로 하나 선택하여 생성
            GameObject newPatient = Object.Instantiate(patientPrefabs[Random.Range(0, patientPrefabs.Length)], spawnArea.GetRandomPointInRange(), Quaternion.identity);
            newPatient.name = "Patient " + i;
            newPatient.GetComponent<PatientController>().num = i;
            patientQueue.Enqueue(newPatient); // 큐에 추가
            newPatient.SetActive(false); // 비활성화
        }


    }

    // 의사 초기화
    private void DoctorInitialize()
    {
        // 의사 프리팹 로드
        GameObject[] DoctorPrefabs = Resources.LoadAll<GameObject>("Prefabs/Doctor");
        int[] officeNumbers = { 0, 1, 2, 5, 6, 7 };
        //병동 의사 생성
        for (int i = 0; i < maxOfWardDoctor; i++)
        {
            int ward = i / 6;
            // 의사 스폰 위치 설정
            DoctorOffice spawnArea = Managers.NPCManager.waypointDictionary[(ward, "DoctorWaypoints")].Find("Doctor'sOffice (" + (officeNumbers[i % 6]) + ")").GetComponent<DoctorOffice>();

            // 프리팹 리스트에서 랜덤으로 하나 선택하여 생성
            GameObject newDoctor = Object.Instantiate(DoctorPrefabs[Random.Range(0, DoctorPrefabs.Length)], spawnArea.GetRandomPointInRange(), Quaternion.identity);
            newDoctor.name = "WardDoctor " + i;

            if (ward >= 0 && ward < Managers.LayerChanger.layers.Length)
            {
                Managers.LayerChanger.SetLayerRecursively(newDoctor, LayerMask.NameToLayer(Managers.LayerChanger.layers[ward]));
            }

            DoctorController doctorController = newDoctor.GetComponent<DoctorController>();
            doctorController.num = i;
            doctorController.ward = ward;
            doctorController.wardComponent = Managers.NPCManager.waypointDictionary[(ward, "DoctorWaypoints")].GetComponentInParent<Ward>();

            // 의사 사무실 할당
            doctorController.waypoints.Add(spawnArea);

            // 외래 환자 대기 구역 할당
            DoctorOffice waypoint = Managers.NPCManager.waypointDictionary[(ward, "OutpatientWaypoints")].Find("Doctor'sOffice (" + (officeNumbers[i % 6]) + ")").GetComponent<DoctorOffice>();
            doctorController.waypoints.Add(waypoint);
            newDoctor.transform.position = spawnArea.transform.position;
            newDoctor.GetComponent<SkinnedMeshRenderer>().enabled = false;
            doctorController.isResting = true;

            newDoctor.GetComponent<Person>().role = Role.Doctor;
            doctorController.role = DoctorRole.Ward;
            spawnArea.doctor = newDoctor;
            waypoint.doctor = newDoctor;
        }
        for (int i = 0; i < maxOfERDoctor; i++)
        {
            // 의사 스폰 위치 설정
            Waypoint spawnArea = Managers.NPCManager.waypointDictionary[(8, "NurseWaypoints")].Find("Counter").GetComponent<Waypoint>();

            // 프리팹 리스트에서 랜덤으로 하나 선택하여 생성
            GameObject newDoctor = Object.Instantiate(DoctorPrefabs[Random.Range(0, DoctorPrefabs.Length)], spawnArea.GetRandomPointInRange(), Quaternion.identity);
            newDoctor.name = "ERDoctor " + i;
            Managers.LayerChanger.SetLayerRecursively(newDoctor, LayerMask.NameToLayer(Managers.LayerChanger.layers[8]));

            DoctorController doctorController = newDoctor.GetComponent<DoctorController>();
            doctorController.waypoints.Add(spawnArea);
            doctorController.num = i;
            doctorController.ward = 8;
            doctorController.wardComponent = Managers.NPCManager.waypointDictionary[(8, "NurseWaypoints")].GetComponentInParent<Ward>();
            newDoctor.GetComponent<Person>().role = Role.Doctor;
            doctorController.role = DoctorRole.ER;
        }
        for (int i = 0; i < maxOfICUDoctor; i++)
        {
            // 의사 스폰 위치 설정
            Waypoint spawnArea = Managers.NPCManager.waypointDictionary[(9, "DoctorWaypoints")].Find("Counter").GetComponent<Waypoint>();

            // 프리팹 리스트에서 랜덤으로 하나 선택하여 생성
            GameObject newDoctor = Object.Instantiate(DoctorPrefabs[Random.Range(0, DoctorPrefabs.Length)], spawnArea.GetRandomPointInRange(), Quaternion.identity);
            newDoctor.name = "ICUDoctor " + i;
            Managers.LayerChanger.SetLayerRecursively(newDoctor, LayerMask.NameToLayer(Managers.LayerChanger.layers[9]));
            DoctorController doctorController = newDoctor.GetComponent<DoctorController>();
            doctorController.waypoints.Add(spawnArea);
            doctorController.num = i;
            doctorController.ward = 9;
            doctorController.wardComponent = Managers.NPCManager.waypointDictionary[(9, "NurseWaypoints")].GetComponentInParent<Ward>();
            newDoctor.GetComponent<Person>().role = Role.Doctor;
            doctorController.role = DoctorRole.ICU;
        }
    }

    // 간호사 초기화
    private void NurseInitialize()
    {
        // 간호사 프리팹 로드
        GameObject[] NursePrefabs = Resources.LoadAll<GameObject>("Prefabs/Nurse");

        //병동 간호사 생성
        for (int i = 0; i < maxOfWardNurse; i++)
        {
            // 간호사 스폰 위치 설정
            int ward = i / 16;
            Waypoint spawnArea = Managers.NPCManager.waypointDictionary[(ward, "NurseWaypoints")].Find("NurseSpawnArea").gameObject.GetComponent<Waypoint>();
            GameObject newNurse = Object.Instantiate(NursePrefabs[Random.Range(0, NursePrefabs.Length)], spawnArea.GetRandomPointInRange(), Quaternion.identity);
            newNurse.name = "WardNurse " + i;

            // 간호사 위치별 Layer 설정
            if (ward >= 0 && ward < Managers.LayerChanger.layers.Length)
            {
                Managers.LayerChanger.SetLayerRecursively(newNurse, LayerMask.NameToLayer(Managers.LayerChanger.layers[ward]));
            }

            NurseController newNurseController = newNurse.GetComponent<NurseController>();
            newNurseController.num = i;
            newNurseController.ward = ward;
            newNurseController.isRest = true;
            newNurseController.wardComponent = spawnArea.gameObject.transform.parent.parent.GetComponent<Ward>();
            newNurseController.role = NurseRole.Ward;
            newNurse.GetComponent<SkinnedMeshRenderer>().enabled = false;
            newNurse.GetComponent<CapsuleCollider>().enabled = false;
            newNurse.GetComponent<Person>().role = Role.Nurse;
        }
        for (int i = 0; i < maxOfInpatientWardNurse; i++)
        {
            int ward = (i / 12) + 4;
            Waypoint spawnArea = Managers.NPCManager.waypointDictionary[(ward, "NurseWaypoints")].Find("NurseSpawnArea").gameObject.GetComponent<Waypoint>();
            GameObject newNurse = Object.Instantiate(NursePrefabs[Random.Range(0, NursePrefabs.Length)], spawnArea.GetRandomPointInRange(), Quaternion.identity);
            newNurse.name = "InpatientWardNurse " + i;
            // 간호사 위치별 Layer 설정
            if (ward >= 0 && ward < Managers.LayerChanger.layers.Length)
            {
                Managers.LayerChanger.SetLayerRecursively(newNurse, LayerMask.NameToLayer(Managers.LayerChanger.layers[ward]));
            }

            NurseController newNurseController = newNurse.GetComponent<NurseController>();
            newNurseController.role = NurseRole.InpateintWard;
            newNurseController.num = i;
            newNurseController.ward = ward;
            newNurseController.isRest = true;
            newNurseController.wardComponent = spawnArea.gameObject.transform.parent.parent.GetComponent<Ward>();
            newNurseController.role = NurseRole.InpateintWard;
            newNurse.GetComponent<SkinnedMeshRenderer>().enabled = false;
            newNurse.GetComponent<CapsuleCollider>().enabled = false;
            newNurse.GetComponent<Person>().role = Role.Nurse;
        }
        for (int i = 0; i < maxOfERNurse; i++)
        {
            int ward = 8;
            Waypoint spawnArea = Managers.NPCManager.waypointDictionary[(ward, "NurseWaypoints")].Find("Counter").gameObject.GetComponent<Waypoint>();
            GameObject newNurse = Object.Instantiate(NursePrefabs[Random.Range(0, NursePrefabs.Length)], spawnArea.GetRandomPointInRange(), Quaternion.identity);
            newNurse.name = "ERNurse " + i;

            // 간호사 위치별 Layer 설정
            if (ward >= 0 && ward < Managers.LayerChanger.layers.Length)
            {
                Managers.LayerChanger.SetLayerRecursively(newNurse, LayerMask.NameToLayer(Managers.LayerChanger.layers[ward]));
            }

            NurseController newNurseController = newNurse.GetComponent<NurseController>();
            newNurseController.num = i;
            newNurseController.ward = ward;
            newNurseController.isRest = true;
            newNurseController.wardComponent = spawnArea.gameObject.transform.parent.parent.GetComponent<Ward>();
            newNurseController.role = NurseRole.ER;
            newNurse.GetComponent<SkinnedMeshRenderer>().enabled = false;
            newNurse.GetComponent<CapsuleCollider>().enabled = false;
            newNurse.GetComponent<Person>().role = Role.Nurse;
        }
        for (int i = 0; i < maxOfICUNurse; i++)
        {
            int ward = 9;
            Waypoint spawnArea = Managers.NPCManager.waypointDictionary[(ward, "DoctorWaypoints")].Find("Counter").gameObject.GetComponent<Waypoint>();
            GameObject newNurse = Object.Instantiate(NursePrefabs[Random.Range(0, NursePrefabs.Length)], spawnArea.GetRandomPointInRange(), Quaternion.identity);
            newNurse.name = "ICUNurse " + i;

            // 간호사 위치별 Layer 설정
            if (ward >= 0 && ward < Managers.LayerChanger.layers.Length)
            {
                Managers.LayerChanger.SetLayerRecursively(newNurse, LayerMask.NameToLayer(Managers.LayerChanger.layers[ward]));
            }
            NurseController newNurseController = newNurse.GetComponent<NurseController>();
            newNurseController.num = i;
            newNurseController.ward = ward;
            newNurseController.wardComponent = spawnArea.gameObject.transform.parent.parent.GetComponent<Ward>();
            newNurseController.role = NurseRole.ICU;
            newNurse.GetComponent<Person>().role = Role.Nurse;
        }
    }
    public GameObject ActiveICUPatient(Vector3 position)
    {
        GameObject newPatient = patientQueue.Dequeue();
        newPatient.transform.position = position;
        newPatient.SetActive(true);
        newPatient.GetComponent<Person>().role = Role.ICUPatient;
        newPatient.tag = "ICUPatient";
        return newPatient;
    }
    public GameObject ActivateInpatient(Vector3 position)
    {
        GameObject newPatient = patientQueue.Dequeue();
        newPatient.transform.position = position;
        newPatient.SetActive(true);
        newPatient.GetComponent<Person>().role = Role.Inpatient;
        newPatient.tag = "Inpatient";
        PatientController newPatientController = newPatient.GetComponent<PatientController>();

        return newPatient;
    }

    public GameObject ActivateOutpatient(Vector3 position)
    {
        GameObject newOutpatient = patientQueue.Dequeue(); // 큐에서 외래 환자 가져오기
        profileWindow.AddOutpatientProfile(newOutpatient);
        newOutpatient.transform.position = position; // 위치 설정
        newOutpatient.SetActive(true); // 활성화
        newOutpatient.tag = "Outpatient";
        Managers.LayerChanger.SetLayerRecursively(newOutpatient, LayerMask.NameToLayer("Floor 1"));
        PatientController newPatientController = newOutpatient.GetComponent<PatientController>();
        newOutpatients++;
        newPatientController.Activate();
        //newOutpatientController.wardComponent.totalOfNPC++;
        newPatientController.wardComponent.outpatients.Add(newPatientController);
        return newOutpatient;
    }

    public void DeactivatePatient(GameObject patient)
    {
        PatientController patientController = patient.GetComponent<PatientController>();
        patientController.waypoints.Clear(); // 웨이포인트 초기화
        patientController.isWaiting = false; // 대기 상태 초기화
        patientController.waypointIndex = 0; // 웨이포인트 인덱스 초기화
        patientController.doctorSignal = false; // 의사 신호 초기화
        patientController.nurseSignal = false; // 간호사 신호 초기화
        patientController.officeSignal = false; // 진료실 신호 초기화
        patientController.isExiting = false;
        patientController.nurse = null;
        patientController.quarantineRoom = null;
        if (patientController.bedWaypoint != null)
        {
            patientController.bedWaypoint.isEmpty = true;
            patientController.bedWaypoint = null;
        }
        patientController.prevWaypointIndex = -1;
        patientController.waypointsTransform = null;
        patientController.ward = -1;
        patientController.excutedHC = false;
        patientController.excutedQC = false;
        Person outpatientPerson = patient.GetComponent<Person>();

        if (outpatientPerson.status != InfectionState.Normal)
        {
            infectedOutpatients++;
            outpatientPerson.status = InfectionState.Normal; // 감염 상태 초기화
            patientController.wardComponent.infectedNPC--;
        }
        outpatientPerson.isImmune = false;
        //outpatientController.wardComponent.outpatients.Remove(outpatientController);
        patientController.wardComponent = null;
        patientQueue.Enqueue(patient); // 큐에 추가
        patient.SetActive(false); // 비활성화
    }

    public GameObject ActiveEmergentcyPatient(Vector3 position, BedWaypoint bed)
    {
        GameObject newEmergencyPatient = patientQueue.Dequeue();
        profileWindow.AddEmerpatientProfile(newEmergencyPatient);
        newEmergencyPatient.transform.position = position;
        newEmergencyPatient.SetActive(true);
        newEmergencyPatient.tag = "EmergencyPatient";
        Managers.LayerChanger.SetLayerRecursively(newEmergencyPatient, LayerMask.NameToLayer("Floor 1"));
        PatientController newPatientController = newEmergencyPatient.GetComponent<PatientController>();
        newPatientController.waypointsTransform = Managers.NPCManager.waypointDictionary[(8, "EmergencyPatientWaypoints")];
        newPatientController.wardComponent = newPatientController.waypointsTransform.GetComponentInParent<Ward>();
        newPatientController.wardComponent.emergencyPatients.Add(newPatientController);
        newPatientController.ward = 8;

        newPatientController.bedWaypoint = bed;
        newPatientController.bedWaypoint.patient = newEmergencyPatient;
        newPatientController.waypoints.Add(bed);

        return newEmergencyPatient;

    }


    // 의사 활성화 및 위치 설정
    public GameObject ActivateDoctor(GameObject newDoctor)
    {
        DoctorController doctorController = newDoctor.GetComponent<DoctorController>();
        profileWindow.AddDoctorProfile(newDoctor);
        doctorController.changeSignal = true; // 신호 설정
        newDoctor.transform.position = doctorController.waypoints[0].GetRandomPointInRange(); // 위치 설정
        newDoctor.GetComponent<SkinnedMeshRenderer>().enabled = true; // 렌더러 활성화
        //doctorController.wardComponent.totalOfNPC++;
        doctorController.wardComponent.doctors.Add(doctorController);
        doctorController.waypoints[0].isEmpty = false;
        doctorController.isResting = false; // 휴식 상태 해제
        return newDoctor;
    }


    // 의사 비활성화 및 초기화
    public void DeactivateDoctor(GameObject doctor)
    {
        DoctorController doctorController = doctor.GetComponent<DoctorController>();
        doctorController.patientCount = 0; // 환자 수 초기화
        doctorController.isWaiting = false; // 대기 상태 초기화
        doctorController.isResting = true; // 휴식 상태 설정
        doctorController.changeSignal = false; // 신호 초기화
        //doctorController.wardComponent.totalOfNPC--;
        doctorController.wardComponent.doctors.Remove(doctorController);
        doctorController.waypoints[0].isEmpty = true;
        doctor.GetComponent<SkinnedMeshRenderer>().enabled = false; // 렌더러 비활성화
        profileWindow.RemoveProfile(doctor.GetComponent<Person>().ID);
    }


    // 간호사 활성화
    public GameObject ActivateNurse(GameObject newNurse)
    {
        newNurse.GetComponent<SkinnedMeshRenderer>().enabled = true; // 렌더러 활성화
        newNurse.GetComponent<CapsuleCollider>().enabled = true;
        profileWindow.AddNurseProfile(newNurse);
        NurseController nurseController = newNurse.GetComponent<NurseController>();
        nurseController.isRest = false;
        nurseController.wardComponent.nurses.Add(nurseController);
        //nurseController.wardComponent.totalOfNPC++;
        return newNurse;
    }

    public void DeactivateNurse(GameObject nurse)
    {
        nurse.GetComponent<SkinnedMeshRenderer>().enabled = false; // 렌더러 활성화
        NurseController nurseController = nurse.GetComponent<NurseController>();
        nurseController.isRest = true;
        nurseController.isWaiting = false;
        nurseController.isWaitingAtDoctorOffice = false;
        nurseController.isWorking = false;
        //nurseController.wardComponent.totalOfNPC--;
        nurseController.wardComponent.nurses.Remove(nurseController);
        profileWindow.RemoveProfile(nurse.GetComponent<Person>().ID);
    }
}
