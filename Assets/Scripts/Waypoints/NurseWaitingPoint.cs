using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NurseWaitingPoint : Waypoint
{
    public DoctorOffice doctorOffice;
    // Start is called before the first frame update
    void Start()
    {
        int num = gameObject.name[gameObject.name.Length - 2] - '0';
        doctorOffice = Managers.NPCManager.waypointDictionary[(ward, "OutpatientWaypoints")].Find("Doctor'sOffice (" + num + ")").GetComponent<DoctorOffice>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
