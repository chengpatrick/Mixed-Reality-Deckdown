using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Start,
    Connected,
    ScrollAppeared,
    CardAppeared,
    CardScanDone,
    End
}

public class PanelController : MonoBehaviour
{
    private GameState gameState; 

    public static PanelController Instance { get; private set; }

    [SerializeField] GameObject selectPanel, lobbyStatus, scanWatchToOpen, tapCard, statusPanel;

    // Start is called before the first frame update
    void Start()
    {
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

    public void UpdatePanelState(GameState state)
    {
        gameState = state;

        switch(gameState)
        {
            case GameState.Start:
                break;
            case GameState.Connected:
                ShowLobbyStatus();
                break;
            case GameState.ScrollAppeared:
                ShowScanWatchToOpen();
                break;
            case GameState.CardAppeared:
                ShowTapCard();
                break;
            case GameState.CardScanDone:
                ShowStatusPanel();
                break;
            case GameState.End:
                statusPanel.SetActive(false);
                break;
            default:
                break;

        }
    }

    public void ShowLobbyStatus()
    {
        selectPanel.SetActive(false);
        lobbyStatus.SetActive(true);
    }

    public void ShowScanWatchToOpen()
    {
        lobbyStatus.SetActive(false);
        scanWatchToOpen.SetActive(true);
    }

    public void ShowTapCard()
    {
        scanWatchToOpen.SetActive(false);
        tapCard.SetActive(true);
    }

    public void ShowStatusPanel()
    {
        tapCard.SetActive(false);
        // statusPanel.SetActive(true);
    }
}
