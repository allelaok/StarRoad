using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingPanel : BasePanel
{

    public BaseButton soundBtn;
    public BaseButton changeNickNameBtn;
    public BaseButton homeBtn;

    // Start is called before the first frame update
    void Start()
    {
        soundBtn.invokeMethod.AddListener(delegate { SceneManager.instance.PanelOn(SceneManager.HOME.sound); });
        changeNickNameBtn.invokeMethod.AddListener(delegate { SceneManager.instance.PanelOn(SceneManager.HOME.setNickName); });
        homeBtn.invokeMethod.AddListener(delegate { SceneManager.instance.PanelOn(SceneManager.HOME.home); });
    }
}
