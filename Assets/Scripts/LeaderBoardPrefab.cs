using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardPrefab : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI Sno;
    [SerializeField] TextMeshProUGUI Name;
    [SerializeField] TextMeshProUGUI Value;
    //[SerializeField] GameObject Crown;

    public void SetPlayerDetails(int sno, string name, int value)
    {
        Sno.text = "#" +(sno + 1).ToString();
        Name.text = name;
        Value.text = value.ToString();

      /*  if (sno == 0)
        {
            Crown.SetActive(true);
        }
        else
        {
            Crown.SetActive(false);
        }
      */
    }
}
