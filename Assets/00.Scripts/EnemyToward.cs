using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyToward : MonoBehaviour
{
    [SerializeField]
    EnemyToward[] nexts;

    public EnemyToward Next(Transform before)
    {
        int i = Random.Range(0, nexts.Length - 1);
        if(before == nexts[i].transform)
        {
            return Next(before);
        }
        else
         return nexts[i];
    }
}
