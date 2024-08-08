using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        PlayHoverAnim();
    }

    // Method to be called when the pointer exits the button
    public void OnPointerExit(PointerEventData eventData)
    {
        ExitHoverAnim();
        // You can also call another method here if needed
    }

 
    GameObject textTransform;
    private void Start()
    {
        textTransform =  transform.GetChild(1).gameObject;
    }

    public void PlayHoverAnim()
    {
        LeanTween.moveLocalY(textTransform, 0f, 0.4f);
    }
    public void ExitHoverAnim()
    {
        LeanTween.moveLocalY(textTransform, -330f, 0.4f);
    }
}
