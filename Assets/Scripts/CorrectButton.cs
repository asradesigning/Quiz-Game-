using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorrectButton : MonoBehaviour
{
    public int CorrectIndex;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BtnCLicked()
    {
        //Debug.Log("Selected Answer :" + CorrectIndex);
        LevelManager.instance.Answer(CorrectIndex);
       /* if (PlayerManager.instance.mode == PlayerMode.Multiplayer)
        {
        }
        else
        {
            LevelManager.instance.Answer(CorrectIndex);
        }   */
    }
}
