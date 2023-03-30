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
using Firebase.Auth;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

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
    //public string PW { set { password = value; } }

    // 인증을 관리할 객체
    Firebase.Auth.FirebaseAuth auth;

    public bool IsSignIn { get { return isSignIn; } }

    public object SystemMessageManager { get; private set; }

    public Firebase.Auth.FirebaseUser user = null; //현재 사용자
    private bool isSignIn = false; //로그인여부

    string uid;

    [SerializeField] TMPro.TMP_Text debugMsg;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        reference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;

    }


    public bool InternetOn()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            return false;
        }

        return true;
    }
    public void InitData()
    {
        print("데이터 강제 초기화");
        debugMsg.text = "데이터 강제 초기화";
        if (ExceptedString(uid)) return;
        int num = 0;
        reference.Child("users").Child(uid).Child("score").SetValueAsync(num);
        reference.Child("users").Child(uid).Child("selectedCharacter").SetValueAsync(num);
        reference.Child("users").Child(uid).Child("characters").SetValueAsync("0");
        reference.Child("users").Child(uid).Child("coin").SetValueAsync(num);
    }


  

    public void GameCenterLogin()
    {
        if (Social.localUser.authenticated == true)
        {
            Debug.Log("Success to true");
            if (user == null)
            {
                SignInWithGameCenterAsync();
            }
        }
        else
        {
            Social.localUser.Authenticate((bool success) =>
            {
                if (success)
                {
                    Debug.Log("Success to authenticate");
                    // 파이어베이스 로그인 연동
                    SignInWithGameCenterAsync();
                }
                else
                {
                    Debug.Log("Faile to login");
                    GuestLogIn();
                }
            });
        }
    }



    public Task SignInWithGameCenterAsync()
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        var credentialTask = Firebase.Auth.GameCenterAuthProvider.GetCredentialAsync();
        var continueTask = credentialTask.ContinueWithOnMainThread(task =>
        {
            if (!task.IsCompleted)
                return null;

            if (task.Exception != null)
                Debug.Log("GC Credential Task - Exception: " + task.Exception.Message);

            var credential = task.Result;

            var loginTask = auth.SignInWithCredentialAsync(credential);
            return loginTask.ContinueWithOnMainThread(HandleSignInWithUser);
        });

        return continueTask;
    }

    // Called when a sign-in without fetching profile data completes.
    void HandleSignInWithUser(Task<Firebase.Auth.FirebaseUser> task)
    {
        //EnableUI();
        if (LogTaskCompletion(task, "Sign-in"))
        {
            user = task.Result;
            Debug.Log(String.Format("{0} signed in", task.Result.DisplayName));
        }
    }

    // Log the result of the specified task, returning true if the task
    // completed successfully, false otherwise.
    protected bool LogTaskCompletion(Task task, string operation)
    {
        bool complete = false;
        if (task.IsCanceled)
        {
            Debug.Log(operation + " canceled.");
        }
        else if (task.IsFaulted)
        {
            Debug.Log(operation + " encounted an error.");
            foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
            {
                string authErrorCode = "";
                Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                if (firebaseEx != null)
                {
                    authErrorCode = String.Format("AuthError.{0}: ",
                      ((Firebase.Auth.AuthError)firebaseEx.ErrorCode).ToString());
                }
                Debug.Log(authErrorCode + exception.ToString());
            }
        }
        else if (task.IsCompleted)
        {
            Debug.Log(operation + " completed");
            complete = true;
        }
        return complete;
    }

    public void GuestLogIn()
    {
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
            debugMsg.text = "guest login success";

            Debug.LogFormat("User signed in successfully: {0} ({1})", user.DisplayName, user.UserId);
        });
    }



    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            //연결된 계정과 기기의 계정이 같은 경우 true
            isSignIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!isSignIn && user != null)
            {
                Debug.LogFormat("Signed out {0}", user.UserId);
            }
            user = auth.CurrentUser;
            if (isSignIn)
            {
                Debug.LogFormat("Signed in {0}", user.UserId);
                uid = user.UserId;

                GetMyInfo(delegate { SceneManager.instance.PanelOn(SceneManager.HOME.home); });
            }
        }
    }


    public void CheckNickName(string nickName, Action<bool> callback)
    {
     //   print("check");
        FirebaseDatabase.DefaultInstance.GetReference("users").OrderByChild("nickName").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
                SceneManager.instance.Popup("get nickname info error");
                print("get nickname info error" + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                long cnt = snapshot.ChildrenCount;
        print("ChildCount" + cnt);
                long i = 0;
                //  print("check");
                foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
                {
        print("check" + i);
                        string snapNickName = childSnapshot.Child("nickName").Value.ToString();
        print("check" + snapNickName);
                 //   print("check");
                    if (nickName.Equals(snapNickName))
                    {
                        print(nickName);
                        print(snapNickName);
                        print("사용불가");
                        callback.Invoke(false);
                        break;
                    }

                    i++;
                    if (i >= cnt)
                    {
                        print("사용가능");
                        callback.Invoke(true);

                        break;
                    }

                }
            }
        });
    }


    public void SetNickName(string nickName)
    {
        Firebase.Auth.FirebaseUser user = auth.CurrentUser;
        if (user != null)
        {
            Firebase.Auth.UserProfile profile = new Firebase.Auth.UserProfile
            {
                DisplayName = nickName
            };
            user.UpdateUserProfileAsync(profile).ContinueWith(task =>
            {
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




    bool ExceptedString(string kye)
    {
        if (string.IsNullOrEmpty(kye))
        {
            print("kye null");
            return true;
        }
        else if (kye.Contains(".") | kye.Contains("$") | kye.Contains("[") | kye.Contains("]"))
        {
            print("kye error");
            return true;
        }

        return false;

    }

    public void SaveScore()
    {
        if (string.IsNullOrEmpty(uid)) return;
        GameManager.instance.coin += GameManager.instance.Score;

        afterSend = SceneManager.HOME.end;
        SendData("score", GameManager.instance.BestScore, 2);
        SendData("coin", GameManager.instance.coin, 2);
    }



    public SceneManager.HOME afterSend;
    public void SendDataAll()
    {
        SceneManager.instance.PanelOn(SceneManager.HOME.loading);
        SendData("score", GameManager.instance.BestScore, 3);
        SendData("nickName", GameManager.instance.nickName, 3);
        //SendData("selectedCharacter", GameManager.instance.selectedCharacter, 5);
        //SendData("characters", GameManager.instance.characters, 5);
        SendData("coin", GameManager.instance.coin, 3);
    }

    int sendCnt = 0;
    public void SendData(string path, int data, int sendNum)
    {
        string kye = uid;
        if (string.IsNullOrEmpty(kye)) return;
        reference.Child("users").Child(kye).Child(path).SetValueAsync(data).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
                print("get rank error");
            }
            else if (task.IsCompleted)
            {
                AfterSend(sendNum);
            }
        });
    }


    public void SendData(string path, string data, int sendNum)
    {
        string kye = uid;
        if (string.IsNullOrEmpty(kye)) return;
        reference.Child("users").Child(kye).Child(path).SetValueAsync(data).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
                print("get rank error");
            }
            else if (task.IsCompleted)
            {
                AfterSend(sendNum);
            }
        });
    }

    public void SendData(string path, float data, int sendNum)
    {
        string kye = uid;
        if (string.IsNullOrEmpty(kye)) return;
        reference.Child("users").Child(kye).Child(path).SetValueAsync(data).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
                print("get rank error");
            }
            else if (task.IsCompleted)
            {
                AfterSend(sendNum);
            }
        });
    }

    void AfterSend(int sendNum)
    {
        sendCnt++;
        if (sendCnt == sendNum)
        {
            if (afterSend != SceneManager.HOME.Count)
            {
                SceneManager.instance.PanelOn(afterSend);
                afterSend = SceneManager.HOME.Count;
            }
            sendCnt = 0;
        }
    }

    public void GetMyInfo( Action callback)
    {
        print(1);
        if (ExceptedString(uid))
        {
            print("id null");
            SceneManager.instance.Popup("id null");
            return;
        }
        print(1);

        reference.Child("users").Child(uid).GetValueAsync().ContinueWithOnMainThread(task =>
        {
        print(1);
            if (task.IsFaulted)
            {
                // Handle the error...
                SceneManager.instance.Popup("get my info error");
                print("get my info error");
                //callback2.Invoke();
            }
            else if (task.IsCompleted)
            {
                print(1);
                DataSnapshot snapshot = task.Result;
                print(1);
                if (snapshot.ChildrenCount > 0)
                {
                print(1);
                    //string nickName = "ㄴㄴㄴ";
                    string nickName = snapshot.Child("nickName").Value.ToString();
                    print(nickName);
                print(1);

                    if (string.IsNullOrEmpty(nickName))
                    {
                        afterSend = SceneManager.HOME.setNickName;
                        AfterSend(1);
                    }
                    else
                    {
                print(1);
                        callback.Invoke();
                        GameManager.instance.BestScore = int.Parse(snapshot.Child("score").Value.ToString());
                        //GameManager.instance.selectedCharacter = int.Parse(snapshot.Child("selectedCharacter").Value.ToString());
                        //GameManager.instance.characters = snapshot.Child("characters").Value.ToString();
                        GameManager.instance.coin = int.Parse(snapshot.Child("coin").Value.ToString());
                        GameManager.instance.nickName = nickName;
                        print("sucssese my info");

                    }


                }
                else
                {
                    print(1);
                    // 닉네임 설정 패널
                    print("nickName");
                    afterSend = SceneManager.HOME.setNickName;
                    SendDataAll();
                }
                print(1);
            }
        });
    }
    public List<RankInfo> rankInfos = new List<RankInfo>();
    public void GetRankInfo()
    {
        FirebaseDatabase.DefaultInstance.GetReference("users").OrderByChild("score").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
                SceneManager.instance.PanelOn(SceneManager.HOME.home);
                SceneManager.instance.Popup("get rank error");
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

                GetMyRank();
            }
        });

    }

    public RankInfo targetRank;
    public RankInfo myRank;
    private string authCode;

    public void GetMyRank()
    {
        FirebaseDatabase.DefaultInstance.GetReference("users").OrderByChild("score").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
                SceneManager.instance.Popup("get my rank error");
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
                        myRank.nickName = GameManager.instance.nickName;
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
            SceneManager.instance.PanelOn(SceneManager.HOME.ranking);
        });
    }


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

    //// 로그인 버튼을 눌렀을 때 작동할 함수
    //public void SignIn(string id, string pw)
    //{
    //    // 로그인 버튼은 인풋 필드가 비어있지 않을 때 작동한다.
    //    if (id.Length != 0 && pw.Length > 5)
    //    {
    //        auth.SignInWithEmailAndPasswordAsync(id, pw).ContinueWith(
    //            task =>
    //            {
    //                if (task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
    //                {
    //                    Firebase.Auth.FirebaseUser newUser = task.Result;


    //                    GetMyInfo(SceneManager.instance.PlayStartPanel);                        
    //                    print("login success");

    //                }
    //                else
    //                {
    //                    print("fail");
    //                }
    //            });
    //    }
    //}


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

    //public void AutoSignIn()
    //{

    //    string id = PlayerPrefs.GetString("Id");
    //    string password = PlayerPrefs.GetString("Password");

    //    id = "";
    //    password = "";

    //    print(id);
    //    print(password);

    //    if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password))
    //    {
    //        string nickName = PlayerPrefs.GetString("NickName");
    //        if (string.IsNullOrEmpty(nickName))
    //        {
    //            SceneManager.instance.LoginStartPanel();
    //            return;
    //        }

    //        auth.SignInAnonymouslyAsync().ContinueWith(task =>
    //        {
    //            if (task.IsCanceled)
    //            {
    //                Debug.LogError("SignInAnonymouslyAsync was canceled.");
    //                return;
    //            }
    //            if (task.IsFaulted)
    //            {
    //                Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
    //                return;
    //            }

    //            user = task.Result;
    //            Debug.LogFormat("User signed in successfully: {0} ({1})", user.DisplayName, user.UserId);
    //        });

    //        return;
    //    }

    //    auth.SignInWithEmailAndPasswordAsync(id, password).ContinueWith(
    //        task =>
    //        {
    //            if (task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
    //            {
    //                Firebase.Auth.FirebaseUser newUser = task.Result;
    //                GetMyInfo(SceneManager.instance.PlayStartPanel);
    //                print("auto login sucssece");
    //            }
    //            else
    //            {
    //                print("fail");

    //            }
    //        });
    //}

    public void LogOut()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.DeleteKey(uid);

        GameManager.instance.InitData();

        SceneManager.instance.PanelOn(SceneManager.HOME.home);

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