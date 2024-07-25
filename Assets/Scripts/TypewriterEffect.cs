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
       
        //TextAnimation();
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
            RandomAnswer(0);
            StartCoroutine(TypeText2());
        }
        else if (gameObject.CompareTag("Loose"))
        {
            RandomAnswer(1);
            StartCoroutine(TypeText2());
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
        if(PlayerManager.instance.mode == PlayerMode.Multiplayer)
        {
            GameManager.instance.NextBtnClicked();
        }
        
    }
    void OnTypingComplete()
    {
        if(PlayerManager.instance.mode == PlayerMode.Multiplayer)
        {
            GameManager.instance.multiplayerManager.OnQuestionComplete();
            GameManager.instance.TimerPlay();
        }
        else
        {
            GameManager.instance.TimerPlay();
        }
        
        
    }

    public void RandomAnswer(int index)
    {
        if(index == 0)
        {
            int randomInt = Random.Range(0, 3);
            if (randomInt == 0)
            {
                Debug.Log(randomInt);
                fullText = "Congratulations!   You're as sharp as Julius Caesar's favorite sword!";
            }
            else if (randomInt == 1)
            {
                Debug.Log(randomInt);
                fullText = "Bingo! You've earned more laurels than a Roman general!";
            }
            else
            {
                Debug.Log(randomInt);
                fullText = "You're on fire! Even Nero would give you a thumbs up!";
            }
        }
        else
        {
            int randomInt = Random.Range(0, 3);
            if (randomInt == 0)
            {
                Debug.Log(randomInt);
                fullText = "Oops! Time to hit the history books again!";
            }
            else if (randomInt == 1)
            {
                Debug.Log(randomInt);
                fullText = "Not quite! Even Cleopatra wouldn't bet on that answer.";
            }
            else
            {
                Debug.Log(randomInt);
                fullText = "Close,          but no laurel wreath this time!";
            }
        }
        

    }

}
