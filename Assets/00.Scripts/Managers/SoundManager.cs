using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Sound
{
    BGM1,
    BGM2,
    BtnClick,
    Coll
}

[RequireComponent(typeof (AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        source = GetComponent<AudioSource>();
        bgm.volume = PlayerPrefs.GetFloat("bgmV");
        source.volume = PlayerPrefs.GetFloat("effectV");
    }

    AudioSource source;
    public AudioSource bgm;
    public AudioClip[] clips;

    public void SetBGMVolume(float bgmV)
    {
        bgm.volume = bgmV;
    }

    public void SetEffectVolume(float effectV)
    {
        source.volume = effectV;
    }



    public void ClickBtnSound()
    {
        source.PlayOneShot(clips[(int)Sound.BtnClick]);
    }

    public void BGM(int num)
    {
        bgm.clip = clips[num];
        bgm.Play();
    }

   public void CollSound()
    {
        source.PlayOneShot(clips[(int)Sound.Coll]);
    }
}
