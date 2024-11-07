using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public enum StandingState
{
    Standing,
    Sitting,
    LayingDown
}

public class NPCController : MonoBehaviour
{
    public Animator animator;
    public StandingState standingState = StandingState.Standing;
    public NavMeshAgent agent;
    public List<Waypoint> waypoints = new List<Waypoint>();
    public Ward wardComponent;
    public int ward;

    public bool isWaiting = false;

    public Person personComponent;
    public InfectionController infectionController;

    public int num;

    private void Awake()
    {
        // 컴포넌트 초기화
        animator = GetComponent<Animator>();
        infectionController = GetComponent<InfectionController>();
        agent = GetComponent<NavMeshAgent>();
        agent.avoidancePriority = Random.Range(0, 100);
        agent.speed = Random.Range(3.0f, 5.0f);
        personComponent = GetComponent<Person>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }
}
