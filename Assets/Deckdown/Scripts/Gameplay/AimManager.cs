using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Fusion.NetworkCharacterController;

public class AimManager : MonoBehaviour
{
    public static AimManager Instance { get; private set; }

    [SerializeField] float raycastDistance = 10f;
    [SerializeField] HealthManager healthManager;


    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] LayerMask shroomLayer;
    [SerializeField] GameObject ballAim;
    [SerializeField] GameObject konAim;
    [SerializeField] GameObject shootAnchor;
    [SerializeField] GameObject AimCursorObject;

    [SerializeField] CursorController cursorController; 

    private bool tracingTarget = false;
    private int monsterType = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
           
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        TrackingCursor();
    }

    public void SetBallAimActive(bool b)
    {
        if(ballAim != null)
        {
            ballAim.SetActive(b);
        }

    }

    public void SetKonAimActive(bool b)
    {
        if (konAim != null)
        {
            konAim.SetActive(b);
        }

    }

    public void SetAimAssistMaterial(Material mat)
    {
        Renderer ballAimRender = ballAim.GetComponent<Renderer>();
        ballAimRender.material = mat;
        Renderer konAimRender = konAim.GetComponent<Renderer>();
        konAimRender.material = mat;
        
    }

    public void ToggleCursor(bool b)
    {
        tracingTarget = b;
    }

    public void TrackingCursor()
    {
        if (tracingTarget)
        {
            Ray ray = new Ray(shootAnchor.transform.position, shootAnchor.transform.forward);

            Vector3 raycastOrigin = shootAnchor.transform.position;
            Vector3 raycastDirection = shootAnchor.transform.right;

            RaycastHit hit;
            Debug.DrawRay(raycastOrigin, raycastDirection, Color.red, 5f);
            if (Physics.Raycast(raycastOrigin, raycastDirection, out hit, raycastDistance, ~ignoreLayer))
            {
                
                shootAnchor.SetActive(true);

                HealthHandler targetHH = hit.collider.gameObject.GetComponent<HealthHandler>();
                if (targetHH != null) //if detect another player
                {
                    MoveCursorToHitCenter(hit);
                    //if not the same monster type, confirm trace hit and turn on toggle
                    cursorController.ToggleCursor(targetHH.GetMonsterType() != monsterType); //enable LockOnCursor
                }
                else
                {
                    MoveCursorToHit(hit);
                    cursorController.ToggleCursor(false); //disable LockOnCursor
                }
            }
            
        }
        else { 
            cursorController.ToggleCursor(false);
        }
    }

    public void MoveCursorToHit(RaycastHit hit)
    {
        //if (!shootAnchor.activeSelf)
        //    shootAnchor.SetActive(true);
        AimCursorObject.SetActive(true);
        //Debug.Log(hit.collider.gameObject.layer);

 

        //move to the hit point
        AimCursorObject.transform.position = hit.point;
    }

    public void MoveCursorToHitCenter(RaycastHit hit)
    {
        //move to the object center
        AimCursorObject.transform.position = hit.transform.position;
    }

    public void SetMonsterType(int i)
    {
        monsterType = i;
        if (i == 0)
        { //add index 20 Smoke to ignoreLayer
            ignoreLayer |= shroomLayer;
        }
    }
}
