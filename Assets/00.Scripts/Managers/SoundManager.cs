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
    }

    AudioSource source;
    public AudioSource bgm;
    public AudioClip[] clips;
    public void ClickBtnSound()
    {
        source.PlayOneShot(clips[(int)Sound.BtnClick]);
    }

    public void BGM(int num)
    {
        bgm.clip = clips[num];
        bgm.Play();
    }

   public void Coll()
    {
        source.PlayOneShot(clips[(int)Sound.Coll]);
    }
}
