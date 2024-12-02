using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;



// 프로필 창 관리 및 표시 스크립트
public class ProfileWindow : MonoBehaviour
{
    public Button profileWindowButton; // ICU와 응급실 버튼
    public Button profileWindowButtonMinus; // 일반 병동에서 사용하는 버튼
    public Button profileWindowButtonPlus; // 옥
    public GameObject windowPanel; // 큰 창 패널
    public Image profileCloseButton;
    public GameObject profileOverlay;

    public Image doctorWindowButton; // 의사 이미지 버튼
    public Image nurseWindowButton; // 간호사 이미지 버튼
    public Image outpatientWindowButton; // 외래환자 이미지 버튼
    public Image inpatientWindowButton; // 입원환자 이미지 버튼
    public Image emergencypatientWindowButton; // 응급환자 이미지 버튼
    public Image icupatientWindowButton;

    public GameObject profilePrefab; // 프로필 프리팹
    public Transform profileContent; // 프로필을 추가할 스크롤 뷰의 콘텐츠 영역

    public ProfileInventoryManager profileInventoryManager;

    private string currentFloor = "응급실";


    public TextMeshProUGUI NowWard; // 현재 병동 위치
    public TextMeshProUGUI DoctorCountText;    // 의사 수
    public TextMeshProUGUI NurseCountText;     // 간호사 수
    public TextMeshProUGUI OutpatientCountText;// 외래 환자 수
    public TextMeshProUGUI InpatientCountText; // 입원 환자 수
    public TextMeshProUGUI EmergencypatientCountText; // 응급 환자 수
    public TextMeshProUGUI ICUpatientCountText; // 중환자 수 

    public GameObject doctorInfo;
    public GameObject nurseInfo;
    public GameObject outpatientInfo;
    public GameObject inpatientInfo;
    public GameObject emergencyInfo;
    public GameObject icupatientInfo;

    public Image changeImage;
    public Sprite emerIcon;
    public Sprite icuIcon;

    private string currentJob = "의사";

    private bool isFirstProfile = false;

