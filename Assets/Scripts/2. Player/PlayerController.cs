using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int maxNumOfBread;
    int numOfBread;

    public GameObject[] breads;
    public GameObject breadPutOutAnim;
    public GameObject breadPutInAnim;
    public Canvas breadMax;

    public Animator anim;

    Coroutine coroutine;

    // 상호작용 물체
    Basket basketStat;
    Basket displayBasketStat;
    Cashier cashierStat;
    Money moneyStat;
    PayArea payStat;
    Table tableStat;

    // 이동 및 회전 속도, 이동/회전 UI
    public float moveSpeed;
    public float rotateSpeed;
    public GameObject cameraObj;
    public float cameraSpeed;

    // 현재 보유중인 돈 계산 및 UI 갱신
    public int money;
    public TMP_Text moneyText;

    // 빵, 돈 관련 사운드
    public AudioSource sound;
    public AudioClip breadInClip;
    public AudioClip breadOutClip;
    public AudioClip moneyGetClip;

    private void Awake()
    {

    }

    private void LateUpdate()                               // 코루틴 방식으로 수정 필요
    {
        if (numOfBread >= maxNumOfBread)
        {
            // MAX 표시
            breadMax.gameObject.SetActive(true);
            breadMax.gameObject.transform.LookAt(Camera.main.transform.position);
            breadMax.gameObject.transform.forward = Camera.main.transform.forward;
        }
    }

    // 플레이어 이동/회전은 UI에서 작동
    public void Move(Vector3 dist)
    {
        // UI의 중심과 드래그 위치의 거리차 입력
        transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed);
        transform.position = new Vector3(transform.position.x, 0.4f, transform.position.z);
        cameraObj.transform.position = new Vector3(transform.position.x, cameraObj.transform.position.y, transform.position.z);

        // 이동 시 애니메이션 출력
        anim.SetBool("Move", true);

    }
    public void Idle()
    {
        anim.SetBool("Move", false);
    }

    public void Rotate(Vector3 dir)
    {
        // UI의 중심과 드래그 위치의 각도 입력
        Vector3 newDir = new Vector3 (-dir.x, 0f, -dir.y);
        transform.LookAt(newDir);
    }

    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.name == "Basket" && numOfBread<maxNumOfBread)
        {
            basketStat = other.gameObject.GetComponent<Basket>();
            Debug.LogFormat("PlayerController: {0} 정보 가져옴, 현재 basket 빵의 개수: {1}", basketStat.gameObject.name, basketStat.numOfBread);
            
            coroutine = StartCoroutine(BringBread());

            
        }
        else if(other.gameObject.name == "Show Basket" )
        {
            displayBasketStat = other.gameObject.GetComponent<Basket>();
            Debug.LogFormat("PlayerController: {0} 정보 가져옴, 현재 basket 빵의 개수: {1}", basketStat.gameObject.name, displayBasketStat.numOfBread);
            
            coroutine = StartCoroutine(PutInBread());

            
        }
        else if(other.gameObject.name == "Cashier")
        {
            Debug.LogFormat("PlayerController: {0} 정보 가져옴", other.gameObject.name);
            cashierStat = other.gameObject.GetComponent<Cashier>();
            coroutine = StartCoroutine(PlayerStayingOnCashier());
        }
        else if(other.gameObject.name == "Table")
        {
            Debug.LogFormat("PlayerController: {0} 정보 가져옴", other.gameObject.name);
            tableStat = other.gameObject.GetComponent<Table>();
            tableStat.ClearTable();
            
        }
    }

    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.name == "Basket")
        {
            Debug.LogFormat("PlayerController: 빵 가져오기 중단");
            if(coroutine!=null) StopCoroutine(coroutine);
        }
        else if (other.gameObject.name == "Show Basket")
        {
            Debug.LogFormat("PlayerController: 빵 채우기 중단");
            if (coroutine != null) StopCoroutine(coroutine);

           
        }
        else if (other.gameObject.name == "Cashier")
        {
            Debug.LogFormat("PlayerController: 계산 중단");
            if (coroutine != null) StopCoroutine(coroutine);
            cashierStat.isPlayerOn = false;
        }
       
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.LogFormat("PlayerController: {0}에 도달", other.name);
        // 돈 영역 이동 -> 돈
        if (other.name == "BreadMoneyArea" || other.name == "SeatMoneyArea")
        {
            
            moneyStat = other.gameObject.GetComponent<Money>();
            coroutine = StartCoroutine(GettingMoney());
        }

        // 가구 영역 이동 -> 가구 생성
        if(other.name == "Lock")
        {
            payStat = other.gameObject.GetComponent<PayArea>();
            //payStat.payForObject();
            coroutine = StartCoroutine(PayingMoney());
        }
    }

   

    private void OnTriggerExit(Collider other)
    {
        Debug.LogFormat("PlayerController: {0}에서 벗어남", other.name);

        if (other.name == "BreadMoneyArea" || other.name == "SeatMoneyArea")
        {
            Debug.LogFormat("PlayerController: 돈 회수 중단");
            if(coroutine !=null) StopCoroutine(coroutine);
        }

        if (other.name == "Lock")
        {
            Debug.LogFormat("PlayerController: 가구 생성 중단");
            //payStat = other.gameObject.GetComponent<PayArea>();
            payStat.StopPay();
            if (coroutine != null) StopCoroutine(coroutine);
        }
    }

    IEnumerator BringBread()
    {
        while (numOfBread < maxNumOfBread)
        {
            if (basketStat.numOfBread > 0)
            {
                basketStat.PutOutBread();                   // 바구니의 빵이 감소하는 연산 수행

                // 빵 가져오는 애니메이션
                breadPutOutAnim.transform.position = new Vector3(0f, breads[numOfBread].transform.position.y, 0f);
                breadPutOutAnim.SetActive(true);
                sound.PlayOneShot(breadInClip);
                yield return new WaitForSeconds(0.05f);
                breadPutOutAnim.SetActive(false);

                // 빵 개수 확인: 빵 오브젝트 활성화
                breads[numOfBread].SetActive(true);
                numOfBread++;

                // 빵이 1개 이상이면 애니메이션 변경
                if (numOfBread > 0)
                {
                    // 애니메이션 변경
                    anim.SetBool("Bring", true);
                }
                yield return new WaitForSeconds(1f);
            }
            else
            {
                yield return new WaitForSeconds(1f);
                continue;
            }
        }
    }

    IEnumerator PutInBread()
    {
        while (displayBasketStat.numOfBread < displayBasketStat.maxNumOfBread )
        {
            if (numOfBread > 0)
            {
                displayBasketStat.PutInBread();

                // 빵 개수 확인: 빵 오브젝트 비활성화
                numOfBread--;

                // 빵 놓는 애니메이션
                breadPutInAnim.transform.position = new Vector3(0f, breads[numOfBread].transform.position.y, 0f);
                breadPutInAnim.SetActive(true);
                sound.PlayOneShot(breadOutClip);
                yield return new WaitForSeconds(0.06f);
                breadPutInAnim.SetActive(false);

                breads[numOfBread].SetActive(false);

                // 빵이 하나도 없다면 애니메이션 변경
                if (numOfBread == 0)
                {
                    anim.SetBool("Bring", false);
                }
                // MAX 비활성화
                if (numOfBread < maxNumOfBread)
                {
                    breadMax.gameObject.SetActive(false);
                }
                yield return new WaitForSeconds(1f);
            }
            else
            {
                yield return new WaitForSeconds(1f);
                continue;
            }
        }

    }
    IEnumerator GettingMoney()
    {
        while (true)
        {
            if (moneyStat.muchOfMoney == 0)
            {
                break;
            }
            moneyStat.GetMoney();
            sound.PlayOneShot(moneyGetClip);
            money++;
            moneyText.text = money.ToString();
            yield return null;
        }
    }
    IEnumerator PlayerStayingOnCashier()
    {
        while (true)
        {
            Debug.LogFormat("PlayerController: 계산대와의 거리제곱: {0}", (cashierStat.gameObject.transform.position - transform.position).sqrMagnitude);
            //if ((cashierStat.gameObject.transform.position - transform.position).sqrMagnitude < 2.0f)
            //{
                cashierStat.isPlayerOn = true;
                yield return new WaitForSeconds(0.5f);
            //}
            cashierStat.isPlayerOn = false;
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator PayingMoney()
    {
        while (money>0)
        {
            if (!payStat.gameObject.activeSelf) yield break;
            money--;
            payStat.requiredMoney--;
            Debug.LogFormat("Player: 자리값 지불 중, 남은 소지금액: {0}", money);
            payStat.payForObject();
            moneyText.text = money.ToString();
            yield return new WaitForSeconds(0.1f);
        }
    }
}
