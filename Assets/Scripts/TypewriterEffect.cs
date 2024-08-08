using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TypewriterEffect : MonoBehaviour
{
    public TMP_Text tmpText;  // Assign the TextMeshPro Text component in the inspector
    public string fullText;  // The full text you want to display
    public float typeSpeed = 0.05f;  // Time delay between each character
    public string text;
    public GameObject textback;
    private void Start()
    {
       // TextAnimation();
    }

    public void TextAnimation()
    {
        LeanTween.scaleX(textback, 1, 1.2f);
        LeanTween.delayedCall(1.2f, StartWriting);
    }

    public void ResetText()
    {
        LeanTween.scaleX(textback, 0, 0.1f);
        fullText = null;
        tmpText.text = null;
    }


    public void StartWriting()
    {
        if (gameObject.CompareTag("QuestText"))
        {
            StartCoroutine(TypeText());
        }
        else if (gameObject.CompareTag("Win"))
        {
            RandomAnswer(0, "");
            StartCoroutine(TypeText2());
        }
        else if (gameObject.CompareTag("Loose"))
        {
            RandomAnswer(1, "");
            StartCoroutine(TypeText2());
        }
    }

    public void StartWritingForTimeLose(string state)
    {
        switch (state)
        {
            case "NotAccepted":
                RandomAnswer(5, name);
                StartCoroutine(TypeText2());
                break;
            case "Accepted":
                RandomAnswer(6, name);
                StartCoroutine(TypeText2());
                break;
            default:
                break;
        }
    }


    public void StartWritingForOpponent(string state, string name)
    {
        switch (state)
        {
            case "Win":
                RandomAnswer(2, name);
                StartCoroutine(TypeText2());
                break;  
            case "Lose":
                RandomAnswer(3, name);
                StartCoroutine(TypeText2());
                break;  
            case "LoseByTimeAcceptedOpponent":
                RandomAnswer(4, name);
                StartCoroutine(TypeText2());
                break;  
            default:
                break;
        }
    }

    private IEnumerator TypeText()
    {
        tmpText.text = "";
        foreach (char letter in fullText.ToCharArray())
        {
            tmpText.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }
        OnTypingComplete();
    }
    private IEnumerator TypeText2()
    {

        tmpText.text = "";
        foreach (char letter in fullText.ToCharArray())
        {
            tmpText.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }
    }
    void OnTypingComplete()
    {
        GameManager.instance.TimerPlay();
    }

    public void RandomAnswer(int index, string name)
    {
        if (index == 0)
        {
            int randomInt = Random.Range(0, 3);
            if (randomInt == 0)
            {
                fullText = "Congratulations! You're as sharp as Julius Caesar's favorite sword!";
            }
            else if (randomInt == 1)
            {
                fullText = "Bingo! You've earned more laurels than a Roman general!";
            }
            else
            {
                fullText = "You're on fire! Even Nero would give you a thumbs up!";
            }
        }
        else if (index == 1)
        {
            int randomInt = Random.Range(0, 3);
            if (randomInt == 0)
            {
                fullText = "Oops! Time to hit the history books again!";
            }
            else if (randomInt == 1)
            {
                fullText = "Not quite! Even Cleopatra wouldn't bet on that answer.";
            }
            else
            {
                fullText = "Close, but no laurel wreath this time!";
            }
        }
        else if (index == 2) 
        {
            fullText = name + " provided the correct answer and won";
        }
        else if (index == 3)
        {
            fullText = name + " provided the wrong answer and lose";
        }
        else if (index == 4)
        {
            fullText = name + " press the buzzer but didn't answer";
        }
        else if (index == 5)
        {
            fullText ="No one press the buzzer, question changed";
        }
        else if (index == 6)
        {
            fullText = "You didn't answer the question";
        }


    }

}
