using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.UI; 

public class HealthManager : NetworkBehaviour
{
    //[SerializeField] private PlayerManager playerManager;
    [SerializeField] NetworkRunner runner;
    [SerializeField] private Slider HPSlider;
    [SerializeField] GameObject HealthLostFilter;

    [SerializeField] private float maxHP = 5f;
    [SerializeField] private float minHP = 0f;

    // if there is health regen
    [SerializeField] private float regenInterval = 1f;  //how long every interval
    [SerializeField] private float regenAmountPerSec = 1f;

    // check when to cheer
    [SerializeField] CheerSoundHandler cheerSoundHandler;

    //spawn monster death VFX
    [SerializeField] GameObject monsterDeath;
    //reference to lobbySetup to manipulate monster head
    [SerializeField] LobbySetup lobbySetup; 

    private int playerID = 0;
    private bool canRegen = false;
    private float currHP = 5;
    private float regenPerInterval;



    // Start is called before the first frame update
    void Start()
    {
        SetHPbar();
        currHP = maxHP;
        UpdateStatus();
        //StartCoroutine(ManaRegen(regenInterval));
        regenPerInterval = regenAmountPerSec * regenInterval;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetHPbar()
    {
        HPSlider.maxValue = maxHP;
        HPSlider.minValue = minHP;
    }

    public void RegenerateHP()
    {
        currHP = Mathf.Clamp(currHP + regenPerInterval, minHP, maxHP);
        UpdateStatus();
        //Debug.Log("REGEN MANA: " + currMana);
    }

    public void SetCanRegenMana(bool b)
    {
        canRegen = b;
        if (canRegen)
        {
            StartCoroutine(HPRegen(regenInterval));
            StartCoroutine(ResetHPRegenState(12f));
        }
    }

    public bool ReductHP(float cost)
    {
        StartCoroutine(HealthLoseFilterTrigger(0.5f));

        if (currHP - cost > 0)
        {
            currHP = currHP - cost;
            SoundManager.Instance.PlaySFXClip("SFX_LoseHealth");
            DebugPanel.Instance.UpdateMessage("Played LoseHealth SFX");
            UpdateStatus();
            cheerSoundHandler.CheckCheerSide();

            DebugPanel.Instance.UpdateMessage("Losing health, current health: ");
            // Debug.Log("Losing health, current health: " + currHP);
            return true;
        }
        else
        {
            // DebugPanel.Instance.UpdateMessage("DEAD! curr health: " + currHP);
            currHP = 0;
            UpdateStatus();
            playerID = PlayerManager.Instance.SetAndReturnPlayerID();
            PlayerManager.Instance.SetWinStateRpc(playerID);
            PlayerManager.Instance.CheckWinStateRpc();
            cheerSoundHandler.CheckCheerSide();

            NetworkObject monsterRef = lobbySetup.GetMonsterHeadRef();
            runner.Spawn(monsterDeath, monsterRef.transform.position, Quaternion.identity);
            runner.Despawn(monsterRef);

            return false;
        }
    }

    public void UpdateStatus()
    {
        HPSlider.value = currHP;
    }

    public void AddCurrHP(float f)
    {
        currHP = Mathf.Clamp(currHP + f, minHP, maxHP);
        UpdateStatus();
    }

    public float GetCurrHealth()
    {
        return currHP;
    }

    public float GetMaxHealth()
    {
        return maxHP;
    }

    public int GetMonsterType()
    {
        return playerID;
    }

    IEnumerator HPRegen(float delay)
    {
        while (canRegen)
        {
            yield return new WaitForSeconds(delay);
            RegenerateHP(); // Regenerate [regenAmount] mana every [regenInterval] seconds
        }
    }

    IEnumerator ResetHPRegenState(float delay)
    {
        yield return new WaitForSeconds(delay);
        canRegen = false;
    }

        IEnumerator HealthLoseFilterTrigger(float t)
    {
        HealthLostFilter.SetActive(true);
        yield return new WaitForSeconds(t);
        HealthLostFilter.SetActive(false);
    }
}
