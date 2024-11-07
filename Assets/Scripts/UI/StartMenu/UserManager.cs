using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using MyApp.DataAccess;
using System;
using System.Collections;
using UnityEditor.PackageManager.Requests;
using UnityEngine.Networking;
using JetBrains.Annotations;

namespace MyApp.UserManagement
{
    [System.Serializable]
    public class User
    {
        public int userId;
        public string userName;
        public string userPassword;
        public int tutorial;
        public string easyCompleted;  
        public string normalCompleted;  
        public string hardCompleted;  
    }

    public class UserManager : MonoBehaviour
    {
        public static UserManager Instance { get; private set; }

        private string urlGetLogin = "http://220.69.209.164:3333/get_login"; // Flask 서버 IP

        private List<User> users;
        private List<string> steps;

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

        public void Start()
        {
            users = new List<User>();
            StartCoroutine(GetUsersData());
        }

        // Flask API에서 사용자 데이터 가져오기
        IEnumerator GetUsersData()
        {
            UnityWebRequest www = UnityWebRequest.Get(urlGetLogin);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error fetching user data: " + www.error);
            }
            else
            {
                string jsonResponse = www.downloadHandler.text;
                Debug.Log("User Login JSON Response: " + jsonResponse);

                try
                {
                    List<User> fetchedUsers = MyApp.Utilities.JsonHelper.FromJson<User>(jsonResponse);
                    if (fetchedUsers != null)
                    {
                        users = fetchedUsers;
                        Debug.Log($"Successfully fetched {users.Count} users.");
                    }
                    else
                    {
                        Debug.LogError("Failed to parse user data, response was: " + jsonResponse);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("Error parsing JSON: " + e.Message + ". JSON Response: " + jsonResponse);
                }
            }
        }

        // 사용자 클리어 단계 가져오기
        public List<string> GetUserStep(string id)
        {
            foreach (var user in users)
            {
                if (user.userId.ToString() == id)
                {
                    steps[0] = user.easyCompleted;
                    steps[1] = user.normalCompleted;
                    steps[2] = user.hardCompleted;
                    // string userstep = $"{user.easyCompleted}{user.normalCompleted}{user.hardCompleted}";  //전부 클리어한 경우: 111
                    return steps;
                }
            }
            return null;
        }

        // 로그인 정보 확인 (둘다 일치 시 true)
        public bool ValidateUser(string id, string passwd)
        {
            foreach (var user in users)
            {
                if (user.userId.ToString() == id && user.userPassword == passwd)
                {
                    return true;
                }
            }
            Debug.LogError("로그인DB에서 사용자 정보 찾기 실패");
            return false;
        }

        // 로그인 ID로 사용자 이름 가져오기
        public string GetNameById(string id)
        {
            foreach (var user in users)
            {
                if (user.userId.ToString() == id)
                {
                    return user.userName;
                }
            }
            Debug.LogError("로그인DB에서 Name을 찾지 못했습니다.");
            return null;
        }

        // 유저의 튜토리얼 진행 여부 확인
        public int GetUserTutorialStatus(string id)
        {
            foreach (var user in users)
            {
                if (user.userId.ToString() == id)
                {
                    return user.tutorial;
                }
            }
            return -1;  // 유저를 찾지 못했을 때
        }

        // 사원 번호 유효성 검사 (숫자만 허용)
        public bool IsValidID(string id)
        {
            return Regex.IsMatch(id, @"^[0-9]+$");
        }

        // 사용자 이름 유효성 검사 (문자만 허용)
        public bool IsValidUsername(string username)
        {
            return Regex.IsMatch(username, @"^[a-zA-Z가-힣]+$");
        }

        // ID 존재 유무
        public bool IsIDExists(string id)
        {
            foreach (var user in users)
            {
                if (user.userId.ToString() == id)
                {
                    return true;
                }
            }
            return false;
        }

        // 이름 존재 유무
        public bool IsUsernameExists(string username)
        {
            foreach (var user in users)
            {
                if (user.userName == username)
                {
                    return true;
                }
            }
            return false;
        }

        // 회원가입을 통해 로그인DB에 회원 정보 추가
        // 튜토리얼 진행 여부 업데이트
        public void AddUser(string id, string username, string password, int istutorial, string eazyMode, string normalMode, string hardMode)
        {
            WWWForm form = new WWWForm();
            form.AddField("userId", id);
            form.AddField("userName", username);
            form.AddField("userPassword", password);
            form.AddField("tutorial", istutorial);
            form.AddField("easyCompleted", eazyMode);
            form.AddField("normalCompleted", normalMode);
            form.AddField("hardCompleted", hardMode);

            Debug.Log($"DB: Sending Data: nurse_id={id}, nurse_name={username}, nurse_passwd={password}");

            string urlSignupData = "http://220.69.209.164:3333/signup_or_update";

            StartCoroutine(PostRequest(urlSignupData, form));
        }

        // Send a POST request to the Flask API
        IEnumerator PostRequest(string url, WWWForm form)
        {
            using (UnityWebRequest www = UnityWebRequest.Post(url, form))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Error sending request: " + www.error);
                }
                else
                {
                    Debug.Log("Request successfully sent: " + www.downloadHandler.text);

                    // 회원가입 성공 시 새로고침
                    StartCoroutine(GetUsersData());
                }
            }
        }
    }
}
