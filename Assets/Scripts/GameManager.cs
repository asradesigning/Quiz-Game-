using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] GameObject[] background_books;
    [SerializeField] GameObject[] bookFlipbtns1, bookFlupBtns2;
    [SerializeField] Image[] medalImages;
    [SerializeField] GameObject loosePanel, WinPanel;
    [SerializeField] CanvasGroup content_main;
    [SerializeField] GameObject book_main, bookParent, giveRewardPanel;
    [SerializeField] TypewriterEffect text_01, text_02;
    [SerializeField] GameObject medalsObj, medalAnim;
    [SerializeField] GameObject medal_Mainparent, medalFakeparent;

    public MultiplayerQuizManager multiplayerManager;
    public Slider level_Slider;
    public TextMeshProUGUI score_TXT;
    public TimeController timerScript;
    public Sprite MedalImage;
    public AudioSource reward1, reward2;
    bool changingState = false;
    Vector3 bookScale = new Vector3(6f, 6f, 6.2f);
    public int medalIndex = 0;

    private void Awake()
    {
        instance = this;
  

    }

    // Start is called before the first frame update
    void Start()
    {

        
        if(PlayerManager.instance != null && PlayerManager.instance.state == PlayerLevels.Beginner)
        {
            MedalChange(0);
            background_books[0].SetActive(true);
            bookFlipbtns1[0].SetActive(true);
            bookFlupBtns2[0].SetActive(true);
        }
            
        
        book_main.SetActive(false);
        giveRewardPanel.SetActive(false);
        book_main.transform.localScale = new Vector3(0, 0, 0);
        if (PlayerManager.instance != null)
        {
            PlayerManager.instance.LoadPlayerItems();
        }
        content_main.transform.GetChild(0).gameObject.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

  

    public void YouWIn()
    {
        TimerStop();
        LeanTween.alphaCanvas(content_main, 0, 0.6f);
        
        text_01.ResetText();
        text_02.ResetText();
        
        content_main.transform.GetChild(0).gameObject.SetActive(false);
        book_main.SetActive(true);
        LeanTween.scale(book_main, bookScale, 0.5f);
        book_main.GetComponent<Animator>().Play("anim_01");

        LeanTween.delayedCall(1.4f, WritingDelay1);
    }

    void WritingDelay1()
    {
        WinPanel.SetActive(true);
        WinPanel.transform.GetChild(0).GetComponent<TypewriterEffect>().StartWriting();
    }

    public void YouLoose()
    {
        TimerStop();
        LeanTween.alphaCanvas(content_main, 0, 0.6f);
        text_01.ResetText();
        text_02.ResetText();

        content_main.transform.GetChild(0).gameObject.SetActive(false);
        book_main.SetActive(true);
        LeanTween.scale(book_main, bookScale, 0.5f);
        book_main.GetComponent<Animator>().Play("anim_01");
        LeanTween.delayedCall(1.4f, WritingDelay2);
    }

    void WritingDelay2()
    {
        loosePanel.SetActive(true);
        loosePanel.transform.GetChild(0).GetComponent<TypewriterEffect>().StartWriting();
    }

    public void NextBtnClicked()
    {
        WinPanel.SetActive(false);
        loosePanel.SetActive(false);
        book_main.SetActive(false);
        LeanTween.delayedCall(1.5f , NextLevelDelay);

    }

    void NextLevelDelay()
    {
        if (!changingState)
        {

            LoadNextLevel();
        }
        else
        {
            giveRewardPanel.gameObject.SetActive(true);
            LeanTween.alphaCanvas(giveRewardPanel.GetComponent<CanvasGroup>(), 1, 0.6f);
        }
        
        

    }

    public void LoadNextLevel()
    {
        Vector3 scale = new Vector3(0, 0, 0);
        book_main.SetActive(false);
        LeanTween.scale(book_main, scale, 0.5f);
        if(PlayerManager.instance.mode == PlayerMode.Offline)
        {
            LevelManager.instance.StartGame();
        }
        else
        {
           multiplayerManager.StartGame();
        }
        
    }

    public void TimerPlay()
    {
       // Animator timerAnimator = content_main.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Animator>();
        content_main.transform.GetChild(0).gameObject.SetActive(true);
        timerScript.StartTimer();
    }
    public void TimerStop()
    {
        timerScript.StopTimer();
        timerScript.ResetTimer();
        content_main.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void RestartBtnClicked()
    {

    }

    public void PlayClickSound()
    {
        if(SoundManager.instance != null)
        {
            SoundManager.instance.PlayClick();
        }
    }

    public void QuitGame()
    {
        SceneManager.LoadScene(0);
    }

    public void Pause()
    {
        timerScript.StopTimer();
    }

    public void ContinueGame()
    {
        timerScript.ResumeTimer();
        timerScript.StartTimer();
    }

    public void StateChanged()
    {
        if(PlayerManager.instance != null)
        {
            if(PlayerManager.instance.state == PlayerLevels.Competent)
            {
                changingState = true;
                medalIndex = 1;
               
                medalsObj.transform.SetParent(medalFakeparent.transform);
            }
            else if(PlayerManager.instance.state == PlayerLevels.Veteran)
            {
                changingState = true;
                medalIndex = 2;
               
                medalsObj.transform.SetParent(medalFakeparent.transform);
            }
            else if (PlayerManager.instance.state == PlayerLevels.Elite)
            {
                changingState = true;
                medalIndex = 3;
               
                medalsObj.transform.SetParent(medalFakeparent.transform);
            }
            else if (PlayerManager.instance.state == PlayerLevels.Genius)
            {
                changingState = true;
                medalIndex = 4;
              
                medalsObj.transform.SetParent(medalFakeparent.transform);
            }
       
        }
        
    }

    public void PlayReward(int index)
    {
        if(index == 0)
        {
            reward1.Play();
        }
        else
        {
            reward2.Play();
        }
    }

    public void CloseRewards()
    {
        LeanTween.alphaCanvas(giveRewardPanel.GetComponent<CanvasGroup>(), 0, 0.6f);
        LeanTween.delayedCall(0.7f, CloseDelay);
    }

    void CloseDelay()
    {
        medalsObj.transform.SetParent(medal_Mainparent.transform);
        giveRewardPanel.SetActive(false);
        changingState = false;
        TimerPlay();
        LoadNextLevel();
    }

    public void ChangeFakeMedal()
    {
        MedalChange(medalIndex);
    }

    void MedalChange(int index)
    {
        for(int i = 0; i< medalImages.Length; i++)
        {
            
            medalImages[i].enabled = false;
        }
        medalImages[index].enabled = true;
       
    }

    public void ChangeBigImage()
    {
        for (int i = 0; i < medalImages.Length; i++)
        {
            medalAnim.transform.GetChild(i).GetComponent<Image>().enabled = false;
          
        }
        medalAnim.transform.GetChild(medalIndex - 1).GetComponent<Image>().enabled = true;
    }

    public void OpenBook(int index)
    {
        for(int i=0; i < background_books.Length; i++)
        {
            background_books[i].SetActive(false);
            bookFlipbtns1[i].SetActive(false);
            bookFlupBtns2[i].SetActive(false);
        }
        bookFlipbtns1[index].SetActive(true);
        bookFlupBtns2[index].SetActive(true);
        background_books[index].SetActive(true);
    }
}
