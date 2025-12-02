using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreadOven : MonoBehaviour
{
    // 바구니 정보
    public GameObject basket;
    Basket basketStat;

    // 빵 애니메이션
    public GameObject makeBasketAnim;
    private void Awake()
    {
        basketStat = basket.GetComponent<Basket>();
        makeBasketAnim.SetActive(false);
    }

    void Start()
    {
        StartCoroutine(MakeBread());
    }

    IEnumerator MakeBread()
    {
        while (true)
        {
            if(basketStat.numOfBread == basketStat.maxNumOfBread)
            {
                yield return new WaitForSeconds(1f);
                continue;
            }
            makeBasketAnim.SetActive(true);
            yield return new WaitForSeconds(2f);
            makeBasketAnim.SetActive(false);
            basketStat.PutInBread();
        }
    }
}
