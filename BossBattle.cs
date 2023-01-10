using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBattle : MonoBehaviour
{

    private CameraController theCam;
    public Transform camPosition;
    public float camSpeed;

//setting phases based on health
    public int threshold1, threshold2;

    public float activeTime, fadeoutTime, inactiveTime;
    private float activeCounter, fadeCounter, inactiveCounter;

    public Transform[] spawnPoints;
    private Transform targetPoint;
    public float moveSpeed;

    public Animator anim;

    public Transform theBoss;

    public float timeBetweenShots1, timeBetweenShots2;
    private float shotCounter;
    public GameObject bullet;
    public Transform shotPoint;

    public GameObject winObjects;

    private bool battleEnded;

    // Start is called before the first frame update
    void Start()
    {
        theCam = FindObjectOfType<CameraController>();
        theCam.enabled = false;

        activeCounter = activeTime;

        shotCounter = timeBetweenShots1;

        AudioManager.instance.PlayBossMusic();
    }

    // Update is called once per frame
    void Update()
    {
        theCam.transform.position = Vector3.MoveTowards(theCam.transform.position, camPosition.position, camSpeed * Time.deltaTime);

        if(!battleEnded)
        {


            if(BossHealthController.instance.currentHealth > threshold1)
            {
                if(activeCounter > 0)
                {
                    activeCounter -= Time.deltaTime;
                    if(activeCounter <= 0)
                    {
                        fadeCounter = fadeoutTime;
                        anim.SetTrigger("Vanish");
                    }

                    shotCounter -= Time.deltaTime;
                    if(shotCounter <= 0)
                    {
                        shotCounter = timeBetweenShots1;

                        Instantiate(bullet, shotPoint.position, Quaternion.identity);
                    }
                }
                else if(fadeCounter > 0)
                {
                    fadeCounter -= Time.deltaTime;
                    if(fadeCounter <= 0)
                    {
                        theBoss.gameObject.SetActive(false);
                        inactiveCounter = inactiveTime;
                    }
                }
                else if(inactiveCounter > 0)
                {
                    inactiveCounter -= Time.deltaTime;
                    if(inactiveCounter <= 0)
                    {
                        theBoss.position = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
                        theBoss.gameObject.SetActive(true);

                        activeCounter = activeTime;

                        shotCounter = timeBetweenShots1;
                    }
                }
            }
            else
            {
                if(targetPoint == null)
                {
                    targetPoint = theBoss;
                    fadeCounter = fadeoutTime;
                    anim.SetTrigger("Vanish");
                } else
                {
                    if(Vector3.Distance(theBoss.position, targetPoint.position) > 0.2f)
                    {
                        theBoss.position = Vector3.MoveTowards(theBoss.position, targetPoint.position, moveSpeed * Time.deltaTime);


                        if(Vector3.Distance(theBoss.position, targetPoint.position) <= 0.2f)
                        {
                            fadeCounter = fadeoutTime;
                            anim.SetTrigger("Vanish");
                        }

                        shotCounter -= Time.deltaTime;
                        if(shotCounter <= 0)
                        {   
                            if(PlayerHealthController.instance.currentHealth > threshold2)
                            {
                                shotCounter = timeBetweenShots1;
                            } else
                            {
                                shotCounter = timeBetweenShots2;
                            }

                            Instantiate(bullet, shotPoint.position, Quaternion.identity);
                        }
                    }
                    else if(fadeCounter > 0)
                    {
                        fadeCounter -= Time.deltaTime;
                        if(fadeCounter <= 0)
                        {
                            theBoss.gameObject.SetActive(false);
                            inactiveCounter = inactiveTime;
                        }
                    }
                    else if(inactiveCounter > 0)
                    {
                        inactiveCounter -= Time.deltaTime;
                        if(inactiveCounter <= 0)
                        {
                            theBoss.position = spawnPoints[Random.Range(0, spawnPoints.Length)].position;


                            targetPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                            int whileBreaker = 0;
                            while(targetPoint.position == theBoss.position && whileBreaker < 100)
                            {
                                targetPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

                                whileBreaker++;
                            }

                            theBoss.gameObject.SetActive(true);

                            if(PlayerHealthController.instance.currentHealth > threshold2)
                            {
                                shotCounter = timeBetweenShots1;
                            } else
                            {
                                shotCounter = timeBetweenShots2;
                            }

                        }
                    }
                }
            }
        } else
        {
            fadeCounter -= Time.deltaTime;
            if(fadeCounter < 0)
            {
                if(winObjects != null)
                {
                    winObjects.SetActive(true);
                    winObjects.transform.SetParent(null);
                }

                theCam.enabled = true;

                gameObject.SetActive(false);
            }
        }
    }

    public void EndBattle()
    {
        battleEnded = true;

        fadeCounter = fadeoutTime;
        anim.SetTrigger("Vanish");
        theBoss.GetComponent<Collider2D>().enabled = false;

        BossBullet[] bullets = FindObjectsOfType<BossBullet>();
        if(bullets.Length > 0)
        {
            foreach(BossBullet bb in bullets)
            {
                Destroy(bb.gameObject);
            }
        }
    }
}
