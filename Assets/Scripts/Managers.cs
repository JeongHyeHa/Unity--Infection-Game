﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    public static Managers Instance { get; private set; } = new Managers();
    public static InfectionManager Infection { get; private set; } = new InfectionManager();
    public static ObjectPoolingManager ObjectPooling { get; private set; } = new ObjectPoolingManager();
    public static NPCMovementManager NPCManager { get; private set; } = new NPCMovementManager();
    public static StageManager Stage { get; private set; } = new StageManager();
    public static PatientCreator PatientCreator { get; private set; } = new PatientCreator();
    public static LayerChangeManager LayerChanger { get; private set; } = new LayerChangeManager();

    private void Awake()
    {
        Instance = this;
        LayerChanger = LayerChanger;
        ObjectPooling = ObjectPooling;
        NPCManager = NPCManager;
        Stage = Stage;
        Infection = Infection;

        LayerChanger.Init();
        NPCManager.Init();
        ObjectPooling.Init();
    }
    // Start is called before the first frame update
    void Start()
    {
        Infection.Init();
        PatientCreator.Init();
    }

    // Update is called once per frame
    void Update()
    {
        Infection.UpdateInfectionProbability();
        // 대기 중이 아니고, 환자 수가 최대치보다 적을 때 환자 생성
        if (!PatientCreator.outpatientWaiting && PatientCreator.numberOfOutpatient < ObjectPooling.maxOfOutpatient)
        {
            StartCoroutine(PatientCreator.SpawnOutpatient());
        }
        if(!PatientCreator.emergencyPatientWaiting && PatientCreator.numberOfEmergencyPatient < ObjectPooling.maxOfEmergencyPatient)
        {
            StartCoroutine(PatientCreator.SpawnEmergencyPatient());
        }
    }
}
