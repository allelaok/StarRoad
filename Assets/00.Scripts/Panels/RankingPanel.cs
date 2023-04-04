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
        if (on)
        {
            if (FirebaseManager.instance.InternetOn() == false)
            {
                SceneManager.instance.Popup("���ͳ� ���� �ȵ�");
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
        for (int i = 0; i < 10; i++)
        {
            if (i < FirebaseManager.instance.rankInfos.Count)
                ranks[i].SetInfo(FirebaseManager.instance.rankInfos[i]);
            else
                ranks[i].SetInfo();
        }
        gameObject.SetActive(true);
    }


    void OnClick_ExitBtn()
    {
        SceneManager.instance.PanelOn(SceneManager.HOME.home);
    }
}
