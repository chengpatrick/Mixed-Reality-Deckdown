using System.Collections;
using System.Collections.Generic;

using Discover.Networking;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Unity.Collections.AllocatorManager;

public class PlayCard : NetworkBehaviour
{
    [SerializeField] private NetworkRunner m_networkRunner;
    [SerializeField] private VFXmanager vfxmanager;
    [SerializeField] private ManaManager manamanager;
    [SerializeField] private LobbySharedData lobbyshareddata;
    [SerializeField] private AimManager aimManager;
    [SerializeField] private LobbySharedData lobbyData;

    [SerializeField] Transform attackAnchor;
    [SerializeField] Transform blockerAnchor;
    [SerializeField] Transform monsterAnchor;
    [SerializeField] Transform rightHandAnchor;
    [SerializeField] Transform shieldPanelAnchor;


    [SerializeField] GameObject BallPrefab;
    [SerializeField] GameObject BlockPrefab;
    [SerializeField] GameObject WatchPanel;

    [SerializeField] GameObject MonsterLaunchPrefab;
    [SerializeField] GameObject MonsterCollectPrefab;
    [SerializeField] GameObject SPPrefab;
    [SerializeField] float SPCoolDownTime;
    [SerializeField] Image CooldownBar;
    [SerializeField] GameObject SPReadyLogo;

    [SerializeField] List<GameObject> ChargeAttackHoldBar;
    [SerializeField] List<Image> ChargeAttackHoldBarImg;
    private bool CanChargeAttackHoldBar;
    private int ChargeAttackHoldIndex;

    // decide monster->attack type
    // earth 0, fire 1, water 2
    private int monsterType = -1;
    private string defaultAttackSFXName = "";

    private bool spCooldown = false;
    private bool startCooldown = false;
    private float cooldownTimer = 0;

    public List<GameObject> selectedAttacks = new List<GameObject>();
    private NetworkObject currentShield;
    private GameObject tutorialCurrentShield;
    public bool block = false;
    [SerializeField] int attackIndex = 0;

    private bool canSpawn = false;
    private bool canSpawnShield = false;

    private BlockProjectile currBP;
    private NetworkObject currentMonster;

    private NetworkObject currentAttack;

    private bool canTutorialAttack = false;
    private bool canTutorialBlock = false;
    private bool canTutorialSPAttack = false;

    public static PlayCard PC;

    private void Awake()
    {
        PC = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentAttack = null;
        cooldownTimer = 0;

        CanChargeAttackHoldBar = false;
        ChargeAttackHoldIndex = 0;

        // check if active scene if tutorial
        if(SceneManager.GetActiveScene().buildIndex == 0)
        {
            StartCoroutine(EnableTutorialPoses());
        }
    }

    // Update is called once per frame
    void Update()
    {
        CooldownBar.fillAmount = cooldownTimer / SPCoolDownTime;
        SPReadyLogo.SetActive(!startCooldown);

        // Debug.DrawRay(rightHandAnchor.position, -rightHandAnchor.up, Color.green);
        if (startCooldown && cooldownTimer < SPCoolDownTime)
        {
            cooldownTimer += Time.deltaTime;

            if (cooldownTimer > SPCoolDownTime)
                startCooldown = false;
        }

        if (CanChargeAttackHoldBar)
        {
            ChargeAttackHoldBarImg[ChargeAttackHoldIndex].fillAmount += Time.deltaTime;
        }
    }

    public void SetMonsterType(int i)
    {
        monsterType = i;
        SetMonsterTypeSFXNames();
        aimManager.SetMonsterType(i);
        startCooldown = true;
    }

    private void SetMonsterTypeSFXNames()
    {
        switch (monsterType)
        {
            case 0:
                defaultAttackSFXName = "SFX_VAGrassAttack";
                break;
            case 1:
                defaultAttackSFXName = "SFX_VAFireAttack";
                break;
            case 2:
                defaultAttackSFXName = "SFX_VAWaterAttack";
                break;
        }
    }

