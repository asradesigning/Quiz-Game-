using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerQuizManager : MonoBehaviour
{
    public static MultiplayerQuizManager instance;

    [SerializeField] QuizDataMultiplayer quizData;
    [SerializeField] Image[] levelImg;
    [SerializeField] GameObject[] sureBgImg;
    [SerializeField] GameObject[] surePanel;
    [SerializeField] GameObject contentPanel;
    [SerializeField] TextMeshProUGUI quizPoint_txt;
    
    public GameObject[] levelTxt;
    [SerializeField] Button buzzerButton; // Button for buzzer

    private int quizPoints = 0;
    private int correct = 0;
    private int questionIndex = 0;
    private List<int> shuffledIndices;
    private bool isQuestionActive = false;
    private float buzzerStartTime;
    private float buzzerTimeout = 5f; // Time allowed for buzzer press
    string levelName;
    private void Awake()
    {
        instance =this;
    }

    private void Start()
    {
        buzzerButton.onClick.AddListener(OnBuzzerPressed);
        SetLevels();
        ShuffleQuestions();
        LoadQuestion();
        OpenQuestPanel();
        CorrectBtn();
        surePanel[0].SetActive(false);
        surePanel[1].SetActive(false);
        buzzerButton.interactable = false;
    }

    private void Update()
    {
        if (isQuestionActive && Time.time - buzzerStartTime > buzzerTimeout)
        {
            // Handle timeout - no one pressed the buzzer in time
            EndQuestion();
        }
    }

    public void StartGame()
    {
        LoadQuestion();
        OpenQuestPanel();
        CorrectBtn();
        surePanel[0].SetActive(false);
        surePanel[1].SetActive(false);
        buzzerButton.interactable = false;
    }

    public void SetLevels()
    {
        if (PlayerManager.instance != null)
        {
            levelName = PlayerManager.instance.levelName;
        }

        if (levelName == "Classic")
        {
            
        }
        else if (levelName == "Survival")
        {
            
         
        }
        else if (levelName == "SpeedRound")
        {
           
        }
       
    }

    public void OpenQuestPanel()
    {
        LeanTween.alphaCanvas(contentPanel.GetComponent<CanvasGroup>(), 1, 0.5f);
        for (int i = 0; i < levelTxt.Length; i++)
        {
       //     levelTxt[i].GetComponent<TypewriterEffect>().TextAnimation();
        }
    }

    void ShuffleQuestions()
    {
        shuffledIndices = new List<int>();
        for (int i = 0; i < quizData.multiplayerQuizs.Length; i++)
        {
            shuffledIndices.Add(i);
        }

        // Fisher-Yates shuffle
        for (int i = 0; i < shuffledIndices.Count; i++)
        {
            int randomIndex = Random.Range(i, shuffledIndices.Count);
            int temp = shuffledIndices[i];
            shuffledIndices[i] = shuffledIndices[randomIndex];
            shuffledIndices[randomIndex] = temp;
        }
    }

    public void LoadQuestion()
    {
        if (questionIndex < shuffledIndices.Count)
        {
            int levelIndex = shuffledIndices[questionIndex];
            questionIndex++;

            levelImg[0].sprite = quizData.multiplayerQuizs[levelIndex].levelImg[0];
            levelTxt[0].GetComponent<TypewriterEffect>().fullText = quizData.multiplayerQuizs[levelIndex].question[0];

            levelImg[1].sprite = quizData.multiplayerQuizs[levelIndex].levelImg[1];
            levelTxt[1].GetComponent<TypewriterEffect>().fullText = quizData.multiplayerQuizs[levelIndex].question[1];

            correct = quizData.multiplayerQuizs[levelIndex].correctAnswerIndex;
            
        }
        else
        {
            Debug.Log("All questions have been answered!");
            // Optionally, you can reshuffle and start again or end the game
        }
    }

    public void OnBuzzerPressed()
    {
        if (isQuestionActive)
        {
            UIManager.instance.OpenSorryPanel();
           
        }
    }

  
    void OnBuzzerPressedRPC()
    {
        // Determine which player pressed the buzzer first
        // You will need to manage this logic to identify the player
        EndQuestion();
    }

    public void OnQuestionComplete()
    {
        isQuestionActive = true;
        buzzerStartTime = Time.time;
        buzzerButton.interactable = true;
    }

    void EndQuestion()
    {
        isQuestionActive = false;
        // Handle the end of the question and determine the next step
    }

    public void QuizAnswer(int index)
    {
        if (index == correct)
        {
            quizPoints++;
            quizPoint_txt.text = quizPoints.ToString();
            if(quizPoints >= 6)
            {

            }
           // GameManager.instance.YouWIn();
        }
        else
        {
          //  GameManager.instance.YouLoose();
        }
    }

    public void CheckPanel(int index)
    {
        if (index == 0)
        {
            sureBgImg[0].SetActive(true);
            sureBgImg[1].SetActive(false);
            surePanel[0].SetActive(true);
            surePanel[1].SetActive(false);
            surePanel[1].GetComponent<CanvasGroup>().alpha = 0;
            LeanTween.alphaCanvas(surePanel[0].GetComponent<CanvasGroup>(), 1, 0.6f);
        }
        else
        {
            sureBgImg[1].SetActive(true);
            sureBgImg[0].SetActive(false);
            surePanel[1].SetActive(true);
            surePanel[0].SetActive(false);
            surePanel[0].GetComponent<CanvasGroup>().alpha = 0;
            LeanTween.alphaCanvas(surePanel[1].GetComponent<CanvasGroup>(), 1, 0.7f);
        }
    }

    public void CorrectBtn()
    {
        for (int i = 0; i < surePanel.Length; i++)
        {
            surePanel[i].transform.GetChild(0).GetChild(0).GetComponent<CorrectButton>().CorrectIndex = 5;
            surePanel[i].SetActive(false);
            sureBgImg[i].SetActive(false);
            surePanel[i].GetComponent<CanvasGroup>().alpha = 0;
        }


        surePanel[correct].transform.GetChild(0).GetChild(0).GetComponent<CorrectButton>().CorrectIndex = correct;
    }
}
