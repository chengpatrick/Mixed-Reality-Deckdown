using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PlayerSetupHandler : NetworkBehaviour
{
    [SerializeField] public NetworkRunner NetworkRunner;
    [SerializeField] private GameObject m_monsterSelectionTable;

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void SpawnMonsterSelectionTableRpc(Vector3 hostPos)
    {
        
        // DebugPanel.Instance.UpdateMessage("Host pos is " + hostPos + " client pos is " + transform.position);

        var midPoint = new Vector3 (transform.position.x + (hostPos.x - transform.position.x) / 2,
                                         0.5f,
                                            transform.position.z + (hostPos.z - transform.position.z) / 2);

        var table = NetworkRunner.Spawn(m_monsterSelectionTable, midPoint, Quaternion.identity);

        if(table == null)
        {
            DebugPanel.Instance.UpdateMessage("Table is null");
        }
        // DebugPanel.Instance.UpdateMessage("Table spawned at " + table.transform.position);
    }
}
