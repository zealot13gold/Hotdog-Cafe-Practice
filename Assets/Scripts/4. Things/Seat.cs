using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seat : MonoBehaviour
{
    public bool isUsing;

    public GameObject bread;
    public GameObject trash;
    public GameObject table;

    Quaternion beforeUsed;
    Quaternion afterUsed;

    public void Start()
    {
        isUsing = true;


        beforeUsed = transform.rotation;
        afterUsed = transform.rotation * Quaternion.Euler(new Vector3(0f, 30f, 0f));
    }

    public Vector3 GetSeat()                 // 손님이 자리를 사용
    {
        Vector3 seatPos = transform.position + new Vector3(0f, 0.1f, 0f);
        //bread.SetActive(true);
        isUsing = false;

        return seatPos;
    }

    public void UsedSeat()
    {
        bread.SetActive(false);
        trash.SetActive(true);

        transform.rotation = afterUsed;
    }

    public void ClearSeat()                 // 플레이어가 자리를 청소(Table에서 처리)
    {
        trash.SetActive(false);

        transform.rotation = beforeUsed;

        isUsing = true;
    }
}
