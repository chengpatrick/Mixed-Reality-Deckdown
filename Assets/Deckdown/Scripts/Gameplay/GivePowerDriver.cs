using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using POpusCodec.Enums;
using UnityEngine;
using UnityEngine.VFX;

public class GivePowerDriver : MonoBehaviour
{
    [SerializeField] float displayMonsterDelay = 0.5f;
    [SerializeField] float hideMonsterDelay = 3f;
    [SerializeField] GameObject monsterMesh;
    [SerializeField] VisualEffect poof; 
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DisplayMonster());
        StartCoroutine(HideMonster());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator DisplayMonster()
    {
        yield return new WaitForSeconds(displayMonsterDelay);
        
        monsterMesh.SetActive(true);
    }

    IEnumerator HideMonster() 
    {
        yield return new WaitForSeconds(hideMonsterDelay);
        poof.Play();
        monsterMesh.SetActive(false);
        Destroy(gameObject, 1f);
    }


}
