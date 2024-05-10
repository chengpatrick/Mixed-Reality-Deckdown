using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.UI; 

public class ManaManager : MonoBehaviour
{
    [SerializeField] private Slider manaSlider;
    [SerializeField] GameObject InRainFilter;

    [SerializeField] private float maxMana = 5f;
    [SerializeField] private float minMana = 0f;

    [SerializeField] private float regenInterval = 1f;  //how long every interval
    [SerializeField] private float regenAmountPerSec = 1f;

    private bool infiniteMana = false;

    private bool canRegenMana = true;
    private float currMana = 5;
    private float regenPerInterval; 

    // Start is called before the first frame update
    void Start()
    {
        regenPerInterval = regenAmountPerSec * regenInterval;
        StartCoroutine(ManaRegen(regenInterval));
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetManabar()
    {
        manaSlider.maxValue = maxMana;
        manaSlider.minValue = minMana;
    }

    public void RegenerateMana()
    {
        currMana = Mathf.Clamp(currMana + regenPerInterval, minMana, maxMana);
        manaSlider.value = currMana;
        //Debug.Log("REGEN MANA: " + currMana);
    }

    public void SetCanRegenMana(bool b)
    {
        canRegenMana=b;
        if (b)
        {
            StartCoroutine(ManaRegen(regenInterval));
        }
    }

    public bool UseMana(float cost)
    {
        if (infiniteMana)
        {
            return true;
        }
        else
        {
            if (currMana - cost > 0)
            {
                if (!infiniteMana)
                {
                    currMana = currMana - cost;

                    manaSlider.value = currMana;
                }
                return true;
            }
            else
            {
                Debug.Log("cant use mana, current mana: " + currMana);
                return false;
            }
        }
    }

    public void SetInfiniteMana(bool b)
    {
        infiniteMana = b;

        if (infiniteMana)
        {
            InRainFilter.SetActive(true);
        }
        else
        {
            InRainFilter.SetActive(false);
        }
    }

    IEnumerator ManaRegen(float delay)
    {
        while (canRegenMana)
        {
            yield return new WaitForSeconds(delay);
            RegenerateMana(); // Regenerate [regenAmount] mana every [regenInterval] seconds
        }
    }
}
