using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    [SerializeField] GameObject regularCursor;
    [SerializeField] GameObject lockOnCursor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Billboarding();
    }

    public void ToggleCursor(bool toggle)  
    {
        lockOnCursor.SetActive(toggle);
    }

    private void Billboarding()
    {
        transform.rotation = Camera.main.transform.rotation;
    }
}