    private void Awake()
    {
        profileCloseButton = Assign(profileCloseButton, "ProfileCloseButton");
        profileWindowButton = Assign(profileWindowButton, "ProfileWindowButton");
        profileWindowButtonMinus = Assign(profileWindowButtonMinus, "ProfileWindowButtonMinus");
        profileWindowButtonPlus = Assign(profileWindowButtonPlus, "ProfileWindowButtonPlus");
        windowPanel = Assign(windowPanel, "WindowPanel");
        profileOverlay = Assign(profileOverlay, "ProfileOverlay");
        doctorWindowButton = Assign(doctorWindowButton, "DoctorWindowButton");
        nurseWindowButton = Assign(nurseWindowButton, "NurseWindowButton");
        outpatientWindowButton = Assign(outpatientWindowButton, "OutPatientWindowButton");
        inpatientWindowButton = Assign(inpatientWindowButton, "InPatientWindowButton");
        emergencypatientWindowButton = Assign(emergencypatientWindowButton, "EmergencyPatientWindowButton");
        icupatientWindowButton = Assign(icupatientWindowButton, "ICUPatientWindowButton");
        profilePrefab = Assign(profilePrefab, "ProfilePrefab");
        profileContent = Assign(profileContent, "ProfileContent");
        profileInventoryManager = Assign(profileInventoryManager, "Inventory");

        NowWard = Assign(NowWard, "NowWard");
        DoctorCountText = Assign(DoctorCountText, "DoctorCountText");
        NurseCountText = Assign(NurseCountText, "NurseCountText");
        OutpatientCountText = Assign(OutpatientCountText, "OutpatientCountText");
        InpatientCountText = Assign(InpatientCountText, "InpatientCountText");
        EmergencypatientCountText = Assign(EmergencypatientCountText, "EmergencypatientCountText");
        ICUpatientCountText = Assign(ICUpatientCountText, "ICUpatientCountText");

        doctorInfo = Assign(doctorInfo, "ProfileButtonInfoDoctor");
        nurseInfo = Assign(nurseInfo, "ProfileButtonInfoNurse");
        outpatientInfo = Assign(outpatientInfo, "ProfileButtonInfoOutpatient");
        inpatientInfo = Assign(inpatientInfo, "ProfileButtonInfoInpatient");
        emergencyInfo = Assign(emergencyInfo, "ProfileButtonInfoEmerpatient");
        icupatientInfo = Assign(icupatientInfo, "ProfileButtonInfoICUpatient");
        changeImage = Assign(changeImage, "ChangeImage");


        // 버튼 클릭 이벤트에 메서드 추가
        profileWindowButton.onClick.AddListener(() => { ToggleBigPanel(); BtnSoundManager.Instance.PlayButtonSound(); OneClearManager.Instance.CloseDisinfectionMode(); });
        profileWindowButtonMinus.onClick.AddListener(() => { ToggleBigPanel(); BtnSoundManager.Instance.PlayButtonSound(); });
        profileWindowButtonPlus.onClick.AddListener(() => { ToggleBigPanel(); BtnSoundManager.Instance.PlayButtonSound(); });

        // 각 이미지를 버튼으로 설정
        SetupButton(doctorWindowButton, OnDoctorClick);
        SetupButton(nurseWindowButton, OnNurseClick);
        SetupButton(outpatientWindowButton, OnOutpatientClick);
        SetupButton(inpatientWindowButton, OnInpatientClick);
        SetupButton(emergencypatientWindowButton, OnEmerpatientClick);
        SetupButton(icupatientWindowButton, OnICUpatientClick);
        SetupButton(profileCloseButton, OnProfileClosePanelClick);


        // GridLayoutGroup 설정
        GridLayoutGroup gridLayoutGroup = profileContent.GetComponent<GridLayoutGroup>();
        if (gridLayoutGroup != null)
        {
            // 프로필 양식 바뀌면 이것도 함께 수정할 것
            gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayoutGroup.constraintCount = 3; // 한 줄에 들어가는 프로필 개수
            gridLayoutGroup.cellSize = new Vector2(165, 90); // 프로필의 크기 (필요에 따라 조정)
            gridLayoutGroup.spacing = new Vector2(10, 10); // 프로필 간의 간격
            gridLayoutGroup.padding = new RectOffset(10, 10, 10, 10); // 스크롤 뷰와의 간격
        }
    }


    public void AddDoctorProfile(GameObject doctorObject)
    {
        AddPersonProfile(doctorObject, "의사", Role.Doctor, PersonManager.Instance.GetPersonCountByJob("의사") + 1);
        if (currentJob == "의사") RefreshProfiles();
    }

    public void AddNurseProfile(GameObject nurseObject)
    {
        AddPersonProfile(nurseObject, "간호사", Role.Nurse, PersonManager.Instance.GetPersonCountByJob("간호사") + 1);
        if (currentJob == "간호사") RefreshProfiles();
    }

    public void AddOutpatientProfile(GameObject outpatientObject)
    {
        AddPersonProfile(outpatientObject, "외래 환자", Role.Outpatient, PersonManager.Instance.GetPersonCountByJob("외래 환자") + 1);
        if (currentJob == "외래 환자") RefreshProfiles();
    }

    public void AddInpatientProfile(GameObject inpatientObject)
    {
        AddPersonProfile(inpatientObject, "입원 환자", Role.Inpatient, PersonManager.Instance.GetPersonCountByJob("입원 환자") + 1);
        if (currentJob == "입원 환자") RefreshProfiles();
    }

    public void AddEmerpatientProfile(GameObject emergencyPatientObject)
    {
        AddPersonProfile(emergencyPatientObject, "응급 환자", Role.EmergencyPatient, PersonManager.Instance.GetPersonCountByJob("응급 환자") + 1);
        if (currentJob == "응급 환자") RefreshProfiles();
    }
    public void AddICUPateintProfile(GameObject ICUPatientObject)
    {
        AddPersonProfile(ICUPatientObject, "중환자", Role.ICUPatient, PersonManager.Instance.GetPersonCountByJob("중환자") + 1);
        if (currentJob == "중환자") RefreshProfiles();
    }


