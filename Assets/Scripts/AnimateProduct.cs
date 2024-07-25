using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateProduct : MonoBehaviour
{
    Animator animator;
    [SerializeField] GameObject glowObj;
    [SerializeField] TypewriterEffect typeScript;
    // Start is called before the first frame update
    void Start()
    {
        /*animator = GetComponent<Animator>();
        glowObj.SetActive(false);
        animator.Play("Product Animation");*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void OnANimComplete()
    {
        glowObj.SetActive(true);
        glowObj.GetComponent<Animator>().Play("glowAnim");
        LeanTween.delayedCall(1f, typeScript.TextAnimation);
    }
}
