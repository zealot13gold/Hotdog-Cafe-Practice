using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static CustomerController;

public class Display : Interaction
{
    public int maxNumWaitCustomers;
    //int numWaitCustomers;
    public Vector3 [] waitSites;
    int waitSiteNum = 0;
    public List<GameObject> waitCustomersList = new List<GameObject>();
    int callCount = -1;

    Basket basketStat;

    private void Awake()
    {
        basketStat = GetComponent<Basket>();
    }

    private void Start()
    {

    }

    public override Vector3 PutInCustomer(GameObject customer)
    {
        callCount++;
        Debug.LogFormat("Display: 현재 대기 중인 손님: {0}", waitCustomersList.Count);
        if (waitCustomersList.Count < maxNumWaitCustomers)
        {

            // 지정된 위치에 손님이 없다면 손님을 지정된 위치로 이동
            //for (int i = 0; i < waitSites.Count(); i++)
            //{
                Debug.LogFormat("Display: {0} 위치에 손님이 있는지 확인", waitSites[callCount%3]);
                //if (!Physics.Raycast(waitSites[callCount % 3], Vector3.up, 2f, customer.layer))
                //{
                    // 손님 목록에 추가 
                    waitCustomersList.Add(customer);
                    Debug.LogFormat("Display: {0} 위치로 {1} 이동", waitSites[callCount % 3], customer);
                    // 지정된 위치로 손님을 불러옴
                    return waitSites[callCount % 3];
                //}
            //}
        }
        return customer.transform.position;     // 현재 위치에서 대기
    }

    public override void PopOutCustomer(GameObject customer)
    {
        if (waitCustomersList.Count > 0)
        {
            // 손님 목록에서 제거
            waitCustomersList.Remove(customer);
        }
    }
}
