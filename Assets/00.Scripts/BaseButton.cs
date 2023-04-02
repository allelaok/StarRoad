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
[HideInInspector]
    public bool scaleEffect;

    private void Awake()
    {
        if (scaleEffect) return;

        if (image == null)
        {
            if (transform.parent != null)
            {
                image = GetComponentInParent<Image>();
            }
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if(OnEnterMethod != null)
        OnEnterMethod.Invoke();

        if(scaleEffect == false)
        {
            image.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (OnExitMethod != null)
            OnExitMethod.Invoke();


        if (scaleEffect == false)
        {
            image.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SoundManager.instance.ClickBtnSound();
        if(OnClickMethod != null)
        OnClickMethod.Invoke();

        if (scaleEffect == false)
        {
            image.transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
