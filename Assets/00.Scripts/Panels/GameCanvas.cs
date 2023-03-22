using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCanvas : MonoBehaviour
{
    public EndPanel endPnl;
    // Start is called before the first frame update
    
    public void GamePanelOn()
    {
        endPnl.gameObject.SetActive(false);
        gameObject.SetActive(true);
    }
}
