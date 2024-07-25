using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;


    [SerializeField] LevelsData levelData;
    [SerializeField] Image[] levelImg;
    [SerializeField] GameObject[] sureBgImg;
    [SerializeField] GameObject[] surePanel;
    [SerializeField] GameObject contentPanel;
    public GameObject[] levelTxt;

    [SerializeField] int levelIndex = 0;
    [SerializeField] int correct = 0;
    private int questionIndex = 0;
    private int player_Xp = 0;
    private List<int> shuffledIndices;

    public string levelName;
   
    // Start is called before the first frame update

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        SetLevels();
        ShuffleQuestions();
        LoadQuestion();
        OpenQuestPanel();
        CorrectBtn();
        surePanel[0].SetActive(false);
        surePanel[1].SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        LoadQuestion();
        OpenQuestPanel();
        CorrectBtn();
        surePanel[0].SetActive(false);
        surePanel[1].SetActive(false);
    }

    public void SetLevels()
    {
        if(PlayerManager.instance != null)
        {
            levelName = PlayerManager.instance.levelName;
        }
       
        if (levelName == "Ancient")
        {
            levelData.TakeLevels = levelData.Ancient_levels;
            GameManager.instance.OpenBook(0);
        }
        else if (levelName == "Science")
        {
            levelData.TakeLevels = levelData.Science_levels;
            GameManager.instance.OpenBook(1);
        }
        else if (levelName == "Arts")
        {
            levelData.TakeLevels = levelData.Arts_levels;
            GameManager.instance.OpenBook(2);
        }
        else if (levelName == "Wars")
        {
            levelData.TakeLevels = levelData.Wars_levels;
            GameManager.instance.OpenBook(3);
        }
    }

    public void OpenQuestPanel()
    {
        LeanTween.alphaCanvas(contentPanel.GetComponent<CanvasGroup>(), 1, 0.5f);
        for(int i = 0; i < levelTxt.Length; i++) 
        {
            levelTxt[i].GetComponent<TypewriterEffect>().TextAnimation();
        }
    }

    void ShuffleQuestions()
    {
        shuffledIndices = new List<int>();
        for (int i = 0; i < levelData.TakeLevels.Length; i++)
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

            levelImg[0].sprite = levelData.TakeLevels[levelIndex].levelImg[0];
            levelTxt[0].GetComponent<TypewriterEffect>().fullText = levelData.TakeLevels[levelIndex].question1[0];

            levelImg[1].sprite = levelData.TakeLevels[levelIndex].levelImg[1];
            levelTxt[1].GetComponent<TypewriterEffect>().fullText = levelData.TakeLevels[levelIndex].question1[1];

            correct = levelData.TakeLevels[levelIndex].correctAnswerIndex;
        }
        else
        {
            Debug.Log("All questions have been answered!");
            // Optionally, you can reshuffle and start again or end the game
        }
    }

    public void LoadLevel()
    {
        levelIndex++;

        levelImg[0].sprite = levelData.TakeLevels[levelIndex].levelImg[0];
        levelTxt[0].GetComponent<TypewriterEffect>().fullText = levelData.TakeLevels[levelIndex].question1[0];

        levelImg[1].sprite = levelData.TakeLevels[levelIndex].levelImg[1];
        levelTxt[1].GetComponent<TypewriterEffect>().fullText = levelData.TakeLevels[levelIndex].question1[1];

        correct = levelData.TakeLevels[levelIndex].correctAnswerIndex;
    }

    public void Answer(int index)
    {
        if (index == correct)
        {
            
            player_Xp = 100;
            PlayerManager.instance.IncreaseScore(player_Xp);
            GameManager.instance.YouWIn();
        }
        else
        {
            
            GameManager.instance.YouLoose();
        }

        
    }

    public void CheckPanel(int index)
    {
        if(index == 0)
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
        for(int i = 0; i < surePanel.Length; i++)
        {
            surePanel[i].transform.GetChild(0).GetChild(0).GetComponent<CorrectButton>().CorrectIndex = 5;
            surePanel[i].SetActive(false);
            sureBgImg[i].SetActive(false);
            surePanel[i].GetComponent<CanvasGroup>().alpha = 0;
        }

        
        surePanel[correct].transform.GetChild(0).GetChild(0).GetComponent<CorrectButton>().CorrectIndex = correct;
        
    }

   
}
