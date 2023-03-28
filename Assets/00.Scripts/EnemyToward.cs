using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyToward : MonoBehaviour
{
    [SerializeField]
    EnemyToward[] nexts;

    public EnemyToward Next(EnemyToward before)
    {
        int i = Random.Range(0, nexts.Length);

        if (before == nexts[i])
        {
            return Next(before);
        }
        else
        {
            print(before.gameObject.name);
            print(nexts[i].gameObject.name);
            print("=======================");
            return nexts[i];
        }
    }
}
