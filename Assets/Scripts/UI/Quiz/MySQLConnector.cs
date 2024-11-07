using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace MyApp.DataAccess
{
    [System.Serializable]
    public class DataPath
    {
        public string path;
    }

    [System.Serializable]
    public class User
    {
        public string quest;
        public string answer1;
        public string answer2;
        public string answer3;
        public string answer4;
        public int right;
    }

    public class MySQLConnector : MonoBehaviour
    {
        private string url1 = "http://220.69.209.164:3333/get_levels"; // Flask 서버의 로컬 IP 주소로 변경
        private string url2 = "http://220.69.209.164:3333/get_answers"; // Flask 서버의 로컬 IP 주소로 변경

        private List<User> users;
        private List<User> answers;

        public void Start()
        {
            users = new List<User>();
            answers = new List<User>();

            StartCoroutine(GetUsersData((result) => users = result));
            StartCoroutine(GetAnswerData((result) => answers = result));
        }

        public List<User> GetUsers() => users;
        public List<User> GetAnswers() => answers;

        IEnumerator GetUsersData(Action<List<User>> callback)
        {
            UnityWebRequest www = UnityWebRequest.Get(url1);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                string jsonResponse = www.downloadHandler.text;
                Debug.Log("JSON Response from /get_levels: " + jsonResponse);

                try
                {
                    List<User> users = MyApp.Utilities.JsonHelper.FromJson<User>(jsonResponse);
                    if (users != null)
                    {
                        callback(users);
                    }
                    else
                    {
                        Debug.LogError("Failed to parse User data, response was: " + jsonResponse);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("Error parsing JSON: " + e.Message + ". JSON Response: " + jsonResponse);
                }
            }
        }

        IEnumerator GetAnswerData(Action<List<User>> callback)
        {
            UnityWebRequest www = UnityWebRequest.Get(url2);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                string jsonResponse = www.downloadHandler.text;
                Debug.Log("JSON Response from /get_answers: " + jsonResponse);

                try
                {
                    List<User> answers = MyApp.Utilities.JsonHelper.FromJson<User>(jsonResponse);
                    if (answers != null)
                    {
                        callback(answers);
                    }
                    else
                    {
                        Debug.LogError("Failed to parse User data, response was: " + jsonResponse);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("Error parsing JSON: " + e.Message + ". JSON Response: " + jsonResponse);
                }
            }
        }
    }
}
