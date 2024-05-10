using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCollide : MonoBehaviour
{
    [SerializeField] MonsterFollowOffset MFO;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Blocker")
        {
            MFO.collide = true;
            MFO.idx = (MFO.idx + 1) % MFO.n;
        }
    }
}