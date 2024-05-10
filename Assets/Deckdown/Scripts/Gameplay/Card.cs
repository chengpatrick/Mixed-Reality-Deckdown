using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public CardData cardData;
    public bool isGrabbed = false;
    [SerializeField] GameObject cardInfo;

    public void GrabCard()
    {
        isGrabbed = true;
        UpdateCardInfoPanel();
    }

    public void UngrabCard()
    {
        isGrabbed = false;
        UpdateCardInfoPanel();
    }

    private void UpdateCardInfoPanel()
    {
        cardInfo.SetActive(isGrabbed);
    }
}
