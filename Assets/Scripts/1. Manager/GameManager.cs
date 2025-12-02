using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 싱글톤
    public static GameManager Instance;

    public GameObject display;
    public GameObject calculator;

    Display displayStat;

    // 진열대의 손님 수 -> 손님 추가 여부
    private void Awake()
    {
        // 싱글톤
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        displayStat = display.GetComponent<Display>();
    }
    void Start()
    {
        // 게임 시작하자마자 손님 생성
        StartCoroutine(CreateCustomer());
    }

    IEnumerator CreateCustomer()
    {
        while (true)
        {
            Debug.LogFormat("GameManager: 현재 진열대의 손님이 {0}명", displayStat.waitCustomersList.Count);
            if (displayStat.waitCustomersList.Count < displayStat.maxNumWaitCustomers && CustomerPool.Instance.queue.Count>0)
            {
                //Debug.LogFormat("GameManager: 새로운 손님 호출");
                CustomerPool.Instance.CustomerIn();
            }
            float delayTime = Random.Range(3f, 10f);
            yield return new WaitForSeconds(delayTime);
        }
    }
}
