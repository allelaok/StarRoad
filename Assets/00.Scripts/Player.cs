using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField]
    TMPro.TMP_Text scoreText;
    [SerializeField]
    TMPro.TMP_Text bestScoreText;
    [SerializeField]
    Image[] tornadoImg;
    [SerializeField]
    Transform subwayPool;
    [SerializeField]
    Transform tornado;
    [SerializeField]
    Image arrow;
    [SerializeField]
    GameObject camMoveBG;
    [SerializeField]
    Transform heartPositions;
   
    public Image[] lifeImg;
    int lifeCnt;

    public Animator anim;

    Camera cam;
    int dieCnt;
    int tornadoCnt;
    //int score;
    int point = 1;
    List<Transform> hearts = new List<Transform>();
    List<int> targetIndx = new List<int>();
    SpriteRenderer animSprite;
    Transform[] subways;
    GameObject heartPool;
    Vector3 initSize;
  public  int inverse = 1;
  public List<Vector3> points = new List<Vector3>();
  public static  STATE state;
    Transform nowTarget;
   public Transform target;
    Transform[] heartPos;
    int beforePosIndx = 0;
    int idx;
    float inverseTime = 5f;
    float inversedTime;

    private void Start()
    {
        state = STATE.Defualt;
        animSprite = GetComponentInChildren<SpriteRenderer>();
        //PlayerAndHeart.Add(transform);
        inversedTime = inverseTime;
        heartPos = heartPositions.GetComponentsInChildren<Transform>();
        subways = subwayPool.GetComponentsInChildren<Transform>();
        cam = GetComponentInChildren<Camera>();
        //anim = GetComponentInChildren<Animator>();
        initSize = transform.localScale;
        lifeCnt = lifeImg.Length;
    }

    private void Update()
    {
        switch (state)
        {
            case (STATE.Defualt):
                break;
            case (STATE.Start):
                break;
            case (STATE.Play):
                Move2();
                InverseTime();
                SetHeartsPosition(1, 0);
                if (nowTarget)
                {
                    Vector3 dir = nowTarget.position - transform.position;
                    int i = 1;
                    if (dir.x > 0)
                    {
                        i = -1;
                    }
                    float ang = Vector3.Angle(dir, Vector3.up);
                    arrow.transform.rotation = Quaternion.Euler(new Vector3(0, 0, ang * i));
                }
                break;
            case (STATE.CamMove):
                CamMove();
                break;
            case (STATE.Tornado):
                Tornado();
                break;
            case (STATE.Ready):
                Ready();
                break;
            case (STATE.Die):
                break;
            case (STATE.GameOver):
                break;
        }
    }

    private void LateUpdate()
    {
        cam.transform.localEulerAngles = -transform.eulerAngles;
    }

    public void Ready()
    {
        tornado.gameObject.SetActive(false);
        for (int i = 0; i < lifeCnt; i++)
        {
            lifeImg[i].enabled = true;
        }

        for (int i = 0; i < GameManager.instance.TornadoCnt; i++)
        {
            tornadoImg[i].enabled = true;
        }
        scoreText.text = "0";
        initPos();
        DestroyAllHearts();
        CreateHeart();
        tornadoCnt = GameManager.instance.TornadoCnt;
        dieCnt = 0;
        bestScoreText.text = GameManager.instance.BestScore.ToString();
        targetIndx.Clear();
        points.Clear();
        points.Add(transform.position);
        GameManager.instance.Score = 0;
        inverse = 1;
        camMoveBG.SetActive(false);
        GameManager.instance.Speed = GameManager.instance.BaseSpeed;
        state = STATE.Play;
    }


    void Move()
    {
        transform.position += transform.up * GameManager.instance.Speed * Time.deltaTime;

        if (target)
        {
            if (Vector3.Distance(transform.position, target.position) > 0.5f)
            {
                target = null;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.A) || GameManager.instance.turnLeft)
            {
                transform.rotation *= Quaternion.Euler(new Vector3(0, 0, inverse * 45));
                points.Add(transform.position);

                GameManager.instance.turnLeft = false;
            }
            else if (Input.GetKeyDown(KeyCode.D) || GameManager.instance.turnRight)
            {
                transform.rotation *= Quaternion.Euler(new Vector3(0, 0, -inverse * 45));
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

    [SerializeField] Transform controller;
    void Move2()
    {
        transform.position += transform.up * GameManager.instance.Speed * Time.deltaTime;

        if (target)
        {
            if (Vector3.Distance(transform.position, target.position) > 0.5f)
            {
                target = null;
            }
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
    float interval = 0.95f;
    void SetHeartsPosition(int pointIdx, int heartIdx, bool pointDis = true, float tmpDis = 0)
    {
        if(points.Count == 0)
        {
            // ???? ???? ???????? ??????
            for (int i = 0; i < hearts.Count; i++)
                hearts[i].position = Vector3.one * -1;
        }
        else if (points.Count == 1)
        {
            float dis = Vector3.Distance(transform.position, points[0]);
            float gap = 0;
            // ???????? ???????? ???? ???? ????
            for (int i = 0; i < hearts.Count; i++)
            {
                gap += GameManager.instance.Interval * Mathf.Pow(interval, i + 1);
                if (dis > gap)
                {
                    hearts[i].position = transform.position - transform.up * gap;
                    hearts[i].up = transform.up;
                }
            }
        }
        else if (points.Count > pointIdx - 1 && hearts.Count > heartIdx)
        {
            // ???? ???????? ???? ???????? ????
            float dis;
            if (pointDis)
            {
                if (pointIdx == 1)
                    dis = Vector3.Distance(transform.position, points[points.Count - pointIdx]);
                else
                    dis = Vector3.Distance(points[points.Count - pointIdx + 1], points[points.Count - pointIdx]);
            }
            else
            {
                dis = Vector3.Distance(hearts[heartIdx - 1].position, points[points.Count - pointIdx]);
            }

            // ?????? ?????? ????
            if (dis < GameManager.instance.Interval * Mathf.Pow(interval, (heartIdx)) - tmpDis)
            {
                pointIdx++;
                SetHeartsPosition(pointIdx, heartIdx, true, dis + tmpDis);
            }
            // ?????? ?????? ????
            else
            {
                Vector3 pos;
                Vector3 up;
                if (pointDis)
                {
                    if (pointIdx == 1)
                    {
                        pos = Vector3.Lerp(transform.position, points[points.Count - pointIdx], (GameManager.instance.Interval * Mathf.Pow(interval, (heartIdx)) - tmpDis) / dis);
                        up = transform.position - points[points.Count - pointIdx];
                    }
                    else
                    {
                        pos = Vector3.Lerp(points[points.Count - pointIdx + 1], points[points.Count - pointIdx], (GameManager.instance.Interval * Mathf.Pow(interval, (heartIdx)) - tmpDis) / dis);
                        up = points[points.Count - pointIdx + 1] - points[points.Count - pointIdx];

                    }
                }
                else
                {
                    pos = hearts[heartIdx - 1].position - hearts[heartIdx - 1].up * GameManager.instance.Interval * Mathf.Pow(interval, (heartIdx));
                    up = hearts[heartIdx - 1].up;
                }

                // ???? ????
                hearts[heartIdx].position = pos;
                hearts[heartIdx].up = up;
                heartIdx++;
                if (hearts.Count > heartIdx)
                    SetHeartsPosition(pointIdx, heartIdx, false);
                else
                {
                    // ?????? ?????? ????
                    for (int i = points.Count - pointIdx - 1; i > -1; i--)
                    {
                        points.RemoveAt(i);
                    }
                }
            }
        }
    }
    void Tornado()
    {
        if (tornado.gameObject.activeSelf == false)
            tornado.gameObject.SetActive(true);

        transform.Rotate(new Vector3(0,0, 300 * Time.deltaTime));
        transform.localScale -= Vector3.one * GameManager.instance.BaseSpeed * Time.deltaTime;
        if (transform.localScale.x < 0.1f)
        {
            transform.localScale = initSize;
            tornado.gameObject.SetActive(false);
            SelectSubway();
        }
    }

    void SelectSubway(Transform beforeSubway = null)
    {
        transform.GetComponentInChildren<SpriteRenderer>().enabled = false;

        int subwayIdx = Random.Range(1, subways.Length);
        if (beforeSubway != null)
        {
            while (beforeSubway == subways[subwayIdx])
            {
                subwayIdx = Random.Range(1, subways.Length);
            }
        }
        target = subways[subwayIdx];
        state = STATE.CamMove;
    }

    void CamMove()
    {
        transform.position += (target.position - transform.position).normalized * GameManager.instance.Speed * 10 * Time.deltaTime;

        if (Vector3.Distance(transform.position, target.position) < 0.5f)
        {
            transform.position = target.position;
            Vector3 ang = target.eulerAngles;
            transform.eulerAngles = ang;
            points.Add(target.position);
            transform.GetComponentInChildren<SpriteRenderer>().enabled = true;
            state = STATE.Play;
            camMoveBG.SetActive(false);
        }
        else if (Vector3.Distance(transform.position, target.position) < 2f)
        {
            camMoveBG.SetActive(true);
        }
    }

    void initPos()
    {

        controller.rotation = Quaternion.identity;
        inverse = 1;
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        state = STATE.Play;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (target == collision.transform)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Subway") || collision.gameObject.layer == LayerMask.NameToLayer("Stair"))
            {
                    target = null;
                    return;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (state != STATE.Play) return;
        // ???? ??
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            DestroyAllHearts();
            dieCnt++;
            lifeImg[lifeCnt - dieCnt].enabled = false;
            GameManager.instance.Speed = GameManager.instance.BaseSpeed;
            SoundManager.instance.CollSound();

            if (dieCnt < lifeCnt)
            {

                CreateHeart();
                state = STATE.Defualt;
                initPos();
            }
            else
            {
                GameOver();
            }
        }
        // ????
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Heart"))
        {

            state = STATE.Pop;

            hearts.Add(collision.transform);
            // ???? ????
            GameManager.instance.Score += point;
            scoreText.text = GameManager.instance.Score.ToString();
            if (hearts.Count < 10)
            {
                GameManager.instance.Speed = GameManager.instance.BaseSpeed + (hearts.Count + 1) * GameManager.instance.IntervalSpeed;
            }
            if (GameManager.instance.Score > GameManager.instance.BestScore)
            {
                GameManager.instance.BestScore = GameManager.instance.Score;
                bestScoreText.text = GameManager.instance.Score.ToString();
            }

            CreateHeart();
            CreatePop(collision.transform);
        }
        // ????
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Tornado"))
        {
            inversedTime = 0;
            inverse = -1;
            GameManager.instance.SetSpring(collision.transform);
        }
        // ??????
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Subway"))
        {
            if (target == collision.transform)
            {
                target = null;
                return;
            }
            points.Clear();
            SelectSubway(collision.transform);
        }
        // ????
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Stair"))
        {
            if (target == collision.transform)
            {
                target = null;
                return;
            }
            Stair stair = collision.transform.GetComponent<Stair>();
            transform.GetComponentInChildren<SpriteRenderer>().enabled = false;
            target = stair.otherStair.transform;
            points.Clear();

            state = STATE.CamMove;
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Poison"))
        {
            inversedTime = 0;
            inverse = -1;
            //anim.SetInteger("Inverse", inverse);
            GameManager.instance.SetSpring(collision.transform);
        }
    }
    void CreatePop(Transform pet)
    {
        GameObject popAnimPbj = Instantiate(Resources.Load<GameObject>("PopAnim"));
        popAnimPbj.transform.position = pet.position;
        popAnimPbj.transform.rotation = transform.rotation;
        PopAnim popAnim = popAnimPbj.AddComponent<PopAnim>();
        popAnim.pet = pet.gameObject;

        popAnim.first = !(hearts.Count > 1);
    }


    void InverseTime()
    {
        if (inverse == 1) 
        {
            inverseTime = 5f;
            animSprite.color = new Color(1, 1, 1, 1);
            //anim.SetInteger("Inverse", inverse);
            return;
        }

        inversedTime += Time.deltaTime;

        if (inversedTime >= inverseTime)
        {
            animSprite.color = new Color(1, 1, 1, 1);
            inverse = 1;
            //anim.SetInteger("Inverse", inverse);
        }
        else
        {
            if (inversedTime % 0.5f < 0.25f)
                animSprite.color = new Color(1, 1, 1, 0.5f);
            else
                animSprite.color = new Color(1, 1, 1, 1);
            inversedTime += Time.deltaTime;
        }
    }


    void DestroyAllHearts()
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            Destroy(hearts[i].gameObject);
        }
        if (nowTarget)
        {
            Destroy(nowTarget.gameObject);
        }
        hearts.Clear();
    }

    void CreateHeart()
    {
        idx = Random.Range(1, heartPos.Length);
        while (idx == beforePosIndx)
        {
            idx = Random.Range(1, heartPos.Length);
        }
        beforePosIndx = idx;

        if (heartPool == null || heartPool.transform.childCount <= 0)
            heartPool = Instantiate(Resources.Load<GameObject>("HeartPool"));

        Transform heartPrefab = heartPool.transform.GetChild(0);
        heartPrefab.SetParent(null, true);
        heartPrefab.position = heartPos[idx].position;
        heartPrefab.rotation = Quaternion.identity;
        nowTarget = heartPrefab;
    }

  
    void GameOver()
    {
        SoundManager.instance.BGM((int)Sound.BGM2);
        state = STATE.Defualt;
        SceneManager.instance.LoadingPanelOn();
        FirebaseManager.instance.SaveScore();
    }

}
