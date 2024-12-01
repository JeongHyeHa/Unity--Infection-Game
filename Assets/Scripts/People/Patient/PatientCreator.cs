using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PatientCreator
{
    public int numberOfInpatient;
    public int numberOfOutpatient;
    public int numberOfEmergencyPatient;
    public int numberOfICUPatient;
    public List<Waypoint> spawnAreas = new List<Waypoint>();

    public float infectionRate = 0.03f; // 감염 확률
    public float spawnDelay = 1f; // 생성 대기 시간

    public bool outpatientWaiting;
    public bool emergencyPatientWaiting;
    public bool startSignal = false;

    ProfileWindow profileWindow;

    // Start is called before the first frame update
    public void Init()
    {
        profileWindow = Object.FindObjectOfType<ProfileWindow>();

        Transform gatewayTransform = Managers.NPCManager.gatewayTransform; // "Gateways" 오브젝트 찾기



        if (gatewayTransform != null)
        {

            for (int i = 0; i < gatewayTransform.childCount; i++)
            {
                Waypoint waypointRange = gatewayTransform.GetChild(i).GetComponent<Waypoint>();
                if (waypointRange != null)
                {
                    spawnAreas.Add(waypointRange); // Waypoint 리스트에 추가
                }
                else
                {
                    Debug.LogError("Gateways 자식 오브젝트에 Waypoint 컴포넌트가 없습니다.");
                }
            }
        }
        else
        {
            Debug.LogError("Gateways 게임 오브젝트를 찾을 수 없습니다.");
        }

        for (int i = 0; i < Managers.ObjectPooling.maxOfInpatient * 0.7; i++)
        {
            // 입원 환자 스폰 위치 설정
            int ward = (i / 40) + 4;
            BedWaypoint spawnArea = Managers.NPCManager.waypointDictionary[(ward, "InpatientWaypoints")].Find("BedWaypoint (" + (i % 40) + ")").gameObject.GetComponent<BedWaypoint>();
            GameObject newInpatient = Managers.ObjectPooling.ActivateInpatient(spawnArea.GetMiddlePointInRange());
            newInpatient.name = "Inpatient " + i;

            // 입원 환자 위치별 Layer 설정


            if (ward >= 0 && ward < Managers.LayerChanger.layers.Length)
            {
                Managers.LayerChanger.SetLayerRecursively(newInpatient, LayerMask.NameToLayer(Managers.LayerChanger.layers[ward]));
            }

            spawnArea.patient = newInpatient;
            PatientController newInpatientController = newInpatient.GetComponent<PatientController>();
            newInpatientController.waypointsTransform = Managers.NPCManager.waypointDictionary[(ward, "InpatientWaypoints")];
            newInpatientController.ward = ward;
            newInpatientController.num = i;
            newInpatientController.bedWaypoint = spawnArea;
            Transform waypointTransform = newInpatientController.bedWaypoint.transform.parent;
            newInpatientController.wardComponent = waypointTransform.parent.GetComponent<Ward>();
            //newInpatientController.wardComponent.totalOfNPC++;
            newInpatientController.wardComponent.inpatients.Add(newInpatientController);


            profileWindow.AddInpatientProfile(newInpatient);
            numberOfInpatient++;
        }

        for (int i = 0; i < Managers.ObjectPooling.maxOfEmergencyPatient * 0.5f; i++)
        {
            int ward = 8;
            BedWaypoint spawnArea = Managers.NPCManager.waypointDictionary[(ward, "EmergencyPatientWaypoints")].Find("BedWaypoint (" + i + ")").gameObject.GetComponent<BedWaypoint>();
            GameObject newEmergencyPatient = Managers.ObjectPooling.ActiveEmergentcyPatient(spawnArea.transform.position, spawnArea); // 응급 환자 활성화
            newEmergencyPatient.gameObject.layer = LayerMask.NameToLayer("Floor 1 L");
            newEmergencyPatient.GetComponent<Person>().role = Role.EmergencyPatient;
            if (newEmergencyPatient != null)
            {
                Person newEmergencyPatientPerson = newEmergencyPatient.GetComponent<Person>(); // Person 컴포넌트 가져오기
                Renderer renderer = newEmergencyPatientPerson.GetComponent<Renderer>();         // Renderer 컴포넌트 가져오기

                if (newEmergencyPatientPerson != null)
                {
                    NPCManager.Instance.RegisterNPC(newEmergencyPatient, newEmergencyPatientPerson, renderer);
                    numberOfEmergencyPatient++;
                }
                else
                {
                    Debug.LogError("새 응급 환자에 Person 컴포넌트가 없습니다.");
                }
            }
            else
            {
                Debug.LogError("새 응급 환자를 활성화하는 데 실패했습니다.");
            }
        }

        for (int i = 0; i < Managers.ObjectPooling.maxOfICUPateint * 0.7f; i++)
        {
            int ward = 9;
            BedWaypoint spawnArea = Managers.NPCManager.waypointDictionary[(ward, "DoctorWaypoints")].Find("BedWaypoint (" + i + ")").gameObject.GetComponent<BedWaypoint>();
            GameObject newICUPatient = Managers.ObjectPooling.ActiveICUPatient(spawnArea.GetMiddlePointInRange());
            newICUPatient.name = $"ICUPatient {i}";

            if (ward >= 0 && ward < Managers.LayerChanger.layers.Length)
            {
                Managers.LayerChanger.SetLayerRecursively(newICUPatient, LayerMask.NameToLayer(Managers.LayerChanger.layers[ward]));
            }

            spawnArea.patient = newICUPatient;
            PatientController newICUPatientController = newICUPatient.GetComponent<PatientController>();
            newICUPatientController.waypointsTransform = Managers.NPCManager.waypointDictionary[(ward, "DoctorWaypoints")];
            newICUPatientController.ward = ward;
            newICUPatientController.num = i;
            newICUPatientController.bedWaypoint = spawnArea;
            newICUPatientController.bedWaypoint.isEmpty = false;
            Transform waypointTransform = newICUPatientController.bedWaypoint.transform.parent;
            newICUPatientController.wardComponent = waypointTransform.parent.GetComponent<Ward>();
            newICUPatientController.wardComponent.icuPatients.Add(newICUPatientController);
            profileWindow.AddICUPateintProfile(newICUPatient);
            numberOfICUPatient++;
        }
    }
    public IEnumerator SpawnOutpatient()
    {
        outpatientWaiting = true; // 대기 상태로 설정
        yield return new WaitUntil(() => startSignal);
        if (AreAllWardsClosed())
        {
            outpatientWaiting = false;
            yield break;
        }
        Vector3 spawnPosition = spawnAreas[Random.Range(0, 2)].GetRandomPointInRange(); // 랜덤 생성 위치 설정
        GameObject newOutpatient = Managers.ObjectPooling.ActivateOutpatient(spawnPosition); // 외래 환자 활성화
        newOutpatient.GetComponent<Person>().role = Role.Outpatient;
        if (newOutpatient != null)
        {
            Person newOutPatientPerson = newOutpatient.GetComponent<Person>(); // Person 컴포넌트 가져오기
            Renderer renderer = newOutpatient.GetComponent<Renderer>();

            if (newOutPatientPerson != null)
            {
                NPCManager.Instance.RegisterNPC(newOutpatient, newOutPatientPerson, renderer);

                // 감염 상태 설정
                if (Random.value < infectionRate) //  && QuarantineManager.quarantineStep > 0
                {
                    if (Managers.Stage.stage == 1)
                    {
                        newOutPatientPerson.ChangeStatus(InfectionStatus.CRE);
                    }
                    else if (Managers.Stage.stage == 2)
                    {
                        newOutPatientPerson.ChangeStatus(InfectionStatus.Covid);
                    }
                }
                numberOfOutpatient++; // 외래 환자 수 증가
            }
            else
            {
                Debug.LogError("새 외래 환자에 Person 컴포넌트가 없습니다.");
            }
        }
        else
        {
            Debug.LogError("새 외래 환자를 활성화하는 데 실패했습니다.");
        }

        yield return YieldInstructionCache.WaitForSeconds(spawnDelay); // 대기 시간
        outpatientWaiting = false; // 대기 상태 해제
    }

    public IEnumerator SpawnEmergencyPatient()
    {
        emergencyPatientWaiting = true;
        yield return new WaitUntil(() => startSignal);
        yield return YieldInstructionCache.WaitForSeconds(Random.Range(spawnDelay, spawnDelay + 15f)); // 대기 시간

        BedWaypoint nextBed = Ward.wards[8].beds.FirstOrDefault(bed => bed.isEmpty && bed.patient == null);
        if (nextBed != null)
        {
            nextBed.isEmpty = false;
        }
        else
        {
            emergencyPatientWaiting = false;
            yield break;
        }

        Vector3 spawnPosition = spawnAreas[2].GetRandomPointInRange(); // 랜덤 생성 위치 설정
        GameObject newEmergencyPatient = Managers.ObjectPooling.ActiveEmergentcyPatient(spawnPosition, nextBed); // 응급 환자 활성화
        newEmergencyPatient.gameObject.layer = LayerMask.NameToLayer("Floor 1 L");
        newEmergencyPatient.GetComponent<Person>().role = Role.EmergencyPatient;
        if (newEmergencyPatient != null)
        {
            Person newEmergencyPatientPerson = newEmergencyPatient.GetComponent<Person>(); // Person 컴포넌트 가져오기
            Renderer renderer = newEmergencyPatientPerson.GetComponent<Renderer>();         // Renderer 컴포넌트 가져오기

            if (newEmergencyPatientPerson != null)
            {
                NPCManager.Instance.RegisterNPC(newEmergencyPatient, newEmergencyPatientPerson, renderer);
                // 감염 상태 설정
                if (Random.value < infectionRate) //  && QuarantineManager.quarantineStep > 0
                {
                    if (Managers.Stage.stage == 1)
                    {
                        newEmergencyPatientPerson.ChangeStatus(InfectionStatus.CRE);
                    }
                    else if (Managers.Stage.stage == 2)
                    {
                        newEmergencyPatientPerson.ChangeStatus(InfectionStatus.Covid);
                    }
                }
                numberOfEmergencyPatient++;
            }
            else
            {
                Debug.LogError("새 응급 환자에 Person 컴포넌트가 없습니다.");
            }
        }
        else
        {
            Debug.LogError("새 응급 환자를 활성화하는 데 실패했습니다.");
        }


        emergencyPatientWaiting = false; // 대기 상태 해제

    }

    private bool AreAllWardsClosed()
    {
        for (int i = 0; i < 4; i++)
        {
            var ward = Managers.NPCManager.waypointDictionary[(i, "OutpatientWaypoints")].gameObject.GetComponentInParent<Ward>();
            if (ward.status == Ward.WardStatus.Normal)
            {
                return false;
            }
        }
        return true;
    }
}
