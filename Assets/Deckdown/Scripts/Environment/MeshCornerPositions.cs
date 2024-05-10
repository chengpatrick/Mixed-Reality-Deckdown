using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCornerPositions : MonoBehaviour
{
    [SerializeField] GameObject obj;
    [SerializeField] GameObject Cloud;

    public GameObject parent;
    private float max_x, min_x, max_z, min_z;

    public void StartFill()
    {
        GameObject[] allGameObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in allGameObjects)
        {
            if (go.name.StartsWith("Room"))
            {
                parent = go;
                break;
            }
        }

        foreach (Transform child in parent.transform)
        {
            max_x = Mathf.Max(max_x, child.position.x);
            min_x = Mathf.Min(min_x, child.position.x);
            max_z = Mathf.Max(max_z, child.position.z);
            min_z = Mathf.Min(min_z, child.position.z);
        }

        //_ = Instantiate(obj, new Vector3(min_x, 5f, min_z), Quaternion.identity);
        //_ = Instantiate(obj, new Vector3(max_x, 5f, min_z), Quaternion.identity);
        //_ = Instantiate(obj, new Vector3(min_x, 5f, max_z), Quaternion.identity);
        //_ = Instantiate(obj, new Vector3(max_x, 5f, max_z), Quaternion.identity);

        for (var i = min_x; i <  max_x; i += (max_x - min_x) / 10)
        {
            for (var j = min_z; j < max_z; j += (max_z - min_z) / 10)
            {
                int randomInt = Random.Range(0, 100);
                if(randomInt >= 90)
                {
                    _ = Instantiate(Cloud, new Vector3(i, 3f, j), Cloud.transform.rotation);
                }
            }
        }
            
    }
}