    public void LaunchCurrentAttack()
    {
        if (canSpawn)
        {
            var attack = m_networkRunner.Spawn(selectedAttacks[attackIndex], attackAnchor.position, attackAnchor.rotation);
            BallProjectile BP = attack.GetComponent<BallProjectile>();
            if (BP != null)
            {
                BP.SetBallSpeed(10);
            }
        }
    }

    public void LaunchChargeAttack()
    {
        //DebugPanel.Instance.UpdateMessage("CanSpawn: " + canSpawn);
        if (canSpawn && manamanager.UseMana(2f))
        {
            StartCoroutine(ChargeAttackHoldBarFill());

            //update scoreboard for attacks made
            PlayerStatsRecorder.Instance.UsedAttack();

            SoundManager.Instance.PlaySFXClip(defaultAttackSFXName);

            var attack = m_networkRunner.Spawn(BallPrefab, Vector3.zero, Quaternion.identity);
            attack.transform.position = attackAnchor.position;
            attack.transform.rotation = attackAnchor.rotation;
            BallProjectile BP = attack.GetComponent<BallProjectile>();


            if (BP != null)
            {
                //setting VFX 
                BP.SetNetworkRunner(m_networkRunner);
                BP.SetVFXmanager(vfxmanager);
                //basic settings 
                BP.SetCardManager(this);
                vfxmanager.SpawnChargeup(attackAnchor, attackAnchor);
                //set the enemy it's chasing
                SetAttackTarget(BP);

                BP.SetBallGrowth(true, attackAnchor, this);
                //enable aim
                AimManager.Instance.SetBallAimActive(true);
            }
        }
    }

    public void LaunchTutorialChargeAttack()
    {
        if (canSpawn && canTutorialAttack)
        {
            StartCoroutine(ChargeAttackHoldBarFill());

            SoundManager.Instance.PlaySFXClip(defaultAttackSFXName);

            var attack = Instantiate(BallPrefab, Vector3.zero, Quaternion.identity);
            attack.transform.position = attackAnchor.position;
            attack.transform.rotation = attackAnchor.rotation;
            BallProjectile BP = attack.GetComponent<BallProjectile>();


            if (BP != null)
            {
                BP.SetBallGrowth(true, attackAnchor, this);
            }
        }
    }


    public void SetAttackTarget(BallProjectile BP)
    {
        var enemyRef = lobbyshareddata.GetEnemyRef(HasStateAuthority);
        if (enemyRef != null)
        {
            BP.SetChaseTarget(enemyRef);
        }
    }

    public void LaunchBlocker()
    {

        if (currentShield == null && manamanager.UseMana(1f) && canSpawnShield)
        {
            block = true;

            SoundManager.Instance.PlaySFXClip("SFX_Umbrella");

            Debug.Log("spawn blocker");
            var blocker = m_networkRunner.Spawn(BlockPrefab, blockerAnchor.position, blockerAnchor.rotation);
            currentShield = blocker;
            blocker.transform.SetParent(blockerAnchor);
            currBP = blocker.GetComponent<BlockProjectile>();

            manamanager.SetCanRegenMana(false);

            WatchPanel.transform.RotateAround(WatchPanel.transform.position, shieldPanelAnchor.transform.right, 90);
            WatchPanel.transform.Rotate(new Vector3(0, 90f, 0));
            WatchPanel.transform.Rotate(new Vector3(90f, 0, 0));
            WatchPanel.transform.RotateAround(shieldPanelAnchor.transform.position, shieldPanelAnchor.transform.right, -90);
            StartCoroutine(ChannelBlocker(0.1f, 0.12f));
        }
    }

    public void LaunchTutorialBlocker()
    {

        if (tutorialCurrentShield == null && canTutorialBlock)
        {
            block = true;

            SoundManager.Instance.PlaySFXClip("SFX_Umbrella");

            var blocker = Instantiate(BlockPrefab, blockerAnchor.position, blockerAnchor.rotation);
            tutorialCurrentShield = blocker;
            blocker.transform.SetParent(blockerAnchor);
            currBP = blocker.GetComponent<BlockProjectile>();

            WatchPanel.transform.RotateAround(WatchPanel.transform.position, shieldPanelAnchor.transform.right, 90);
            WatchPanel.transform.Rotate(new Vector3(0, 90f, 0));
            WatchPanel.transform.Rotate(new Vector3(90f, 0, 0));
            WatchPanel.transform.RotateAround(shieldPanelAnchor.transform.position, shieldPanelAnchor.transform.right, -90);
        }
    }

