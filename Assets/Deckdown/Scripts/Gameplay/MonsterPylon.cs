using System.Collections;
using System.Collections.Generic;
using Fusion;
using Photon.Voice;
using UnityEngine;
using UnityEngine.UIElements;

public class MonsterPylon : MonoBehaviour
{
    [SerializeField] GameObject myMonster;

    [SerializeField] float radius = 2f;
    [SerializeField] int typeID;
    [SerializeField] GameObject captureZone;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("spawn monster projectile!");
        Destroy(gameObject, 9);
        ScanArea();

    }

    // Update is called once per frame
    void Update()
    {
        // transform.Translate(Vector3.left * speed * Time.deltaTime);
    }

    public GameObject GetMonsterObj()
    {
        return myMonster;
    }


    public void ScanArea()
    {
        GameObject sphere = Instantiate(captureZone, transform.position, Quaternion.identity);
        sphere.SetActive(true);
        sphere.transform.localScale = Vector3.one * radius;
        Destroy(sphere, 9f);

        //myMonster.SetActive(false);
        //StartCoroutine(RetrieveMonster(5f, sphere));

        Collider[] allColliders = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider collider in allColliders)
        {
            GameObject hitObject = collider.gameObject;
            Soul currSoul = hitObject.transform.parent.GetComponent<Soul>();

            if (currSoul != null)
            {
                //DebugPanel.Instance.UpdateMessage("object contains soul");
                currSoul.OnCapture(typeID, 8f, transform);
            }

            // Do something with the hitObject
            //Debug.Log("Hit object: " + hitObject.name);
        }
    }


    IEnumerator RetrieveMonster(float t, GameObject sphere)
    {
        yield return new WaitForSeconds(t);

        if(GetComponent<MonsterCollectPickUp>() != null)
        {
            Destroy(sphere);
        }
            
    }

    public void SetTypeID(int id)
    {
        typeID = id;
    }

    public int GetTypeID()
    {
        return typeID;
    }
}
