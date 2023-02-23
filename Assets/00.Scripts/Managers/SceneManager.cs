using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public static SceneManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        loginStartPanel.SetActive(false);
        playStartPanel.SetActive(false);
        signUpPanel.SetActive(false);
        loginPanel.SetActive(false);
        rankingPanel.SetActive(false);

        startCanvas.SetActive(true);
        gameCanvas.SetActive(false);

        ranks = content.GetComponentsInChildren<Rank>();
    }

    [SerializeField]
    GameObject startCanvas;
    [SerializeField]
    GameObject gameCanvas;

    [SerializeField]
    GameObject loginStartPanel;
    [SerializeField]
    GameObject playStartPanel;
    [SerializeField]
    GameObject signUpPanel;
    [SerializeField]
    GameObject loginPanel;
    [SerializeField]
    GameObject rankingPanel;

    [SerializeField]
    TMPro.TMP_InputField signUpID;
    [SerializeField]
    TMPro.TMP_InputField signUpPW;
    [SerializeField]
    TMPro.TMP_InputField logInID;
    [SerializeField]
    TMPro.TMP_InputField logInPW;

    public void LoginStartPanel()
    {
        loginStartPanel.SetActive(true);
        playStartPanel.SetActive(false);
        signUpPanel.SetActive(false);
        loginPanel.SetActive(false);
        rankingPanel.SetActive(false);

        startCanvas.SetActive(true);
        gameCanvas.SetActive(false);
    }

    public void PlayStartPanel()
    {
        loginStartPanel.SetActive(false);
        playStartPanel.SetActive(true);
        signUpPanel.SetActive(false);
        loginPanel.SetActive(false);
        rankingPanel.SetActive(false);

        gameCanvas.SetActive(false);
        startCanvas.SetActive(true);
    }

    public void LoginPanel()
    {
        loginStartPanel.SetActive(true);
        signUpPanel.SetActive(false);
        loginPanel.SetActive(true);

        playStartPanel.SetActive(false);
        logInID.text = "";
        logInPW.text = "";

        gameCanvas.SetActive(false);
        startCanvas.SetActive(true);
    }

    public void SignUpPanel()
    {
        loginStartPanel.SetActive(true);
        signUpPanel.SetActive(true);
        loginPanel.SetActive(false);

        playStartPanel.SetActive(false);
        rankingPanel.SetActive(false);

        signUpID.text = "";
        signUpPW.text = "";

        gameCanvas.SetActive(false);
        startCanvas.SetActive(true);
    }

    public void RankingPanel()
    {
        loginStartPanel.SetActive(false);

        playStartPanel.SetActive(true);
        rankingPanel.SetActive(true);

        gameCanvas.SetActive(false);
        startCanvas.SetActive(true);
    }

    public void GamePanle()
    {
        gameCanvas.SetActive(true);
        startCanvas.SetActive(false);
    }

    public void OnClick_SignUp()
    {
        FirebaseManager.instance.ID = signUpID.text;
        FirebaseManager.instance.PW = signUpPW.text;
        FirebaseManager.instance.SingUp();
    }
    public void OnClick_LogIn()
    {
        FirebaseManager.instance.ID = logInID.text;
        FirebaseManager.instance.PW = logInPW.text;
        FirebaseManager.instance.LogIn();
    }

    [SerializeField]
    Player player;
    public void OnClick_PlayBtn()
    {
        GamePanle();
        player.Ready();
    }

    public void OnClick_RankingBtn()
    {
        FirebaseManager.instance.GetRankInfo(AfterGetTopTen);
    }

    [SerializeField]
    Transform content;
    Rank[] ranks;
    void AfterGetTopTen()
    {
        for (int i = 0; i < 10; i++)
        {
            if (i < FirebaseManager.instance.rankInfos.Count)
                ranks[i].SetInfo(FirebaseManager.instance.rankInfos[i]);
            else
                ranks[i].SetInfo();
        }
        FirebaseManager.instance.GetMyRank(AfterGetMyRank);
    }

    void AfterGetMyRank()
    {
        if (FirebaseManager.instance.targetRank.rank == 0)
            FirebaseManager.instance.targetRank = null;

        ranks[10].SetInfo(FirebaseManager.instance.targetRank);
        ranks[11].SetInfo(FirebaseManager.instance.myRank);
        RankingPanel();
    }
}
