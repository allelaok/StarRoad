using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.U2D;

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
        playerItems = chractersContent.GetComponentsInChildren<CharacterIContent>();
    }


    [SerializeField] GameObject startCanvas;
    [SerializeField] GameObject gameCanvas;

    // startCanvas
    [SerializeField] GameObject loginStartPanel;
    [SerializeField] GameObject playStartPanel;
    // loginStartPanel
    [SerializeField] GameObject signUpPanel;
    [SerializeField] GameObject loginPanel;
    // playStartPanel
    [SerializeField] GameObject rankingPanel;
    [SerializeField] GameObject settingPanel;
    // settingPanel
    [SerializeField] GameObject CharacterPanel;
    [SerializeField] GameObject logoutPanel;

    // gameCanvas
    [SerializeField] GameObject endPanel;



    [SerializeField]
    TMPro.TMP_InputField signUpNickName;
    [SerializeField]
    TMPro.TMP_InputField signUpID;
    [SerializeField]
    TMPro.TMP_InputField signUpPW;
    [SerializeField]
    TMPro.TMP_InputField logInID;
    [SerializeField]
    TMPro.TMP_InputField logInPW;

    [SerializeField]
    Player player;

    public SpriteAtlas sprite;

    CharacterIContent[] playerItems;
    private void Start()
    {
        sprite = Resources.Load<SpriteAtlas>("Members");
    }
    enum STARTCANVAS
    {
        Default,
        loginStartPanel,
        playStartPanel
    }



    void StartCanvas(STARTCANVAS state)
    {
        loginStartPanel.SetActive(false);
        playStartPanel.SetActive(false);

        switch (state)
        {
            case STARTCANVAS.loginStartPanel:
                loginStartPanel.SetActive(true);
                break;
            case STARTCANVAS.playStartPanel:
                playStartPanel.SetActive(true);
                break;
        }
    }

    enum LOGINSTART
    {
        Default,
        signUpPanel,
        loginPanel
    }
    void LoginStartPanel(LOGINSTART state)
    {
        print(2);
        signUpPanel.SetActive(false);
        loginPanel.SetActive(false);
        print(3);

        switch (state)
        {
            case LOGINSTART.signUpPanel:

                signUpID.text = "";
                signUpPW.text = "";

                signUpPanel.SetActive(true);
                break;
            case LOGINSTART.loginPanel:

                logInID.text = "";
                logInPW.text = "";

                loginPanel.SetActive(true);
                break;
        }
    }

    enum PLAYSTART
    {
        Default,
        rankingPanel,
        settingPanel
    }
    void PlayStartPanel(PLAYSTART state)
    {
        rankingPanel.SetActive(false);
        settingPanel.SetActive(false);

        switch (state)
        {
            case PLAYSTART.rankingPanel:
                rankingPanel.SetActive(true);
                break;
            case PLAYSTART.settingPanel:
                settingPanel.SetActive(true);
                break;
        }
    }

    enum GAMECANVAS
    {
        Default,
        endPanel
    }
    void GameCanvas(GAMECANVAS state)
    {
        endPanel.SetActive(false);

        switch (state)
        {
            case GAMECANVAS.endPanel:
                endPanel.SetActive(true);
                break;
        }
    }

    enum SETTING
    {
        Default,
        CharacterPanel,
        logoutPanel
    }

    void SettingPanel(SETTING state)
    {
        CharacterPanel.SetActive(false);
        logoutPanel.SetActive(false);

        switch (state)
        {
            case SETTING.CharacterPanel:
                CharacterPanel.SetActive(true);
                break;
            case SETTING.logoutPanel:
                logoutPanel.SetActive(true);
                break;
        }
    }


    public void LoginStartPanel()
    {
        print(1);
        LoginStartPanel(LOGINSTART.Default);
        StartCanvas(STARTCANVAS.loginStartPanel);

        startCanvas.SetActive(true);
        gameCanvas.SetActive(false);

    }
    public void PlayStartPanel()
    {
        player.anim.SetInteger("SC", GameManager.instance.selectedCharacter);

        PlayStartPanel(PLAYSTART.Default);
        StartCanvas(STARTCANVAS.playStartPanel);

        gameCanvas.SetActive(false);
        startCanvas.SetActive(true);
    }

    public void LoginPanel()
    {
        LoginStartPanel(LOGINSTART.loginPanel);
        StartCanvas(STARTCANVAS.loginStartPanel);

        gameCanvas.SetActive(false);
        startCanvas.SetActive(true);
    }

    public void SignUpPanel()
    {
        LoginStartPanel(LOGINSTART.signUpPanel);

        StartCanvas(STARTCANVAS.loginStartPanel);

        gameCanvas.SetActive(false);
        startCanvas.SetActive(true);
    }

    public void RankingPanel()
    {
        PlayStartPanel(PLAYSTART.rankingPanel);

        StartCanvas(STARTCANVAS.playStartPanel);

        gameCanvas.SetActive(false);
        startCanvas.SetActive(true);
    }

    public void GoSettingPanel()
    {

        SettingPanel(SETTING.Default);
        PlayStartPanel(PLAYSTART.settingPanel);

        StartCanvas(STARTCANVAS.playStartPanel);

        gameCanvas.SetActive(false);
        startCanvas.SetActive(true);
    }

    public void GoCharacterPanel()
    {
        player.anim.SetInteger("SC", 0);

        SetChracters();

        SettingPanel(SETTING.CharacterPanel);
        PlayStartPanel(PLAYSTART.settingPanel);

        StartCanvas(STARTCANVAS.playStartPanel);

        gameCanvas.SetActive(false);
        startCanvas.SetActive(true);
    }
    public void GoLogoutPanel()
    {
        SettingPanel(SETTING.logoutPanel);
        PlayStartPanel(PLAYSTART.settingPanel);

        StartCanvas(STARTCANVAS.playStartPanel);

        gameCanvas.SetActive(false);
        startCanvas.SetActive(true);
    }
   
    public void GamePanle()
    {
        SetCharacter();
        this.GameCanvas(GAMECANVAS.Default);
        gameCanvas.SetActive(true);
        startCanvas.SetActive(false);
        SoundManager.instance.BGM((int)Sound.BGM1);
    }

    public void EndPanel()
    {
        this.GameCanvas(GAMECANVAS.endPanel);

        gameCanvas.SetActive(true);
        startCanvas.SetActive(false);
    }

    public void OnClick_SignUp()
    {
        FirebaseManager.instance.SignUp(signUpNickName.text, signUpID.text, signUpPW.text);
    }
    public void OnClick_LogIn()
    {
        FirebaseManager.instance.SignIn(logInID.text, logInPW.text);
    }

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
        print(1);
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

    string[] characters =
      {
        "Exy",
        "SA",
        "BN",
        "SB",
        "RD",
        "DW",
        "ES",
        "YR",
        "DY",
        "YJ",
    };

    public void SetCharacter()
    {
        for (int i = 0; i < player.lifeImg.Length; i++)
        {
            player.lifeImg[i].sprite = GetProfileSprite(GameManager.instance.selectedCharacter);
        }

        player.anim.SetInteger("SC", GameManager.instance.selectedCharacter);
    }

    public Sprite GetProfileSprite(int num, string path = null)
    {
        if (num < characters.Length)
            return sprite.GetSprite(characters[num] + path);
        else
            return null;
    }


    [SerializeField]
    GameObject chractersContent;
    [SerializeField]
    TMPro.TMP_Text coin;
    void SetChracters()
    {
        coin.text = GameManager.instance.coin.ToString();
        for (int i = 0; i < playerItems.Length; i++)
        {
            playerItems[i].SetContent();
        }
    }

    public void SelectPlayer(int num)
    {
        GameManager.instance.selectedCharacter = num;
        FirebaseManager.instance.SaveData("selectedCharacter", num);
        SetChracters();
        SetCharacter();
    }

    public void BuyPlayer(int num)
    {
        if (GameManager.instance.coin < GameManager.instance.Price) return;

        GameManager.instance.coin -= GameManager.instance.Price;
        GameManager.instance.characters += num.ToString();

        FirebaseManager.instance.SaveData("coin", GameManager.instance.coin);
        FirebaseManager.instance.SaveData("characters", GameManager.instance.characters);
        SetChracters();
    }
    public void OnClick_SignOut()
    {
        FirebaseManager.instance.LogOut();
        LoginStartPanel();
    }
}
