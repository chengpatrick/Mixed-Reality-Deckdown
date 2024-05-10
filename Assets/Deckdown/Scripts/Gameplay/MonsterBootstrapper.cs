using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterBootstrapper : MonoBehaviour
{
    [SerializeField] int monsterType; 
    //reference to all the launchers/targets
    [SerializeField] PlayCard myCardManager;
    [SerializeField] VFXmanager myVFXmanager;
    private Image myWatchUI; 

    private SoulHandler myHead; 

    //projectile prefab setup
    [SerializeField] GameObject BallPrefab;
    [SerializeField] GameObject BlockPrefab;
    [SerializeField] GameObject MonsterCollectPrefab;
    [SerializeField] GameObject SoulPrefab;
    [SerializeField] GameObject SpecialAttackPrefab;

    //VFX prefab setup
    [SerializeField] GameObject vfxChargeup;
    [SerializeField] GameObject vfxHitBlock;
    [SerializeField] GameObject vfxHitWall;
    [SerializeField] GameObject vfxHitGround;

    //Monster-give-power prefab
    [SerializeField] GameObject myMonsterGivePower; 

    //aim assist color setup
    [SerializeField] Material AimAssistMaterial;

    //UI panel
    [SerializeField] Sprite statUIPanel; 

    // Start is called before the first frame update
    void Awake()
    {
        if (myCardManager == null)
        {
            myCardManager = FindObjectOfType<PlayCard>();
        }
        if (myVFXmanager == null)
        {
            myVFXmanager = FindObjectOfType<VFXmanager>();
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ApplyBootstrap()
    {
        //assuming every reference is drawn
        myCardManager.SetBallPrefab(BallPrefab);
        myCardManager.SetBlockPrefab(BlockPrefab);
        myCardManager.SetMonsterCapturePrefab(MonsterCollectPrefab);
        myCardManager.SetMonsterType(monsterType);
        myCardManager.SetSpecialPrefab(SpecialAttackPrefab);
        AimManager.Instance.SetAimAssistMaterial(AimAssistMaterial);

        //setup VFX for the player
        myVFXmanager.SetChargePrefab(vfxChargeup);
        myVFXmanager.SetBlockHitPrefab(vfxHitBlock);
        myVFXmanager.SetWallHitPrefab(vfxHitWall);
        myVFXmanager.SetGroundHitPrefab(vfxHitGround);
        //setup monster give power with VFXmanager
        myVFXmanager.SpawnGivePowerPrefab(myMonsterGivePower);
        //setup the status panel 
        myWatchUI.sprite = statUIPanel;

        //enable attacks after going into the game
        myCardManager.SetCanSpawn(true);
        myCardManager.SetCanSpawnShield(true);
    }

    public void SetupHead(SoulHandler sh)
    {
        myHead = sh; 
        sh.SetSoulPrefab(SoulPrefab);
    }

    public void SetupManagers(PlayCard cardManage, VFXmanager vfxManage, Image watchUI)
    {
        myCardManager = cardManage;
        myVFXmanager = vfxManage;
        myWatchUI = watchUI;
    }
}
