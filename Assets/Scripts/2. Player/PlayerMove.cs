using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Unity.VisualScripting;

public class PlayerMove : MonoBehaviour
{
    public RawImage moveUI;
    public RawImage UIDir;
    public RawImage cursor;
    RectTransform uiRect;
    RectTransform dirRect;
    RectTransform cursorRect;
    public float maxDist;

    
    public GameObject player;
    PlayerController playerC;
    Touch touch;

    private void Awake()
    {
        playerC = player.GetComponent<PlayerController> ();
        uiRect = moveUI.GetComponent<RectTransform>();
        dirRect = UIDir.GetComponent<RectTransform>();
        cursorRect = cursor.GetComponent<RectTransform>();

        Input.simulateMouseWithTouches = true;
        
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) /*|| touch.phase == TouchPhase.Began*/)
        {
            moveUI.gameObject.SetActive(true);
            uiRect.position = Input.mousePosition;
            dirRect.position = Input.mousePosition;
            cursorRect.position = Input.mousePosition;
            //Debug.LogFormat("UIMove: 클릭, 마우스 위치: {0}, dirRect 위치: {1}", Input.mousePosition, dirRect.position);
        }
        else if (Input.GetMouseButtonUp(0) /*|| touch.phase == TouchPhase.Ended*/)
        {
            //Debug.LogFormat("UIMove: 클릭 해제");
            playerC.Idle();
            moveUI.gameObject.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        //ebug.LogFormat("마우스 UI 작동 중");

        //if (Input.touchCount > 0)
        //{
            //touch = Input.GetTouch(0);
            // 마우스 클릭하는 동안 생성 유지
            

        if (Input.GetMouseButton(0) /*|| touch.phase == TouchPhase.Moved*/)
            {
                //Debug.LogFormat("UIMove: 드래그");
                // 커서 위치 추적
                cursorRect.position = Input.mousePosition;

                // UI Dist는 일정 거리까지는 cursor와 함께 이동, 일정 dist 이상은 벗어날 수 없음
                Vector3 playerDir = Vector3.zero;
                Vector3 playerDist = Vector3.zero;
                float sqrDist = 0;

                playerDir = cursorRect.position - uiRect.position;            // 중심과 커서 사이의 벡터 방향
                sqrDist = playerDir.sqrMagnitude;                             // 중심과 커서 사이의 벡터 거리 제곱
                
                if (sqrDist > maxDist * maxDist)
                {
                    // UI 영역을 벗어나지 않도록 거리 고정, 방향은 계속 반영
                    playerDist = playerDir.normalized * maxDist;
                }
                else
                {
                    // 중심과 커서 사이 거리보다 일정 비율로 적게 이동
                    playerDist = playerDir * 0.8f;
                }
                dirRect.position = uiRect.position + playerDist;
                playerC.Rotate(playerDir);
                playerC.Move(playerDist);
            
            }
        

            //if (Input.GetMouseButtonUp(0) /*|| touch.phase == TouchPhase.Ended*/)
            //{
            //    Debug.LogFormat("UIMove: 클릭 해제");
            //    playerC.Idle();
            //    moveUI.gameObject.SetActive(false);
            //}
        //}

    }
}
