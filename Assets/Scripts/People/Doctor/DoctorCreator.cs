using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

public class DoctorCreator : MonoBehaviour
{
    public static DoctorCreator Instance;
    public int numberOfDoctor = 0;
    private int[] doctorCount = { 6, 6, 6, 6 };

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        GameObject newDoctor = GameObject.Find("ERDoctor 0");
        newDoctor.GetComponent<DoctorController>().nurse = GameObject.Find("ERNurse 9");
        Managers.ObjectPooling.ActivateDoctor(newDoctor);

        newDoctor = GameObject.Find("ICUDoctor 0");
        DoctorController newDoctorController = newDoctor.GetComponent<DoctorController>();
        newDoctorController.nurse = GameObject.Find("ICUNurse 11");
        newDoctorController.waypoints.AddRange(Managers.NPCManager.waypointDictionary[(9, "DoctorWaypoints")].GetComponentsInChildren<BedWaypoint>());
        Managers.ObjectPooling.ActivateDoctor(newDoctor);

        for (int i = 0; i < Managers.ObjectPooling.maxOfWardDoctor; i++)
        {
            newDoctor = GameObject.Find("WardDoctor " + i);
            Managers.ObjectPooling.ActivateDoctor(newDoctor);
        }
    }

    public void ChangeDoctor(GameObject endDoctor)
    {
        string name = endDoctor.name;
        int num = name[name.Length - 1] - '0';

        for (int i = 0; i < 5; i++)
        {
            GameObject newDoctor = GameObject.Find("Doctor " + doctorCount[num / 5]++ % 5);
            if (newDoctor == null)
            {
                Debug.LogError("새로운 닥터를 찾을 수 없습니다.");
            }
            if (!newDoctor.GetComponent<DoctorController>().isResting)
            {
                continue;
            }
            Managers.ObjectPooling.DeactivateDoctor(endDoctor);
            Managers.ObjectPooling.ActivateDoctor(newDoctor);
            break;
        }
    }
}
