using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading;
using Fusion;
using UnityEngine;

public class VFXmanager : NetworkBehaviour
{
    private GameObject currGivePower; 

    [SerializeField] private NetworkRunner m_networkRunner;
    [SerializeField] private GameObject chargeUp;

    //give power stuff
    [SerializeField] private Transform givePowerPositionAnchor;
    [SerializeField] private Transform givePowerLookatAnchor;
    [SerializeField] private Vector3 givePowerOffset;
    [SerializeField] private GameObject testGivePower;

    public List<GameObject> hitParticles = new List<GameObject>();     //0=hit-block, 1 = hit-wall, 2 = hit-ground,
    // Start is called before the first frame update
    void Start()
    {
        //SpawnGivePowerPrefab(testGivePower);
    }

    // Update is called once per frame
    void Update()
    {
        if (currGivePower != null)
        {
            currGivePower.gameObject.transform.position = givePowerPositionAnchor.position;
            currGivePower.transform.LookAt(givePowerLookatAnchor);
        }
    }

    public void SpawnChargeup(Transform launchT, Transform parentT)
    {
        if (m_networkRunner != null)
        {
            Debug.Log("m_networkRunner isnt null");
            var networkVFX = m_networkRunner.Spawn(chargeUp, launchT.position, launchT.rotation);
            networkVFX.transform.parent = parentT.transform;
        }
        else
        {
            Debug.Log("m_networkRunner null ref");
        }
    }

    public void SpawnHitEffect(int hitFeedbakcType, Vector3 position, Quaternion rotation)
    {
        NetworkObject vfxObj;
        if(hitFeedbakcType == 2)
            vfxObj = m_networkRunner.Spawn(hitParticles[hitFeedbakcType], position, Quaternion.Euler(-90, 0, 0));
        else
            vfxObj = m_networkRunner.Spawn(hitParticles[hitFeedbakcType], position, rotation);

        StartCoroutine(VFXDespawn(3f, vfxObj));
    }

    public void SpawnGivePowerPrefab(GameObject monster)
    {
        currGivePower = Instantiate(monster, givePowerPositionAnchor);
        currGivePower.transform.LookAt(givePowerLookatAnchor);
    }


    private IEnumerator VFXDespawn(float delay, NetworkObject obj)
    {
        yield return new WaitForSeconds(delay);
        m_networkRunner.Despawn(obj);
    }
    public void SetBlockHitPrefab(GameObject obj)
    {
        hitParticles[0] = obj;
    }
    public void SetWallHitPrefab(GameObject obj)
    {
        hitParticles[1] = obj;
    }

    public void SetGroundHitPrefab(GameObject obj)
    {
        hitParticles[2] = obj;
    }

    public void SetChargePrefab(GameObject obj)
    {
        chargeUp = obj;
    }
}
