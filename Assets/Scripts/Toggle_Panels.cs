using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toggle_Panels : MonoBehaviour
{
    CanvasGroup panelAlpha;

    private void Start()
    {
        
    }

    // Method to explicitly set the GameObject active
    public void SetActiveState(bool isActive)
    {
        if(isActive)
        {
            panelAlpha = GetComponent<CanvasGroup>();
            gameObject.SetActive(true);
            LeanTween.alphaCanvas(panelAlpha, 1, 0.6f);
        }
        else
        {
            panelAlpha = GetComponent<CanvasGroup>();
            LeanTween.alphaCanvas(panelAlpha, 0, 0.6f);
            LeanTween.delayedCall(0.6f, DelayClose);
        }
        
    }

    void DelayClose()
    {
        gameObject.SetActive(false);
    }
}
