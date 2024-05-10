using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBlocker : MonoBehaviour, IBlocker
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int OnBlock()
    {
        return 1; //return type 0 which is a shield
    }
}
