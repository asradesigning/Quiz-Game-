using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
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
    [SerializeField] GameObject medal_Mainparent, medalFakeparent, player;
    int bookIndex = 0;
    public MultiplayerQuizManager multiplayerManager;
    public Slider level_Slider;
    public TextMeshProUGUI score_TXT;
    public TimeController timerScript;
    public Sprite MedalImage;
    public AudioSource reward1, reward2;
    bool changingState = false;
    Vector3 bookScale = new Vector3(6f, 6f, 6.2f);
    public int medalIndex = 0;
    string OpponentName = "";
    bool nextClicked = false;
    bool nextDelayCalled = false;
    public float timer = 0.0f;

    private void Awake()
    {
        instance = this;
  

    }

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonManager.instance.playerMode != PlayerMode.Offline)
        {
            var photonPlayer = PhotonNetwork.Instantiate(player.name, transform.position, transform.rotation);
            LevelManager.instance.player = photonPlayer.GetComponent<PhotonView>();
        }

        if (PlayerManager.instance != null && PlayerManager.instance.state == PlayerLevels.Beginner)
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
        timer = timerScript.GetCurrentTime();
    }

  

    public void AnswerGiven(string state, string name)
    {
        TimerStop();
        timerScript.StopTimer("Play");
        LeanTween.alphaCanvas(content_main, 0, 0.6f);
        text_01.ResetText();
        text_02.ResetText();
        OpponentName = name;
        content_main.transform.GetChild(0).gameObject.SetActive(false);
        book_main.SetActive(true);
        LeanTween.scale(book_main, bookScale, 0.5f);
        book_main.GetComponent<Animator>().Play("anim_01");
        OpenBook(bookIndex);
        nextDelayCalled = false;
        nextClicked = false;
        switch (state)
        {
            case "Win":
                LeanTween.delayedCall(1.4f, WritingDelay1);
                break;
            case "Lose":
                LeanTween.delayedCall(1.4f, WritingDelay2);
                break;
            case "OpponentWin":
                LeanTween.delayedCall(1.4f, WritingDelay3);
                break;
            case "OpponentLose":
                LeanTween.delayedCall(1.4f, WritingDelay4);
                break;
            case "LoseByTimeAccepted":
                LeanTween.delayedCall(1.4f, WritingDelay5);
                break;
            case "LoseByTimeNotAccepted":
                LeanTween.delayedCall(1.4f, WritingDelay6);
                break;
            case "LoseByTimeAcceptedOpponent":
                LeanTween.delayedCall(1.4f, WritingDelay7);
                break;
            default:
                break;
        }
    }

    void WritingDelay1()
    {
        WinPanel.SetActive(true);
        WinPanel.transform.GetChild(0).GetComponent<TypewriterEffect>().StartWriting();
        Invoke("NextBtnClicked", 3f);
    }

    void WritingDelay2()
    {
        loosePanel.SetActive(true);
        loosePanel.transform.GetChild(0).GetComponent<TypewriterEffect>().StartWriting();
        Invoke("NextBtnClicked", 3f);
    }
    void WritingDelay3()
    {
        WinPanel.SetActive(true);
        bookFlipbtns1[1].gameObject.SetActive(false);
        bookFlupBtns2[1].gameObject.SetActive(false);
        WinPanel.transform.GetChild(0).GetComponent<TypewriterEffect>().StartWritingForOpponent("Win", OpponentName);
        OpponentName = null;
    }

    void WritingDelay4()
    {
        loosePanel.SetActive(true);
        bookFlipbtns1[1].gameObject.SetActive(false);
        bookFlupBtns2[1].gameObject.SetActive(false);
        loosePanel.transform.GetChild(0).GetComponent<TypewriterEffect>().StartWritingForOpponent("Lose", OpponentName);
        OpponentName = null;
    }

    void WritingDelay5()
    {
        loosePanel.SetActive(true);
        loosePanel.transform.GetChild(0).GetComponent<TypewriterEffect>().StartWritingForTimeLose("Accepted");
        Invoke("NextBtnClicked", 3f);
    }


    void WritingDelay6()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            loosePanel.SetActive(true);
            loosePanel.transform.GetChild(0).GetComponent<TypewriterEffect>().StartWritingForTimeLose("NotAccepted");
            Invoke("NextBtnClicked", 3f);
        }
        else
        {
            loosePanel.SetActive(true);
            bookFlipbtns1[1].gameObject.SetActive(false);
            bookFlupBtns2[1].gameObject.SetActive(false);
            loosePanel.transform.GetChild(0).GetComponent<TypewriterEffect>().StartWritingForTimeLose("NotAccepted");
            OpponentName = null;
        }
    }

    void WritingDelay7()
    {
        loosePanel.SetActive(true);
        bookFlipbtns1[1].gameObject.SetActive(false);
        bookFlupBtns2[1].gameObject.SetActive(false);
        loosePanel.transform.GetChild(0).GetComponent<TypewriterEffect>().StartWritingForOpponent("LoseByTimeAcceptedOpponent", OpponentName);
        OpponentName = null;
    }


    public void NextBtnClicked()
    {
        if (!nextClicked)
        {
            nextClicked = true;
            if (!PhotonNetwork.IsConnected)
            {
                WinPanel.SetActive(false);
                loosePanel.SetActive(false);
                book_main.SetActive(false);
                LeanTween.delayedCall(1.5f, NextLevelDelay);
            }
            else
            {
                LevelManager.instance.player.RPC("RPC_NextButtonClicked", RpcTarget.AllBuffered);
            }
        }
    }

    public void RPC_NextButtonClicked()
    {
        WinPanel.SetActive(false);
        loosePanel.SetActive(false);
        book_main.SetActive(false);
        LeanTween.delayedCall(1.5f, NextLevelDelay);
    }

    void NextLevelDelay()
    {
        if (!nextDelayCalled)
        {
            nextDelayCalled = true;
            if (!PhotonNetwork.IsConnected)
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
            else
            {
                if(PhotonNetwork.IsMasterClient)
                LevelManager.instance.player.RPC("RPC_NextLevelDelay", RpcTarget.AllBuffered);
            }
        }  
    }

    public void RPC_NextLevelDelay()
    {
        if (!changingState)
        {

            LoadNextLevel();
            Debug.LogWarning("Calling Load With RPC");
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
        LevelManager.instance.StartGame();
        /*if(PlayerManager.instance.mode == PlayerMode.Offline)
        {
        }
        else
        {
           multiplayerManager.StartGame();
        }*/
    }

    public void TimerPlay()
    {
        LevelManager.instance.buzzer.gameObject.SetActive(true);
       // Animator timerAnimator = content_main.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Animator>();
        content_main.transform.GetChild(0).gameObject.SetActive(true);
        timerScript.StartTimer("Buzzer");
    }
    public void TimerStop()
    {
        LevelManager.instance.buzzer.gameObject.SetActive(false);
        timerScript.ResetTimer("Buzzer");
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
        if (timerScript.isBuzzerTimerRunning)
        {
            timerScript.StopTimer("Buzzer");
        }
        else if (timerScript.isPlayTimerRunning)
        {
            timerScript.StopTimer("Play");
        }
        
    }

    public void ContinueGame()
    {
        if (timerScript.isBuzzerTimerRunning)
        {
            timerScript.StartTimer("Buzzer");
        }
        else if (timerScript.isPlayTimerRunning)
        {
            timerScript.StartTimer("Play");
        }
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
        Debug.LogWarning("Calling Load With CloseDelay()");
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
        bookIndex = index;
        for (int i = 0; i < background_books.Length; i++)
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
