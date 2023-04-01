using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetNickNamePanel : BasePanel
{

    [SerializeField] TMP_InputField nickname;
    [SerializeField] TMP_Text state;
    [SerializeField] BaseButton checkBtn;
    [SerializeField] BaseButton okBtn;
    [SerializeField] BaseButton exitBtn;


    // Start is called before the first frame update
    void Start()
    {
        checkBtn.OnClickMethod.AddListener(OnClickCheckBtn);
        okBtn.OnClickMethod.AddListener(OnClickOkBtn);
        exitBtn.OnClickMethod.AddListener(OnClickExitBtn);
        nickname.onValueChanged.AddListener(ChangeText);
    }

    public override void SetActice(bool on)
    {
        nickname.text = "";
        checkBtn.image.gameObject.SetActive(true);
        state.text = "";
        base.SetActice(on);
    }

    void ChangeText(string text)
    {
        if (text.Length > 10)
            nickname.text = text.Remove(10);
        else
            checkBtn.image.gameObject.SetActive(true);
    }

    void OnClickCheckBtn() 
    {
        FirebaseManager.instance.CheckNickName(nickname.text, State);
    }

    void State(bool avaliable)
    {
        if (avaliable)
            this.state.text = "Avaliable";
        else
            this.state.text = "Not avaliable";

        checkBtn.image.gameObject.SetActive(!avaliable);
    }

    void OnClickOkBtn()
    {
        SceneManager.instance.PanelOn(SceneManager.HOME.loading);
        FirebaseManager.instance.afterSend = SceneManager.HOME.home;
        GameManager.instance.nickName = nickname.text;
        FirebaseManager.instance.SendData("nickName", nickname.text, 1);
    }

    void OnClickExitBtn()
    {
        SceneManager.instance.PanelOn(SceneManager.HOME.home);
    }

}
