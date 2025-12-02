using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cashier : MonoBehaviour
{
    public Vector3[] line;

    public Queue<GameObject> customerList=new Queue<GameObject>();

    public bool isPlayerOn;

    public GameObject bag;

    public Vector3 PutInCustomer(int order)
    {
        Vector3 target;
        if(order==0) target = new Vector3(-2f, 0.5f, -2f);
        else target = new Vector3(-4f, 0.5f, -2f);

        return target;
    }

    public void PutInCustomer(GameObject customer)
    {
        customerList.Enqueue(customer);
        //Debug.LogFormat("Cashier: {0}이 큐 내부로 이동, 현재 큐 내부의 오브젝트 개수: {1}", customer, customerList.Count);
    }

    // 손님이 계산대 줄에 들어설 때, 계산대 줄에서 대기하고 있을 때 주기적으로 실행
    public Vector3 Sorting(GameObject customer)
    {
        Vector3 target=customer.transform.position;

        int count = 0;
        foreach (GameObject c in customerList)
        {
            //Debug.LogFormat("cashier: 큐의 {0}번째 오브젝트 이름 확인", count);
            if(customer.name==c.name)
            {
                Debug.LogFormat("Cashier: {0}과 {1}이 일치", c.name, customer.name);
                target = line[count];
                //Debug.LogFormat("Cashier: {0}은 계산대의 {1}번째로 이동", customer.name, count);
            }
            count++;
            if (line.Length <= count) break;
            
        }
        return target;
    }

    public void PopOutCustomer()
    {
        Debug.LogFormat("Cashier: {0}은 계산이 끝나 줄에서 이탈, 현재 큐 내부의 오브젝트 개수: {1}", customerList.First().name, customerList.Count);
        customerList.Dequeue();
    }

    
}
