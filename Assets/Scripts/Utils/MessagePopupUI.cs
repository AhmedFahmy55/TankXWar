using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessagePopupUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI messageText;
    [SerializeField] GameObject spinner;
    [SerializeField] GameObject closeButton;



    public void PopUpWithTryingMessage(string message)
    {
        gameObject.SetActive(true);
        closeButton.SetActive(false);
        spinner.SetActive(true);
        messageText.text = message;
    }

    public void PopUpWithEndMessage(string message)
    {
        gameObject.SetActive(true);
        closeButton.SetActive(true);
        spinner.SetActive(false);
        messageText.text = message;
    }


}
