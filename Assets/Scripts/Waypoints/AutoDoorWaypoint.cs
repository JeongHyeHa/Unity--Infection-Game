using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDoorWaypoint : Waypoint
{
    public GameObject quarantineRoom; 
    // Start is called before the first frame update
    void Start()
    {
        quarantineRoom = FindClosestAutoDoor().transform.parent.gameObject;
    }

    private GameObject FindClosestAutoDoor()
    {
        GameObject[] AutoDoors = GameObject.FindGameObjectsWithTag("AutoDoor");
        GameObject closestAutoDoor = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (GameObject obj in AutoDoors)
        {
            float distance = Vector3.Distance(currentPosition, obj.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestAutoDoor = obj;
            }
        }
        return closestAutoDoor;
    }
}
