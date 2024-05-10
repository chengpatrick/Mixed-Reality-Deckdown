using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion; 

public class PlayerHealthHandler : NetworkBehaviour, IDamagable
{
    [Networked(OnChanged = nameof(OnHPChanged))] byte HP {  get; set; }

    [Networked(OnChanged = nameof(OnStateChanged))] public bool isDead { get; set; }

    bool isInitialized = false;

    const byte startingHP = 5;


    // Start is called before the first frame update
    void Start()
    {
        HP = startingHP;
        isDead = false;

        isInitialized = true;
    }

    public void OnTakeDamage(Damage d)
    {
        if (isDead) return;

        HP -= d.GetDamage();
        Debug.Log($"{Time.time} {transform.name} took damage got {HP} left");
        if (HP <= 0 )
        {
            Debug.Log($"{Time.time} {transform.name} died");
            isDead = true;
        }
    }

    static void OnHPChanged(Changed<PlayerHealthHandler> changed)
    {
        Debug.Log($"{Time.time} OnHPChanged value {changed.Behaviour.HP}");
    }

    static void OnStateChanged(Changed<PlayerHealthHandler> changed)
    {
        Debug.Log($"{Time.time} OnHPChanged isDead {changed.Behaviour.isDead}");
    }


}
