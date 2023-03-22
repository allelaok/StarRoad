using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPanel : BasePanel
{
    [SerializeField] TMPro.TMP_Text score;
    [SerializeField] TMPro.TMP_Text best;

    [SerializeField] BaseButton retryBtn;
    [SerializeField] BaseButton homeBtn;

    // Start is called before the first frame update
    void Start()
    {
        retryBtn.invokeMethod.AddListener(OnClick_RetryBtn);
        homeBtn.invokeMethod.AddListener(delegate { SceneManager.instance.PanelOn(SceneManager.HOME.home); });

    }

    private void OnEnable()
    {
        score.text = GameManager.instance.Score.ToString();
        best.text = GameManager.instance.BestScore.ToString();
    }

    private void OnClick_RetryBtn()
    {
        SceneManager.instance.GamePanelOn();
        Player.state = STATE.Ready;
    }
}
