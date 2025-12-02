using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerPool : MonoBehaviour
{
    public GameObject customer;
    public int initNum;
    public static CustomerPool Instance = null;

    public Queue<GameObject> queue = new Queue<GameObject>();
    public GameObject initDestObject;
    //public GameObject exit;

    // 빵 진열대 -> 오브젝트 풀링에서 지정
    public GameObject display;
    //Display displayStat;
    //Basket basketStat;

    // 계산대 -> 오브젝트 풀링에서 지정
    public GameObject cashier;
    //Cashier cashierStat;

    // 의자 -> 오브젝트 풀에서 지정
    public GameObject[] chairs;

    // 돈 지불 위치 지정
    public GameObject breadMoney;
    public GameObject seatMoney;
    //Money breadMoneyStat;
    //Money seatMoneyStat;

    // 손님 번호
    int num = 0;

    void Awake()
    {
        // 싱글톤
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 손님 오브젝트 미리 생성
        for (int i = 0; i < initNum; i++)
        {
            Debug.LogFormat("CustomerPool: 손님 미리 생성");
            Create();
        }
    }
    void Initiate(GameObject obj)
    {
        // 오브젝트 이름 초기화
        obj.name = "Ty";

        // pool 위치 오브잭트의 위치/방향으로 초기화
        obj.transform.position = transform.position;
        obj.transform.rotation = transform.rotation;

        // 진행 여부 모두 초기화
        CustomerController stat = obj.GetComponent<CustomerController>();
        stat.customerProcess = CustomerController.process.goBread;

        // 빵 초기화
        stat.requiredBreads = 0;

        // 포장/자리 선택 초기화
        stat.customerOrder = 0;

        // 진열대, 계산대 지정
        stat.display = display;
        stat.displayStat = display.GetComponent<Display>();
        stat.basketStat = display.GetComponent<Basket>();
        stat.cashier = cashier;
        stat.cashierStat = cashier.GetComponent<Cashier>();

        // 생각(Canvas), 가방 초기화
        stat.happy.SetActive(false);
        stat.bag.SetActive(false);

        // 의자 위치 지정
        for(int i=0; i<chairs.Length; i++)
        {
            stat.chairs[i] = chairs[i];
        }

        // 빵 지불 위치 지정
        stat.breadMoney = breadMoney;
        stat.seatMoney = seatMoney;
        stat.breadMoneyStat = breadMoney.GetComponent<Money>();
        stat.seatMoneyStat = seatMoney.GetComponent<Money>();

        // 출구
        stat.exit =transform.position;

    }
    void SetCustomer(CustomerController stat)
    {
        // 자리/포장 여부
        int rand = Random.Range(0, 2);
        stat.customerOrder = (CustomerController.order)rand;

        // 필요한 빵 개수(자리에 앉을 경우 1개, 포장할 경우 1~5개)
        if (rand == 0) stat.requiredBreads = Random.Range(1, 6);
        else stat.requiredBreads = 1;
    }

    void Create()
    {
        GameObject obj = Instantiate(customer);
        obj.SetActive(false);
        Initiate(obj);
        obj.name = obj.name.Replace("(Clone)", "");
        queue.Enqueue(obj);
    }
    public GameObject CustomerIn()
    {
        Debug.LogFormat("CustomerPool: 손님 입장, 현재 큐 내부 오브젝트 수: {0}", queue.Count);
        GameObject obj;      
        obj = queue.Dequeue();
        num++;
        obj.name = obj.name+num.ToString();
        CustomerController stat = obj.GetComponent<CustomerController>();
        SetCustomer(stat);
        Debug.LogFormat("CustomerPooling: {0}의 필요 빵 개수: {1}, 자리/포장: {2}", obj.name, stat.requiredBreads, stat.customerOrder);
        obj.SetActive(true);
        // Customer에게 목적지를 일려줌 -> Customer는 NavMesh를 이용하여 이동
        stat.Move(initDestObject.transform.position);

        //Debug.LogFormat("Pool: {0} 입장", obj.name);
        return obj;
    }
    public void CustomerOut(GameObject customer)
    {
        // Enterance에서 도달하는 Customer를 큐로 되돌림
        //Instantiate(customer);
        customer.SetActive(false);
        queue.Enqueue(customer);
        Initiate(customer);
        Debug.LogFormat("CustomerPool: {0} 큐로 이동, 현재 큐 내부 오브젝트 수: {1}", customer, queue.Count);
    }
}
