using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerationMap : MonoBehaviour
{
    /*
     * Object Generation Map will output a 2D array of map data showing where 
     * to generate virtual objects in scene.
     * 
     * After finding the bounding box of the room, we can know the range of 
     * coordinates of where to start checking for possible locations of 
     * spawning virtual objects.
     * 
     * We will use two main algorithms for generation of virtual room objects.
     * 1. Wave Function Collapse
     * 2. Point-Bound Checking
     * 
     */

    private List<GameObject> walls;
    private HashSet<Vector3> cornerPos;

    private bool roomCornerDataLoaded = false;


    // Start is called before the first frame update
    void Start()
    {
        walls = new List<GameObject>();
        cornerPos = new HashSet<Vector3>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
