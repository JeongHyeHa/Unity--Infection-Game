using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoctorOffice : Waypoint
{
    public Queue<PatientController> waitingQueue = new Queue<PatientController>();
    public GameObject doctor;
    public GameObject chair;
    // Start is called before the first frame update
    private void Awake()
    {
        chair = FindClosestChair();
    }

    private GameObject FindClosestChair()
    {
        GameObject[] chairObjects = GameObject.FindGameObjectsWithTag("Chair");
        GameObject closestChair = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (GameObject obj in chairObjects)
        {
            float distance = Vector3.Distance(currentPosition, obj.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestChair = obj;
            }
        }
        return closestChair;
    }
    void Start()
    {
        wardComponent = transform.parent.parent.GetComponent<Ward>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isEmpty && waitingQueue.Count > 0)
        {
            PatientController next = waitingQueue.Peek();
            if (next.isWaitingForDoctor)
            {
                next = waitingQueue.Dequeue();
                if (next.isQuarantined || next.isWaitingForNurse || next.isFollowingNurse)
                {
                    return;
                }
                isEmpty = false;
                next.officeSignal = true;
            }
        }
        if (wardComponent.isClosed)
        {
            waitingQueue.Clear();
        }
    }
}
