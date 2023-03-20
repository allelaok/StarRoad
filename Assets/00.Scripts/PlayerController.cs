using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    Vector2 m_vecNormal;
    [SerializeField] Player player;

    void Start()
    {
        m_vecNormal = new Vector2(transform.position.x, transform.position.y);
    }

    Vector2 start;
    Quaternion startRot;
    Quaternion playerStartRot;
    public void OnDrag(PointerEventData eventData)
    {
        if (player.target == null)
        {
            Vector2 now = eventData.position - m_vecNormal;
            float ang = Vector2.Angle(now, start);
            Vector3 cross = Vector3.Cross(new Vector3(now.x, now.y, 0), new Vector3(start.x, start.y, 0));
            cross = cross.normalized;
            transform.rotation = startRot * Quaternion.Euler(new Vector3(0, 0, -cross.z * ang));
            player.transform.rotation = playerStartRot * Quaternion.Euler(new Vector3(0, 0, player.inverse * -cross.z * ang));

            player.points.Add(player.transform.position);
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        startRot = transform.rotation;
        playerStartRot = player.transform.rotation;
        start = eventData.position - m_vecNormal;
    }
}
