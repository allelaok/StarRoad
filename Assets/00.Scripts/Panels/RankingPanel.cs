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
        exitBtn.invokeMethod.AddListener(OnClick_ExitBtn);
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

            ranks[10].SetInfo();
            ranks[11].SetInfo();
            gameObject.SetActive(true);

        if (FirebaseManager.instance.targetRank.rank == 0)
            FirebaseManager.instance.targetRank = null;

        ranks[10].SetInfo(FirebaseManager.instance.targetRank);
        ranks[11].SetInfo(FirebaseManager.instance.myRank);
        gameObject.SetActive(true);
    }


    void OnClick_ExitBtn()
    {
        SceneManager.instance.PanelOn(SceneManager.HOME.home);
    }
}
