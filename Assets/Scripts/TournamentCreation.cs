using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TournamentCreation : MonoBehaviour
{
    public TMP_InputField QuestTxt, optionA, optionB, optionC;
    public Toggle toggleA, toggleB, toggleC;
    public Question_Item questionItem;
    public GameObject ContentParent;
    public TextMeshProUGUI timertxt, questiontxt;
    public int timerCount, questionCount;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddBtnClicked()
    {
        questionItem.questionText.text = QuestTxt.text;
        questionItem.optionA.text = optionA.text;
        questionItem.optionB.text = optionB.text;
        questionItem.optionC.text = optionC.text;
        ClearQuestArea();
        if (toggleA.isOn)
        {
            questionItem.correctIndex = 0;
      
        }
        else if (toggleB.isOn)
        {
            questionItem.correctIndex = 1;
            
        }
        else if(toggleC.isOn)
        {
            questionItem.correctIndex = 2;
            
        }

        GameObject gb = Instantiate(questionItem.gameObject, ContentParent.transform);
        gb.SetActive(true);
        Debug.Log(questionItem.correctIndex);
    }

    public void ClearQuestArea()
    {
        QuestTxt.text = "";
        optionA.text = "";
        optionB.text = "";
        optionC.text = "";
        toggleA.isOn = false; 
        toggleB.isOn = false; 
        toggleC.isOn = false; 
    }

    public void TimerCount(bool isPlus)
    {
        if (isPlus)
        {
            timerCount++;
            timertxt.text = timerCount.ToString();
        }
        else
        {
            timerCount--;
            timertxt.text = timerCount.ToString();
        }
    }
    public void QuestCount(bool isPlus)
    {
        if (isPlus)
        {
            questionCount++;
            questiontxt.text = questionCount.ToString();
        }
        else
        {
            questionCount--;
            questiontxt.text = questionCount.ToString();
        }
    }
}
