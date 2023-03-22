using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundPanel : BasePanel
{
    [SerializeField] GameObject bgmContent;
    [SerializeField] BaseButton bgmUp;
    [SerializeField] BaseButton bgmDown;
    [SerializeField] GameObject effectContent;
    [SerializeField] BaseButton effectUp;
    [SerializeField] BaseButton effectDown;
    Image[] bgms;    
    Image[] effects; 

    [SerializeField] BaseButton okBtn;
    [SerializeField] BaseButton exitBtn;

    // Start is called before the first frame update
    void Start()
    {
        okBtn.invokeMethod.AddListener(OnCLickSoundOKBtn);
        exitBtn.invokeMethod.AddListener(OnCLickSoundXBtn);

        bgmUp.invokeMethod.AddListener(delegate{ OnClick_BGMBtn(true); });
        bgmDown.invokeMethod.AddListener(delegate { OnClick_BGMBtn(false); });
        effectUp.invokeMethod.AddListener(delegate { OnClick_EffectBtn(true); });
        effectDown.invokeMethod.AddListener(delegate { OnClick_EffectBtn(false); });

        bgms = bgmContent.GetComponentsInChildren<Image>();
        effects = effectContent.GetComponentsInChildren<Image>();

        for (int i = 0; i < bgms.Length; i++)
        {
            float bgmv = i / 5f;
            bgms[i].gameObject.AddComponent<Button>().onClick.AddListener(delegate { SetBGMContents(bgmv); });
        }
        for (int i = 0; i < effects.Length; i++)
        {
            float effectV = i  / 5f;
            effects[i].gameObject.AddComponent<Button>().onClick.AddListener(delegate { SetEffectContents(effectV); });
        }

        if (PlayerPrefs.HasKey("bgmV"))
            bgmV = PlayerPrefs.GetFloat("bgmV");
        else bgmV = 1;

        if (PlayerPrefs.HasKey("effectV"))
            effectV = PlayerPrefs.GetFloat("effectV");
        else
            effectV = 1;

        SetPanel();
    }

    public void SetPanel()
    {
        SetBGMContents(bgmV);
        SetEffectContents(effectV);
    }

    public void SaveVolume()
    {
        PlayerPrefs.SetFloat("bgmV", bgmV);
        PlayerPrefs.SetFloat("effectV", effectV);
    }

    float bgmV;
    float effectV;
    public void OnClick_BGMBtn(bool up)
    {
        if (up)
            bgmV += 1f / bgms.Length;
        else
            bgmV -= 1f / bgms.Length;

        bgmV = Mathf.Clamp(bgmV, 0, 1);

        SetBGMContents(bgmV);
    }

    public void OnClick_EffectBtn(bool up)
    {
        if (up)
            effectV += 1f / effects.Length;
        else
            effectV -= 1f / effects.Length;

        effectV = Mathf.Clamp(effectV, 0, 1);

        SetEffectContents(effectV);
    }

    void SetBGMContents(float bgmV)
    {
        for (int i = 0; i < bgms.Length; i++)
        {
            bgms[i].gameObject.SetActive(i < Mathf.Round(bgmV * bgms.Length));
        }
        SoundManager.instance.SetBGMVolume(bgmV);
    }

    void SetEffectContents(float effectV)
    {
        for (int i = 0; i < effects.Length; i++)
        {
            effects[i].gameObject.SetActive(i < Mathf.Round(effectV * effects.Length));
        }
        SoundManager.instance.SetEffectVolume(effectV);
    }

    public void OnCLickSoundXBtn()
    {
        bgmV = PlayerPrefs.GetFloat("bgmV");
        effectV = PlayerPrefs.GetFloat("effectV");
        SetPanel();
        SceneManager.instance.PanelOn(SceneManager.HOME.setting);
    }
    public void OnCLickSoundOKBtn()
    {
        SaveVolume();
        SceneManager.instance.PanelOn(SceneManager.HOME.setting);
    }
}
