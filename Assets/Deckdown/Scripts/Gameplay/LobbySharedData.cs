using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class LobbySharedData : NetworkBehaviour
{
    [Networked]
    public Vector3 HostLocation { get; set; } = Vector3.zero;

    [Networked]
    public Vector3 ClientLocation { get; set; } = Vector3.zero;

    [Networked]
    public NetworkObject hostRef { get; set; }

    [Networked]
    public NetworkObject clientRef { get; set; }

    [Networked]
    public NetworkObject hostHeadRef { get; set; }

    [Networked]
    public NetworkObject clientHeadRef { get; set; }

    public override void FixedUpdateNetwork()
    {

    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void SetRefRpc(NetworkObject reference,bool hasStateAuth)
    {
        if (hasStateAuth)
        {
            hostRef = reference;
        }
        else
        {
            clientRef = reference;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void SetHeadRefRpc(NetworkObject reference, bool hasStateAuth)
    {
        if (hasStateAuth)
        {
            hostHeadRef = reference;
        }
        else
        {
            clientHeadRef = reference;
        }
    }

    public Vector3 GetOppLocation(bool hasStateAuth)
    {
        if (hasStateAuth)
        {
            return clientRef.transform.position;
        }
        else
        {
            return hostRef.transform.position;
        }
    }

    public NetworkObject GetMyHeadRef(bool hasStateAuth)
    {
        if (hasStateAuth)
        {
            return hostHeadRef;
        }
        else
        {
            return clientHeadRef;
        }
    }

    public NetworkObject GetEnemyRef(bool hasStateAuth)
    {
        if (hasStateAuth)
        {
            return clientRef;
        }
        else
        {
            return hostRef;
        }
    }

    public NetworkObject GetMyRef(bool hasStateAuth)
    {
        if (hasStateAuth)
        {
            return hostRef;
        }
        else
        {
            return clientRef;
        }
    }
}