    public void DisableBlocker()
    {
        // Debug.Log("disabling shield");
        if (currentShield != null)
        {
            manamanager.SetCanRegenMana(true);
            currentShield.Despawn();

            WatchPanel.transform.RotateAround(shieldPanelAnchor.transform.position, shieldPanelAnchor.transform.right, 90);
            WatchPanel.transform.Rotate(new Vector3(-90f, 0, 0));
            WatchPanel.transform.Rotate(new Vector3(0, -90f, 0));
            WatchPanel.transform.RotateAround(WatchPanel.transform.position, shieldPanelAnchor.transform.right, -90);

            // block = false;
        }
    }

    public void DisableTutorialBlocker()
    {
        if (tutorialCurrentShield != null)
        {
            Destroy(tutorialCurrentShield);

            WatchPanel.transform.RotateAround(shieldPanelAnchor.transform.position, shieldPanelAnchor.transform.right, 90);
            WatchPanel.transform.Rotate(new Vector3(-90f, 0, 0));
            WatchPanel.transform.Rotate(new Vector3(0, -90f, 0));
            WatchPanel.transform.RotateAround(WatchPanel.transform.position, shieldPanelAnchor.transform.right, -90);
        }
    }

    public void DisableBlockerDelay(float delay)
    {
        currBP.Folding();
        Invoke("DisableBlocker", delay);
    }

    public void DisableTutorialBlockerDelay(float delay)
    {
        currBP.Folding();
        Invoke("DisableTutorialBlocker", delay);
    }

    IEnumerator ChannelBlocker(float delay, float cost) //1
    {
        while (currentShield != null)
        {
            yield return new WaitForSeconds(delay);
            bool canAfford = manamanager.UseMana(cost); // use mana every interval
            if (!canAfford)
            {
                DisableBlockerDelay(0.2f);
                break;
            }
        }
        block = false;
    }

    public void PrepMonsterLaunch()
    {
        AimManager.Instance.SetKonAimActive(true);
    }

    public void LaunchMonsterCapture()
    {
        // Debug.Log("spawn monster to capture");
        var monster = m_networkRunner.Spawn(MonsterLaunchPrefab, monsterAnchor.position, monsterAnchor.rotation);
        //enable aim
        AimManager.Instance.SetKonAimActive(false);
    }

    public void DeployMonsterCapture()
    {
        if (currentMonster == null)
        {
            currentMonster = m_networkRunner.Spawn(MonsterCollectPrefab, monsterAnchor.position, monsterAnchor.rotation);
        }
    }

    public void SpawnAttackOrb()
    {
        if (canSpawn && manamanager.UseMana(2f) && currentAttack == null)
        {
            var attack = m_networkRunner.Spawn(BallPrefab, blockerAnchor.position, attackAnchor.rotation);
            attack.GetComponent<Rigidbody>().isKinematic = false;
            attack.GetComponent<BallProjectile>().SetCardManager(this);
            attack.GetComponent<BallProjectile>().SetNetworkRunner(m_networkRunner);
            currentAttack = attack;
            StartCoroutine(AttackOrbDelay(10f));
        }
    }

