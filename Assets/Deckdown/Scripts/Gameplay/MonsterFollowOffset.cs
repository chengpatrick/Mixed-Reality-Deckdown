using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using Meta.Utilities;
using Oculus.Interaction.Input;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class MonsterFollowOffset : MonoBehaviour
{
    // target to follow (Center Eye Anchor)
    private GameObject target;
    private Transform handTransform;
    [SerializeField] CapsuleCollider collider;

    // Store offsets from target, more positions to avoid collisions
    [SerializeField] Vector3 monsterOffset = Vector3.forward;
    [SerializeField] Vector3 monsterOffset2 = Vector3.forward;
    [SerializeField] Vector3 blockOffset = Vector3.zero; // monster block position
    [SerializeField] float minHeight = 0.1f; // min y height monster can be
    private Vector3 offset = Vector3.zero;

    // Collision checks
    public int n = 1; // # target positions
    public float radius = 0.7f; // radius
    public int idx;
    public bool collide;
    public Vector3[] offsets;


    // second order dynamics variables
    private Vector3 xPrev; // previous input
    private Vector3 y, dy; // state variables: position, velocity
    private float k1, k2, k3; // dynamic const
    [SerializeField] float f = 1.0f;
    [SerializeField] float z = 0.5f;
    [SerializeField] float r = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        target = transform.parent.gameObject;
        // collider = gameObject.GetComponent<CapsuleCollider>();

        InitSecondOrderDynamics(f, z, r, target.transform.position);
        InitTargetPositions();

        transform.SetParent(null);
    }

    // Update monster position
    void Update()
    {
        if (PlayCard.PC.block) // Block case
        {
            offset = handTransform.TransformPoint(blockOffset);
            //offset.Set(handTransform.position.x + worldOffset.x, handTransform.position.y + worldOffset.y, handTransform.position.z + worldOffset.z);
        } 
        else
        {
            // Move monster back if typical position no longer colliding with wall
            if (collide && !Physics.CheckSphere(target.transform.TransformPoint(monsterOffset), collider.radius))
            {
                collide = false;
                idx = n;
            }

            // Need to get offset of all parent
            // Debug.Log("OFFSET " + idx + " : " + offsets[idx]);
            offset = target.transform.TransformPoint(offsets[idx]);
            offset.y = Mathf.Max(minHeight, 3*target.transform.position.y/4);
            //offset.Set(target.transform.position.x, Mathf.Max(min_height, target.transform.position.y / 2), target.transform.position.z + radius);
            // offset = target.transform.TransformPoint();
            // offset.Set(offset.x, Mathf.Max(min_height, target.transform.position.y / 2), offset.z);
        }

        // transform.position = UpdateDynamics(Time.deltaTime, offset, Vector3.one);
        transform.position = Vector3.MoveTowards(transform.position, offset, Time.deltaTime*0.75f);
        transform.rotation = target.transform.rotation;
    }

    // Setup possible target positions for monster
    void InitTargetPositions()
    {
        offsets = new Vector3[n + 1];

        // Leftside reflection of typical
        // float theta = Mathf.PI/2; //radians
        offsets[0] = monsterOffset2; // new Vector3(Mathf.Sin(theta), 0, Mathf.Cos(theta)); // new Vector3(-0.4f, 0.0f, 0.7f);

        // Behind
        // offsets[1] = Vector3.back;

        //for (int i = 0; i < n; i++)
        //{
        //    float theta = (2*Mathf.PI/n)*i; //radians
        //    offsets[i] = new Vector3(radius*Mathf.Sin(theta), 0, radius*Mathf.Cos(theta));
        //}

        // typical monster position
        offsets[n] = monsterOffset;
        idx = n;
  
    }

    public void SetLeftHandAnchor(Transform PlayerLeftHandTransform)
    {
        handTransform = PlayerLeftHandTransform;
    }

    /*** Computes dynamic constants, initialises prev input and state variables
        f: natural frequency of system, speed of response to change in position, cannot = 0
        z: damping coefficient, how fast system settles @ target position
        r: initial response to system (immediate, overshoot, anticipate)
    ***/
    void InitSecondOrderDynamics(float f, float z, float r, Vector3 x0)
    {
        // compute constants
        k1 = z / (Mathf.PI * f);
        k2 = 1 / ((2 * Mathf.PI * f) * (2 * Mathf.PI * f));
        k3 = (r * z) / (2 * Mathf.PI * f);

        // initialise variables
        xPrev = x0;
        y = x0;
        dy = Vector3.zero;
    }

    // Updates state variables, T: time between frames, x: position, dx: velocity
    Vector3 UpdateDynamics(float T, Vector3 x, Vector3 dx)
    {
        // If input velocity is null, compute average velocity
        if (dx == Vector3.zero)
        {
            dx = (x -xPrev) / T;
            xPrev = x;
        }
        y = y + T * dy; // update position w/ velocity   
        float k2_stable = Mathf.Max(Mathf.Max(k2, T * T / 2 + T * k1 / 2), T * k1); // stabalise k2, reduces jitter
        Vector3 ddy = (x + k3 * dx - y - k1 * dy) / k2_stable; //compute acceleration
        dy = dy + T * ddy; // update velocity w/ acceleration
        return y;
    }
}
