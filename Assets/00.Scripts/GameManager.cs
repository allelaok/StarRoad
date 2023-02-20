using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum STATE
{
    Ready,
    Start,
    Play,
    Defualt,
    CamMove,
    Tornado,
    Subway,
    Die,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
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
    }
    public float Speed { get { return speed; } set { speed = value; } }

    [SerializeField]
    float speed = 2f;
    [HideInInspector]
    public bool turnLeft;
    [HideInInspector]
    public bool turnRight;
    [HideInInspector]
    public bool tornado;
    public void OnClickLeftBtn()
    {
        turnLeft = true;
    }
    public void OnClickRightBtn()
    {
        turnRight = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
