using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class WinHoldController : MonoBehaviour
{
    [SerializeField] float duration = 3f; // Duration in seconds
    [SerializeField] GameObject FillingBar, winScreen; 
    [SerializeField] private Image FillSlider;

    private float timer = 0f;
    private float currentValue = 0f;
    private bool fillingBar = false;
    private bool internalLock = true;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (fillingBar && internalLock)
        {
            // Increment the timer
            timer += Time.deltaTime;

            // Calculate the current value based on time
            currentValue = Mathf.Clamp01(timer / duration);
            FillSlider.fillAmount = currentValue;

            if (currentValue >= 1)
            {
                currentValue = 1;
                internalLock = false;
                FillingBar.SetActive(false);
                winScreen.SetActive(true);
            }
        }
    }

    public void FillBar()
    {
        fillingBar = true;
    }
}
