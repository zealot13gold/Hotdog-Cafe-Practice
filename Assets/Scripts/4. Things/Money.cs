using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : MonoBehaviour
{
    public int breadPrice;
    public int muchOfMoney;
    public List<GameObject> moneyStock;


    void Start()
    {
        
    }

    public void PayMoneyForBread(int order, int breads)                  // 손님이 발동
    {
        int buffer = muchOfMoney + breadPrice*breads/(order+1);    // order가 0이면 포장, 1이면 자리 착석
        Debug.LogFormat("Money: 최대로 쌓을 수 있는 돈 액수: {0}", moneyStock.Count);
        if (buffer > moneyStock.Count)
        {
            buffer = moneyStock.Count;
            Debug.LogFormat("Money: 보관 한도 초과, 회수 필요");
        }

        for(int i=muchOfMoney; i<buffer; i++)
        {
            moneyStock[i].SetActive(true);
        }

        muchOfMoney = buffer;
        Debug.LogFormat("Money: 현재 쌓여 있는 돈: {0}", muchOfMoney);
    }

    public void PayMoneyFoSeat()                  // 손님이 발동
    {
        int buffer = muchOfMoney + breadPrice;    // 자리 착석일 경우에만 발동
        Debug.LogFormat("Money: 최대로 쌓을 수 있는 돈 액수: {0}", moneyStock.Count);
        if (buffer > moneyStock.Count)
        {
            buffer = moneyStock.Count;
            Debug.LogFormat("Money: 보관 한도 초과, 회수 필요");
        }

        for (int i = muchOfMoney; i < buffer; i++)
        {
            moneyStock[i].SetActive(true);
        }

        muchOfMoney = buffer;
        Debug.LogFormat("Money: 현재 쌓여 있는 돈: {0}", muchOfMoney);
    }

    void CreateMoney()
    {

    }

    public void GetMoney()          // 플레이어가 발동
    {
        if (muchOfMoney > 0)
        {
            muchOfMoney--;
            moneyStock[muchOfMoney].SetActive(false);
            Debug.LogFormat("Money: 남은 돈: {0}", muchOfMoney);
        }
        Debug.LogFormat("Money: 모두 회수");

        //StartCoroutine(GettingMoney());
    }

    IEnumerator GettingMoney()
    {
        while(muchOfMoney>0)
        {
            muchOfMoney--;
            moneyStock[muchOfMoney].SetActive(false);
            Debug.LogFormat("Money: 회수 중, 남은 돈: {0}", muchOfMoney);
            yield return null;
        }
        Debug.LogFormat("Money: 모두 회수");
    }
}
