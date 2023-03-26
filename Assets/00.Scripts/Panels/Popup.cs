using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup : MonoBehaviour
{
    public BaseButton exitBtn;

    void Start()
    {
        exitBtn.invokeMethod.AddListener(OnClick_ExitBtn);

    }

    private void OnClick_ExitBtn()
    {
        SceneManager.instance.Popup(false);
    }

}
