using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BaseButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image image;
    [HideInInspector]
    public UnityEvent OnClickMethod;
    [HideInInspector]
    public UnityEvent OnEnterMethod;
    [HideInInspector]
    public UnityEvent OnExitMethod;

    public bool soundContents;

    private void Awake()
    {
        if (soundContents) return;

        if (image == null)
        {
            if (transform.parent != null)
            {
                image = GetComponentInParent<Image>();
            }
        }

        if (image)
            image.enabled = false;
    }

    private void Start()
    {
        if (soundContents)
        {
            if (image)
                image.enabled = true;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(OnEnterMethod != null)
        OnEnterMethod.Invoke();

        if (image && soundContents == false)
            image.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (image && soundContents == false)
            image.enabled = false;

        if (OnExitMethod != null)
            OnExitMethod.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SoundManager.instance.ClickBtnSound();
        if(OnClickMethod != null)
        OnClickMethod.Invoke();

        if (image && soundContents == false)
            image.enabled = false;
    }
}
