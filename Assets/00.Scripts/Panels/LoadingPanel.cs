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
        for(int i = 0; i < GameManager.instance.tasks.Count; i++)
        {
            if(GameManager.instance.tasks[i] != null)
            GameManager.instance.tasks[i].Dispose();
        }
        GameManager.instance.tasks.Clear();
        SceneManager.instance.PanelOn(SceneManager.HOME.home);
    }
}
