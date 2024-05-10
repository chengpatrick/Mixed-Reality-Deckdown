using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneObjectBlocker : MonoBehaviour, IBlocker
{
    [SerializeField] int type;

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
        return type; //return type 0 which is a shield
    }
}
