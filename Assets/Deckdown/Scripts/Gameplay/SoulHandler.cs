using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class SoulHandler : NetworkBehaviour
{
    [SerializeField] public NetworkRunner NetworkRunner;

    [Networked]
    private byte SoulCount { get; set; }

    [SerializeField] GameObject SoulPrefab;

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void DropSoulsHostRpc(RpcInfo info = default)
    {
        //DebugPanel.Instance.UpdateMessage("Damage issue host");
        _ = NetworkRunner.Spawn(SoulPrefab, gameObject.transform.position, Quaternion.identity);
        _ = NetworkRunner.Spawn(SoulPrefab, gameObject.transform.position, Quaternion.identity);
        _ = NetworkRunner.Spawn(SoulPrefab, gameObject.transform.position, Quaternion.identity);
    }

    [Rpc(RpcSources.All, RpcTargets.InputAuthority)]
    public void DropSoulsClientRpc()
    {
        //DebugPanel.Instance.UpdateMessage("Damage issue client");
        _ = NetworkRunner.Spawn(SoulPrefab, gameObject.transform.position, Quaternion.identity);
    }

    [Rpc]
    public RpcInvokeInfo RpcFoo() 
    {
        return default;
    }

    public void SetSoulPrefab(GameObject prefab)
    {
        SoulPrefab = prefab;
    }
}
