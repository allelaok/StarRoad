using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Firebase;
using Firebase.Database;
using Firebase.Unity;
//using UnityEngine.UI;
using Firebase.Extensions;
using System.Linq;
using TMPro;
using System.Threading.Tasks;

public class RankInfo
{
    public long rank;
    public string nickName;
    public int score;
}

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager instance;
    DatabaseReference reference;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public string ID { set { id = value; } }
    public string PW { set { password = value; } }

    string id;
    string password;


#if UNITY_EDITOR
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (ExceptedString(id, password)) return;
            reference.Child("users").Child(id).Child("score").SetValueAsync(0).Wait(5);
        }
    }
#endif

    public void LogIn()
    {
        if (ExceptedString(id, password)) return;

        reference.Child("users").Child(id).Child("password").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                print("login error");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if ((string)snapshot.Value == password)
                {

                    PlayerPrefs.SetString("userId", id);
                    PlayerPrefs.SetString("password", password);

                    GetBestScore(SceneManager.instance.PlayStartPanel);
                    print("login sucssece");
                }
                else
                {
                    print("login fail.");
                }
            }
        });
    }

    public void AutoLogin()
    {
        id = PlayerPrefs.GetString("userId");
        string password = PlayerPrefs.GetString("password");

        if (ExceptedString(id, password))
        {
            SceneManager.instance.LoginStartPanel();
            return;
        } 

        reference.Child("users").Child(id).Child("password").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if ((string)snapshot.Value == password)
                {
                    GetBestScore(SceneManager.instance.PlayStartPanel);
                    print("auto login sucssece");
                }
                else
                {
                    SceneManager.instance.LoginStartPanel();
                    print("auto login fail.");
                }
            }
        });
    }

    bool ExceptedString(string id, string password)
    {
        if (string.IsNullOrEmpty(id))
        {
            print("id null");
            return true;
        }
        else if (id.Contains(".") | id.Contains("$")| id.Contains("[") | id.Contains("]"))
        {
            print("id error");
            return true;
        }
        else if (string.IsNullOrEmpty(password))
        {
            print("password null");
            return true;
        }
        else if (password.Contains(".") | password.Contains("$") | password.Contains("[") | password.Contains("]"))
        {
            print("password error");
            return true;
        }

        return false;

    }


    public void SingUp()
    {
        if (ExceptedString(id, password)) return;

        reference.Child("users").Child(id).Child("password").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                print("sign up error");
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                print("sign up 1");
                DataSnapshot snapshot = task.Result;
                if (string.IsNullOrEmpty((string)snapshot.Value))
                {
                    reference.Child("users").Child(id).Child("password").SetValueAsync(password);
                    reference.Child("users").Child(id).Child("score").SetValueAsync(0);
                    SceneManager.instance.LoginStartPanel();
                    print("sucssese");
                }
                else
                {
                    print("already exist");
                }
            }
        });
    }
    public void SaveScore(int score)
    {
        print(1);
        if (string.IsNullOrEmpty(id)) return;

        reference.Child("users").Child(id).Child("score").SetValueAsync(score).Wait(5);
        print(2);

        GameManager.instance.BestScore = score;

    }

    public void GetBestScore(Action callback)
    {
        if (string.IsNullOrEmpty(id)) return; 

        reference.Child("users").Child(id).Child("score").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
                print("get bestScore error");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                string str = snapshot.Value.ToString();
                GameManager.instance.BestScore = int.Parse(str);
                print("sucssese my best score");
                callback.Invoke();
            }
        });
    }
    public List<RankInfo> rankInfos = new List<RankInfo>();
    public void GetRankInfo(Action callback)
    {
        print(4);
        FirebaseDatabase.DefaultInstance.GetReference("users").OrderByChild("score").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
                print("get rank error");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                int rank = 0;
                rankInfos.Clear();
                long topNum = 10;
                if(snapshot.ChildrenCount < topNum)
                {
                    topNum = snapshot.ChildrenCount;
                }
                int beforeScore = 0;
                int beforeRank = 0;
                foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
                {
                    int score = int.Parse(childSnapshot.Child("score").Value.ToString());
                    string nickName = childSnapshot.Key.ToString();

                    rank++;
                    if (rank <= topNum)
                    {
                        RankInfo info = new RankInfo();
                        info.nickName = nickName;
                        info.score = score;
                        if (beforeScore == score)
                            info.rank = beforeRank;
                        else
                        {
                            info.rank = rank;
                            beforeScore = score;
                            beforeRank = rank;
                        }

                        rankInfos.Add(info);
                    }
                    else
                    {
                        print("break");
                        break;
                    }
                }

                callback.Invoke();
            }
        });

    }

    public RankInfo targetRank;
    public RankInfo myRank;
    public void GetMyRank(Action callback)
    {
        FirebaseDatabase.DefaultInstance.GetReference("users").OrderByChild("score").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
                print("get my rank error");
            }
            else if (task.IsCompleted)
            {
                targetRank = new RankInfo();
                DataSnapshot snapshot = task.Result;
                int rank = 0;
                foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
                {
                    int score = int.Parse(childSnapshot.Child("score").Value.ToString());
                    rank++;

                    if (score <= GameManager.instance.BestScore)
                    {
                        //reference.Child("users").Child(GameManager.instance.userId).Child("rank").SetValueAsync(rank);
                        myRank = new RankInfo();
                        myRank.nickName = id;
                        myRank.score = GameManager.instance.BestScore;
                        myRank.rank = rank;
                        break;
                    }

                    string nickName = childSnapshot.Key.ToString();

                    targetRank.nickName = nickName;
                    targetRank.score = score;
                    targetRank.rank = rank;

                }
            }
            callback.Invoke();
        });
    }


   
}