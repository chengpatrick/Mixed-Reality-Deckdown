using System.Collections;
using System.Collections.Generic;
using POpusCodec.Enums;
using UnityEngine;
using Fusion;
using Oculus.Avatar2;
using System.Linq;
using ExitGames.Client.Photon.StructWrapping;

public class BallProjectile : NetworkBehaviour
{
    private Damage dmg;
    private VFXmanager vfxmanager;
    [SerializeField] private NetworkRunner m_networkRunner;

    [SerializeField] bool isGrowing = false;
    [SerializeField] bool canExplode = false;
    [SerializeField] float growthRate;
    [SerializeField] float scaleFactor = 0.05f;
    [SerializeField] float homingFactor = 3.0f;

    [SerializeField] private float speed = 0;
    [SerializeField] float enabledSpeed = 5f;
    [SerializeField] Vector3 direction = Vector3.zero;

    private PlayCard playCard;
    private GameObject otherValidTarget = null;
    private GameObject enemyTarget; 

    //0=hit-block, 1 = hit-wall, 2 = hit-ground, 
    [SerializeField] int damageType;
    // Start is called before the first frame update
    void Start()
    {
        //assuming there is a damage component
        dmg = GetComponent<Damage>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isGrowing && transform.localScale.x < 0.5f)
        {
            //Debug.Log("growing");
            transform.localScale += new Vector3(scaleFactor, scaleFactor, scaleFactor) * Time.deltaTime;
        }
        else if (enemyTarget != null && speed != 0f)
        {
            //lerp to target
            ChaseTarget();
        }
        else
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }
    }

    private void ChaseTarget()
    {

        //DebugPanel.Instance.UpdateMessage("lerping to " + enemyTarget.name);

        // Calculate the direction to the target 
        //Vector3 directionToTarget = enemyTarget.transform.position - transform.position;

        // Rotate the to face the target
        //Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, homingFactor * Time.deltaTime);

        transform.Translate(Vector3.right * speed * Time.deltaTime);
        // Move the projectile towards target
        transform.position = Vector3.MoveTowards(transform.position, enemyTarget.transform.position, homingFactor * Time.deltaTime);

        //transform.Translate(Vector3.right * speed * Time.deltaTime + Vector3.MoveTowards(transform.position, enemyTarget.transform.position, homingFactor * Time.deltaTime));
    }


    public void SetChaseTarget(NetworkObject nj)
    {
        DebugPanel.Instance.UpdateMessage("Setting chase target to " + nj.gameObject.name);
        enemyTarget = nj.gameObject;
        DebugPanel.Instance.UpdateMessage("Game object is " + enemyTarget.name);
    }
    public void SetBallSpeed(float f)
    {
        //Debug.Log("set ball speed to " + f);
        speed = f;
    }

    public void SetBallDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    public void SetCardManager(PlayCard pc)
    {
        playCard = pc;
    }

    public void SetNetworkRunner(NetworkRunner runner)
    {
        DebugPanel.Instance.UpdateMessage("SetNetworkRunner for projectile");
        m_networkRunner = runner;
    }

    public void LaunchBall()
    {
        if (playCard != null)
            playCard.LaunchAttackOrb();
    }

    public void SetBallThrowState()
    {
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().velocity *= 3;
    }

    public void SetBallGrowth(bool b, Transform t, PlayCard pc)
    {
        isGrowing = b;
        gameObject.transform.SetParent(t);
        pc.SetCanSpawn(false);

        StartCoroutine(DelaySetSpeed(1f, pc));
    }

    public void SetVFXmanager(VFXmanager vfxm)
    {
        vfxmanager = vfxm;
    }

    public Quaternion GetOtherNormal(Collider other) //used to allign impact VFX to the surface of an object
    {
        // Get the hit point
        Vector3 hitPoint = other.ClosestPoint(transform.position);

        // Cast a ray from the center of the sphere trigger towards the hit point
        RaycastHit hit;
        if (Physics.Raycast(transform.position, hitPoint - transform.position, out hit, Mathf.Infinity, other.gameObject.layer))
        {
            // Get the normal of the hit face
            Vector3 faceNormal = hit.normal;

            // Calculate the rotation to make the face normal point upwards
            Quaternion faceRotation = Quaternion.FromToRotation(Vector3.up, faceNormal);
            // Convert the rotation to world rotation
            Quaternion worldRotation = other.transform.rotation * faceRotation;

            //Debug.Log("World Rotation of the face: " + worldRotation.eulerAngles);

            worldRotation.y += 90f;
            return worldRotation;
        }
        return Quaternion.identity;
    }

    public void OnTriggerStay(Collider other)
    {
        if (otherValidTarget == null && other.tag == "Blocker" && canExplode)
        {

            IBlocker bObj = other.gameObject.GetComponent<IBlocker>();
            if (bObj != null) //when running into blocker
            {
                int hitFeedbackType = bObj.OnBlock();
                // Get the position of contact
                Vector3 contactPosition = other.ClosestPointOnBounds(transform.position);

                Quaternion otherNormal = GetOtherNormal(other);
                // Spawn a VFX with the calculated rotation
                vfxmanager.SpawnHitEffect(hitFeedbackType, contactPosition, otherNormal);
                otherValidTarget = other.gameObject;
            }
            NetworkObject myNO = gameObject.GetComponent<NetworkObject>();
            if (myNO != null)
            {
                m_networkRunner.Despawn(myNO);
                Destroy(gameObject);
            }
        }
        else if (other.tag == "PlayerHitbox" && canExplode) //need to add PlayerHealthHandler component to any PlayerHitbox tagged obj
        {
            HealthHandler otherHH = other.gameObject.transform.parent.GetComponent<HealthHandler>();
            //DebugPanel.Instance.UpdateMessage("otherHH " + otherHH.name + " has monster type: " + otherHH.GetMonsterType());
            PlayHitboxSFX(otherHH.GetMonsterType());
            PlayerStatsRecorder.Instance.HitConfirm();
            if (!otherHH.CompareType(damageType))
            {
                canExplode = false;
                //DebugPanel.Instance.UpdateMessage("target registered");
                Quaternion rot = gameObject.transform.rotation;
                rot.y -= 90;
                vfxmanager.SpawnHitEffect(0, gameObject.transform.position, rot);

                otherHH.LoseHealthHostRpc(1f);

                m_networkRunner.Despawn(gameObject.GetComponent<NetworkObject>());
                Destroy(gameObject);

                // Spawn a VFX with the calculated rotation

            }
        }
    }

    private void PlayHitboxSFX(int type)
    {
        switch (type)
        {
            case 0:
                SoundManager.Instance.PlaySFXClip("SFX_VAGrassHit");
                break;
            case 1:
                SoundManager.Instance.PlaySFXClip("SFX_VAFireHit");
                break;
            case 2:
                SoundManager.Instance.PlaySFXClip("SFX_VAWaterHit");
                break;
        }
    }

    private IEnumerator DelaySetSpeed(float delay, PlayCard pc)
    {
        DebugPanel.Instance.UpdateMessage("enlarging for " + delay + " seconds");
        Debug.Log("DelaySetSpeed called");
        yield return new WaitForSeconds(delay);
        gameObject.transform.SetParent(null);
        isGrowing = false;
        speed = enabledSpeed;
        pc.SetCanSpawn(true);
        canExplode = true;
        AimManager.Instance.SetBallAimActive(false);
        DebugPanel.Instance.UpdateMessage("delay finished");
        Debug.Log("delay finished");
    }

}
