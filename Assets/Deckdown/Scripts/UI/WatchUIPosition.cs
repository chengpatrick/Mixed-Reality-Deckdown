using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatchUIPosition : MonoBehaviour
{
    [SerializeField] Transform trans;

    // Start is called before the first frame update
    void Start()
    {
        transform.parent = trans;
    }
}
