using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class NPCMovementManager
{
    public Dictionary<(int, string), Transform> waypointDictionary = new Dictionary<(int, string), Transform>();
    public Transform gatewayTransform;
    public Transform passPointTransform;

    public void Init()
    {
        gatewayTransform = GameObject.Find("Waypoints/Gateways").transform;
        passPointTransform = GameObject.Find("Waypoints/Discharge PassPoints").transform;
        Transform wardTransform;


        for (int i = 0; i < 10; i++)
        {
            wardTransform = GameObject.Find("Waypoints/Ward (" + i + ")").transform;
            Transform waypointsGameObject;
            if (i < 4)
            {
                waypointsGameObject = wardTransform.Find("OutpatientWaypoints");
                waypointDictionary.Add((i, "OutpatientWaypoints"), waypointsGameObject);

                waypointsGameObject = wardTransform.Find("DoctorWaypoints");
                waypointDictionary.Add((i, "DoctorWaypoints"), waypointsGameObject);
            }
            else if (i >= 4 && i <= 7)
            {
                waypointsGameObject = wardTransform.Find("InpatientWaypoints");
                waypointDictionary.Add((i, "InpatientWaypoints"), waypointsGameObject);
            }
            else if (i == 8)
            {
                waypointsGameObject = wardTransform.Find("EmergencyPatientWaypoints").transform;
                waypointDictionary.Add((i, "EmergencyPatientWaypoints"), waypointsGameObject);
            }
            else if(i == 9)
            {
                waypointsGameObject = wardTransform.Find("DoctorWaypoints");
                waypointDictionary.Add((i, "DoctorWaypoints"), waypointsGameObject);
            }
            waypointsGameObject = wardTransform.Find("NurseWaypoints");
            waypointDictionary.Add((i, "NurseWaypoints"), waypointsGameObject);
        }

    }
    public void FaceEachOther(GameObject obj1, GameObject obj2)
    {
        obj1.transform.LookAt(obj2.transform.position); // obj1이 obj2를 바라보게 설정
        obj2.transform.LookAt(obj1.transform.position); // obj2가 obj1을 바라보게 설정
    }

    public void UpdateAnimation(NavMeshAgent agent, Animator animator)
    {
        // 애니메이션
        if (!agent.isOnNavMesh)
        {
            if (animator.GetFloat("MoveSpeed") != 0)
                animator.SetFloat("MoveSpeed", 0);
            if (animator.GetBool("Grounded"))
                animator.SetBool("Grounded", false);
            return;
        }

        if (agent.remainingDistance > agent.stoppingDistance)
        {
            if (animator.GetFloat("MoveSpeed") != agent.velocity.magnitude / agent.speed)
                animator.SetFloat("MoveSpeed", agent.velocity.magnitude / agent.speed);
        }
        else
        {
            if (animator.GetFloat("MoveSpeed") != 0)
            {
                animator.SetFloat("MoveSpeed", 0);
            }

        }

        if (animator.GetBool("Grounded") != (!agent.isOnOffMeshLink && agent.isOnNavMesh))
            animator.SetBool("Grounded", !agent.isOnOffMeshLink && agent.isOnNavMesh);

    }

    // 앉기
    public void PlaySittingAnimation(NPCController nPCController)
    {
        nPCController.animator.SetBool("Sitting", true);
        nPCController.animator.SetBool("Sleeping", false);
        nPCController.animator.SetBool("Talking", false);
        nPCController.standingState = StandingState.Sitting;
    }

    // 눕기
    public void PlayLayDownAnimation(NPCController nPCController)
    {
        nPCController.animator.SetBool("Sitting", false);
        nPCController.animator.SetBool("Sleeping", true);
        nPCController.animator.SetBool("Talking", false);
        nPCController.standingState = StandingState.LayingDown;
    }


    // 누운 상태에서 앉고 대화하기
    public void WakeUpAndSittingAndTalking(NPCController nPCController)
    {
        nPCController.animator.SetBool("Sitting", true);
        nPCController.animator.SetBool("Sleeping", false);
        nPCController.animator.SetBool("Talking", true);
        nPCController.standingState = StandingState.Sitting;
    }

    // 누운 상태에서 일어나기
    public void PlayWakeUpAnimation(NPCController nPCController)
    {
        nPCController.animator.SetBool("Sitting", false);
        nPCController.animator.SetBool("Sleeping", false);
        nPCController.animator.SetBool("Talking", false);
        nPCController.standingState = StandingState.Standing;
    }


    public Vector3 GetPositionInFront(Transform thisTransform, Transform targetTransform, float distance)
    {
        // 대상 오브젝트와 현재 오브젝트 사이의 방향 벡터를 구함
        Vector3 direction = -(targetTransform.position - thisTransform.position).normalized;

        // 대상 오브젝트의 위치로부터 그 방향으로 일정 거리만큼 떨어진 위치 계산
        Vector3 destination = targetTransform.position + (direction * distance);

        // 네비게이션 메시 상의 위치 샘플링
        NavMeshHit navHit;
        NavMesh.SamplePosition(destination, out navHit, distance, NavMesh.AllAreas);

        // 샘플링된 위치 반환
        return navHit.position;
    }
    public bool isArrived(NavMeshAgent agent)
    {
        // Check if the agent is on a path and is still calculating the path
        if (agent.pathPending)
            return false;

        // Check if the path status is invalid or partial (unable to reach destination)
        if (agent.pathStatus == NavMeshPathStatus.PathInvalid || agent.pathStatus == NavMeshPathStatus.PathPartial)
            return false;

        // Check if the agent has a path and the remaining distance is greater than the stopping distance plus a small threshold
        if (agent.hasPath && agent.remainingDistance > agent.stoppingDistance + 0.2f)
            return false;

        // Check if the agent has no path and is not moving (velocity is very low)
        if (!agent.hasPath || agent.velocity.sqrMagnitude < 0.01f)
            return true;

        // Default to not arrived
        return false;
    }

}