    private void AddPersonProfile(GameObject personObject, string job, Role role, int index)
    {
        List<Item> inventory = new List<Item>();

        // 외래 환자와 입원 환자는 N95 마스크와 Dental 마스크만 인벤토리에 추가
        if (role == Role.Doctor || role == Role.Nurse)
        {
            inventory = Managers.Item.items; // 모든 아이템을 인벤토리에 추가
        }
        if (role == Role.Outpatient || role == Role.Inpatient || role == Role.EmergencyPatient || role == Role.ICUPatient)
        {
            inventory.Add(Managers.Item.items[0]);
            inventory.Add(Managers.Item.items[1]);
        }
        int personID = PersonManager.Instance.GeneratePersonID();
        Person person = personObject.GetComponent<Person>();
        if (person == null)
        {
            person = personObject.AddComponent<Person>();
        }

        person.Initialize(personID, $"{job} {index}", job, false, role);
        person.AssignName();
        PersonManager.Instance.AddPerson(person);


        GameObject profile = Instantiate(profilePrefab, profileContent);
        RectTransform rectTransform = profile.GetComponent<RectTransform>();
        rectTransform.localScale = Vector3.one;
        rectTransform.anchoredPosition3D = Vector3.zero;
        rectTransform.localPosition = Vector3.zero;
        rectTransform.localRotation = Quaternion.identity;

        if (currentJob == "의사")
        {
            UpdateProfile(profile, $"{job} {index}", person.Name, person.ID, person.gameObject.activeSelf);
            RefreshProfiles();
        }
        if (currentJob == "간호사")
        {
            UpdateProfile(profile, $"{job} {index}", person.Name, person.ID, person.gameObject.activeSelf);
            RefreshProfiles();
        }
        if (currentJob == "외래 환자")
        {
            UpdateProfile(profile, $"{job} {index}", person.Name, person.ID, person.gameObject.activeSelf);
            RefreshProfiles();
        }
        if (currentJob == "입원 환자")
        {
            UpdateProfile(profile, $"{job} {index}", person.Name, person.ID, person.gameObject.activeSelf);
            RefreshProfiles();
        }

        if (currentJob == "응급 환자")
        {
            UpdateProfile(profile, $"{job} {index}", person.Name, person.ID, person.gameObject.activeSelf);
            RefreshProfiles();
        }

        if (currentJob == "중환자")
        {
            UpdateProfile(profile, $"{job} {index}", person.Name, person.ID, person.gameObject.activeSelf);
            RefreshProfiles();
        }
        UpdateButtonTexts(NowWard.text);
    }

    private void ToggleBigPanel()
    {
        // 다른 UI 창이 열려 있지 않으면 창을 열 수 있음
        if (!windowPanel.activeSelf && IsAbleManager.Instance.CanOpenNewWindow())
        {
            // 창을 열기
            windowPanel.SetActive(true);
            profileOverlay.SetActive(true);
            IsAbleManager.Instance.OpenWindow(windowPanel);
            ClearProfilesList(); // 창을 열 때 기존 프로필을 지웁니다.
            ShowProfiles(currentJob); // 현재 선택된 직업에 따라 프로필을 다시 표시합니다.
        }
        else if (windowPanel.activeSelf) // 이미 창이 열려 있는 경우
        {
            // 창을 닫기
            windowPanel.SetActive(false);
            profileOverlay.SetActive(false);
            ClearProfilesList();
            profileInventoryManager.ClearInventory();
            IsAbleManager.Instance.CloseWindow(windowPanel);
        }
    }


