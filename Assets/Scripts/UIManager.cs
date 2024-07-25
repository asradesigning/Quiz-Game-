using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] CanvasGroup buzzerSorryPanel;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenSorryPanel()
    {
        buzzerSorryPanel.gameObject.SetActive(true);
        LeanTween.alphaCanvas(buzzerSorryPanel, 1, 0.5f);
        LeanTween.delayedCall(1f, CloseSorryPanel);
    }
    public void CloseSorryPanel()
    {
        LeanTween.alphaCanvas(buzzerSorryPanel, 0, 0.5f);
    }
}
