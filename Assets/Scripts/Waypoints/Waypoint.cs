﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public bool isEmpty = true;
    public int ward;
    public Vector3 rangeSize; // 웨이포인트 범위의 크기
    public Ward wardComponent;
    public Dictionary<int, (GameObject, bool)> chairsDictionary = new Dictionary<int, (GameObject, bool)>();

    private void Awake()
    {
        GameObject[] chairsInRange = FindChairs();
        int i = 0;
        foreach (GameObject chair in chairsInRange)
        {
            chairsDictionary.Add(i++, (chair, true));
        }
    }
    // 범위 내에서 랜덤 위치를 반환
    public Vector3 GetRandomPointInRange()
    {

        Vector3 randomPoint = new Vector3(
            Random.Range(-rangeSize.x / 2, rangeSize.x / 2),
            -rangeSize.y / 2,
            Random.Range(-rangeSize.z / 2, rangeSize.z / 2)
        );
        return transform.position + randomPoint;
    }

    public Vector3 GetMiddlePointInRange()
    {
        return new Vector3(transform.position.x, transform.position.y - (rangeSize.y / 2), transform.position.z);
    }

    private GameObject[] FindChairs()
    {
        // 현재 오브젝트의 위치
        Vector3 currentPosition = transform.position;

        // Physics.OverlapBox로 지정한 범위 내에 있는 모든 Collider를 가져옴
        Collider[] hitColliders = Physics.OverlapBox(currentPosition, rangeSize / 2, Quaternion.identity);

        // 충돌한 오브젝트들 중 "Chair" 태그를 가진 오브젝트만 필터링
        List<GameObject> chairsInRange = new List<GameObject>();

        foreach (Collider collider in hitColliders)
        {
            // 충돌한 오브젝트가 "Chair" 태그를 가진 경우 리스트에 추가
            if (collider.CompareTag("Chair"))
            {
                chairsInRange.Add(collider.gameObject);
            }
        }

        // 리스트를 배열로 반환
        return chairsInRange.ToArray();
    }

    // Gizmos를 사용하여 범위를 시각적으로 표시
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, rangeSize);
    }
}
