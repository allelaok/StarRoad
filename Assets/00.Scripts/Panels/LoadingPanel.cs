using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;


public class LoadingPanel : MonoBehaviour
{
    public BaseButton homeBtn;

    private void Start()
    {
        homeBtn.OnClickMethod.AddListener(OnClick_HomeBtn);
    }

    void OnClick_HomeBtn()
    {

        SceneManager.instance.PanelOn(SceneManager.HOME.home);
    }
}
