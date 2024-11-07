using System.Collections.Generic;
using UnityEngine;

// 캐릭터 속성 관리 매니저
public class PersonManager : MonoBehaviour
{
    public static PersonManager Instance { get; private set; }

    private List<Person> persons = new List<Person>();
    private int nextPersonID = 1; // 고유한 ID 생성용 변수

    private Dictionary<string, int> jobCounters = new Dictionary<string, int>
    {
        { "의사", 0 },
        { "간호사", 0 },
        { "외래 환자", 0 },
        { "입원 환자", 0 },
        { "응급 환자", 0 }
    };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Person 객체 추가
    public void AddPerson(Person person)
    {
        persons.Add(person);
    }

    // 모든 Person 객체 반환
    public List<Person> GetAllPersons()
    {
        return new List<Person>(persons);
    }

    // ID로 Person 객체 검색
    public Person GetPerson(int id)
    {
        foreach (Person person in persons)
        {
            if (person.ID == id)
            {
                return person;
            }
        }
        Debug.Log($"Person with ID {id} not found.");
        return null;
    }

    // 이름으로 Person 객체 검색
    public Person GetPersonByName(string name)
    {
        foreach (Person person in persons)
        {
            if (person.Name == name)
            {
                return person;
            }
        }
        Debug.Log($"Person {name} not found.");
        return null;
    }

    // 직업으로 Person 객체 검색
    public List<Person> GetAllPersonsByJob(string job)
    {
        List<Person> jobPersons = new List<Person>();
        foreach (Person person in persons)
        {
            if (person.Job == job)
            {
                jobPersons.Add(person);
            }
        }
        return jobPersons;
    }

    // 직업별 캐릭터 수 반환
    public int GetPersonCountByJob(string job)
    {
        return GetAllPersonsByJob(job).Count;
    }

    // 새로운 고유 ID 생성
    public int GeneratePersonID()
    {
        return nextPersonID++;
    }

    // 다음 직업 인덱스 생성
    public int GetNextPersonIndex(string job)
    {
        if (jobCounters.ContainsKey(job))
        {
            jobCounters[job]++;
            return jobCounters[job];
        }
        else
        {
            Debug.LogError($"Unknown job type: {job}");
            return -1;
        }
    }

    // Person 객체 업데이트
    public void UpdatePerson(Person person)
    {
        for (int i = 0; i < persons.Count; i++)
        {
            if (persons[i].ID == person.ID)
            {
                persons[i] = person;
                return;
            }
        }
        Debug.Log($"Person with ID {person.ID} not found for update.");
    }

    // Person 객체 삭제
    public void RemovePerson(int id)
    {
        Person personToRemove = GetPerson(id);
        if (personToRemove != null)
        {
            persons.Remove(personToRemove);
        }
        else
        {
            Debug.Log($"Person with ID {id} not found for removal.");
        }
    }
}
