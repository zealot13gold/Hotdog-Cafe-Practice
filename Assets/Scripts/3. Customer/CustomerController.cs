using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CustomerController : MonoBehaviour
{
    // 빵
    [HideInInspector] public int requiredBreads;
    public int maxNumOfBreads;
    [HideInInspector] public int numOfBreads=0;
    public GameObject[] breads;
    public GameObject putBreadAnim;
    public GameObject removeBreadAnim;

    // 빵 진열대 -> 오브젝트 풀링에서 지정
    public GameObject display;
    [HideInInspector] public Display displayStat;
    [HideInInspector] public Basket basketStat;

    // 계산대 -> 오브젝트 풀링에서 지정
    public GameObject cashier;
    [HideInInspector] public Cashier cashierStat;

    // 의자 -> 오브젝트 풀에서 지정
    public GameObject[] chairs;
    //public GameObject targetChair;

    // 돈 지불 -> 오브젝트 풀링에서 지정
    public GameObject breadMoney;               // 계산대에서 지불
    public GameObject seatMoney;                // 자리에서 지불
    [HideInInspector] public Money breadMoneyStat;
    [HideInInspector] public Money seatMoneyStat;

    // 사운드
    public AudioSource sound;
    public AudioClip breadInClip;
    public AudioClip breadOutClip;
    public AudioClip moneyPayClip;

    // 자리
    //public List<GameObject> seat = new List<GameObject>();           // 게임 진행 중에 추가
    Seat seatStat;

    // 출구
    public Vector3 exit;


    // 계산 후 손님 포장/자리 여부
    public enum order 
    { 
        togo,                       // 포장
        seat                        // 자리 착석
    };
    public order customerOrder;

    // 이동
    public NavMeshAgent nav;
    public Animator anim;

    // 진행 상태
    public enum process 
    { 
        goBread,                    // 빵 진열대로 이동
        getBread,                   // 빵 획득
        goCalculate,                // 빵 획득 후 계산대로 이동
        waitCalculate,           // 계산 대기
        waitSeat,                   // 계산 후 자리를 위해 계산대에서 대기
        goSeat,                     // 계산 후 자리로 이동
        getSeat,                    // 자리에서 대기
        goOut                       // 계산 후 밖으로 이동
    };
    public process customerProcess;

    // 주요 point
    public GameObject point;

    // 종이 봉투
    public GameObject bag;
    //public Vector3 beforePos;
    //public Vector3 afterPos;

    //UI, 이펙트
    public Canvas canvas;
    public GameObject thinkingImage;
    public GameObject breadImage;
    public TMP_Text requiredBreadText;
    public GameObject payImage;
    public GameObject SeatImage;
    public GameObject happy;
    //public ParticleSystem happy;


    private void LateUpdate()           // 코루틴 방식으로 수정 필요
    {
        // 이미지는 항상 카메라를 향해 보도록 함
        canvas.gameObject.transform.LookAt(Camera.main.transform.position);
        canvas.gameObject.transform.forward = Camera.main.transform.forward;
    }

    // 분기점(startPoint), 대기열, 자리, 출구 도달 여부 확인
    private void OnTriggerEnter(Collider other)
    {
        // 분기점(startPoint) -> 손님 진행 상태에 따라 빵 진열대/계산대로 이동
        if (other.gameObject.name == "StartPoint")
        {
            Debug.LogFormat("CoustomerController: {0} 진입", other.name);

            if (customerProcess == process.goBread)
            {
                Vector3 target = displayStat.PutInCustomer(gameObject);
                goBread(target);

            }
            else if (customerProcess == process.goCalculate)
            {
                    StartCoroutine(GoingCalculate());
            }
        }

        // 의자에 도달 -> 의자에 머무른 후 출구로 이동
        else if(other.gameObject.name == "Chair" && customerProcess == process.goSeat)
        {
            customerProcess = process.getSeat;
            seatStat = other.gameObject.GetComponent<Seat>();

            // 손에 있는 빵 모두 제거
            while(numOfBreads>0)
            {
                numOfBreads--;
                breads[numOfBreads].SetActive(false);
            }

            StartCoroutine(getSeat());
        }

        // 출구 도달 -> 오브젝트 풀로 이동
        else if (other.gameObject.name == "CustomerPool" && customerProcess == process.goOut)
        {
            Debug.LogFormat("CustomerProcess: {0} 상태의 손님 나감", customerOrder);
            Debug.LogFormat("CustomerProcess: 손님 진행 상태: {0}", customerProcess);
            CustomerPool.Instance.CustomerOut(gameObject);
        }
    }
    public void Move(Vector3 dest)
    {
        nav.destination = dest;
        // 애니메이션 출력
        anim.SetBool("Walk", true);
    }
    public void Idle()
    {
        // 도착 시 애니메이션 해제 
        anim.SetBool("Walk", false);
    }

    public void goBread(Vector3 target)
    {
        StartCoroutine(GoingToBread(target));   
    }

    IEnumerator GoingToBread(Vector3 target)
    {
        Move(target);
        while (true)
        {
            float sqrDist = (target - transform.position).sqrMagnitude;
            if (sqrDist <= 5f) break;

            yield return null;
        }
        customerProcess = process.getBread;
        
        Debug.LogFormat("CustomerController: {0} 필요 빵 개수: {1}", gameObject.name, requiredBreads);
        StartCoroutine(BreadGetting(basketStat));
    }

    IEnumerator BreadGetting(Basket basket)
    {
        Idle();
        if (customerProcess == process.getBread)
        {
            thinkingImage.SetActive(true);
            breadImage.SetActive(true);
            requiredBreadText.gameObject.SetActive(true);
            requiredBreadText.text = requiredBreads.ToString();
        }
        yield return new WaitForSeconds(0.5f);

        // 필요한 빵 개수에 도달할 때까지 대기
        while (numOfBreads <requiredBreads)
        {
            Debug.LogFormat("CustomerController: 바구니 안의 빵 개수: {0}", basket.numOfBread);
            if (basket.numOfBread == 0)
            {
                Debug.LogFormat("CustomerController: 현재 빵 없음, {0} 대기",gameObject.name);
                yield return new WaitForSeconds(0.5f);
                //continue;
            }
            else
            {
                // 빵 가져오는 애니메이션 실행
                basket.PutOutBread();               // 바구니의 빵 개수가 먼저 사라짐
                Debug.LogFormat("CustomerController: {0} 빵이 {1}로 이동", basket.breads[basket.numOfBread], breads[numOfBreads]);
                putBreadAnim.transform.position = new Vector3(0f, breads[numOfBreads].transform.position.y, 0f);
                putBreadAnim.SetActive(true);
                sound.PlayOneShot(breadInClip);

                yield return new WaitForSeconds(0.06f);
                putBreadAnim.SetActive(false);
                breads[numOfBreads].SetActive(true);
                numOfBreads++;
                if (numOfBreads >= 1) anim.SetBool("Bring", true);
                requiredBreadText.text = (requiredBreads-numOfBreads).ToString();
            }
        }
        Debug.LogFormat("CustomerController: numOfBread: {0}, requiredBreads:{1}이므로 진열대에서 벗어남", numOfBreads, requiredBreads);
        display.GetComponent<Display>().PopOutCustomer(gameObject);

        // 빵을 들고 startPoint로 이동
        Debug.LogFormat("CustomerController: required: {0}, 현재 보유 빵: {1}", requiredBreads, numOfBreads);
        customerProcess = process.goCalculate;
        point = GameObject.Find("StartPoint");
        Debug.LogFormat("CustomerController: {0}을 {1}로 이동", name, point.name);

        // 이미지 교체
        breadImage.SetActive(false);
        requiredBreadText.gameObject.SetActive(false);
        payImage.SetActive(true);
        Move(point.transform.position);
    }

    IEnumerator GoingCalculate()
    {
        // 계산대 줄 첫 진입
        customerProcess = process.waitCalculate;
        cashierStat.PutInCustomer(gameObject);
        Debug.LogFormat("Customer: 계산대 첫 진입");
        Debug.LogFormat("CustomerProcess: 손님 진행 상태: {0}", customerProcess);
        while (true)
        {
            Vector3 target = cashierStat.Sorting(gameObject);
            yield return new WaitForSeconds(1f);
            if ((target - transform.position).sqrMagnitude <= 1f)
            {
                Idle();
            }
            else
            {
                Move(target);
                //Debug.LogFormat("Customer: {0}은 cashier의 {1}번째로 이동", gameObject.name, target.z);
            }

            Debug.LogFormat("Customer: 계산대 대기, {0}으로 이동", target.z);
            Debug.LogFormat("Customer: {0}과 계산대 사이의 거리제곱: {1}", gameObject.name, (cashier.transform.position - transform.position).sqrMagnitude);
            if (Mathf.Abs(transform.position.z-(-1.5f))<0.1f)
            {
                break;
            }
        }
        StartCoroutine(waitingCalculate());
    }

    IEnumerator waitingCalculate()
    {
        Idle();
        Debug.LogFormat("CustomerProcess: 손님 진행 상태: {0}", customerProcess);
        while (true)
        {
            
            // 플레이어가 계산대에 존재하는지 확인 -> break
            if (customerProcess == process.waitCalculate && cashierStat.isPlayerOn)
            {
                Debug.LogFormat("CustomerConroller: 플레이어 존재");
                break;

            }
            yield return new WaitForSeconds(1f);
        }
        sound.PlayOneShot(moneyPayClip);
        breadMoneyStat.PayMoneyForBread((int)customerOrder, requiredBreads);                           // 돈 지불

        payImage.SetActive(false);
        
        if (customerOrder == order.seat)
        {
            // 큐에서 벗어남 -> 계산대 옆으로 이동 -> 테이블로 이동
            SeatImage.SetActive(true);
            cashierStat.PopOutCustomer();
            customerProcess = process.waitSeat;
            // 계산대 옆으로 이동
            Vector3 pos = transform.position + new Vector3(-2.5f, 0f, 0f);
            Move(pos);
            if (pos.x - transform.position.x <= 0.1f) Idle();

            // 사용가능한 테이블이 나타날 때까지 대기
            StartCoroutine(waitingSeat());
        }
        else
        {
            // 포장 후 퇴장(큐에서 벗어남)
            thinkingImage.SetActive(false);
            StartCoroutine(Packing());
            cashierStat.PopOutCustomer();
        }
        
    }

    IEnumerator waitingSeat()
    {
        Idle();
        Vector3 target = transform.position;
        Debug.LogFormat("CustomerProcess: 손님 진행 상태: {0}", customerProcess);
        // 앉을 수 있는 자리가 있는지 확인
        while (true)
        {
            foreach (GameObject chair in chairs)
            {
                if (chair.activeSelf)
                {
                    seatStat = chair.GetComponent<Seat>();
                    if (seatStat.isUsing)
                    {
                        //target = chair.transform.position;
                        target = seatStat.GetSeat();                                 // 의자 위로 이동
                        customerProcess = process.goSeat;
                        Debug.LogFormat("CustomerController: 손님이 자리를 찾음, {0}으로 이동", target);
                        break;
                    }
                }
                yield return null;
            }
            if (customerProcess == process.goSeat) break;
            yield return new WaitForSeconds(1f);
        }
        /*if (customerProcess == process.goSeat)  */StartCoroutine(GoSeat(target));
    }
    IEnumerator GoSeat(Vector3 target)
    { 
        Move(target);
        SeatImage.SetActive(false);
        thinkingImage.SetActive(false);
        yield return null;
    }    

    IEnumerator getSeat()
    {
        Vector3 beforeSeat = transform.position;            // 앉기 전 위치 저장
        
        anim.SetBool("Sit", true);                          // 앉기 애니메이션
        seatStat.bread.SetActive(true);
        Idle();
        //SeatImage.SetActive(false);

        //일정 시간이 지나면 자리를 벗어남
        yield return new WaitForSeconds(5f);
        anim.SetBool("Sit", false);
        transform.position = beforeSeat;                    // 앉기 전 위치로 이동
        seatStat.UsedSeat();                                // 자리를 떠남
        seatMoneyStat.PayMoneyFoSeat();
        customerProcess = process.goOut;
        happy.gameObject.SetActive(true);
        //thinkingImage.SetActive(false);
        Move(exit);
        Debug.LogFormat("CustomerProcess: 손님 진행 상태: {0}", customerProcess);
    }

    IEnumerator Packing()
    {
        Debug.LogFormat("CustomerController: {0} 등장", bag.name);
        
        GameObject bagPacking = cashierStat.bag;
        //Animator packingAnim = bagPacking.GetComponent<Animator>();
        //removeBreadAnim = cashierStat.breadPackingAnim;
        bagPacking.SetActive(true);
        
        yield return new WaitForSeconds(1f);

        while(numOfBreads>0)
        {
            transform.LookAt(bagPacking.transform);
            numOfBreads--;
            breads[numOfBreads].SetActive(false);
            removeBreadAnim.transform.position = new Vector3(0f, breads[numOfBreads].transform.position.y, 0f);
            removeBreadAnim.SetActive(true);
            sound.PlayOneShot(breadOutClip);
            yield return new WaitForSeconds(0.1f);
            removeBreadAnim.SetActive(false);
            //transform.LookAt(bagPacking.transform);
            yield return new WaitForSeconds(0.9f);
            Debug.LogFormat("CustomerController: 포장 중, {0}개 남음", numOfBreads);
        }
        //bag.GetComponent<Animator>().SetBool("Full", true);
        Vector3 target = breads[0].transform.position;
        while ((target-bagPacking.transform.position).sqrMagnitude >2f)
        {
            bagPacking.transform.position = Vector3.Slerp(bag.transform.position, target, 1f);
            yield return new WaitForSeconds(0.1f);
            
            Debug.LogFormat("CustomerController: 포장 남은 거리: {0}", (target - bag.transform.position).sqrMagnitude);
        }
        bagPacking.SetActive(false);

        //bag.transform.position = beforePos;
        bag.SetActive(true);
        Debug.LogFormat("CustomerController: 포장 완료");

        customerProcess = process.goOut;
        happy.gameObject.SetActive(true);
        Move(exit);
    }
}
