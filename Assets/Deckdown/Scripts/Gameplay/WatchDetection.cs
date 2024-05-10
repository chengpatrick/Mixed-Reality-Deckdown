using System.Collections;
using System.Collections.Generic;
using Fusion;
using Oculus.Interaction;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class WatchDetection : NetworkBehaviour
{
    [SerializeField] NetworkRunner NetworkRunner;
    [SerializeField] PlayCard sceneCardManage;
    [SerializeField] VFXmanager sceneVfxManage;
    [SerializeField] UnityEngine.UI.Image watchUIRef;
    [SerializeField] Transform headAnchor;
    [SerializeField] GameObject MenuPanel;
    [SerializeField] GameObject StatusPanel;
    [SerializeField] LobbySetup lobbySetup; 

    [SerializeField] GameObject throwMonster;
    [SerializeField] Transform wristAnchor;
    [SerializeField] IInteractor rightHandGrabInteractor;
    [SerializeField] MonsterProjectile monsterProjectile;

    [SerializeField] EnemyHealthbar enemyHP;

    public int CardType;
    public List<MonsterBootstrapper> monsterBootstrappers = new List<MonsterBootstrapper>(); //earth 0, fire 1, water 2

    private bool hostTouch = false, clientTouch = false;

    [Networked]
    private bool hostRead { get; set; } = false;

    [Networked]
    private bool clientRead {get; set;} = false;

    private bool canSetupHealthbar = true; 

    private GameObject table;

    private void Update()
    {
        if(hostRead && clientRead && canSetupHealthbar)
        {
            canSetupHealthbar = false;
            enemyHP.SetupEnemyHealthbar();
            hostRead = false;
            clientRead = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Card")
        {
            if (other.gameObject.transform.parent.GetComponent<Card>().isGrabbed)
            {
                PanelController.Instance.UpdatePanelState(GameState.CardScanDone);
                SoundManager.Instance.PlaySFXClip("SFX_TapCard");

                CardType = other.gameObject.transform.parent.GetComponent<Card>().cardData.type;

                DebugPanel.Instance.UpdateMessage("Type is " + CardType);
                MonsterBootstrapper currMonster = monsterBootstrappers[CardType];
                currMonster.SetupManagers(sceneCardManage, sceneVfxManage, watchUIRef);

                currMonster.ApplyBootstrap();
                NetworkRunner.Despawn(other.gameObject.transform.parent.GetComponent<NetworkObject>());

                WatchReadMonsterRpc(HasStateAuthority);

                table = other.gameObject.transform.parent.parent.gameObject;
                table.SetActive(false);

                StartCoroutine(PanelDelay());

                //setup monster hitbox type
                lobbySetup.SetMonsterType(CardType);
            }
        }
        else if (other.tag == "Scroll")
        {
            DebugPanel.Instance.UpdateMessage("Touched Scroll");
            SoundManager.Instance.PlaySFXClip("SFX_Deng");
            if (HasStateAuthority && !hostTouch)
            {
                other.gameObject.GetComponent<ScrollSetup>().HostWatchTouchRpc(HasStateAuthority);
                hostTouch = true;
            }
            else if(!HasStateAuthority && !clientTouch)
            {
                other.gameObject.GetComponent<ScrollSetup>().PlayerWatchTouchRpc(HasStateAuthority);
                clientTouch = true;
            }
            other.gameObject.GetComponent<ScrollSetup>().DebugScroll();
        }
    }

    private IEnumerator PanelDelay()
    {
        MenuPanel.SetActive(false);
        yield return new WaitForSeconds(7f);
        StatusPanel.SetActive(true);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void WatchReadMonsterRpc(bool hasAuth)
    {
        if (hasAuth)
        {
            hostRead = true;
        }
        else
        {
            clientRead = true;
        }
        DebugPanel.Instance.UpdateMessage("Host Read is " + hostRead + " client read is " + clientRead);
    }

    public bool CheckForSelection()  //if both players have selected monsters, return true
    {
        return hostRead && clientRead;
    }

    private void ThrowMonsterSetup()
    {
        var monster = NetworkRunner.Spawn(throwMonster, Vector3.zero, Quaternion.identity);
        monster.GetComponent<MonsterCollectPickUp>().WristAnchorSetup(wristAnchor);
    }
}
