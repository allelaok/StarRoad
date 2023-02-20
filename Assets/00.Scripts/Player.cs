using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }
    Transform target;
    public List<Vector3> points;
    int inverse = 1;
    int tornadoCnt;
    [SerializeField]
    Image[] tornadoImg;
    [HideInInspector]
    public STATE state;

    public void Move()
    {
        transform.position -= transform.up * GameManager.instance.Speed * Time.deltaTime;

        if (target)
        {
            if (Vector3.Distance(transform.position, target.position) > 2.5f)
            {
                target = null;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.A) || GameManager.instance.turnLeft)
            {
                transform.rotation *= Quaternion.Euler(new Vector3(0, 0, -1 * inverse * 45));
                points.Add(transform.position);

                GameManager.instance.turnLeft = false;
            }
            else if (Input.GetKeyDown(KeyCode.D) || GameManager.instance.turnRight)
            {
                transform.rotation *= Quaternion.Euler(new Vector3(0, 0, inverse * 45));
                points.Add(transform.position);

                GameManager.instance.turnRight = false;
            }
            else if (Input.GetKeyDown(KeyCode.Space) || GameManager.instance.tornado)
            {
                if (tornadoCnt > 0)
                {
                    tornadoImg[tornadoCnt - 1].enabled = false;
                    points.Clear();
                    state = STATE.Tornado;
                    tornadoCnt--;
                }
                GameManager.instance.tornado = false;
            }

        }
    }
}
