using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;

public class EnemyHealthbar : NetworkBehaviour
{
    [SerializeField] public NetworkRunner NetworkRunner;

    [SerializeField] private LobbySharedData lobbyshareddata;
    [SerializeField] private NetworkObject enemyReference; 
    [SerializeField] private Slider currHealthbar;
    [SerializeField] private GameObject healthbarPrefab;

    private bool startTracking = false;
    private HealthHandler enemyHH; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetupEnemyHealthbar()
    {
        enemyReference = lobbyshareddata.GetEnemyRef(HasStateAuthority);

        healthbarPrefab.SetActive(true);

        healthbarPrefab.transform.position = enemyReference.transform.position;
        healthbarPrefab.transform.rotation = enemyReference.transform.rotation;
        healthbarPrefab.transform.SetParent(enemyReference.transform);

        enemyHH = enemyReference.gameObject.GetComponent<HealthHandler>();

        if (enemyHH != null)
        {
            //setup basic healthbar stats 

            startTracking = true;
            currHealthbar.maxValue = enemyHH.GetMaxHealth();
            currHealthbar.value = enemyHH.CurrHealth;
        }

    }

    private void FixedUpdate()
    {
        if (startTracking)
        {
            currHealthbar.value = enemyHH.CurrHealth;
        }
    }
}
