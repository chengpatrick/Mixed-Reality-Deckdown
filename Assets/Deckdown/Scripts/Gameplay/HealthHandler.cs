using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class HealthHandler : NetworkBehaviour
{
    [Networked] 
    public int monsterType { get; set; }

    [SerializeField] public NetworkRunner NetworkRunner;
    private HealthManager myHPManager;

    //[Networked]
    public List<GameObject> MonsterMeshes; //3 monster meshes: 0 nooki, 1 chochi, 2 kappi
    

    [Networked]
    public float CurrHealth { get; set; }

    [Networked]
    public float MaxHealth { get; set; }


    public override void FixedUpdateNetwork()
    {
        CurrHealth = GetCurrentHealth();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void LoseHealthHostRpc(float damage, RpcInfo info = default)
    {
        DebugPanel.Instance.UpdateMessage("Damage issue host");
        myHPManager.ReductHP(damage);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void LoseHealthClientRpc(float damage, RpcInfo info = default)
    {
        DebugPanel.Instance.UpdateMessage("Damage issue client");
        myHPManager.ReductHP(damage);
    }

    public void SetupHPmanager(HealthManager hpManager) { 
        myHPManager = hpManager;
        MaxHealth = GetMaxHealth();
    }

    public float GetCurrentHealth()
    {
        return myHPManager.GetCurrHealth();
    }

    public float GetMaxHealth()
    {
        return myHPManager.GetMaxHealth();
    }

    public bool CompareType(int type)
    {
        return type == monsterType;
    }

    public int GetMonsterType()
    {
        return monsterType;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void SetMonsterTypeRPC(int type)  //called when monsters have been selected 
    {
        monsterType = type;

        Invoke("EnableMonsterMesh", 7f);
    }

    private void EnableMonsterMesh()//enabling monster meshes
    {
        MonsterMeshes[monsterType].SetActive(true);
    }

    public void SetHealthRegen(bool b)
    {
        myHPManager.SetCanRegenMana(b);
    }
}
