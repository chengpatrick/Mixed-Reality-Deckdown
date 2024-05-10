using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// virtual debug panel anchored to the left hand to debug in runtime
public class DebugPanel : MonoBehaviour
{
    public static DebugPanel Instance { get; private set; }

    public TextMeshProUGUI MessageText;

    private void Awake()
    {
        // Ensure only one instance of UIManager exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateMessage(string message)
    {
        if (MessageText != null)
        {
            MessageText.text += "\n" + message;
        }
    }


}
