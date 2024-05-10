using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class CapturePoint : NetworkBehaviour
{
    [SerializeField] private NetworkRunner m_networkRunner;

    [Tooltip("anchor for when monster captures")] [SerializeField] GameObject currentMonster;

    [Tooltip("the cube that shows the status of the capture point")] [SerializeField] GameObject captureVisual; 
    [SerializeField] Material unreadyMat;
    [SerializeField] Material readyMat;
    [SerializeField] Material doneMat;

    private bool canCapture = false;
    private Renderer visualRenderer;

    // Start is called before the first frame update
    void Start()
    {
        visualRenderer = captureVisual.GetComponent<Renderer>();
        visualRenderer.material = unreadyMat;

        Invoke("SetReady", 6f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerStay(Collider other)
    {


        if (other.gameObject.tag == "Monster" && canCapture)
        {
            Debug.Log("Monster start capturing");
            var capturingMonster = other.gameObject.GetComponent<MonsterProjectile>(); //get monster prefab ref from MonsterProjectile that hits the target
            if (capturingMonster != null)
            {
                GameObject objMonster = capturingMonster.GetMonsterObj();
                if (objMonster != null) //check if objMonster is valid 
                {
                    var currMonster = m_networkRunner.Spawn(objMonster, currentMonster.transform.position, currentMonster.transform.rotation);
                    currentMonster = currMonster.gameObject;
                    canCapture = false;
                }
            }

            StartCoroutine(CheckForCapture(3f));
        }
    }

    private void SetReady()
    {
        canCapture = true;
        visualRenderer.material = readyMat;
    }

    private IEnumerator CheckForCapture(float delay)
    {
        //after delay, decide if capture is done
        yield return new WaitForSeconds(delay);
        if (currentMonster != null)
        {
            visualRenderer.material = doneMat; 
        }


    }
}