    public void SpawnSPAttack()
    {
        if (!spCooldown)
        {
            // play SP atk sfx
            SoundManager.Instance.PlaySFXClip("SFX_MenuSelect");

            Vector3 spawnPos = monsterAnchor.position;

            //udpate to scoreboard
            PlayerStatsRecorder.Instance.UsedSpecial();

            // If fire, spawn lava below enemy
            if (monsterType == 1)
            {
                spawnPos = lobbyshareddata.GetOppLocation(HasStateAuthority);
            }

            spawnPos.y = 0f;
            Quaternion spawnRot = Quaternion.Euler(0f, lobbyshareddata.GetMyHeadRef(HasStateAuthority).gameObject.transform.rotation.eulerAngles.y, 0f);
            var sp = m_networkRunner.Spawn(SPPrefab, spawnPos, spawnRot);


            sp.GetComponent<SpecialAttackHandler>().NetworkRunner = m_networkRunner;
            sp.GetComponent<SpecialAttackHandler>().SetType(monsterType);

            StartCoroutine(SPAttackCooldown(sp));

            if (monsterType == 2)
                sp.GetComponent<SpecialAttackHandler>().mm = manamanager;
        }
    }

    public void SpawnTutorialSPAttack()
    {
        if (!spCooldown && canTutorialSPAttack)
        {
            // play SP atk sfx
            SoundManager.Instance.PlaySFXClip("SFX_MenuSelect");

            Vector3 spawnPos = rightHandAnchor.position;

            spawnPos.y = .2f;
            var sp = Instantiate(SPPrefab, spawnPos, Quaternion.identity);
            
            StartCoroutine(TutorialSPCooldown());
        }
    }

    public void LaunchAttackOrb()
    {
        currentAttack.GetComponent<BallProjectile>().SetBallDirection(-rightHandAnchor.up);
        currentAttack.GetComponent<BallProjectile>().SetBallSpeed(1f);
    }



    private IEnumerator AttackOrbDelay(float t)
    {
        yield return new WaitForSeconds(t);
        if (currentAttack != null)
        {
            m_networkRunner.Despawn(currentAttack);
            Destroy(currentAttack);
            currentAttack = null;
        }
    }

    private IEnumerator SPAttackCooldown(NetworkObject obj)
    {
        spCooldown = true;
        cooldownTimer = 0;

        // duration that sp last
        float spDuration = obj.GetComponent<SpecialAttackHandler>().GetSPDuration();
        yield return new WaitForSeconds(spDuration);
        m_networkRunner.Despawn(obj);

        startCooldown = true;

        // turn infinite mana off
        manamanager.SetInfiniteMana(false);

        yield return new WaitForSeconds(SPCoolDownTime);
        startCooldown = false;
        spCooldown = false;
    }

    private IEnumerator ChargeAttackHoldBarFill()
    {
        ChargeAttackHoldIndex = Random.Range(0, ChargeAttackHoldBar.Count);
        ChargeAttackHoldBar[ChargeAttackHoldIndex].SetActive(true);
        CanChargeAttackHoldBar = true;
        ChargeAttackHoldBarImg[ChargeAttackHoldIndex].fillAmount = 0;
        yield return new WaitForSeconds(1.2f);
        CanChargeAttackHoldBar = false;
        ChargeAttackHoldBar[ChargeAttackHoldIndex].SetActive(false);
    }

    private IEnumerator EnableTutorialPoses()
    {
        yield return new WaitForSeconds(80);
        canTutorialBlock = true;
        yield return new WaitForSeconds(3);
        canTutorialAttack = true;
        yield return new WaitForSeconds(3);
        canTutorialSPAttack = true;
    }

    private IEnumerator TutorialSPCooldown()
    {
        spCooldown = true;
        yield return new WaitForSeconds(10);
        spCooldown = false;
    }

    public void SetBallPrefab(GameObject obj)
    { BallPrefab = obj; }

    public void SetBlockPrefab(GameObject obj)
    { BlockPrefab = obj; }

    public void SetMonsterLaunchPrefab(GameObject obj)
    { MonsterLaunchPrefab = obj; }

    public void SetMonsterCapturePrefab(GameObject obj)
    { MonsterCollectPrefab = obj; }

    public void SetSpecialPrefab(GameObject obj)
    { SPPrefab = obj; }

    public void SetCanSpawn(bool b)
    {
        canSpawn = b;
    }

    public void SetCanSpawnShield(bool b)
    {
        canSpawnShield = b;
    }
}
