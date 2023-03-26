using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHeart : MonoBehaviour
{
    [SerializeField]
    Transform enemys;
    EnemyToward[] targets;

    EnemyToward target;
    EnemyToward before;

    private void Start()
    {
        targets = enemys.GetComponentsInChildren<EnemyToward>();
        RandomPosition();
    }

    public void RandomPosition()
    {
        int i = Random.Range(0, targets.Length - 1);
        before = targets[i];
        transform.position = before.transform.position;

        target = before.Next(before.transform);
        transform.up = target.transform.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(Player.state == STATE.Ready)
        {
            int i = Random.Range(0, targets.Length - 1);
            target = targets[i];

            transform.position = target.transform.position;
        }

        if (Player.state != STATE.Play) return;

        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, GameManager.instance.Speed * Time.deltaTime);
    }
    private void LateUpdate()
    {
        if (Player.state != STATE.Play) return;

        if (transform.position == target.transform.position)
        {
            before = target;
            target = target.Next(before.transform);

            transform.up = target.transform.position - transform.position;
        }
    }

}
