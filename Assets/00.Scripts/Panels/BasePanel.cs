using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePanel : MonoBehaviour
{
   public  virtual void SetActice(bool on)
    {
        gameObject.SetActive(on);
    }
}
