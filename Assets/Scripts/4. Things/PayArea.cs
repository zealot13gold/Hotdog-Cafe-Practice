using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PayArea : MonoBehaviour
{
    // 요구되는 액수
    public TMP_Text requiredMoneyText;
    public int requiredMoney;

    // 사라지는 오브젝트
    public GameObject[] disappearedObj;

    // 생성되는 오브젝트
    public GameObject[] appearedObj;

    public void payForObject()
    {
        StartCoroutine(Paying());
    }

    public void StopPay()
    {
        StopCoroutine(Paying());
    }

     void AppearedObject()
    {
        foreach(GameObject obj in appearedObj)
        {
            obj.SetActive(true);
        }
    }
     void DisappearedObject()
    {
        foreach (GameObject obj in disappearedObj)
        {
            obj.SetActive(false);
        }
    }
    
    IEnumerator Paying()
    {
        while(requiredMoney>0)
        {
            //requiredMoney--;
            Debug.LogFormat("PayArea: 남은 요구금액: {0}", requiredMoney);
            requiredMoneyText.text = requiredMoney.ToString();
            yield return new WaitForSeconds(0.1f);
        }

        AppearedObject();
        DisappearedObject();
        gameObject.SetActive(false);
    }
}
