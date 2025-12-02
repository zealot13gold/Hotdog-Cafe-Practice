using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basket : MonoBehaviour
{
    public int maxNumOfBread;
    [HideInInspector] public int numOfBread=0;

    public GameObject[] breads;
    //GameObject breadAnim;

    public void PutInBread()
    {
        if(numOfBread<maxNumOfBread)
        {
            // »§ Ã¤¿ì±â
            breads[numOfBread].SetActive(true);
            numOfBread++;
        }
    }

    public void PutOutBread()
    {
        Debug.LogFormat("Basket: ÇöÀç ³²Àº »§ÀÇ °³¼ö: {0}", numOfBread);

        // »§ »©±â
        numOfBread--;
        breads[numOfBread].SetActive(false);
    }

}




