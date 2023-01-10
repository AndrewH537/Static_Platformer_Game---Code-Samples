using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{

    public GameObject virtualCam;

    public Animator anim;

    private bool playerExiting;
    public Transform exitPoint;
    public float moveSpeed;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && !other.isTrigger)
        {   
            virtualCam.SetActive(false);

            virtualCam.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player") && !other.isTrigger)
        {
            //virtualCam.SetActive(false);
            anim.gameObject.GetComponent<Animator>().enabled = false;
            StartPause();
        }
    }

    public void StartPause()
    {
        StartCoroutine(PausePlayerMovement(1f));
    }

    public IEnumerator PausePlayerMovement(float pauseTime)
    {
        Time.timeScale = 0f;
        float pauseEndTime = Time.realtimeSinceStartup + pauseTime;
        while (Time.realtimeSinceStartup < pauseEndTime)
        {
            yield return 0;
        }
        Time.timeScale = 1f;
        anim.gameObject.GetComponent<Animator>().enabled = true;
    }
   
}
