using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    public GameObject[] seats;
    Seat seatStat;
    //public ParticleSystem setEffect;
    public ParticleSystem clearEffect;

    public AudioSource sound;
    public AudioClip setClip;
    public AudioClip clearClip;

    private void OnEnable()
    {
        sound.PlayOneShot(setClip);
        Debug.LogFormat("Table: {0} 설치 완료", gameObject.name);
        //setEffect.gameObject.SetActive(true);
        //setEffect.Play();
    }

    public void ClearTable()
    {
        bool isClear = false;
        foreach(GameObject seat in seats)
        {
            seatStat = seat.GetComponent<Seat>();
            if (!seatStat.isUsing)
            {
                seatStat.ClearSeat();
                isClear = true;
            }
        }
        if(isClear) StartCoroutine(CleanEffect());
    }

    IEnumerator CleanEffect()
    {
        clearEffect.gameObject.SetActive(true);
        clearEffect.Play();
        sound.PlayOneShot(clearClip);
        Debug.LogFormat("Table: 청소 완료");
        yield return new WaitForSeconds(5f);
        clearEffect.Stop();
        clearEffect.gameObject.SetActive(false);
    }
}
