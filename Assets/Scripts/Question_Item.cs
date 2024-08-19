using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Question_Item : MonoBehaviour
{

    public TextMeshProUGUI questionText;
    public TextMeshProUGUI optionA;
    public TextMeshProUGUI optionB;
    public TextMeshProUGUI optionC;
    public int correctIndex;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Disappear()
    {
        Destroy(gameObject);
    }
}
