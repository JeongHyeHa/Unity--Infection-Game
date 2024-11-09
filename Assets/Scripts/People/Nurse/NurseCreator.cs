using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NurseCreator : MonoBehaviour
{
    public static NurseCreator Instance; // NurseCreator의 싱글톤 인스턴스
    public int numberOfNurse = 0; // 현재 간호사 수

    // Start는 첫 프레임 업데이트 전에 호출됩니다.
    void Start()
    {
        Instance = this; // 싱글톤 인스턴스 설정
        //병동 간호사 생성
        for (int i = 0; i < Managers.ObjectPooling.maxOfWardNurse; i++)
        {
            GameObject newNurse = GameObject.Find("WardNurse " + i); // 간호사 객체 찾기
            Managers.ObjectPooling.ActivateNurse(newNurse); // 간호사 활성화
            NurseController newNurseController = newNurse.GetComponent<NurseController>();
            for (int j = 0; j < Managers.NPCManager.waypointDictionary[(newNurseController.ward, "NurseWaypoints")].childCount; j++)
            {
                newNurseController.waypoints.Add(Managers.NPCManager.waypointDictionary[(newNurseController.ward, "NurseWaypoints")].GetChild(j).GetComponent<Waypoint>());
            }
            newNurseController.waypoints.Add(Managers.NPCManager.waypointDictionary[(newNurseController.ward, "OutpatientWaypoints")].Find("CounterWaypoint (0)").GetComponent<Waypoint>());
            newNurseController.waypoints.Add(Managers.NPCManager.waypointDictionary[(newNurseController.ward, "OutpatientWaypoints")].Find("CounterWaypoint (1)").GetComponent<Waypoint>());
            newNurseController.waypoints.Add(Managers.NPCManager.waypointDictionary[(newNurseController.ward, "OutpatientWaypoints")].Find("SofaWaypoint (0)").GetComponent<Waypoint>());
            int roleNum = i % 16;
            // 간호사의 종류에 따라 웨이포인트 설정
            if (roleNum >= 0 && roleNum <= 3)
            {
                for (int j = 0; j < newNurseController.waypoints[2].chairsDictionary.Count; j++)
                {
                    if (newNurseController.waypoints[2].chairsDictionary[j].Item2)
                    {
                        newNurseController.waypoints[2].chairsDictionary[j] = (newNurseController.waypoints[2].chairsDictionary[j].Item1, false);
                        newNurseController.chair = newNurseController.waypoints[2].chairsDictionary[j].Item1;
                        break;
                    }
                }
            }
            else if (roleNum >= 4 && roleNum <= 7)
            {
                for (int j = 0; j < newNurseController.waypoints[3].chairsDictionary.Count; j++)
                {
                    if (newNurseController.waypoints[3].chairsDictionary[j].Item2)
                    {
                        newNurseController.waypoints[3].chairsDictionary[j] = (newNurseController.waypoints[3].chairsDictionary[j].Item1, false);
                        newNurseController.chair = newNurseController.waypoints[3].chairsDictionary[j].Item1;
                        break;
                    }
                }
            }
        }
        for (int i = 0; i < Managers.ObjectPooling.maxOfERNurse; i++)
        {
            GameObject newNurse = GameObject.Find("ERNurse " + i);
            Managers.ObjectPooling.ActivateNurse(newNurse);
            NurseController newNurseController = newNurse.GetComponent<NurseController>();
            newNurseController.waypoints.Add(Managers.NPCManager.waypointDictionary[(newNurseController.ward, "NurseWaypoints")].Find("Counter").GetComponent<Waypoint>());
            newNurseController.waypoints.Add(Managers.NPCManager.waypointDictionary[(newNurseController.ward, "NurseWaypoints")].Find("Counter (1)").GetComponent<Waypoint>());
            for (int j = 0; j < 28; j++)
            {
                newNurseController.waypoints.Add(Managers.NPCManager.waypointDictionary[(newNurseController.ward, "EmergencyPatientWaypoints")].Find("BedWaypoint (" + j + ")").GetComponent<BedWaypoint>());
            }
            if (i >= 0 && i <= 5)
            {
                newNurseController.chair = newNurseController.waypoints[0].chairsDictionary[i].Item1;
            }
            else if (i >= 6 && i <= 8)
            {
                newNurseController.chair = newNurseController.waypoints[1].chairsDictionary[i - 6].Item1;
            }
            else if (i == 9)
            {
                newNurseController.doctor = GameObject.Find("ERDoctor " + (i - 9)).GetComponent<DoctorController>();
            }

        }
        for (int i = 0; i < Managers.ObjectPooling.maxOfInpatientWardNurse; i++)
        {
            GameObject newNurse = GameObject.Find("InpatientWardNurse " + i);
            Managers.ObjectPooling.ActivateNurse(newNurse);
            NurseController newNurseController = newNurse.GetComponent<NurseController>();

            newNurseController.waypoints.AddRange(Managers.NPCManager.waypointDictionary[(newNurseController.ward, "NurseWaypoints")].GetComponentsInChildren<Waypoint>().ToList());
            newNurseController.waypoints.AddRange(Managers.NPCManager.waypointDictionary[(newNurseController.ward, "InpatientWaypoints")].GetComponentsInChildren<Waypoint>().ToList());
            int roleNum = i % 12;
            if (0 <= roleNum && roleNum <= 3)
            {
                for (int j = 0; j < newNurseController.waypoints[2].chairsDictionary.Count; j++)
                {
                    if (newNurseController.waypoints[2].chairsDictionary[j].Item2)
                    {
                        newNurseController.waypoints[2].chairsDictionary[j] = (newNurseController.waypoints[2].chairsDictionary[j].Item1, false);
                        newNurseController.chair = newNurseController.waypoints[2].chairsDictionary[j].Item1;
                        break;
                    }
                }
            }
            else if (4 <= roleNum && roleNum <= 7)
            {
                for (int j = 0; j < newNurseController.waypoints[3].chairsDictionary.Count; j++)
                {
                    if (newNurseController.waypoints[3].chairsDictionary[j].Item2)
                    {
                        newNurseController.waypoints[3].chairsDictionary[j] = (newNurseController.waypoints[3].chairsDictionary[j].Item1, false);
                        newNurseController.chair = newNurseController.waypoints[3].chairsDictionary[j].Item1;
                        break;
                    }
                }
            }

        }
        for (int i = 0; i < Managers.ObjectPooling.maxOfICUNurse; i++)
        {
            GameObject newNurse = GameObject.Find("ICUNurse " + i);
            Managers.ObjectPooling.ActivateNurse(newNurse);
            NurseController newNurseController = newNurse.GetComponent<NurseController>();
            newNurseController.waypoints.AddRange(Managers.NPCManager.waypointDictionary[(newNurseController.ward, "DoctorWaypoints")].GetComponentsInChildren<Waypoint>());
            if (0 <= newNurseController.num && newNurseController.num <= 5)
            {
                for (int j = 0; j < newNurseController.waypoints[0].chairsDictionary.Count; j++)
                {
                    if (newNurseController.waypoints[0].chairsDictionary[j].Item2)
                    {
                        newNurseController.waypoints[0].chairsDictionary[j] = (newNurseController.waypoints[0].chairsDictionary[j].Item1, false);
                        newNurseController.chair = newNurseController.waypoints[0].chairsDictionary[j].Item1;
                        break;
                    }
                }
            }
            else if (6 <= newNurseController.num && newNurseController.num <= 10)
            {
                for (int j = 0; j < newNurseController.waypoints[1].chairsDictionary.Count; j++)
                {
                    if (newNurseController.waypoints[1].chairsDictionary[j].Item2)
                    {
                        newNurseController.waypoints[1].chairsDictionary[j] = (newNurseController.waypoints[1].chairsDictionary[j].Item1, false);
                        newNurseController.chair = newNurseController.waypoints[1].chairsDictionary[j].Item1;
                        break;
                    }
                }
            }
            else if(newNurseController.num == 11)
            {
                newNurseController.doctor = GameObject.Find("ICUDoctor 0").GetComponent<DoctorController>();
            }
        }
    }

}