    private void SetupButton(Image image, System.Action onClick)
    {
        if (image == null)
        {
            Debug.LogError("Button image is not assigned.");
            return;
        }

        Button button = image.GetComponent<Button>();
        if (button == null)
        {
            button = image.gameObject.AddComponent<Button>();
        }
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => {onClick(); BtnSoundManager.Instance.PlayButtonSound(); });
    }

    private void OnProfileClosePanelClick()
    {
        windowPanel.SetActive(false);
        profileOverlay.SetActive(false);
        ClearProfilesList();
        profileInventoryManager.ClearInventory();
        IsAbleManager.Instance.CloseWindow(windowPanel);
        Time.timeScale = 1.0f;
    }

    private void OnDoctorClick()
    {
        currentJob = "의사";
        profileInventoryManager.ClearInventory();
        ShowProfiles(currentJob);
    }

    private void OnNurseClick()
    {
        currentJob = "간호사";
        profileInventoryManager.ClearInventory();
        ShowProfiles(currentJob);
    }

    private void OnOutpatientClick()
    {
        currentJob = "외래 환자";
        profileInventoryManager.ClearInventory();
        ShowProfiles(currentJob);
    }

    private void OnInpatientClick()
    {
        currentJob = "입원 환자";
        profileInventoryManager.ClearInventory();
        ShowProfiles(currentJob);
    }

    private void OnEmerpatientClick()
    {
        currentJob = "응급 환자";
        profileInventoryManager.ClearInventory();
        ShowProfiles(currentJob);
    }

    private void OnICUpatientClick()
    {
        currentJob = "중환자";
        profileInventoryManager.ClearInventory();
        ShowProfiles(currentJob);
    }

    private void ShowProfiles(string job)
    {
        currentJob = job;
        ClearProfilesList();

        List<Person> persons = PersonManager.Instance.GetAllPersonsByJob(job);
        int index = 1;
        foreach (Person person in persons)
        {
            GameObject profile = Instantiate(profilePrefab, profileContent);
            RectTransform rectTransform = profile.GetComponent<RectTransform>();
            rectTransform.localScale = Vector3.one;
            rectTransform.anchoredPosition3D = Vector3.zero;
            rectTransform.localPosition = Vector3.zero;
            rectTransform.localRotation = Quaternion.identity;

            UpdateProfile(profile, $"{job} {index++}", person.Name, person.ID, person.gameObject.activeSelf);
        }
    }

    private void UpdateProfile(GameObject profile, string job, string name, int id, bool isActive)
    {
        TextMeshProUGUI numberText = profile.transform.Find("NumberText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI jobText = profile.transform.Find("JobText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI nameText = profile.transform.Find("NameText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI restingText = profile.transform.Find("RestingText").GetComponent<TextMeshProUGUI>();
        Image restingImage = profile.transform.Find("RestingImage").GetComponent<Image>();

        Person person = PersonManager.Instance.GetPerson(id);


        if (jobText != null && numberText != null && job != null)
        {
            string[] jobParts = job.Split(' ');

            if (jobParts.Length == 2)
            {
                jobText.text = jobParts[0];
                numberText.text = $"No.{jobParts[1]}";
            }
            else if (jobParts.Length == 3)
            {
                jobText.text = $"{jobParts[0]} {jobParts[1]}";
                numberText.text = $"No.{jobParts[2]}";
            }
        }

        if (nameText != null) nameText.text = name;

        // 기존의 RestingText 설정은 주석 처리
        // if (restingText != null)
        // {
        //     if (person.role == NurseRole.Doctor || person.role == NurseRole.Nurse)
        //     {
        //         restingText.text = person.IsResting ? "휴식 중" : "근무 중";
        //         restingText.color = person.IsResting ? Color.green : Color.red;
        //     }
        //     else
        //     {
        //         restingText.text = ""; // 다른 역할은 표시하지 않음
        //     }
        // }

        //// 새로운 RestingText와 RestingImage 활성화 상태 설정
        if (restingText != null)
        {
            restingText.gameObject.SetActive(person.IsResting);
        }
        else Debug.Log("레스팅텍스트 없어요");

        if (restingImage != null)
        {
            restingImage.gameObject.SetActive(person.IsResting);
        }
        else Debug.Log("레스팅텍스트 없어요");

        // Avatar 스프라이트 설정
        Image avatarImage = profile.transform.Find("Avatar").GetComponent<Image>();
        if (avatarImage != null && person.AvatarSprite != null)
        {
            avatarImage.sprite = person.AvatarSprite;
        }
        else
        {
            Debug.LogError("Avatar Image component not found in the ProfilePrefab or AvatarSprite is null.");
        }

        ProfileClickHandler clickHandler = profile.GetComponent<ProfileClickHandler>();
        if (clickHandler == null)
        {
            clickHandler = profile.AddComponent<ProfileClickHandler>();
        }
        clickHandler.Initialize(id, profileInventoryManager);
    }


    private void ClearProfilesList()
    {
        foreach (Transform child in profileContent)
        {
            Destroy(child.gameObject);
        }
    }

    public void UpdateButtonTexts(string floorName)
    {
        // floorName이 null이 아니면 현재 층수 업데이트
        if (!string.IsNullOrEmpty(floorName))
        {
            currentFloor = floorName; // 현재 층 정보를 갱신
        }

        // 현재 선택된 병동 정보를 가져옴
        Ward nowWard = Ward.wards.Find(w => w.WardName == floorName);



        if (currentFloor == "옥상")
        {
            // 옥상일 경우 모든 직업의 인원 수를 표시
            profileWindowButtonPlus.gameObject.SetActive(true);
            profileWindowButton.gameObject.SetActive(false);
            profileWindowButtonMinus.gameObject.SetActive(false);
            emergencyInfo.SetActive(true);
            icupatientInfo.SetActive(true);
            changeImage.sprite = emerIcon;
            NowWard.text = "옥상";
            DoctorCountText.text = $"{PersonManager.Instance.GetPersonCountByJob("의사")}";
            NurseCountText.text = $"{PersonManager.Instance.GetPersonCountByJob("간호사")}";
            OutpatientCountText.text = $"{PersonManager.Instance.GetPersonCountByJob("외래 환자")}";
            InpatientCountText.text = $"{PersonManager.Instance.GetPersonCountByJob("입원 환자")}";
            EmergencypatientCountText.text = $"{PersonManager.Instance.GetPersonCountByJob("응급 환자")}";
            ICUpatientCountText.text = $"{PersonManager.Instance.GetPersonCountByJob("중환자")}";
        }
        else if (currentFloor == "응급실")
        {
            if (nowWard != null)
            {
                // 응급실일 경우 응급 환자 표시
                profileWindowButtonPlus.gameObject.SetActive(false);
                profileWindowButton.gameObject.SetActive(true);
                profileWindowButtonMinus.gameObject.SetActive(false);
                emergencyInfo.SetActive(true);
                icupatientInfo.SetActive(false);
                changeImage.sprite = emerIcon;
                NowWard.text = $"{nowWard.WardName}";
                DoctorCountText.text = $"{nowWard.doctorCount}";
                NurseCountText.text = $"{nowWard.nurseCount}";
                OutpatientCountText.text = $"{nowWard.outpatientCount}";
                InpatientCountText.text = $"{nowWard.inpatientCount}";
                EmergencypatientCountText.text = $"{nowWard.emergencypatientCount}";
            }
            else
            {
                Debug.LogWarning("해당하는 병동을 찾을 수 없습니다: " + floorName);
            }

        }
        else if (currentFloor == "중환자실/격리실")
        {
            if (nowWard != null)
            {
                // 중환자실일 경우 중환자 표시
                profileWindowButtonPlus.gameObject.SetActive(false);
                profileWindowButton.gameObject.SetActive(true);
                profileWindowButtonMinus.gameObject.SetActive(false);
                emergencyInfo.SetActive(true);
                icupatientInfo.SetActive(false);
                changeImage.sprite = icuIcon;
                NowWard.text = $"중환자실/격리실";
                DoctorCountText.text = $"{nowWard.doctorCount}";
                NurseCountText.text = $"{nowWard.nurseCount}";
                OutpatientCountText.text = $"{nowWard.outpatientCount}";
                InpatientCountText.text = $"{nowWard.inpatientCount}";
                EmergencypatientCountText.text = $"{nowWard.icupatientCount}";
            }
            else
            {
                Debug.LogWarning("해당하는 병동을 찾을 수 없습니다: " + floorName);
            }
        }
        else if (currentFloor.StartsWith("입원병동"))
        {
            WardState state;
            switch (currentFloor)
            {
                case "입원병동 1":
                    state = PolicyWard.Instance.wardStates[4];
                    break;
                case "입원병동 2":
                    state = PolicyWard.Instance.wardStates[5];
                    break;
                case "입원병동 3":
                    state = PolicyWard.Instance.wardStates[6];
                    break;
                case "입원병동 4":
                    state = PolicyWard.Instance.wardStates[7];
                    break;
                default:
                    state = PolicyWard.Instance.wardStates[0];
                    break;
            }
            if (state.IsClosed)
            {
                if (nowWard != null)
                {
                    profileWindowButtonPlus.gameObject.SetActive(false);
                    profileWindowButton.gameObject.SetActive(false);
                    profileWindowButtonMinus.gameObject.SetActive(true);
                    emergencyInfo.SetActive(false);
                    icupatientInfo.SetActive(false);
                    NowWard.text = $"{nowWard.WardName}";
                    DoctorCountText.text = $"0";
                    NurseCountText.text = $"0";
                    OutpatientCountText.text = $"0";
                    InpatientCountText.text = $"0";
                }
                else
                {
                    Debug.LogWarning("해당하는 병동을 찾을 수 없습니다: " + floorName);
                }
            }
            else
            {
                if (nowWard != null)
                {
                    profileWindowButtonPlus.gameObject.SetActive(false);
                    profileWindowButton.gameObject.SetActive(false);
                    profileWindowButtonMinus.gameObject.SetActive(true);
                    emergencyInfo.SetActive(false);
                    icupatientInfo.SetActive(false);
                    NowWard.text = $"{nowWard.WardName}";
                    DoctorCountText.text = $"{nowWard.doctorCount}";
                    NurseCountText.text = $"{nowWard.nurseCount}";
                    OutpatientCountText.text = $"{nowWard.outpatientCount}";
                    InpatientCountText.text = $"{nowWard.inpatientCount}";
                }
                else
                {
                    Debug.LogWarning("해당하는 병동을 찾을 수 없습니다: " + floorName);
                }
            }
        }
        else
        {
            // 그 외 병동일 경우 직업별 인원 수를 업데이트
            if (nowWard != null)
            {
                profileWindowButtonPlus.gameObject.SetActive(false);
                profileWindowButton.gameObject.SetActive(false);
                profileWindowButtonMinus.gameObject.SetActive(true);
                emergencyInfo.SetActive(false);
                icupatientInfo.SetActive(false);
                NowWard.text = $"{nowWard.WardName}";
                DoctorCountText.text = $"{nowWard.doctorCount}";
                NurseCountText.text = $"{nowWard.nurseCount}";
                OutpatientCountText.text = $"{nowWard.outpatientCount}";
                InpatientCountText.text = $"{nowWard.inpatientCount}";
            }
            else
            {
                Debug.LogWarning("해당하는 병동을 찾을 수 없습니다: " + floorName);
            }
        }
    }



    public void RefreshProfiles()
    {
        ShowProfiles(currentJob);
    }

    public void ShowInventoryForPerson(int personID)
    {
        Person person = PersonManager.Instance.GetPerson(personID);
        if (person != null)
        {
            profileInventoryManager.ShowInventory(personID);
        }
        else
        {
            Debug.LogError("Person not found for ID: " + personID);
        }
    }

    public void RemoveProfile(int personID)
    {
        Person person = PersonManager.Instance.GetPerson(personID);
        if (person == null)
        {
            //Debug.LogError($"Person with ID {personID} not found.");
            return;
        }

        // Find the profile GameObject in the profileContent
        Transform profileToRemove = profileContent.Cast<Transform>().FirstOrDefault(t => t.GetComponent<ProfileClickHandler>().personID == personID);
        if (profileToRemove != null)
        {
            Destroy(profileToRemove.gameObject);
            Debug.Log($"Removed profile for person: {person.Name} with ID: {person.ID}");
        }
        // Remove the person from PersonManager
        PersonManager.Instance.RemovePerson(personID);

        // Update button texts to reflect changes
        UpdateButtonTexts(NowWard.text);
    }

    // 자동 할당 코드
    private T Assign<T>(T obj, string objectName) where T : Object
    {
        if (obj == null)
        {
            GameObject foundObject = GameObject.Find(objectName);
            if (foundObject != null)
            {
                if (typeof(Component).IsAssignableFrom(typeof(T))) obj = foundObject.GetComponent(typeof(T)) as T;
                else if (typeof(GameObject).IsAssignableFrom(typeof(T))) obj = foundObject as T;
            }
            if (obj == null) Debug.LogError($"{objectName} 를 찾을 수 없습니다.");
        }
        return obj;
    }
}
