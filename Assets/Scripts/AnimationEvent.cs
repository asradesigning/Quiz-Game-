using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationEvent : MonoBehaviour
{
    [SerializeField] Animator maskAnim;
    [SerializeField] Image medalImg;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TimeOver()
    {
       GameManager.instance.YouLoose();
    }

    public void PlayMusic()
    {
        SoundManager.instance.TickSound();
    }

    public void ChangeSprite()
    {
        for(int i = 0; i < 5; i++)
        {
            this.transform.GetChild(0).GetChild(i).GetComponent<Image>().enabled = false;
        }
       this.transform.GetChild(0).GetChild(GameManager.instance.medalIndex).GetComponent<Image>().enabled = true;
    }

    public void PlayerMask()
    {
        maskAnim.Play("Mask_Anim");
        GameManager.instance.PlayReward(1);
    }

    public void RewardSound()
    {
        GameManager.instance.ChangeBigImage();
        GameManager.instance.PlayReward(0);
    }

    public void Reward_CLose()
    {
        GameManager.instance.CloseRewards();
    }

    public void FakeMedal()
    {
        GameManager.instance.ChangeFakeMedal();
    }
}
