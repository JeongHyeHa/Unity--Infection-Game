using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DividerCollisionHandler : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        int colliderLayer = gameObject.layer;

        // Outpatient 태그를 가진 오브젝트와 충돌했는지 확인
        if (other.CompareTag("Outpatient") || other.CompareTag("Inpatient") || other.CompareTag("EmergencyPatient"))
        {
            if (!Managers.LayerChanger.layerMapping.TryGetValue(colliderLayer, out int[] layers))
            {
                Debug.LogWarning($"Layer {colliderLayer} not found in layer mapping.");
            }
            PatientController patientController = other.GetComponent<PatientController>();

            if (patientController != null)
            {
                if (patientController.isFollowingNurse || patientController.isExiting)
                {
                    if (patientController.isFollowingNurse && LayerMask.LayerToName(colliderLayer) == "Floor 1 R")
                    {
                        Managers.LayerChanger.SetLayerRecursively(other.gameObject, layers[0]);

                    }
                    else
                    {
                        Managers.LayerChanger.SetLayerRecursively(other.gameObject, layers[1]);

                    }
                }
                else
                {
                    if (patientController.personComponent.role == Role.Outpatient)
                    {
                        if (patientController.waypointIndex == 4)
                        {
                            Managers.LayerChanger.SetLayerRecursively(other.gameObject, layers[1]);
                        }
                        else
                        {
                            Managers.LayerChanger.SetLayerRecursively(other.gameObject, layers[0]);
                        }
                    }

                    else if (patientController.personComponent.role == Role.Inpatient)
                    {
                        Managers.LayerChanger.SetLayerRecursively(other.gameObject, layers[0]);
                    }

                    else if (patientController.personComponent.role == Role.EmergencyPatient)
                    {
                        Managers.LayerChanger.SetLayerRecursively(other.gameObject, layers[0]);
                    }
                }

            }
            else
            {
                Debug.LogWarning($"Colliding object {other.gameObject.name} does not have an OutpatientController component.");
            }
        }


        // Nurse 태그를 가진 오브젝트와 충돌했는지 확인
        if (other.CompareTag("Nurse"))
        {
            if (!Managers.LayerChanger.layerMapping.TryGetValue(colliderLayer, out int[] layers))
            {
                Debug.LogWarning($"Layer {colliderLayer} not found in layer mapping.");
            }
            NurseController Nurse = other.GetComponent<NurseController>();

            if (Nurse != null)
            {
             
                
                if (Nurse.isReturning)
                {
                    

                    if (LayerMask.LayerToName(colliderLayer) == "Floor 1 R")
                    {
                        Managers.LayerChanger.SetLayerRecursively(other.gameObject, layers[1]);

                    }
                    else
                    {
                        Managers.LayerChanger.SetLayerRecursively(other.gameObject, layers[0]);
                    }


                }
                else
                {
                    if (LayerMask.LayerToName(colliderLayer) == "Floor 1 R")
                    {
                        Managers.LayerChanger.SetLayerRecursively(other.gameObject, layers[0]);

                    }
                    else
                    {
                        Managers.LayerChanger.SetLayerRecursively(other.gameObject, layers[1]);
                    }
                }

            }
            else
            {
                Debug.LogWarning($"Colliding object {other.gameObject.name} does not have an NurseController component.");
            }
        }
    }
}
