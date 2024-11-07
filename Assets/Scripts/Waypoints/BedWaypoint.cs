using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedWaypoint : Waypoint
{
    public int bedNum;
    public GameObject patient;
    public GameObject bedGameObject;
    public Waypoint toilet;

    private void Awake()
    {
        bedGameObject = FindClosestBed();
        GameObject toiletGameObject = FindClosestToilet();
        if (toiletGameObject != null )
        {
            toilet = toiletGameObject.GetComponent<Waypoint>();
        }
        
        
    }

    private GameObject FindClosestBed()
    {
        GameObject[] bedObjects = GameObject.FindGameObjectsWithTag("Bed");
        GameObject closestBed = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (GameObject obj in bedObjects)
        {
            float distance = Vector3.Distance(currentPosition, obj.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestBed = obj;
            }
        }
        return closestBed;
    }

    private GameObject FindClosestToilet()
    {
        GameObject closestToilet = null;
        float closestDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        // 부모의 자식들을 순회
        foreach (Transform child in transform.parent)
        {
            // 자식 이름에 "Toilet"이 포함되어 있는지 확인
            if (child.name.Contains("Toilet"))
            {
                float yDifference = Mathf.Abs(child.position.y - currentPosition.y);
                if (yDifference <= 0.5f)
                {
                    float distance = Vector3.Distance(currentPosition, child.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestToilet = child.gameObject;
                    }
                }
            }
        }
        return closestToilet;
    }

    public Vector3 GetBedPoint() { return (bedGameObject.transform.position + transform.position) / 2.0f; }
}
