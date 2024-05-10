using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class SpecialAttackHandler : NetworkBehaviour
{
    public NetworkRunner NetworkRunner;
    public ManaManager mm;
    private float spCooldown;
    private int type;

    // Start is called before the first frame update
    void Start()
    {
        // TO-DO: default for fire now, change later
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetType(int t)
    {
        type = t;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "PlayerHitbox" && type == 0)
        {
            // ummm...nothing
        }
        else if (other.gameObject.tag == "PlayerHitbox" && type == 1)
        {
            if (!HasStateAuthority)
            {
                other.gameObject.transform.parent.GetComponent<HealthHandler>().LoseHealthHostRpc(.1f * Time.deltaTime);
            }
            else
            {
                other.gameObject.transform.parent.GetComponent<HealthHandler>().LoseHealthClientRpc(.1f * Time.deltaTime);
            }
        }
        else if (other.gameObject.tag == "PlayerHitbox" && type == 2)
        {
            // mm.SetInfiniteMana(true);
            other.gameObject.transform.parent.GetComponent<HealthHandler>().SetHealthRegen(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "PlayerHitbox" && type == 2)
            other.gameObject.transform.parent.GetComponent<HealthHandler>().SetHealthRegen(false);
        // mm.SetInfiniteMana(false);
    }

    public float GetSPCooldown()
    {
        switch (type)
        {
            case 0:
                spCooldown = 30f;
                break;
            case 1:
                spCooldown = 30f;
                break;
            case 2:
                spCooldown = 30f;
                break;
        }

        return spCooldown;
    }

    public float GetSPDuration()
    {
        switch (type)
        {
            case 0:
                spCooldown = 15f;
                break;
            case 1:
                spCooldown = 10f;
                break;
            case 2:
                spCooldown = 12f;
                break;
        }

        return spCooldown;
    }
}
