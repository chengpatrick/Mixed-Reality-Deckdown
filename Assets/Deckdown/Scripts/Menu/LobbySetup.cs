using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Discover;
using Discover.Networking;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

public class LobbySetup : NetworkBehaviour
{
    [SerializeField] private NetworkRunner m_networkRunner;

    [SerializeField] GameObject NetworkCube;
    [SerializeField] Vector3 SpawnCubePosition;

    [SerializeField] GameObject NetworkCapturePointCube;
    [SerializeField] Vector3 SpawnCapturePointCubePosition;

    [SerializeField] GameObject MonsterHitbox;
    [SerializeField] Transform MonsterHitboxTransform;
    [SerializeField] Transform PlayerLeftHandTransform;

    [SerializeField] GameObject PlayerHead;

    [SerializeField] LobbySharedData LobbyShared;

    [SerializeField] GameObject MonsterSelectionTable;

    [SerializeField] OVRSceneManager m_ovrSceneManager;
    [SerializeField] WaveFunctionCollapse WFC;

    //hp bar manager
    [SerializeField] HealthManager HPManager;

    // lobby status panels
    [SerializeField] GameObject WaitForPlayerPanel;
    [SerializeField] GameObject WaitForScanScrollPanel;
    [SerializeField] GameObject WaitForTapCard;

    private NetworkObject m_playerHeadRef;
    private NetworkObject m_playerHead;
    private NetworkObject m_monsterHitboxRef;
    private HealthHandler m_healthHandler;
    public void StartSetup()
    {
        // Spawn game starting cube
        // _ = m_networkRunner.Spawn(NetworkCube, SpawnCubePosition, Quaternion.identity);

        //switch to connected
        PanelController.Instance.UpdatePanelState(GameState.Connected);

        // Spawn monster hitbox for player
        var spawnMonsterHitbox = m_networkRunner.Spawn(MonsterHitbox, MonsterHitboxTransform.position, Quaternion.identity);
        spawnMonsterHitbox.transform.position = MonsterHitboxTransform.position;
        spawnMonsterHitbox.transform.rotation = MonsterHitboxTransform.rotation;
        spawnMonsterHitbox.transform.parent = MonsterHitboxTransform;

        // Spawn head for player
        var spawnPlayerHead = m_networkRunner.Spawn(PlayerHead, Vector3.zero, Quaternion.identity);
        spawnPlayerHead.transform.position = MonsterHitboxTransform.position;
        spawnPlayerHead.transform.rotation = MonsterHitboxTransform.rotation;
        spawnPlayerHead.transform.parent = MonsterHitboxTransform;

        // Set network runner
        spawnMonsterHitbox.GetComponent<HealthHandler>().NetworkRunner = m_networkRunner;
        spawnMonsterHitbox.GetComponent<PlayerSetupHandler>().NetworkRunner = m_networkRunner;

        // Set left hand anchor for monster follow
        spawnMonsterHitbox.GetComponent<MonsterFollowOffset>().SetLeftHandAnchor(PlayerLeftHandTransform);

        if (spawnMonsterHitbox == null)
        {
            Log.Error("Runner not able to spawn head");
        }

        m_playerHeadRef = spawnMonsterHitbox;
        m_monsterHitboxRef = spawnMonsterHitbox;
        m_playerHead = spawnPlayerHead;

        // Setup healthHandler 
        m_healthHandler = spawnMonsterHitbox.GetComponent<HealthHandler>();
        m_healthHandler.NetworkRunner = m_networkRunner;
        m_healthHandler.SetupHPmanager(HPManager);

        LoadOVRSceneManager();
        // StartCoroutine(FillRoom());
    }

    public void SetMonsterType(int type)
    {
        m_healthHandler.SetMonsterTypeRPC(type);
    }

    public void MonsterSelectionSetup()
    {
        int currPlayerCount = m_networkRunner.SessionInfo.PlayerCount;

        LobbyShared.SetRefRpc(m_playerHeadRef, HasStateAuthority);
        LobbyShared.SetHeadRefRpc(m_playerHead, HasStateAuthority);

        // Start Monster Selection if there are at least 2 players in room
        // if(currPlayerCount == 1)
        // {

        // }
        // m_playerHeadRef.GetComponent<PlayerSetupHandler>().SetHostPosRpc(HostLocation);
        if (currPlayerCount == 2)
        {
            DebugPanel.Instance.UpdateMessage("Pass condition");
            _ = StartCoroutine(SpawnMonsterTableCoroutine(m_playerHeadRef));
        }
    }

    public NetworkObject GetMonsterHeadRef()
    {
        return m_playerHeadRef;
    }

    private void LoadOVRSceneManager()
    {
        if (m_ovrSceneManager != null)
        {
            m_ovrSceneManager.gameObject.SetActive(true);
        }
    }

    private IEnumerator FillRoom()
    {
        DebugPanel.Instance.UpdateMessage("WFC: Starting Coroutine");
        yield return new WaitForSeconds(10f);
        DebugPanel.Instance.UpdateMessage("WFC: Starting Fill");

        if(WFC != null)
            WFC.StartWFC();
    }

    private IEnumerator SpawnMonsterTableCoroutine(NetworkObject spawnPlayerHead)
    {
        yield return new WaitForSeconds(3f);
        if (spawnPlayerHead.GetComponent<PlayerSetupHandler>() == null)
            DebugPanel.Instance.UpdateMessage("Player Setup Handler Null");

        if(MonsterSelectionTable != null)
        {
            spawnPlayerHead.GetComponent<PlayerSetupHandler>().SpawnMonsterSelectionTableRpc(LobbyShared.GetOppLocation(HasStateAuthority));
            yield return new WaitForSeconds(2f);
            MonsterSelectionTable = null;
        }
    }
}
