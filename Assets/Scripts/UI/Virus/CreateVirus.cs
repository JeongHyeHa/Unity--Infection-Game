using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateVirus : MonoBehaviour
{
    public GameObject virusPrefab;

    void Start()
    {
        // VirusPrefab 자동 할당
        if (virusPrefab == null)
        {
            virusPrefab = Resources.Load<GameObject>("Viruse_1");
            if (virusPrefab == null)
            {
                Debug.LogError("Viruse_1 프리팹을 Resources 폴더에서 찾을 수 없습니다.");
            }
        }
    }

    //주어진 위치, 상태로 바이러스 생성
    public GameObject CreateVirusObject(Vector3 position, Person person)
    {
        GameObject virusObject = Object.Instantiate(virusPrefab, position, Quaternion.identity);
        virusObject.layer = person.gameObject.layer;
        Virus virus = virusObject.GetComponent<Virus>(); //바이러스 프리팹을 인스턴스화
        if (virus != null)
        {
            virus.SetLifetime(Virus.virusLifetime); //바이러스 컴포넌트의 수명, 감염상태 설정
            virus.SetInfectionState(person.status);
        }
        return virusObject;  //바이러스 오브젝트 반환
    }
}
