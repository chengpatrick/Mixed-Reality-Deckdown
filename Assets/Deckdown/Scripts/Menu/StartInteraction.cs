using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Discover.Networking;
using Fusion;
using TMPro;

public class StartInteraction : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI m_textMeshPro;

    [Networked]
    public int PlayersTouched { get; set;}

    private bool gameStarted = false;

    private void Awake()
    {
        PlayersTouched = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!gameStarted)
        {
            PlayersTouched++;
            m_textMeshPro.text = "Players touched: " + PlayersTouched;
            if (PlayersTouched >= 2)
            {
                StartGame();
            }
        }
    }

    private void StartGame()
    {
        // m_textMeshPro.text = "Both Players are ready";
    }  
}
