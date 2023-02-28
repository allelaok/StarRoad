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
        auth.StateChanged += AuthStateChanged;

        nickName = PlayerPrefs.GetString("nickName");
    }

    //public string PW { set { password = value; } }

    string nickName;

    string id;
    string pw;
    bool guestLogIn;

    string uid;
    // Update is called once per frame
    void Update()
    {
        //if (guestLogIn)
        //{
        //    PlayerPrefs.SetString("NickName", nickName);
        //    guestLogIn = false;
        //}

        //if (goStartLoginPanel)
        //{
        //    SceneManager.instance.LoginStartPanel();
        //    goStartLoginPanel = false;
        //}

        if (goStartPlay)
        {
            SceneManager.instance.PlayStartPanel();
            goStartPlay = false;
        }

#if UNITY_EDITOR

        if (Input.GetMouseButtonDown(1))
        {
            print("데이터 강제 초기화");
            string kye = uid;
            if (ExceptedString(kye)) return;
            int num = 0;
            reference.Child("users").Child(kye).Child("score").SetValueAsync(num);
            reference.Child("users").Child(kye).Child("selectedCharacter").SetValueAsync(num);
            reference.Child("users").Child(kye).Child("characters").SetValueAsync("3");
            reference.Child("users").Child(kye).Child("coin").SetValueAsync(num);
        }


#endif
    }
    Firebase.Auth.FirebaseUser user = null; //현재 사용자
    public void CheckNickName(string nickName)
    {
        FirebaseDatabase.DefaultInstance.GetReference("users").OrderByChild("nickName").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
                print("get my rank error");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                long cnt = snapshot.ChildrenCount;
                long i = 0;
                foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
                {
                    string snapNickName = childSnapshot.Child("nickName").Value.ToString();
                    if (nickName.Equals(snapNickName))
                    {
                        print(nickName);
                        print(snapNickName);
                        print("사용불가");
                        break;
                    }

                    i++;
                    if (i >= cnt)
                    {
                        print("사용가능");
                        // ok 버튼 활성화
                        this.nickName = nickName;

                        break;
                    }

                }
            }
        });



    }

    public void GuestLogIn()
    {
        if (string.IsNullOrEmpty(nickName)) return;

        auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInAnonymouslyAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                return;
            }

            user = task.Result;

            uid = user.UserId;
            print(uid);

            Debug.LogFormat("User signed in successfully: {0} ({1})", user.DisplayName, user.UserId);
        });
    }

    void SetNickName(string nickName)
    {
        Firebase.Auth.FirebaseUser user = auth.CurrentUser;
        if (user != null)
        {
            Firebase.Auth.UserProfile profile = new Firebase.Auth.UserProfile
            {
                DisplayName = nickName
            };
            user.UpdateUserProfileAsync(profile).ContinueWith(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("UpdateUserProfileAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("UpdateUserProfileAsync encountered an error: " + task.Exception);
                    return;
                }

                Debug.LogFormat("User profile updated successfully: {0} ({1})", user.DisplayName, user.UserId);
            });
        }


    }
    private bool isSignIn = false; //로그인여부
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            //연결된 계정과 기기의 계정이 같은 경우 true
            isSignIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!isSignIn && user != null)
            {
                SceneManager.instance.LoginStartPanel();

                PlayerPrefs.DeleteAll();
                PlayerPrefs.DeleteKey(uid);

                GameManager.instance.InitData();

                Debug.LogFormat("Signed out {0}", user.UserId);
            }
            user = auth.CurrentUser;
            if (isSignIn)
            {
                Debug.LogFormat("Signed in {0}", user.UserId);
                uid = user.UserId;

                if (PlayerPrefs.HasKey(uid) ==false)
                {
                    print("키 없음");
                    PlayerPrefs.SetString("UID", uid);
                    SaveAllData();
                }

                SceneManager.instance.PlayStartPanel();
            }
        }
    }

    
    bool ExceptedString(string kye)
    {
        if (string.IsNullOrEmpty(kye))
        {
            print("kye null");
            return true;
        }
        else if (kye.Contains(".") | kye.Contains("$")| kye.Contains("[") | kye.Contains("]"))
        {
            print("kye error");
            return true;
        }

        return false;

    }

    public void SaveScore(int score)
    {

        string kye = uid;
        if (string.IsNullOrEmpty(kye)) return;

        SaveData("score", GameManager.instance.BestScore);

        GameManager.instance.coin += score;
        SaveData("coin", GameManager.instance.coin);
    }


    void SaveAllData()
    {
        Save("score", 0);
        Save("nickName", nickName);
        Save("selectedCharacter", 0);
        Save("characters", "0");
        Save("coin", 0);
    }


    void Save(string path, int data)
    {
        string kye = uid;
            print(kye);
        if (string.IsNullOrEmpty(kye))
        {
            print(kye);
            return;
        }

        print("저장");

        reference.Child("users").Child(kye).Child(path).SetValueAsync(data);
        print("저장");
    }
    public void SaveData(string path, int data)
    {
        PlayerPrefs.SetInt(path, data);
        Save(path, data);
    }

    void Save(string path, string data)
    {
        string kye = uid;
        if (string.IsNullOrEmpty(kye)) return;
        reference.Child("users").Child(kye).Child(path).SetValueAsync(data);
       
    }
    public void SaveData(string path, string data)
    {
        PlayerPrefs.SetString(path, data);
        Save(path, data);
    }
    public void SaveData(string path, float data)
    {
        PlayerPrefs.SetFloat(path, data);
        string kye = uid;
        if (string.IsNullOrEmpty(kye)) return;
        reference.Child("users").Child(kye).Child(path).SetValueAsync(data);
    }

    public void GetMyInfo( Action callback)
    {
        if (ExceptedString(uid))
        {
            print("id null");
            return;
        }

        reference.Child("users").Child(uid).GetValueAsync().ContinueWithOnMainThread(task =>
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

                print(topNum);

                int beforeScore = 0;
                int beforeRank = 0;
                foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
                {
                    int score = int.Parse(childSnapshot.Child("score").Value.ToString());
                    string nickName = childSnapshot.Child("nickName").Value.ToString();
                    string selectedCharacter = childSnapshot.Child("selectedCharacter").Value.ToString();

                    rank++;
                    RankInfo info = new RankInfo();
                    info.nickName = nickName;
                    info.score = score;
                    info.selectedCharacter = int.Parse(selectedCharacter);
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
                int beforeRank = 0;
                int beforeScore = 0;
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
                        myRank.selectedCharacter = GameManager.instance.selectedCharacter;
                        print("break");
                        break;
                    }

                    string nickName = childSnapshot.Child("nickName").Value.ToString();
                    string selectedCharacter = childSnapshot.Child("selectedCharacter").Value.ToString();

                    targetRank.nickName = nickName;
                    targetRank.score = score;
                    if(targetRank.score == beforeScore)
                    {
                        targetRank.rank = beforeRank;
                    }
                    else
                    {
                        beforeRank = rank;
                        beforeScore = score;
                        targetRank.rank = beforeRank;
                    }


                    targetRank.selectedCharacter = int.Parse(selectedCharacter);

                }
            }
            callback.Invoke();
        });
    }

    // 인증을 관리할 객체
    Firebase.Auth.FirebaseAuth auth;

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


                        GetMyInfo(SceneManager.instance.PlayStartPanel);

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


    public void GuestAutoLogIn()
    {
        auth.SignInAnonymouslyAsync().ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInAnonymouslyAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                return;
            }

            user = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", user.DisplayName, user.UserId);
        });

    }

    public void AutoSignIn()
    {

        string id = PlayerPrefs.GetString("Id");
        string password = PlayerPrefs.GetString("Password");

        id = "";
        password = "";

        print(id);
        print(password);

        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password))
        {
            string nickName = PlayerPrefs.GetString("NickName");
            if (string.IsNullOrEmpty(nickName))
            {
                SceneManager.instance.LoginStartPanel();
                return;
            }

            auth.SignInAnonymouslyAsync().ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("SignInAnonymouslyAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                    return;
                }

                user = task.Result;
                Debug.LogFormat("User signed in successfully: {0} ({1})", user.DisplayName, user.UserId);
                goStartPlay = true;
            });

            return;
        }

        auth.SignInWithEmailAndPasswordAsync(id, password).ContinueWith(
            task =>
            {
                if (task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
                {
                    Firebase.Auth.FirebaseUser newUser = task.Result;
                    GetMyInfo(SceneManager.instance.PlayStartPanel);
                    this.id = id;
                    print("auto login sucssece");
                }
                else
                {
                    print("fail");

                }
            });
    }

    public void LogOut()
    {
        

        auth.SignOut();
    }

    public void SendEmailVerification()
    {
        Firebase.Auth.FirebaseUser user = auth.CurrentUser;
        if (user != null)
        {
            user.SendEmailVerificationAsync().ContinueWith(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("SendEmailVerificationAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SendEmailVerificationAsync encountered an error: " + task.Exception);
                    return;
                }

                Debug.Log("Email sent successfully.");
            });
        }
    }

}