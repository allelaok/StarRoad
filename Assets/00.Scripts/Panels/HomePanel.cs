using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomePanel : BasePanel
{
    public BaseButton rankingBtn;
    public BaseButton settingBtn;
    public BaseButton playBtn;

    private void Start()
    {
        rankingBtn.invokeMethod.AddListener(OnCliCk_RankingBtn);
        settingBtn.invokeMethod.AddListener(OnClick_SettingBtn);
        playBtn.invokeMethod.AddListener(OnClick_playBtn);
    }

    void OnCliCk_RankingBtn()
    {
        SceneManager.instance.LoadingPanelOn();
        FirebaseManager.instance.GetRankInfo();
    }

    void OnClick_SettingBtn()
    {

        SceneManager.instance.PanelOn(SceneManager.HOME.setting);
    }

    void OnClick_playBtn()
    {
        SceneManager.instance.GamePanelOn();
        Player.state = STATE.Ready;
    }
}
