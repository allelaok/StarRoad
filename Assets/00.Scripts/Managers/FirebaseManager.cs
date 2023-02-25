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
    public int selectedCharacter;
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
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
    }

    //public string PW { set { password = value; } }

    string nickName;

    string id;
    string pw;

    // Update is called once per frame
    void Update()
    {
        if (login)
        {
            PlayerPrefs.SetString("Id", id);
            PlayerPrefs.SetString("Password", pw);
            login = false;
        }
        if (goStartLoginPanel)
        {
            SceneManager.instance.LoginStartPanel();
            goStartLoginPanel = false;
        }

        if (goStartPlay)
        {
            SceneManager.instance.PlayStartPanel();
            goStartPlay = false;
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.S))
        {
            string kye = id.Replace(".", "dot");
            if (ExceptedString(kye)) return;
            reference.Child("users").Child(kye).Child("score").SetValueAsync(0);
            reference.Child("users").Child(kye).Child("selectedCharacter").SetValueAsync(0);
            reference.Child("users").Child(kye).Child("characters").SetValueAsync("0");
            reference.Child("users").Child(kye).Child("coin").SetValueAsync(0);
        }
#endif
    }

    //public void LogIn()
    //{
    //    if (ExceptedString(id)) return;

    //    reference.Child("users").Child(id).Child("password").GetValueAsync().ContinueWithOnMainThread(task =>
    //    {
    //        if (task.IsFaulted)
    //        {
    //            print("login error");
    //        }
    //        else if (task.IsCompleted)
    //        {
    //            DataSnapshot snapshot = task.Result;
    //            if ((string)snapshot.Value == password)
    //            {

    //                PlayerPrefs.SetString("userId", id);
    //                PlayerPrefs.SetString("password", password);

    //                GetBestScore(SceneManager.instance.PlayStartPanel);
    //                print("login sucssece");
    //            }
    //            else
    //            {
    //                print("login fail.");
    //            }
    //        }
    //    });
    //}

    //public void AutoLogin()
    //{
    //   string id = PlayerPrefs.GetString("id");
    //    string password = PlayerPrefs.GetString("password");

    //    if (ExceptedString(id))
    //    {
    //        SceneManager.instance.LoginStartPanel();
    //        return;
    //    } 

    //    reference.Child("users").Child(id.Replace(".","dot")).Child("password").GetValueAsync().ContinueWithOnMainThread(task =>
    //    {
    //        if (task.IsFaulted)
    //        {
    //        }
    //        else if (task.IsCompleted)
    //        {
    //            DataSnapshot snapshot = task.Result;
    //            if ((string)snapshot.Value == password)
    //            {
    //                GetMyInfo(id, SceneManager.instance.PlayStartPanel);
    //                print("auto login sucssece");
    //            }
    //            else
    //            {
    //                SceneManager.instance.LoginStartPanel();
    //                print("auto login fail.");
    //            }
    //        }
    //    });
    //}

    bool ExceptedString(string id)
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

        return false;

    }


    //public void SingUp()
    //{
    //    if (ExceptedString(id)) return;

    //    reference.Child("users").Child(id).Child("password").GetValueAsync().ContinueWithOnMainThread(task =>
    //    {
    //        if (task.IsFaulted)
    //        {
    //            print("sign up error");
    //            // Handle the error...
    //        }
    //        else if (task.IsCompleted)
    //        {
    //            print("sign up 1");
    //            DataSnapshot snapshot = task.Result;
    //            if (string.IsNullOrEmpty((string)snapshot.Value))
    //            {
    //                reference.Child("users").Child(id).Child("score").SetValueAsync(0);
    //                SceneManager.instance.LoginStartPanel();
    //                print("sucssese");
    //            }
    //            else
    //            {
    //                print("already exist");
    //            }
    //        }
    //    });
    //}
    public void SaveScore(int score)
    {

        string kye = id.Replace(".", "dot");
        if (string.IsNullOrEmpty(kye)) return;
        if (GameManager.instance.BestScore > score)
        {
            GameManager.instance.BestScore = score;
            reference.Child("users").Child(kye).Child("score").SetValueAsync(score);
        }
        GameManager.instance.coin += score;
        reference.Child("users").Child(kye).Child("coin").SetValueAsync(GameManager.instance.coin);
    }

    public void SaveData(string path, object data)
    {
        string kye = id.Replace(".", "dot");
        if (string.IsNullOrEmpty(kye)) return;
        reference.Child("users").Child(kye).Child(path).SetValueAsync(data);
    }

    public void GetMyInfo(string id, Action callback)
    {
        if (ExceptedString(id))
        {
            print("id null");
            return;
        }

        reference.Child("users").Child(id).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
                print("get bestScore error");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                string score = snapshot.Child("score").Value.ToString();
                string nickName = snapshot.Child("nickName").Value.ToString();
                string selectedCharacter = snapshot.Child("selectedCharacter").Value.ToString();
                string characters = snapshot.Child("characters").Value.ToString();
                string coin = snapshot.Child("coin").Value.ToString();
                GameManager.instance.BestScore = int.Parse(score);
                GameManager.instance.selectedCharacter = int.Parse(selectedCharacter);
                GameManager.instance.characters = characters;
                GameManager.instance.coin = int.Parse(coin);
                this.nickName = nickName;
                print("sucssese my info");
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
                    string nickName = childSnapshot.Child("nickName").Value.ToString();

                    rank++;
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

                    if (rank >= topNum)
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
                        myRank.nickName = this.nickName;
                        myRank.score = GameManager.instance.BestScore;
                        myRank.rank = rank;
                        print("break");
                        break;
                    }

                    string nickName = childSnapshot.Child("nickName").ToString();

                    targetRank.nickName = nickName;
                    targetRank.score = score;
                    targetRank.rank = rank;

                }
            }
            callback.Invoke();
        });
    }

    // 인증을 관리할 객체
    Firebase.Auth.FirebaseAuth auth;
    bool goStartLogin;

    // 회원가입 버튼을 눌렀을 때 작동할 함수
    public void SignUp(string nickName, string id, string password)
    {
        string key = id.Replace(".", "dot");
        if (ExceptedString(key)) return;

        reference.Child("users").Child(key).Child("nickName").GetValueAsync().ContinueWithOnMainThread(task =>
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
                    // 회원가입 버튼은 인풋 필드가 비어있지 않을 때 작동한다.
                    if (id.Length != 0 && password.Length > 5)
                    {
                        auth.CreateUserWithEmailAndPasswordAsync(id, password).ContinueWith(
                            task =>
                            {
                                if (!task.IsCanceled && !task.IsFaulted)
                                {
                                    reference.Child("users").Child(key).Child("score").SetValueAsync(0);
                                    reference.Child("users").Child(key).Child("nickName").SetValueAsync(nickName);
                                    reference.Child("users").Child(key).Child("selectedCharacter").SetValueAsync(0);
                                    reference.Child("users").Child(key).Child("characters").SetValueAsync("0");
                                    reference.Child("users").Child(key).Child("coin").SetValueAsync(0);
                                    goStartPlay = true;
                                    print("success");
                                }
                                else
                                {
                                    print("fail"); 

                                    print(task.Exception);
                                }
                            });
                    }
                }
                else
                {
                    print("already exist");
                }
            }
        });

       
    }
    bool login;
    bool goStartPlay;
    private bool goStartLoginPanel;

    // 로그인 버튼을 눌렀을 때 작동할 함수
    public void SignIn(string id, string pw)
    {
        // 로그인 버튼은 인풋 필드가 비어있지 않을 때 작동한다.
        if (id.Length != 0 && pw.Length > 5)
        {
            auth.SignInWithEmailAndPasswordAsync(id, pw).ContinueWith(
                task =>
                {
                    if (task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
                    {
                        Firebase.Auth.FirebaseUser newUser = task.Result;


                        GetMyInfo(id.Replace(".", "dot"), SceneManager.instance.PlayStartPanel);

                        this.id = id;
                        this.pw = pw;
                        login = true;
                        goStartPlay = true;
                        
                        print("login success");

                    }
                    else
                    {
                        print("fail");
                    }
                });
        }
    }

    public void AutoSignIn()
    {

        string id = PlayerPrefs.GetString("Id");
        string password = PlayerPrefs.GetString("Password");
        print(id);
        print(password);
        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password))
        {
            SceneManager.instance.LoginStartPanel();
            return;
        }

        auth.SignInWithEmailAndPasswordAsync(id, password).ContinueWith(
            task =>
            {
                if (task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
                {
                    Firebase.Auth.FirebaseUser newUser = task.Result;
                    GetMyInfo(id.Replace(".", "dot"), SceneManager.instance.PlayStartPanel);
                    this.id = id;
                    print("auto login sucssece");
                }
                else
                {
                        goStartLoginPanel = true;
                    print("fail");

                }
            });
    }

    public void LogOut()
    {
        PlayerPrefs.DeleteAll();
        id = null;
        pw = null;
        nickName = null;
        auth.SignOut();
    }

}