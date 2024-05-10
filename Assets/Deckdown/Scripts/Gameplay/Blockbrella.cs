using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Blockbrella : MonoBehaviour
{
    [SerializeField] float openDuration = 0.5f; // Duration of the lerp in seconds
    [SerializeField] float closeDuration = 0.5f; // Duration of the lerp in seconds
    [SerializeField] float elapsedTime1 = 0f; // Time elapsed since the lerp started
    [SerializeField] float elapsedTime2 = 0f; // Time elapsed since the lerp started
    private VisualEffect vfxBlockbrella;
    private float size = 0;
    private float sizeWhenFolding = 0;

    private int foldState = 0; //0 is unfolding, 1 is unfolded, 2 is folding

    // Start is called before the first frame update
    void Start()
    {
        vfxBlockbrella = GetComponent<VisualEffect>();
        if (vfxBlockbrella != null)
        {
            Debug.Log("vfxBlockbrella on");
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("folding state 0, Curr size is: " + sizeWhenFolding);
        if (foldState == 0)
        {
            // Update the elapsed time
            elapsedTime1 += Time.deltaTime;
            // Clamp the elapsed time to prevent going beyond the lerp duration
            elapsedTime1 = Mathf.Clamp(elapsedTime1, 0f, openDuration);
            float t = elapsedTime1 / openDuration;
            size = Mathf.Lerp(0f, 1f, t);
            vfxBlockbrella.SetFloat("Size", size);

            if (t >= 1f)
            {
                foldState = 1;
            }
        }
        else if (foldState == 2)
        {
            Debug.Log("folding state 2, Curr size is: " + sizeWhenFolding);
            // Update the elapsed time
            elapsedTime2 += Time.deltaTime;
            // Clamp the elapsed time to prevent going beyond the lerp duration
            elapsedTime2 = Mathf.Clamp(elapsedTime2, 0f, closeDuration);
            float t = elapsedTime2 / closeDuration;
            size = Mathf.Lerp(sizeWhenFolding, 0f, t);
            vfxBlockbrella.SetFloat("Size", size);

            //if (t <= 0f)
            //{
            //    foldState = 3;
            //}
        }
        else
        {
            Debug.Log("unknown folding state:" + foldState);
        }
    }

    public void Folding()
    {
        foldState = 2;
        Debug.Log("is folding, fold state = " + foldState);
        
        sizeWhenFolding = size; 
    }
}
