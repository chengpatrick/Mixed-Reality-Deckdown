using System.Collections;
using System.Collections.Generic;
using Assets.OVR.Scripts;
using ExitGames.Client.Photon;
using Fusion;
using UnityEngine;
using TMPro; 

public class PlayerManager : NetworkBehaviour
{
    [SerializeField] NetworkRunner NetworkRunner;
    [SerializeField] GameObject winUI, loseUI, holdUI;
    [SerializeField] TextMeshProUGUI soulCountText;
    [SerializeField] PlayerStatsRecorder psRecorder;

    [Networked] public int loserState { get; set; } = 0;
    [SerializeField] int winCondition = 3;
    public static PlayerManager Instance { get; private set; }

    private int myID; 
    private int currHealth;
    private int maxHealth = 2;
    private int soulCount = 0;
    private bool canCheckState = true;
    [SerializeField] bool canPoseVictory = false;
    private WinHoldController WHC;


    // Start is called before the first frame update
    void Start()
    {
        WHC = gameObject.GetComponent<WinHoldController>();
        soulCountText.text = "0";

        if (Instance == null)
        {
            Instance = this;

        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetupPlayerID()
    {
        if (HasStateAuthority)
        {
            // DebugPanel.Instance.UpdateMessage("WIN ID IS 1");
            myID = 1;
        }
        else
        {
            // DebugPanel.Instance.UpdateMessage("WIN ID IS " + NetworkRunner.SessionInfo.PlayerCount);
            myID = NetworkRunner.SessionInfo.PlayerCount;
        }
    }

    public int SetAndReturnPlayerID()
    {
        if (HasStateAuthority)
        {
            // DebugPanel.Instance.UpdateMessage("WIN ID IS 1");
            myID = 1;
            return myID;
        }
        else
        {
            // DebugPanel.Instance.UpdateMessage("WIN ID IS " + NetworkRunner.SessionInfo.PlayerCount);
            myID = NetworkRunner.SessionInfo.PlayerCount;
            return myID;
        }
    }

    public int GetMyID()
    {
        return myID;
    }

    public void SoulCollection()
    {
        soulCount++;
        soulCountText.text = soulCount.ToString();
        //DebugPanel.Instance.UpdateMessage("soul collected: " + soulCount);
        if (soulCount >= winCondition && loserState == 0 && canCheckState)
        {
            CheckWinStateRpc();
            SetWinStateRpc(myID);
            
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)] 
    public void SetWinStateRpc(int playerID)
    {
        // DebugPanel.Instance.UpdateMessage("Setting Win State to " + playerID);
        loserState = playerID;
        // DebugPanel.Instance.UpdateMessage("Winstate sat, to " + loserState);
        canCheckState = false;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void CheckWinStateRpc()
    {
        if (loserState == 0)
        {
            // DebugPanel.Instance.UpdateMessage("loserState is 0");
        }
        else if (loserState == myID)
        {
            // DebugPanel.Instance.UpdateMessage("YOU LOSE");
            //StartCoroutine(ShowGameObject(loseUI, 5));
            psRecorder.SetScoreboard(10f);
            Invoke("EnableLoseUI", 10.5f);
        }
        else if (loserState != myID)
        {
            // DebugPanel.Instance.UpdateMessage("YOU WIN");
            //StartCoroutine(ShowGameObject(winUI, 5));
            psRecorder.SetScoreboard(10f);
            Invoke("EnableVictoryHold", 10.5f);
        }
    }

    private IEnumerator ShowGameObject(GameObject gj, float delay)
    {
        gj.SetActive(true);
        yield return new WaitForSeconds(delay);
        gj.SetActive(false);
    }

    public void FillVictoryBar()
    {
        if (WHC != null && canPoseVictory)
        {
            WHC.FillBar();
        }
    }

    private void EnableVictoryHold()
    {
        holdUI.SetActive(true);
        canPoseVictory = true;
    }

    private void EnableLoseUI()
    {
        loseUI.SetActive(true);
    }
}
