using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopAnim : MonoBehaviour
{
    public GameObject pet;
    public bool first;

    public void AfterPop()
    {
        print("��");
        Player.state = STATE.Play;

        if (first)
            pet.layer = LayerMask.NameToLayer("Default");
        else
            pet.layer = LayerMask.NameToLayer("Pet");

        Destroy(this.gameObject);
    }
}
