using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankingPanel : BasePanel
{
    public Transform contents;
    public BaseButton exitBtn;

    Rank[] ranks;
    private void Awake()
    {
        ranks = contents.GetComponentsInChildren<Rank>();
    }

    private void Start()
    {
        exitBtn.OnClickMethod.AddListener(OnClick_ExitBtn);
    }

    public override void SetActice(bool on)
    {
                print(3);
        if (on)
        {
            if (FirebaseManager.instance.InternetOn() == false)
            {
                SceneManager.instance.Popup("인터넷 연결 안됨");
                return;
            }

            AfterGetRank();
        }
        else
        {
            base.SetActice(false);
        }
    }

    void AfterGetRank()
    {
        print(4);
        for (int i = 0; i < 10; i++)
        {
            print(4);
            if (i < FirebaseManager.instance.rankInfos.Count)
                ranks[i].SetInfo(FirebaseManager.instance.rankInfos[i]);
            else
                ranks[i].SetInfo();
        }
        print(4);
        gameObject.SetActive(true);
    }


    void OnClick_ExitBtn()
    {
        SceneManager.instance.PanelOn(SceneManager.HOME.home);
    }
}
